using UnityEngine;
using System.Collections;

namespace VREasy
{
    [System.Serializable]
    public class VRElement : MonoBehaviour
    {
        public bool active = true;
        public float init_transition = 0f;
        public float activation_transition = 0f;
        
        public void DeactivateElement()
        {
            active = false;
            activate(active);
        }

        public void ReactivateElement()
        {
            active = true;
            activate(active);
        }

        protected virtual void activate(bool state)
        {
            activateAll(active);
        }

        private void activateAll(bool state)
        {
            Collider[] cols = GetComponentsInChildren<Collider>();
            foreach (Collider col in cols)
            {
                col.enabled = state;
            }

            Renderer[] rends = GetComponentsInChildren<Renderer>();
            foreach (Renderer rend in rends)
            {
                rend.enabled = state;
            }
            Canvas[] cns = GetComponentsInChildren<Canvas>();
            foreach (Canvas c in cns)
            {
                c.enabled = state;
            }
        }

    }
}