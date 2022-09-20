using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

namespace VREasy
{
    [CustomEditor(typeof(ActivateComponentAction))]
    public class ActivateComponentActionEditor : Editor
    {
        private List<Component> components_list = new List<Component>();
        private List<string> componentNames_list = new List<string>();
        private GameObject componentReceiver = null;
        private int componentIndex = 0;

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

            ActivateComponentAction action = (ActivateComponentAction)target;

            if (action.component== null)
            {
                EditorGUILayout.HelpBox("Component not set, please choose a target game object", MessageType.Warning);
            }
            else
            {
                EditorGUILayout.LabelField("Current active component", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Target object: " + action.component.gameObject.name, EditorStyles.wordWrappedLabel);
                EditorGUILayout.LabelField("Target component: " + action.component.GetType().ToString(), EditorStyles.wordWrappedLabel);
            }

            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("Change target component", EditorStyles.boldLabel);
            EditorGUILayout.Separator();

            GameObject receiver = (GameObject)EditorGUILayout.ObjectField("Receiver", componentReceiver, typeof(GameObject), true);
            bool reloadComponents = false;

            if (componentReceiver != receiver)
            {
                reloadComponents = true;
                componentReceiver = receiver;
            }
            if (componentReceiver == null)
            {
                clearAll();
                return;
            }

            if (reloadComponents)
            {
                clearAll();
                VREasy_utils.LoadComponents(componentReceiver, ref components_list, ref componentNames_list);
            }
            componentIndex = EditorGUILayout.Popup("Component", componentIndex >= 0 ? componentIndex : 0, componentNames_list.ToArray());
            
            Handles.BeginGUI();
            if (GUILayout.Button("Set component"))
            {
                action.component = components_list[componentIndex];
                clearAll();
                componentReceiver = null;
            }
            Handles.EndGUI();
        }

        void clearAll()
        {
            componentIndex = -1;
            components_list.Clear();
            componentNames_list.Clear();
        }
    }
}