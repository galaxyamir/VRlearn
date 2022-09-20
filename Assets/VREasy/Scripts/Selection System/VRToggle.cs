using UnityEngine;
using System.Collections;

namespace VREasy
{
    public class VRToggle : VR2DButton
    {
        public bool toggle_state = true;

        public ActionList IdleActions
        {
            get
            {
                if (idleActions == null)
                {
                    GameObject child = new GameObject("IdleToggleActions");
                    child.transform.parent = transform;
                    idleActions = child.AddComponent<ActionList>();
                }
                return idleActions;
            }
        }
        public ActionList SelectActions
        {
            get
            {
                if (selectActions == null)
                {
                    GameObject child = new GameObject("SelectToggleActions");
                    child.transform.parent = transform;
                    selectActions = child.AddComponent<ActionList>();
                }
                return selectActions;
            }
        }
        public ActionList idleActions;
        public ActionList selectActions;

        protected override void Trigger()
        {
            // trigger one action list depending on the state
            if (toggle_state) ActivateIdle();
            else ActivateSelect();
        }

        protected override void SetState()
        {
            // switch between states
            if (toggle_state)
            {
                setSprite(idleIcon);
            } else
            {
                setSprite(selectIcon);
            }
            
            
        }

        public void ActivateIdle()
        {
            idleActions.Trigger();
            toggle_state = false;
        }

        public void ActivateSelect()
        {
            selectActions.Trigger();
            toggle_state = true;
        }
    }
}