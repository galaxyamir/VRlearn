using UnityEngine;
using System.Collections;
using System;

namespace VREasy
{
#if VREASY_STEAM_SDK
    //[RequireComponent(typeof(SteamVR_TrackedObject))]
#endif
    public class SteamControllerGrab : VRGrabTrigger
    {
#if VREASY_STEAM_SDK
        public Valve.VR.EVRButtonId button = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
        public STEAM_VR_CONTROLLER_SIDE controllerSide = STEAM_VR_CONTROLLER_SIDE.RIGHT;
        public STEAM_VR_CONTROLLER_INPUT_TYPE type = STEAM_VR_CONTROLLER_INPUT_TYPE.PRESS;

        //private SteamVR_TrackedObject trackedObject;
        SteamVR_Controller.Device rightDevice;
        SteamVR_Controller.Device leftDevice;

        void Start()
        {
            //trackedObject = GetComponent<SteamVR_TrackedObject>();
        }
#endif
        private void Update()
        {
#if VREASY_STEAM_SDK
            if (rightDevice == null) loadController(SteamVR_Controller.DeviceRelation.Rightmost, ref rightDevice);
            if (leftDevice == null) loadController(SteamVR_Controller.DeviceRelation.Leftmost, ref leftDevice);
#endif
        }

        public override bool Triggered()
        {
#if VREASY_STEAM_SDK
            switch(controllerSide)
            {
                case STEAM_VR_CONTROLLER_SIDE.LEFT:
                    {
                        return getInput(leftDevice);
                    }
                case STEAM_VR_CONTROLLER_SIDE.RIGHT:
                    {
                        return getInput(rightDevice);
                    }
            }
            return false;
#else
            return false;
#endif
        }
#if VREASY_STEAM_SDK
        private void loadController(SteamVR_Controller.DeviceRelation deviceRelation, ref SteamVR_Controller.Device device)
        {
            int index = SteamVR_Controller.GetDeviceIndex(deviceRelation);
            if (index >= 0) device = SteamVR_Controller.Input(index);
        }

        private bool getInput(SteamVR_Controller.Device device)
        {
            if (device == null) return false;

            switch(type)
            {
                case STEAM_VR_CONTROLLER_INPUT_TYPE.PRESS:
                    {
                        return device.GetPress(button);
                    }
                case STEAM_VR_CONTROLLER_INPUT_TYPE.TOUCH:
                    {
                        return device.GetTouch(button);
                    }
                default:
                    return false;
            }
        }
#endif
    }
}