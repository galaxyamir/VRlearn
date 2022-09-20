using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VREasy
{
    [CustomEditor(typeof(ActivateObjectAction))]
    public class ActivateObjectActionEditor : Editor
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

            ActivateObjectAction elements = (ActivateObjectAction)target;

            EditorGUILayout.Separator();

            var serializedObject = new SerializedObject(target);
            var property = serializedObject.FindProperty("targets");
            serializedObject.Update();
            EditorGUILayout.PropertyField(property, true);
            serializedObject.ApplyModifiedProperties();

            EditorGUI.BeginChangeCheck();
            bool toggle = EditorGUILayout.Toggle("Toggle hide/show", elements.toggle);
            bool activate = elements.toggle;
            if (!toggle)
            {
                activate = EditorGUILayout.Toggle("Set element to state", elements.activate);
                EditorGUILayout.LabelField("VRelements will be set to: " + (elements.activate ? "shown" : "hidden"), EditorStyles.wordWrappedLabel);
            }
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(elements, "changed properties");
                elements.toggle = toggle;
                elements.activate = activate;
            }
        }

    }
}