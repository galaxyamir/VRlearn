using UnityEngine;
using System.Collections;
using UnityEditor;

namespace VREasy
{
    public class AddSelectorHelper : EditorWindow
    {

        [MenuItem("VREasy/VR Selector")]
        public static void ShowWindow()
        {
            GetWindow(typeof(AddSelectorHelper), false, "VR Selector");
        }

        private Vector2 scrollPos;
        private VRSELECTOR_TYPE _type = VRSELECTOR_TYPE.SIGHT;
        private GameObject _ref;

        bool handleRepaintErrors = false;
        void OnGUI()
        {
            // Hack to prevent ArgumentException: GUILayout: Mismatched LayoutGroup.Repaint errors
            // see more: https://forum.unity3d.com/threads/unexplained-guilayout-mismatched-issue-is-it-a-unity-bug-or-a-miss-understanding.158375/
            // and: https://forum.unity3d.com/threads/solved-adding-and-removing-gui-elements-at-runtime.57295/
            if (Event.current.type == EventType.Repaint && !handleRepaintErrors)
            {
                handleRepaintErrors = true;
                return;
            }
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Create selector objects", EditorStyles.boldLabel);
            EditorGUILayout.Separator();
            VRSELECTOR_TYPE type = (VRSELECTOR_TYPE)EditorGUILayout.EnumPopup("Selector type", _type);
            if(type != _type)
            {
                _ref = null;
            }
            _type = type;
            switch(_type)
            {
                case VRSELECTOR_TYPE.SIGHT:
                    displaySightSelectorPanel();
                    break;
                case VRSELECTOR_TYPE.POINTER:
                    displayPointerSelectorPanel();
                    break;
                case VRSELECTOR_TYPE.TOUCH:
                    displayTouchSelectorPanel();
                    break;
            }

            VREasy_utils.DrawHelperInfo();
            EditorGUILayout.EndScrollView();
        }

        void OnInspectorUpdate()
        {
            Repaint();
        }

        // SIGHT SELECTOR //
        private void displaySightSelectorPanel()
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Sight Selectors use Raycasting to interact with VR elements (face direction: along the local Z axis)", EditorStyles.wordWrappedLabel);
            EditorGUILayout.Separator();

            _ref = (GameObject)EditorGUILayout.ObjectField("Selector object",_ref,typeof(GameObject),true);

            EditorGUILayout.Separator();
            if (_ref != null)
            {
                SightSelector _sel = _ref.GetComponent<SightSelector>();
                if(_sel == null)
                {
                    // create sight selector
                    EditorGUILayout.LabelField("SightSelector not found");
                    Handles.BeginGUI();
                    if (GUILayout.Button("Add SightSelector"))
                    {
                        _ref.AddComponent<SightSelector>();
                    }
                    Handles.EndGUI();
                } else
                {
                    // configure properties
                    EditorGUILayout.LabelField("SightSelector found");
                    EditorGUILayout.Separator();
                    SightSelectorEditor.ConfigureSightSelector(_sel);
                }
                
            }
        }

        // POINTER SELECTOR //
        private void displayPointerSelectorPanel()
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Pointer Selectors use Raycasting and triggers to interact with VR elements (face direction: along the local Z axis)", EditorStyles.wordWrappedLabel);
            EditorGUILayout.Separator();

            _ref = (GameObject)EditorGUILayout.ObjectField("Selector object", _ref, typeof(GameObject), true);

            EditorGUILayout.Separator();
            if (_ref != null)
            {
                PointerSelector _sel = _ref.GetComponent<PointerSelector>();
                if (_sel == null)
                {
                    // create sight selector
                    EditorGUILayout.LabelField("PointerSelector not found");
                    Handles.BeginGUI();
                    if (GUILayout.Button("Add PointerSelector"))
                    {
                        Rigidbody rb = _ref.GetComponent<Rigidbody>();
                        bool hadRb = (rb != null);
                        _sel = _ref.AddComponent<PointerSelector>();
                        if(!hadRb)
                        {
                            _sel.ConfigureRigidbody();
                        }
                        LineRenderer line = _ref.GetComponent<LineRenderer>();
                        Material defaultMaterial = Instantiate<Material>(Resources.Load("Pointer", typeof(Material)) as Material);
                        if (defaultMaterial == null)
                        {
                            EditorUtility.DisplayDialog("Warning", "Default material for Pointer Selector \"Pointer\" not found in Resources. Please make sure you assign your own material to the Line Renderer of your new Pointer Selector", "OK");
                        } else
                        {
                            line.sharedMaterial = defaultMaterial;
                        }
                        _sel.LineWidth = 0.02f;
                        line.receiveShadows = false;
                        line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    }
                    Handles.EndGUI();
                }
                else
                {
                    // configure properties
                    EditorGUILayout.LabelField("PointerSelector found");
                    EditorGUILayout.Separator();
                    PointerSelectorEditor.ConfigurePointerSelector(_sel);
                    if (_sel.GetComponent<VRGrabTrigger>() != null)
                    {
                        EditorGUILayout.LabelField("Please go to the gameObject [" + _ref.name + "] to further configure the Grab trigger", EditorStyles.wordWrappedLabel);
                    }
                }
            }

        }

        // TOUCH SELECTOR //
        private void displayTouchSelectorPanel()
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Touch Selectors use the collision system to interact with VR elements (touch selector will require a rigidbody)", EditorStyles.wordWrappedLabel);
            EditorGUILayout.Separator();

            _ref = (GameObject)EditorGUILayout.ObjectField("Selector object", _ref, typeof(GameObject), true);
            EditorGUILayout.Separator();
            if (_ref != null)
            {
                TouchSelector _sel = _ref.GetComponent<TouchSelector>();
                if (_sel == null)
                {
                    // create selector
                    EditorGUILayout.LabelField("TouchSelector not found");
                    Handles.BeginGUI();
                    if (GUILayout.Button("Add TouchSelector"))
                    {
                        Rigidbody rb = _ref.GetComponent<Rigidbody>();
                        bool hadRb = (rb != null);
                        Collider cc = _ref.GetComponent<Collider>();
                        if (cc == null)
                        {
                            BoxCollider col = _ref.AddComponent<BoxCollider>();
                            col.size = new Vector3(0.1f, 0.1f, 0.2f);
                            col.center = Vector3.forward * 0.1f;
                            col.isTrigger = true;
                        } else
                        {
                            if (!cc.isTrigger)
                            {
                                Debug.LogWarning("The current selector object contains a collider that is not set to be a trigger. TouchSelector requires your collider to be a trigger. If this is an issue, please add a second (non-trigger) collider to your selector object.");
                            }
                            cc.isTrigger = true;
                        }
                        TouchSelector sel = _ref.AddComponent<TouchSelector>();
                        if (!hadRb) sel.ConfigureRigidbody();
                    }
                    Handles.EndGUI();
                } else
                {
                    // configure selector
                    EditorGUILayout.LabelField("TouchSelector found");
                    EditorGUILayout.Separator();
                    TouchSelectorEditor.ConfigureTouchSelector(_sel);
                    if(_sel.GetComponent<VRGrabTrigger>() != null)
                    {
                        EditorGUILayout.LabelField("Please go to the gameObject [" + _ref.name + "] to further configure the Grab trigger",EditorStyles.wordWrappedLabel);
                    }
                }
                
            }
        }
    }
}