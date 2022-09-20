using UnityEngine;
using System.Collections;
using UnityEditor;

namespace VREasy
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(GoogleVRControllerTrigger))]
    public class GoogleVRControllerTriggerEditor : Editor
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

#if VREASY_GOOGLEVR_SDK
            GoogleVRControllerTrigger controller = (GoogleVRControllerTrigger)target;
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Current selected input: " + controller.input);

            EditorGUI.BeginChangeCheck();
            GOOGLEVR_CONTROLLER_INPUT input = controller.input;
            drawAndSelectGoogleVRInputSelector(ref input);
            if (EditorGUI.EndChangeCheck())
            {
                foreach(GoogleVRControllerTrigger g in targets)
                {
                    Undo.RecordObject(g, "changed googlevr controller");
                    g.input = input;
                }
            }

#else
            EditorStyles.label.wordWrap = true;
            EditorGUILayout.HelpBox("Google VR SDK not found or not activated. Please make sure the Google VR SDK is imported and you have activated it via the VREasy/SDK Selector GUI", MessageType.Warning);
            
#endif
        }

#if VREASY_GOOGLEVR_SDK
        private void drawAndSelectGoogleVRInputSelector(ref GOOGLEVR_CONTROLLER_INPUT defaultButton)
        {
            Texture2D img = Resources.Load<Texture2D>("Daydream_Controller");
            GUILayout.BeginVertical();
            Rect GraphicRect = GUILayoutUtility.GetAspectRect(1);
            EditorGUI.DrawTextureTransparent(GraphicRect, img);
            GUILayout.EndVertical();
            float referenceSize = 384f;
            float baseWidth = GraphicRect.width;
            float baseHeight = GraphicRect.height;
            GraphicRect.width = baseWidth * 0.1f;
            GraphicRect.height = baseHeight * 0.1f;
            float baseX = GraphicRect.x;
            float baseY = GraphicRect.y;
            // Position all buttons
            if (VREasy_utils.CreateOverlayGUIButton(315 / referenceSize * baseWidth, 70 / referenceSize * baseHeight, ref GraphicRect, "Touchpad touch"))
            {
                defaultButton = GOOGLEVR_CONTROLLER_INPUT.IS_TOUCHING;
            }

            if (VREasy_utils.CreateOverlayGUIButton(190 / referenceSize * baseWidth, 105 / referenceSize * baseHeight, ref GraphicRect, "Touchpad press"))
            {
                defaultButton = GOOGLEVR_CONTROLLER_INPUT.CLICK_BUTTON;
            }

            if (VREasy_utils.CreateOverlayGUIButton(255 / referenceSize * baseWidth, 290 / referenceSize * baseHeight, ref GraphicRect, "App button"))
            {
                defaultButton = GOOGLEVR_CONTROLLER_INPUT.APP_BUTTON;
            }

            if (VREasy_utils.CreateOverlayGUIButton(100 / referenceSize * baseWidth, 170 / referenceSize * baseHeight, ref GraphicRect, "Home button"))
            {
                defaultButton = GOOGLEVR_CONTROLLER_INPUT.HOME_BUTTON_STATE;
            }




        }
#endif
    }
}