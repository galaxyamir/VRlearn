using UnityEngine;
using System.Collections;
using System;

namespace VREasy
{
    public class ActivateObjectAction : VRAction
    {
        public GameObject[] targets;
        public bool toggle = true;
        public bool activate = false;

        public override void Trigger()
        {
            foreach (GameObject target in targets)
            {
                if (toggle)
                {
                    target.SetActive(!target.activeInHierarchy);
                }
                else
                {
                    target.SetActive(!activate);
                }

            }
        }
    }

}