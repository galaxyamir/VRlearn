using UnityEngine;
using System.Collections;
using UnityEditor;

namespace VREasy
{
    [CustomEditor(typeof(SplineController))]
    public class SplineControllerEditor : Editor
    {
        bool handleRepaintErrors = false;

        public override void OnInspectorGUI()
        {
            // Hack to prevent ArgumentException: GUILayout: Mismatched LayoutGroup.Repaint errors
            // see more: https://forum.unity3d.com/threads/unexplained-guilayout-mismatched-issue-is-it-a-unity-bug-or-a-miss-understanding.158375/
            // and: https://forum.unity3d.com/threads/solved-adding-and-removing-gui-elements-at-runtime.57295/
            if (Event.current.type == EventType.Repaint && !handleRepaintErrors)
            {
                handleRepaintErrors = true;
                return;
            }

            ConfigureSplineController((SplineController)target);
            
        }

        public static void ConfigureSplineController(SplineController _controller)
        {

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("General properties", EditorStyles.boldLabel);
            Texture texture = (Texture)EditorGUILayout.ObjectField("Texture", _controller.texture, typeof(Texture), true);
            float width = EditorGUILayout.FloatField("Arrow width", _controller.LineWidth);
            float scrollSpeed = EditorGUILayout.FloatField("Scroll speed", _controller.ScrollSpeed);
            int arrowCount = EditorGUILayout.IntField("Arrows per point", _controller.ArrowCount);
            int angle = EditorGUILayout.IntSlider("Vertical angle", _controller.VerticalAngle,0,360);
            if(EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_controller, "Changed arrow properties");
                _controller.LineWidth = width;
                _controller.ScrollSpeed = scrollSpeed;
                _controller.ArrowCount = arrowCount;
                _controller.VerticalAngle = angle;
                _controller.texture = texture;
            }

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Control points", EditorStyles.boldLabel);
            bool redrawNeeded = false;
            for (int ii = 0; ii < _controller.ControlPoints.Count; ii++)
            {
                if (_controller.ControlPoints[ii] == null) continue;
                _controller.ControlPoints[ii] = (Transform)EditorGUILayout.ObjectField("Point" + ii, _controller.ControlPoints[ii], typeof(Transform), true);
                EditorGUILayout.BeginHorizontal();
                _controller.ControlPoints[ii].position = EditorGUILayout.Vector3Field("", _controller.ControlPoints[ii].position);
                if (GUILayout.Button("Delete"))
                {
                    DestroyImmediate(_controller.ControlPoints[ii].gameObject);
                    _controller.ControlPoints.RemoveAt(ii);
                    redrawNeeded = true;
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Separator();
            }
            if (GUILayout.Button("Add point"))
            {
                redrawNeeded = true;
                GameObject g = new GameObject("point" + _controller.ControlPoints.Count);
                g.transform.parent = _controller.transform;
                // set position equals to last point
                if (_controller.ControlPoints.Count > 0 && _controller.ControlPoints[_controller.ControlPoints.Count - 1] != null)
                {
                    g.transform.position = _controller.ControlPoints[_controller.ControlPoints.Count - 1].position;
                }
                else
                {
                    g.transform.position = Vector3.zero;
                }
                _controller.ControlPoints.Add(g.transform);
                Selection.activeGameObject = g;
            }
            // ensure number of points is adequate 
            if (_controller.ControlPoints.Count < _controller.BEZIER_MULTIPLIER)
            {
                EditorGUILayout.HelpBox("Minimum path points is " + _controller.BEZIER_MULTIPLIER + ". Add " + (_controller.BEZIER_MULTIPLIER - _controller.ControlPoints.Count) + " more", MessageType.Warning);
                
            }
            else
            {
                int reminder = (_controller.ControlPoints.Count - _controller.BEZIER_MULTIPLIER) % (_controller.BEZIER_MULTIPLIER - 1);
                if (reminder > 0)
                {
                    EditorGUILayout.HelpBox("Please add " + ((_controller.BEZIER_MULTIPLIER - 1) - reminder) + " more to complete path", MessageType.Error);
                }
            }

            EditorGUILayout.Separator();

            if(GUILayout.Button("Redraw"))
            {
                redrawNeeded = true;
            }

            if (redrawNeeded)
                _controller.DrawCurve();
            
            
            VREasy_utils.DrawHelperInfo();

            
        }
    }
}