using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VREasy
{
    public class ActionList : MonoBehaviour
    {
        public List<VRAction> list = new List<VRAction>();
        
        public void Trigger()
        {
            VRActionManager.instance.ExecuteActions(list);
            //StartCoroutine(triggerActions());
            
        }

        /*private IEnumerator triggerActions()
        {
            foreach (VRAction a in list)
            {
                if (a != null)
                {
                    yield return new WaitForSeconds(a.delay);
                    a.Trigger();
                }
                
            }
        }*/
    }
}