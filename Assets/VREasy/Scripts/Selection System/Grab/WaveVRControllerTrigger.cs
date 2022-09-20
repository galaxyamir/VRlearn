using UnityEngine;
using System.Collections;
using System;
#if VREASY_WAVEVR_SDK
using wvr;
#endif

namespace VREasy
{
    public class WaveVRControllerTrigger : VRGrabTrigger
    {
#if VREASY_WAVEVR_SDK
        public WVR_DeviceType controller = WVR_DeviceType.WVR_DeviceType_Controller_Right;
        public WVR_InputId button;
#endif
        public override bool Triggered()
        {
#if VREASY_WAVEVR_SDK
            if (!WaveVR_Controller.Input(controller).connected) return false;
            return WaveVR_Controller.Input(controller).GetPress(button);

#else
            return false;
#endif
        }

    }
}