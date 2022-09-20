using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace VREasy
{
    public class VRSimpleFPSLocomotion : MonoBehaviour
    {
        public float speed = 2.0f;
        public Transform head;
        public VRLOCOMOTION_INPUT input = VRLOCOMOTION_INPUT.UNITY_INPUT;
        public float forwardAngle = 30f;
#if VREASY_STEAM_SDK
        public SteamVR_TrackedObject trackedObject;
        private SteamVR_Controller.Device device;
#endif
        public VRGrabTrigger trigger;

        public bool fixedHeight = true;
        public bool fixedForward = false;
        public float fixedMovement = 2f;
        public X_AXIS_TYPE xAxisType = X_AXIS_TYPE.TRANSLATE;

        private bool mobileMoving = false;

        void Awake()
        {
            if(transform == head)
            {
                Debug.LogWarning("VRSimpleLocomotion should not be placed in the HEAD game object but in a parent transform. Automatically fixed.");
                GameObject loc = new GameObject("[VREASY]LocomotionParent");
                //loc.transform.position = Vector3.zero;
                transform.parent = loc.transform;
                VRSimpleFPSLocomotion dest = loc.AddComponent<VRSimpleFPSLocomotion>();
                dest.speed = speed;
                dest.head = head;
                dest.input = input;
                dest.trigger = trigger;
                dest.fixedHeight = fixedHeight;
                dest.fixedForward = fixedForward;
                dest.fixedMovement = fixedMovement;
                dest.xAxisType = xAxisType;
#if VREASY_STEAM_SDK
                dest.trackedObject = trackedObject;
#endif
                Destroy(this);
            }

#if VREASY_GOOGLEVR_SDK
            if(input == VRLOCOMOTION_INPUT.DAYDREAM_CONTROLLER)
            {
                if(FindObjectOfType<GvrControllerInput>() == null)
                {
                    Debug.LogWarning("[VREasy] GvrControllerInput instance not found, adding one for VRSimpleFPSLocomotion");
                    GameObject n = new GameObject("[VREasy] GvrControllerInput");
                    n.AddComponent<GvrControllerInput>();
                }
            }
#endif
        }

        void Update()
        {
#if VREASY_OCULUS_UTILITIES_SDK
            OVRInput.Update();
#endif
            if (head == null)
                return;
            Vector3 move = Vector3.zero;
            switch (input)
            {
                case VRLOCOMOTION_INPUT.UNITY_INPUT:
                case VRLOCOMOTION_INPUT.GENERIC_VR_CONTROLLER:
                    move = Vector3.right * Input.GetAxis("Horizontal") - Vector3.forward * Input.GetAxis("Vertical");
                    break;
                case VRLOCOMOTION_INPUT.STEAM_CONTROLLER:
                    {
#if VREASY_STEAM_SDK
                        try
                        {
                            device = SteamVR_Controller.Input((int)trackedObject.index);
                            Vector2 inp = (device.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0));
                            move.x = inp.x;
                            move.z = inp.y;
                        }
#pragma warning disable 0168
                        catch (System.Exception _) { }
#pragma warning restore 0168
#endif
                    }
                    break;
                case VRLOCOMOTION_INPUT.MOBILE_TILT:
                    {
                        if(head.eulerAngles.x >= forwardAngle && head.eulerAngles.x < 90f)
                        {
                            mobileMoving = true;
                        }
                        if(head.eulerAngles.x >= 360f-forwardAngle)
                        {
                            mobileMoving = false;
                        }
                        if(mobileMoving)
                        {
                            move = Vector3.forward;
                        }
                    }
                    break;
                case VRLOCOMOTION_INPUT.TRIGGER:
                    {
                        if(trigger != null)
                        {
                            mobileMoving = (trigger.Triggered());
                            if(mobileMoving)
                            {
                                move = Vector3.forward;
                            }
                        }
                    }
                    break;
                case VRLOCOMOTION_INPUT.OCULUS_CONTROLLER:
                    {
#if VREASY_OCULUS_UTILITIES_SDK
                        Vector2 inp = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
                        move.x = inp.x;
                        move.z = inp.y;
#endif
                    }
                    break;
                case VRLOCOMOTION_INPUT.GEAR_VR_CONTROLLER:
                    {
#if VREASY_OCULUS_UTILITIES_SDK
                        Vector2 inp = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);
                        move.x = inp.x;
                        move.z = inp.y;
#endif
                    }
                    break;
                case VRLOCOMOTION_INPUT.DAYDREAM_CONTROLLER:
                    {
#if VREASY_GOOGLEVR_SDK
                        Vector2 inp = GvrControllerInput.IsTouching ? GvrControllerInput.TouchPosCentered : Vector2.zero;
                        move.x = inp.x;
                        move.z = inp.y;
#endif
                    }
                    break;
            }
            switch(xAxisType)
            {
                case X_AXIS_TYPE.TRANSLATE:
                    {

                    }
                    break;
                case X_AXIS_TYPE.ROTATE:
                    {
                        float rotate = move.x;
                        move.x = 0;
                        transform.RotateAround(head.transform.position,Vector3.up,rotate * speed);
                    }
                    break;
            }

            if (fixedForward) move.z = fixedMovement;
            else move *= speed;
            move = head.TransformDirection(move);
            if (fixedHeight)
                move.y = 0;
            transform.position += move * Time.deltaTime;
        }
    }
}