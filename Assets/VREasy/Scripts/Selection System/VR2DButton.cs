using UnityEngine;
using System.Collections;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VREasy
{
    //[RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(AudioSource))]
    //[RequireComponent(typeof(ActionList))]
    [ExecuteInEditMode]
    public class VR2DButton : VRSelectable_sprite
    {
        
        #region
        public Text Label
        {
            get
            {
                if (!label)
                {
                    label = GetComponentInChildren<Text>();
                }
#if UNITY_EDITOR
                if (label)
                    UnityEditor.EditorUtility.SetDirty(label);
#endif
                return label;
            }
            set
            {
                label = value;
            }
        }

        #endregion PROPERTIES

        public Text label;

        public VRBUTTON_REFRESH_TYPE type = VRBUTTON_REFRESH_TYPE.NORMAL;
        public VRELEMENT_FACE_DIRECTION faceDirection = VRELEMENT_FACE_DIRECTION.FORWARD;
        
        protected BoxCollider _collider;
        public float _localScale = 1f;

        private Transform _lastParent = null;

        
        protected override void Initialise()
        {
            base.Initialise();
            _collider = GetComponent<BoxCollider>();
            _lastParent = transform.parent;
        }

        void Update()
        {
            Realign();
            checkParentChanged();
        }

        public void ConfigureCollider(float localScale, bool removePreviousCollider = true)
        {
            //Vector3 scale = new Vector3(5.0f, 5.0f, 1.0f) * localScale;//new Vector3(localScale / 10.0f, localScale / 10.0f, localScale / 20.0f);
            //_collider.size = scale;
        }

        public void SetScale(float localScale)
        {
            if (localScale == 0)
            {
                localScale = 1;
                _localScale = 0; // force reload
            }
            if (_localScale != localScale)
            {
                _localScale = localScale;
                width = 0.5f * _localScale;
                height = 0.5f * _localScale;
                refreshSprite();
            }
        }

        /*protected override void Selection()
        {
            base.Selection();
        }

        protected override void Pressed(VRSelector selector)
        {
            base.Pressed(selector);
        }

        protected override void Unselected()
        {
            base.Unselected();
        }*/

        private void Realign()
        {

            switch (type)
            {
                case VRBUTTON_REFRESH_TYPE.BILLBOARD:
#if UNITY_EDITOR
                    if (!EditorApplication.isPlaying) return;
#endif
                    setRotation();
                    break;
                case VRBUTTON_REFRESH_TYPE.STICKY:
                    transform.parent = VREasy_utils.GetMainCameraTransform();
                    break;
                case VRBUTTON_REFRESH_TYPE.NORMAL:
                    break;
            }
            
        }

        private void setRotation()
        {
            switch (faceDirection)
            {
                case VRELEMENT_FACE_DIRECTION.FORWARD:
                    {
                        
                        Vector3 lookAtPosition = VREasy_utils.GetMainCameraTransform().position;
                        lookAtPosition.y = transform.position.y;
                        transform.LookAt(lookAtPosition);
                        transform.Rotate(Vector3.up, 180.0f);
                    }
                    break;
                case VRELEMENT_FACE_DIRECTION.UP:
                    {
                        Vector3 lookAtPosition = VREasy_utils.GetMainCameraTransform().position;
                        lookAtPosition.y = transform.position.y;
                        transform.LookAt(lookAtPosition);
                        transform.Rotate(Vector3.right, 90.0f);
                        transform.Rotate(Vector3.forward, 180.0f);

                    }
                    break;
            
            }

        }

        // when transform.parent is changed, the local scale value is modified accordingly to avoid issues when changing icon
        private void checkParentChanged()
        {
            if(transform.parent != _lastParent)
            {
                if (transform.parent != null) {
                    if(_lastParent != null)
                    {
                        SetScale(_localScale * (_lastParent.transform.localScale.x / transform.parent.localScale.x ) ); 
                    } else
                    {
                        SetScale(_localScale / transform.parent.localScale.x);
                    }
                } else
                {
                    if(_lastParent != null)
                    {
                        SetScale(_localScale * _lastParent.transform.localScale.x);
                    }
                }
                _lastParent = transform.parent;
            }
        }

    }

    
}
