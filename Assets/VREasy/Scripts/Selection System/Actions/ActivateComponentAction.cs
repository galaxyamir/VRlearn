using UnityEngine;
using System.Collections;
using System;
using System.Reflection;

namespace VREasy
{
    public class ActivateComponentAction : VRAction
    {
        public Component component;

        public override void Trigger()
        {
            if (component != null)
            {
                try
                {
#if NETFX_CORE || UNITY_WSA_10_0
                    bool val = (bool)component.GetType().GetTypeInfo().GetDeclaredProperty("enabled").GetValue(component, null);
                    component.GetType().GetTypeInfo().GetDeclaredProperty("enabled").SetValue(component, !val, null);
#else
                    bool val = (bool)component.GetType().GetProperty("enabled").GetValue(component, null);
                    component.GetType().GetProperty("enabled").SetValue(component, !val, null);
#endif
                }
                catch (Exception e)
                {
                    Debug.LogWarning("[VREasy]: ActivateMonoBehaviourAction: Component does not have property 'enabled'. Error: " + e.ToString());
                }
            }
        }
        
    }
}