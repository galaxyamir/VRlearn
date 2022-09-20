using UnityEngine;
using System.Collections;

namespace VREasy
{
    [ExecuteInEditMode]
    public class VRCamera : MonoBehaviour
    {
        public VR_HMD hmd = VR_HMD.OCULUS;

        private VR_HMD previous_hmd = VR_HMD.OCULUS;

#if VREASY_CARDBOARD_SDK
        GvrViewer _gvrviewer = null;
#endif

#if VREASY_STEAM_SDK
        SteamVR_Camera _steamcam = null;
        SteamVR_Render _steamrenderer = null;
#endif

        // Use this for initialization
        void Update()
        {
            if (Application.isPlaying) return; // only create cameras on edit mode

            if(!CleanUp(hmd)) return;

            switch (hmd) { 
#if VREASY_CARDBOARD_SDK
                case VR_HMD.GOOGLE_VR:
                    _gvrviewer = FindObjectOfType<GvrViewer>();
                    if (_gvrviewer == null)
                    {
                        GameObject gvr = new GameObject();
                        gvr.name = "GvrViewer";
                        _gvrviewer = gvr.AddComponent<GvrViewer>();
                    }
                    break;
#endif
#if VREASY_STEAM_SDK
                case VR_HMD.HTC_VIVE:
                    _steamcam = FindObjectOfType<SteamVR_Camera>();
                    _steamrenderer = FindObjectOfType<SteamVR_Render>();
                    if (_steamrenderer == null) {
                        GameObject st = new GameObject();
                        st.name = "SteamRenderer";
                        _steamrenderer = st.AddComponent<SteamVR_Render>();
                    }
                    if (_steamcam == null)
                    {
                        _steamcam = gameObject.AddComponent<SteamVR_Camera>();
#if !UNITY_5_4
                        gameObject.AddComponent<SteamVR_CameraFlip>();
#endif
                        _steamcam.Expand();
                    }
                    break;
#endif
                default:
                    break;
            }
        }

        bool CleanUp(VR_HMD newHmd) {
            if (newHmd == previous_hmd) return false;
            previous_hmd = newHmd;
#if VREASY_CARDBOARD_SDK
            if(_gvrviewer != null) DestroyImmediate(_gvrviewer.gameObject);
#endif

#if VREASY_STEAM_SDK
            if (_steamcam != null)
            {
                _steamcam.Collapse(); 
                DestroyImmediate(_steamcam);
            }
            if (_steamrenderer != null) DestroyImmediate(_steamrenderer.gameObject);
#endif
            return true;
        }
    }

    public enum VR_HMD
    {
        OCULUS,
        GEAR_VR,
        GOOGLE_VR,
        HTC_VIVE,
    }

}