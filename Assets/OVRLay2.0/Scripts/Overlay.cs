using System;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace OVRLay
{
    public class OVRLay
    {
        public static readonly uint EVENT_SIZE = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(Valve.VR.VREvent_t));

        public string Name { get; private set; } = "OpenVR Overlay";
        public string Key { get; private set; } = "openvr-overlay";
        public ulong Handle { get; private set; } = OpenVR.k_ulOverlayHandleInvalid;
        public bool Created { get; private set; }
        public EVROverlayError lastError { get; private set; } = EVROverlayError.None;

        public OVRLay(string name = null, string key = null, bool dontCreate = false)
        {
            if (name != null)
                Name = name;

            if (key != null)
                Key = key;

            if (!dontCreate)
                CreateOverlay();
        }

        ~OVRLay()
        {
            DestroyOverlay();
        }

        public bool CreateOverlay()
        {
            if (Created)
                return true;

            ulong newHandle = OpenVR.k_ulOverlayHandleInvalid;
            lastError = OVR.Overlay.CreateOverlay(Key, Name, ref newHandle);

            var result = (lastError == EVROverlayError.None);

            if (result)
                Handle = newHandle;

            return (Created = result);
        }

        public bool DestroyOverlay()
        {
            if (Created)
            {
                lastError = OVR.Overlay.DestroyOverlay(Handle);
                var result = (lastError == EVROverlayError.None);

                if (result)
                    Created = false;

                return result;
            }
            else
                return true;
        }

        public void UpdateEvents()
        {
            VREvent_t event_T = new VREvent_t();
            while (OVR.Overlay.PollNextOverlayEvent(Handle, ref event_T, EVENT_SIZE))
            {
                EVREventType eventType = (EVREventType)event_T.eventType;
                switch (eventType)
                {
                    case EVREventType.VREvent_FocusEnter:
                        break;
                    case EVREventType.VREvent_FocusLeave:
                        break;
                    case EVREventType.VREvent_DashboardActivated:
                        break;
                    case EVREventType.VREvent_DashboardDeactivated:
                        break;
                    case EVREventType.VREvent_OverlayShown:
                        break;
                    case EVREventType.VREvent_OverlayHidden:
                        break;
                    case EVREventType.VREvent_KeyboardDone:
                        break;
                    case EVREventType.VREvent_KeyboardClosed:
                        break;

                    case EVREventType.VREvent_KeyboardCharInput:
                        {
                            string txt = "";
                            var kd = event_T.data.keyboard;
                            byte[] bytes = new byte[]
                            {
                            kd.cNewInput0,
                            kd.cNewInput1,
                            kd.cNewInput2,
                            kd.cNewInput3,
                            kd.cNewInput4,
                            kd.cNewInput5,
                            kd.cNewInput6,
                            kd.cNewInput7,
                            };
                            int len = 0;
                            while (bytes[len++] != 0 && len < 7) ;
                            string input = System.Text.Encoding.UTF8.GetString(bytes, 0, len);

                            System.Text.StringBuilder txtB = new System.Text.StringBuilder(1024);
                            OVR.Overlay.GetKeyboardText(txtB, 1024);
                            txt = txtB.ToString();

                            break;
                        }

                    case EVREventType.VREvent_MouseMove:
                        {
                            var data = event_T.data.mouse;
                            break;
                        }

                    case EVREventType.VREvent_MouseButtonDown:
                        {
                            var data = event_T.data.mouse;
                            break;
                        }

                    case EVREventType.VREvent_MouseButtonUp:
                        {
                            var data = event_T.data.mouse;
                            break;
                        }
                }
            }
        }

        private float pubFloat = 0f;
        private Color pubColor = Color.white;
        private Vector2 pubVec2 = Vector2.zero;

        public Color Color
        {
            get
            {
                lastError = OVR.Overlay.GetOverlayColor(Handle, ref pubColor.r, ref pubColor.g, ref pubColor.b);
                return pubColor;
            }
            set => lastError = OVR.Overlay.SetOverlayColor(Handle, value.r, value.g, value.b);
        }

        public float Alpha
        {
            get
            {
                lastError = OVR.Overlay.GetOverlayAlpha(Handle, ref pubFloat);
                return pubFloat;
            }
            set => lastError = OVR.Overlay.SetOverlayAlpha(Handle, value);
        }

        public float TexelAspect
        {
            get
            {
                lastError = OVR.Overlay.GetOverlayTexelAspect(Handle, ref pubFloat);
                return pubFloat;
            }
            set => lastError = OVR.Overlay.SetOverlayTexelAspect(Handle, value);
        }

        public bool Visible
        {
            get => OVR.Overlay.IsOverlayVisible(Handle);
            set
            {
                if (value) lastError = OVR.Overlay.ShowOverlay(Handle);
                else lastError = OVR.Overlay.HideOverlay(Handle);
            }
        }

        public bool HighQuality
        {
            get => OVR.Overlay.GetHighQualityOverlay() == Handle;
            set { if (value) OVR.Overlay.SetHighQualityOverlay(Handle); }
        }

        public Vector2 Size
        {
            get
            {
                uint w = 0, h = 0;
                lastError = OVR.Overlay.GetOverlayImageData(Handle, System.IntPtr.Zero, 0, ref w, ref h);
                pubVec2.x = w;
                pubVec2.y = h;
                return pubVec2;
            }
        }

        private Texture lastTex = null;
        public Texture Texture
        {
            get => lastTex;
            set
            {
                Texture_t t = new Texture_t
                {
                    handle = value.GetNativeTexturePtr(),
                    eColorSpace = EColorSpace.Auto,
                    eType = TextureType
                };

                lastError = OVR.Overlay.SetOverlayTexture(Handle, ref t);
            }
        }

        public VRTextureBounds_t TexureBounds
        {
            get
            {
                VRTextureBounds_t t = new VRTextureBounds_t();
                lastError = OVR.Overlay.GetOverlayTextureBounds(Handle, ref t);
                return t;
            }
            set => lastError = OVR.Overlay.SetOverlayTextureBounds(Handle, ref value);
        }

        public ETextureType TextureType { get; set; } = ETextureType.DirectX;

        public HmdMatrix34_t Transform
        {
            get
            {
                HmdMatrix34_t posMatrix = new HmdMatrix34_t();
                switch (transformType)
                {
                    default:
                    case VROverlayTransformType.VROverlayTransform_Absolute:
                        lastError = OVR.Overlay.GetOverlayTransformAbsolute(
                            Handle, ref transformAbsoluteTrackingOrigin, ref posMatrix
                        );
                        break;
                    case VROverlayTransformType.VROverlayTransform_TrackedDeviceRelative:

                        break;
                }
                return posMatrix;
            }
            set
            {
                switch (transformType)
                {
                    default:
                    case VROverlayTransformType.VROverlayTransform_Absolute:
                        lastError = OVR.Overlay.SetOverlayTransformAbsolute(
                            Handle, transformAbsoluteTrackingOrigin, ref value
                        );
                        break;
                    case VROverlayTransformType.VROverlayTransform_TrackedDeviceRelative:
                        lastError = OVR.Overlay.SetOverlayTransformTrackedDeviceRelative(
                            Handle, TransformTrackedDeviceRelativeIndex, ref value
                        );
                        break;
                }
            }
        }

        public uint TransformTrackedDeviceRelativeIndex { get; set; }

        private VROverlayTransformType transformType = VROverlayTransformType.VROverlayTransform_Absolute;
        public VROverlayTransformType TransformType
        {
            get => transformType;
            set => transformType = value;
        }

        private ETrackingUniverseOrigin transformAbsoluteTrackingOrigin = ETrackingUniverseOrigin.TrackingUniverseStanding;
        public ETrackingUniverseOrigin TransformAbsoluteTrackingOrigin
        {
            get => transformAbsoluteTrackingOrigin;
            set => transformAbsoluteTrackingOrigin = value;
        }

        private VROverlayInputMethod inputMethod = VROverlayInputMethod.None;
        public VROverlayInputMethod InputMethod
        {
            get
            {
                lastError = OVR.Overlay.GetOverlayInputMethod(Handle, ref inputMethod);
                return inputMethod;
            }
            set => lastError = OVR.Overlay.SetOverlayInputMethod(Handle, inputMethod = value);
        }

        private HmdVector2_t mouseScale = new HmdVector2_t();
        public HmdVector2_t MouseScale
        {
            get
            {
                lastError = OVR.Overlay.GetOverlayMouseScale(Handle, ref mouseScale);
                return mouseScale;
            }
            set
            {
                mouseScale = value;
                lastError = OVR.Overlay.SetOverlayMouseScale(Handle, ref mouseScale);
            }
        }

    }
}