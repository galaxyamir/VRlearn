using UnityEngine;
using System.Collections;
using UnityEditor;

namespace VREasy
{
    [CustomEditor(typeof(VRSimpleFPSLocomotion))]
    public class VRSimpleFPSLocomotionEditor : Editor
    {
        [MenuItem("VREasy/Components/Simple FPS locomotion")]
        public static void AddScript()
        {
            if (Selection.activeGameObject != null) Selection.activeGameObject.AddComponent<VRSimpleFPSLocomotion>();
            else
            {
                EditorUtility.DisplayDialog("VREasy message", "To add FPS locomotion you must select a game object in the hierarchy first", "OK");
            }
        }

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
            VRSimpleFPSLocomotion locomotion = (VRSimpleFPSLocomotion)target;

            EditorGUI.BeginChangeCheck();
            float speed = EditorGUILayout.FloatField("Move Speed", locomotion.speed);
            
            Transform head = (Transform)EditorGUILayout.ObjectField("Head", locomotion.head, typeof(Transform), true);
            bool fixedForward = EditorGUILayout.Toggle("Fixed forward", locomotion.fixedForward);
            float fixedMovement = locomotion.fixedMovement;
            if (fixedForward)
            {
                fixedMovement = EditorGUILayout.FloatField("Forward speed", locomotion.fixedMovement);
            }
            X_AXIS_TYPE xType = (X_AXIS_TYPE)EditorGUILayout.EnumPopup("X move type", locomotion.xAxisType);
            bool fixedHeight = EditorGUILayout.Toggle("Fixed height", locomotion.fixedHeight);
            
            if(EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(locomotion,"Changed locomotion parameters");
                locomotion.head = head;
                locomotion.speed = speed;
                locomotion.fixedHeight = fixedHeight;
                locomotion.fixedForward = fixedForward;
                locomotion.fixedMovement = fixedMovement;
                locomotion.xAxisType = xType;
            }
            EditorGUILayout.Separator();

            locomotion.input = (VRLOCOMOTION_INPUT)EditorGUILayout.EnumPopup("Input type", locomotion.input);
            switch (locomotion.input)
            {
                case VRLOCOMOTION_INPUT.UNITY_INPUT:
                    EditorGUILayout.LabelField("Movement input based on Horizontal and Vertical axis from the Input System", EditorStyles.wordWrappedLabel);
                    break;
                case VRLOCOMOTION_INPUT.STEAM_CONTROLLER:
                    {
#if VREASY_STEAM_SDK
                        EditorGUILayout.LabelField("Movement input using Steam Controller's D-pad", EditorStyles.wordWrappedLabel);
                        locomotion.trackedObject = (SteamVR_TrackedObject)EditorGUILayout.ObjectField("Tracked controller", locomotion.trackedObject, typeof(SteamVR_TrackedObject), true);
#else
                        EditorGUILayout.HelpBox("Import Steam SDK and activate Steam SDK from the VREasy SDK helper window", MessageType.Error);
#endif
                    }
                    break;
                case VRLOCOMOTION_INPUT.OCULUS_CONTROLLER:
                    {
#if VREASY_OCULUS_UTILITIES_SDK
                        EditorGUILayout.LabelField("Movement input using Oculus touch Controller's left stick", EditorStyles.wordWrappedLabel);
#else
                        EditorGUILayout.HelpBox("Import Oculus Utilities and activate Oculus Utilities from the VREasy SDK helper window", MessageType.Error);
#endif
                    }
                    break;
                case VRLOCOMOTION_INPUT.MOBILE_TILT:
                    {
                        locomotion.forwardAngle = EditorGUILayout.FloatField("Start movement tilt angle", locomotion.forwardAngle);
                        EditorGUILayout.LabelField("Movement based on HMD horizontal tilt, look down to start moving, look up to stop", EditorStyles.wordWrappedLabel);
                    }
                    break;
                case VRLOCOMOTION_INPUT.TRIGGER:
                    GameObject obj = locomotion.gameObject;
                    VRGrabTrigger.DisplayGrabTriggerSelector(ref locomotion.trigger, ref obj);
                    break;
            }

            // add physical embodiment
            if(locomotion.GetComponent<Collider>() == null && locomotion.GetComponent<Rigidbody>() == null)
            {
                EditorGUILayout.Separator();
                EditorGUILayout.HelpBox("Physical body not detected in Locomotion object. You can make your locomotion object have a physical body by adding Rigidbody and collider components", MessageType.Info);
                if (GUILayout.Button("Add body"))
                {
                    locomotion.gameObject.AddComponent<CapsuleCollider>();
                    locomotion.gameObject.AddComponent<Rigidbody>();
                }
            } else
            {
                EditorGUILayout.Separator();
                EditorGUILayout.LabelField("Physical body detected in Locomotion object. Change its physical properties in the Rigidbody and Collider components", EditorStyles.wordWrappedLabel);
            }
        }
    }
}