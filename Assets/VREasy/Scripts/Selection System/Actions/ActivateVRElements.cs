using UnityEngine;
using System.Collections;
using System;

namespace VREasy
{
    public class ActivateVRElements : VRAction
    {
        public VRElement[] targets;
        public bool toggle = true;
        public bool activate = false;

        public override void Trigger()
        {
            foreach (VRElement t in targets)
            {
                if (toggle)
                {
                    if (t.active) t.DeactivateElement();
                    else t.ReactivateElement();
                } else
                {
                    if (activate)
                    {
                        t.ReactivateElement();
                    }
                    else
                    {
                        t.DeactivateElement();
                    }
                }

            }
        }
    }

}