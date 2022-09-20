using UnityEngine;
using System.Collections;
using UnityEditor;

namespace VREasy
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(GenericControllerTrigger))]
    public class GenericControllerTriggerEditor : Editor
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

            GenericControllerTrigger genericController = (GenericControllerTrigger)target;

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Settings for your VR controller", EditorStyles.boldLabel);
            EditorGUILayout.Separator();

            EditorGUI.BeginChangeCheck();

            //GENERIC_VR_BUTTON vr_button = (GENERIC_VR_BUTTON)EditorGUILayout.EnumPopup("Selected button", genericController.vr_button);
            GENERIC_CONTROLLER_TYPE type = (GENERIC_CONTROLLER_TYPE)EditorGUILayout.EnumPopup("Controller type", genericController.type);
            GENERIC_VR_BUTTON vr_button = genericController.vr_button;

            if (EditorGUI.EndChangeCheck())
            {
                foreach(GenericControllerTrigger g in targets)
                {
                    Undo.RecordObject(g, "Change in generic controller");
                    //g.vr_button = vr_button;
                    g.type = type;
                }
            }

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Select input", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Current input: " + genericController.vr_button);
            EditorGUI.BeginChangeCheck();
            switch (genericController.type)
            {
                case GENERIC_CONTROLLER_TYPE.OCULUS_TOUCH:
                    {
                        OculusControllerTriggerEditor.CheckWarningOculusSDKOrder();
                        EditorGUILayout.Separator();
                        vr_button = drawAndSelectOculusInputSelector(vr_button);
                    }
                    break;
                case GENERIC_CONTROLLER_TYPE.OCULUS_REMOTE:
                    {
                        OculusControllerTriggerEditor.CheckWarningOculusSDKOrder();
                        EditorGUILayout.Separator();
                        vr_button = drawAndSelectOculusRemoteInputSelector(vr_button);
                    }
                    break;
                case GENERIC_CONTROLLER_TYPE.STEAM_VR:
                    {
                        vr_button = drawAndSelectViveInputSelector(vr_button);
                    }
                    break;
                case GENERIC_CONTROLLER_TYPE.WINDOWS_MIXED_REALITY:
                    {
#if UNITY_2017_2_OR_NEWER
                        vr_button = drawAndSelectWMRInputSelector(vr_button);
#else
                        EditorGUILayout.Separator();
                        EditorGUILayout.HelpBox("Windows Mixed Reality controllers were only introduced in Unity 2017.2. Please upgrade to use them", MessageType.Warning);
#endif
                    }
                    break;
            }

            if (EditorGUI.EndChangeCheck())
            {
                foreach (GenericControllerTrigger g in targets)
                {
                    Undo.RecordObject(g, "Change in generic controller");
                    g.vr_button = vr_button;
                }
            }
        }

        private GENERIC_VR_BUTTON drawAndSelectOculusInputSelector(GENERIC_VR_BUTTON defaultButton)
        {
            Texture2D img = Resources.Load<Texture2D>("Oculus_Touch_generic");
            GUILayout.BeginVertical();
            Rect GraphicRect = GUILayoutUtility.GetAspectRect(1);
            EditorGUI.DrawTextureTransparent(GraphicRect, img);
            GUILayout.EndVertical();
            float referenceSize = 384f;
            float baseWidth = GraphicRect.width;
            float baseHeight = GraphicRect.height;
            GraphicRect.width = baseWidth * 0.1f;
            GraphicRect.height = baseHeight * 0.1f;

            // Position all buttons
            if(VREasy_utils.CreateOverlayGUIButton(135/ referenceSize * baseWidth, 150 / referenceSize * baseHeight, ref GraphicRect, "Primary thumbstick press"))
            {
                defaultButton = GENERIC_VR_BUTTON.OCULUS_PRIMARY_THUMB_STICK_PRESS;
            }

            if (VREasy_utils.CreateOverlayGUIButton(85 / referenceSize * baseWidth, 180 / referenceSize * baseHeight, ref GraphicRect, "Primary thumbstick touch"))
            {
                defaultButton = GENERIC_VR_BUTTON.OCULUS_PRIMARY_THUMB_STICK_TOUCH;
            }

            if (VREasy_utils.CreateOverlayGUIButton(45 / referenceSize * baseWidth, 240 / referenceSize * baseHeight, ref GraphicRect, "X press"))
            {
                defaultButton = GENERIC_VR_BUTTON.OCULUS_THREE;
            }

            if (VREasy_utils.CreateOverlayGUIButton(230 / referenceSize * baseWidth, 365 / referenceSize * baseHeight, ref GraphicRect, "Primary index trigger"))
            {
                defaultButton = GENERIC_VR_BUTTON.OCULUS_PRIMARY_INDEX_TRIGGER;
            }

            if (VREasy_utils.CreateOverlayGUIButton(265 / referenceSize * baseWidth, 20 / referenceSize * baseHeight, ref GraphicRect, "Secondary thumbstick press"))
            {
                defaultButton = GENERIC_VR_BUTTON.OCULUS_SECONDARY_THUMB_STICK_PRESS;
            }

            if (VREasy_utils.CreateOverlayGUIButton(320 / referenceSize * baseWidth, 50 / referenceSize * baseHeight, ref GraphicRect, "Secondary thumbstick touch"))
            {
                defaultButton = GENERIC_VR_BUTTON.OCULUS_SECONDARY_THUMB_STICK_TOUCH;
            }

            if (VREasy_utils.CreateOverlayGUIButton(355 / referenceSize * baseWidth, 110 / referenceSize * baseHeight, ref GraphicRect, "A press"))
            {
                defaultButton = GENERIC_VR_BUTTON.OCULUS_ONE;
            }

            if (VREasy_utils.CreateOverlayGUIButton(165 / referenceSize * baseWidth, 85 / referenceSize * baseHeight, ref GraphicRect, "Secondary index trigger"))
            {
                defaultButton = GENERIC_VR_BUTTON.OCULUS_SECONDARY_INDEX_TRIGGER;
            }

            return defaultButton;
        }

        private GENERIC_VR_BUTTON drawAndSelectOculusRemoteInputSelector(GENERIC_VR_BUTTON defaultButton)
        {
            Texture2D img = Resources.Load<Texture2D>("Oculus_Remote_Generic");
            GUILayout.BeginVertical();
            Rect GraphicRect = GUILayoutUtility.GetAspectRect(1);
            EditorGUI.DrawTextureTransparent(GraphicRect, img);
            GUILayout.EndVertical();
            float referenceSize = 384f;
            float baseWidth = GraphicRect.width;
            float baseHeight = GraphicRect.height;
            GraphicRect.width = baseWidth * 0.1f;
            GraphicRect.height = baseHeight * 0.1f;

            // Position all buttons
            if (VREasy_utils.CreateOverlayGUIButton(130 / referenceSize * baseWidth, 40 / referenceSize * baseHeight, ref GraphicRect, "One press"))
            {
                defaultButton = GENERIC_VR_BUTTON.OCULUS_REMOTE_ONE;
            }

            if (VREasy_utils.CreateOverlayGUIButton(290 / referenceSize * baseWidth, 310 / referenceSize * baseHeight, ref GraphicRect, "Two press"))
            {
                defaultButton = GENERIC_VR_BUTTON.OCULUS_REMOTE_TWO;
            }

            return defaultButton;
        }

        private GENERIC_VR_BUTTON drawAndSelectViveInputSelector(GENERIC_VR_BUTTON defaultButton)
        {
            Texture2D img = Resources.Load<Texture2D>("Vive_Controller_generic");
            GUILayout.BeginVertical();
            Rect GraphicRect = GUILayoutUtility.GetAspectRect(1);
            EditorGUI.DrawTextureTransparent(GraphicRect, img);
            GUILayout.EndVertical();
            float referenceSize = 384f;
            float baseWidth = GraphicRect.width;
            float baseHeight = GraphicRect.height;
            GraphicRect.width = baseWidth * 0.1f;
            GraphicRect.height = baseHeight * 0.1f;

            // Position all buttons
            if (VREasy_utils.CreateOverlayGUIButton(160/ referenceSize * baseWidth, 120/ referenceSize * baseHeight, ref GraphicRect, "Right trigger touch"))
            {
                defaultButton = GENERIC_VR_BUTTON.STEAMVR_RIGHT_TRIGGER_TOUCH;
            }

            if (VREasy_utils.CreateOverlayGUIButton(365/ referenceSize*baseWidth, 80/ referenceSize*baseHeight, ref GraphicRect, "Right trackpad press"))
            {
                defaultButton = GENERIC_VR_BUTTON.STEAMVR_RIGHT_TRACKPAD_PRESS;
            }
            if (VREasy_utils.CreateOverlayGUIButton(320/ referenceSize*baseWidth, 20/ referenceSize*baseHeight, ref GraphicRect, "Right trackpad touch"))
            {
                defaultButton = GENERIC_VR_BUTTON.STEAMVR_RIGHT_TRACKPAD_TOUCH;
            }

            if (VREasy_utils.CreateOverlayGUIButton(220/ referenceSize*baseWidth, 305/ referenceSize*baseHeight, ref GraphicRect, "Left trigger touch"))
            {
                defaultButton = GENERIC_VR_BUTTON.STEAMVR_LEFT_TRIGGER_TOUCH;
            }

            if (VREasy_utils.CreateOverlayGUIButton(25/ referenceSize*baseWidth, 210/ referenceSize*baseHeight, ref GraphicRect, "Left trackpad press"))
            {
                defaultButton = GENERIC_VR_BUTTON.STEAMVR_LEFT_TRACKPAD_PRESS;
            }
            if (VREasy_utils.CreateOverlayGUIButton(70/ referenceSize*baseWidth, 155/ referenceSize*baseHeight, ref GraphicRect, "Left trackpad touch"))
            {
                defaultButton = GENERIC_VR_BUTTON.STEAMVR_LEFT_TRACKPAD_TOUCH;
            }
            return defaultButton;


        }

        private GENERIC_VR_BUTTON drawAndSelectWMRInputSelector(GENERIC_VR_BUTTON defaultButton)
        {
            Texture2D img = Resources.Load<Texture2D>("MR_controller");
            GUILayout.BeginVertical();
            Rect GraphicRect = GUILayoutUtility.GetAspectRect(1);
            EditorGUI.DrawTextureTransparent(GraphicRect, img);
            GUILayout.EndVertical();
            float referenceSize = 384f;
            float baseWidth = GraphicRect.width;
            float baseHeight = GraphicRect.height;
            GraphicRect.width = baseWidth * 0.1f;
            GraphicRect.height = baseHeight * 0.1f;

            // Position all buttons
            if (VREasy_utils.CreateOverlayGUIButton(70 / referenceSize * baseWidth, 20 / referenceSize * baseHeight, ref GraphicRect, "Left Touchpad touch"))
            {
                defaultButton = GENERIC_VR_BUTTON.WMR_LEFT_TOUCHPAD_TOUCH;
            }
            if (VREasy_utils.CreateOverlayGUIButton(320 / referenceSize * baseWidth, 135 / referenceSize * baseHeight, ref GraphicRect, "Right Touchpad touch"))
            {
                defaultButton = GENERIC_VR_BUTTON.WMR_RIGHT_TOUCHPAD_TOUCH;
            }
            if (VREasy_utils.CreateOverlayGUIButton(180 / referenceSize * baseWidth, 130 / referenceSize * baseHeight, ref GraphicRect, "Left Touchpad press"))
            {
                defaultButton = GENERIC_VR_BUTTON.WMR_LEFT_TOUCHPAD_PRESS;
            }
            if (VREasy_utils.CreateOverlayGUIButton(200 / referenceSize * baseWidth, 250 / referenceSize * baseHeight, ref GraphicRect, "Right Touchpad press"))
            {
                defaultButton = GENERIC_VR_BUTTON.WMR_RIGHT_TOUCHPAD_PRESS;
            }
            if (VREasy_utils.CreateOverlayGUIButton(170 / referenceSize * baseWidth, 65 / referenceSize * baseHeight, ref GraphicRect, "Left thumbstick press"))
            {
                defaultButton = GENERIC_VR_BUTTON.WMR_LEFT_THUMBSTICK_PRESS;
            }
            if (VREasy_utils.CreateOverlayGUIButton(220 / referenceSize * baseWidth, 180 / referenceSize * baseHeight, ref GraphicRect, "Right thumbstick press"))
            {
                defaultButton = GENERIC_VR_BUTTON.WMR_RIGHT_THUMBSTICK_PRESS;
            }
            if (VREasy_utils.CreateOverlayGUIButton(40 / referenceSize * baseWidth, 150 / referenceSize * baseHeight, ref GraphicRect, "Left select trigger press"))
            {
                defaultButton = GENERIC_VR_BUTTON.WMR_LEFT_SELECT_TRIGGER_PRESS;
            }
            if (VREasy_utils.CreateOverlayGUIButton(340 / referenceSize * baseWidth, 270 / referenceSize * baseHeight, ref GraphicRect, "Right select trigger press"))
            {
                defaultButton = GENERIC_VR_BUTTON.WMR_RIGHT_SELECT_TRIGGER_PRESS;
            }
            if (VREasy_utils.CreateOverlayGUIButton(170 / referenceSize * baseWidth, 190 / referenceSize * baseHeight, ref GraphicRect, "Left grip press"))
            {
                defaultButton = GENERIC_VR_BUTTON.WMR_LEFT_GRIP_PRESS;
            }
            if (VREasy_utils.CreateOverlayGUIButton(210 / referenceSize * baseWidth, 310 / referenceSize * baseHeight, ref GraphicRect, "Right grip press"))
            {
                defaultButton = GENERIC_VR_BUTTON.WMR_RIGHT_GRIP_PRESS;
            }
            if (VREasy_utils.CreateOverlayGUIButton(65 / referenceSize * baseWidth, 185 / referenceSize * baseHeight, ref GraphicRect, "Left menu button press"))
            {
                defaultButton = GENERIC_VR_BUTTON.WMR_LEFT_MENU_BUTTON_PRESS;
            }
            if (VREasy_utils.CreateOverlayGUIButton(320 / referenceSize * baseWidth, 310 / referenceSize * baseHeight, ref GraphicRect, "Right menu button press"))
            {
                defaultButton = GENERIC_VR_BUTTON.WMR_RIGHT_MENU_BUTTON_PRESS;
            }

            return defaultButton;


        }
    }
}