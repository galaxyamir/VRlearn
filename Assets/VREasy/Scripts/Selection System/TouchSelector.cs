using UnityEngine;
using System.Collections;
using System;

namespace VREasy
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class TouchSelector : VRSelector
    {
        public VRGrabTrigger grabTrigger;

        private VRSelectable _selectObject = null;
        private VRGrabbable _grabObject = null;
        
        protected override VRSelectable GetSelectable()
        {
            return _selectObject;
        }

        protected override VRGrabbable GetGrabbable()
        {
            return _grabObject;
        }

        public void ConfigureRigidbody()
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        void OnTriggerStay(Collider col)
        {
            getTouchSelectable(col);
            getTouchGrabbable(col);
        }

        void OnCollisionStay(Collision col)
        {
            getTouchSelectable(col.collider);
            getTouchGrabbable(col.collider);
        }

        void OnTriggerExit(Collider col)
        {
            _selectObject = null;
        }

        void OnCollisionExit(Collision col)
        {
            _selectObject = null;
        }

        private void getTouchSelectable(Collider col)
        {
            if (_selectObject != null) return;
            _selectObject = col.gameObject.GetComponent<VRSelectable>();
            if (_selectObject != null && !_selectObject.CanSelectWithTouch())
            {
                _selectObject = null;
            }
        }

        private void getTouchGrabbable(Collider col)
        {
            if (_grabObject != null) return;
            if (grabTrigger != null)
            {
                if (grabTrigger.Triggered())
                {
                    // if object already grabbed, ignore anything else
                    if (_grabObject == null)
                    {
                        _grabObject = col.gameObject.GetComponent<VRGrabbable>();
                    }
                    return;
                }
            }
            _grabObject = null;
        }

        protected override void ChildUpdate()
        {
            // if object is grabbed but grabbable goes out of collider / trigger, clean up if no longer triggered
            if(_grabObject != null && grabTrigger != null)
            {
                if(!grabTrigger.Triggered())
                {
                    _grabObject.StopGrab(this);
                    _grabObject = null;
                }
            }
        }
    }
}