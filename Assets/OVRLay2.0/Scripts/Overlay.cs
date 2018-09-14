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
        public bool IsDashboard { get; private set; } = false;
        public ulong Handle { get; private set; } = OpenVR.k_ulOverlayHandleInvalid;
        public ulong IconHandle { get; private set; } = OpenVR.k_ulOverlayHandleInvalid;
        public bool Created { get; private set; }
        public EVROverlayError lastError { get; private set; } = EVROverlayError.None;

        public OVRLay(string name = null, string key = null, bool isDashboard = false, bool dontCreate = false)
        {
            if (name != null)
                Name = name;

            if (key != null)
                Key = key;

            IsDashboard = isDashboard;

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
            ulong iconHandle = OpenVR.k_ulOverlayHandleInvalid;

            if (IsDashboard)
                lastError = OVR.Overlay.CreateDashboardOverlay(Key, Name, ref newHandle, ref iconHandle);
            else
                lastError = OVR.Overlay.CreateOverlay(Key, Name, ref newHandle);

            var result = (lastError == EVROverlayError.None);

            if (result)
            {
                Handle = newHandle;
                IconHandle = iconHandle;
            }

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

        public delegate void D_OnFocusChange(bool hasFocus);
        public D_OnFocusChange OnFocusChange = (hasFocus) => { };
        public delegate void D_OnDashboardChange(bool active);
        public D_OnDashboardChange OnDashboardChange = (active) => { };
        public delegate void D_OnVisibilityChange(bool visible);
        public D_OnVisibilityChange OnVisibilityChange = (visible) => { };
        public delegate void D_OnKeyboardDone();
        public D_OnKeyboardDone OnKeyboardDone = () => { };
        public delegate void D_OnKeyboardClose();
        public D_OnKeyboardClose OnKeyboardClose = () => { };
        public delegate void D_OnKeyboardInput(string minimal, string full);
        public D_OnKeyboardInput OnKeyboardInput = (minimal, full) => { };
        public delegate void D_OnMouseMove(VREvent_Mouse_t data);
        public D_OnMouseMove OnMouseMove = (data) => { };
        public delegate void D_OnMouseDown(VREvent_Mouse_t data);
        public D_OnMouseDown OnMouseDown = (data) => { };
        public delegate void D_OnMouseUp(VREvent_Mouse_t data);
        public D_OnMouseUp OnMouseUp = (data) => { };

        public void UpdateEvents()
        {
            VREvent_t event_T = new VREvent_t();
            while (OVR.Overlay.PollNextOverlayEvent(Handle, ref event_T, EVENT_SIZE))
            {
                EVREventType eventType = (EVREventType)event_T.eventType;
                switch (eventType)
                {
                    case EVREventType.VREvent_FocusEnter:
                        OnFocusChange(true);
                        break;
                    case EVREventType.VREvent_FocusLeave:
                        OnFocusChange(false);
                        break;
                    case EVREventType.VREvent_DashboardActivated:
                        OnDashboardChange(true);
                        break;
                    case EVREventType.VREvent_DashboardDeactivated:
                        OnDashboardChange(false);
                        break;
                    case EVREventType.VREvent_OverlayShown:
                        OnVisibilityChange(true);
                        break;
                    case EVREventType.VREvent_OverlayHidden:
                        OnVisibilityChange(false);
                        break;

                    case EVREventType.VREvent_KeyboardDone:
                        OnKeyboardDone();
                        break;
                    case EVREventType.VREvent_KeyboardClosed:
                        OnKeyboardClose();
                        break;
                    case EVREventType.VREvent_KeyboardCharInput:
                        {
                            var kd = event_T.data.keyboard;
                            byte[] bytes = new byte[] {
                                kd.cNewInput0, kd.cNewInput1, kd.cNewInput2, kd.cNewInput3,
                                kd.cNewInput4, kd.cNewInput5, kd.cNewInput6, kd.cNewInput7,
                            };
                            int len = 0;
                            while (bytes[len++] != 0 && len < 7) ;
                            string minTxt = System.Text.Encoding.UTF8.GetString(bytes, 0, len);

                            System.Text.StringBuilder txtB = new System.Text.StringBuilder(1024);
                            OVR.Overlay.GetKeyboardText(txtB, 1024);
                            string fullTxt = txtB.ToString();

                            OnKeyboardInput(minTxt, fullTxt);
                            break;
                        }

                    case EVREventType.VREvent_MouseMove:
                        OnMouseMove(event_T.data.mouse);
                        break;
                    case EVREventType.VREvent_MouseButtonDown:
                        OnMouseDown(event_T.data.mouse);
                        break;
                    case EVREventType.VREvent_MouseButtonUp:
                        OnMouseUp(event_T.data.mouse);
                        break;
                }
            }
        }

        private float pubFloat = 0f;
        private Color pubColor = Color.white;
        private Vector2 pubVec2 = Vector2.zero;

        public float WidthInMeters
        {
            get
            {
                lastError = OVR.Overlay.GetOverlayWidthInMeters(Handle, ref pubFloat);
                return pubFloat;
            }
            set => lastError = OVR.Overlay.SetOverlayWidthInMeters(Handle, value);
        }

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

        private Texture lastIconTex = null;
        public Texture IconTexture
        {
            get => lastIconTex;
            set
            {
                Texture_t t = new Texture_t
                {
                    handle = value.GetNativeTexturePtr(),
                    eColorSpace = EColorSpace.Auto,
                    eType = IconTextureType
                };

                lastError = OVR.Overlay.SetOverlayTexture(IconHandle, ref t);
            }
        }

        public VRTextureBounds_t TextureBounds
        {
            get
            {
                VRTextureBounds_t t = new VRTextureBounds_t();
                lastError = OVR.Overlay.GetOverlayTextureBounds(Handle, ref t);
                return t;
            }
            set => lastError = OVR.Overlay.SetOverlayTextureBounds(Handle, ref value);
        }

        public VRTextureBounds_t IconTextureBounds
        {
            get
            {
                VRTextureBounds_t t = new VRTextureBounds_t();
                lastError = OVR.Overlay.GetOverlayTextureBounds(IconHandle, ref t);
                return t;
            }
            set => lastError = OVR.Overlay.SetOverlayTextureBounds(IconHandle, ref value);
        }

        public ETextureType TextureType { get; set; } = ETextureType.DirectX;
        public ETextureType IconTextureType { get; set; } = ETextureType.DirectX;

        public HmdMatrix34_t TransformAbsolute
        {
            get
            {
                HmdMatrix34_t posMatrix = new HmdMatrix34_t();
                lastError = OVR.Overlay.GetOverlayTransformAbsolute(
                    Handle, ref transformAbsoluteTrackingOrigin, ref posMatrix
                );
                return posMatrix;
            }
            set => lastError = OVR.Overlay.SetOverlayTransformAbsolute(
                Handle, transformAbsoluteTrackingOrigin, ref value
            );
        }

        public HmdMatrix34_t TransformTrackedDeviceRelative
        {
            get
            {
                HmdMatrix34_t posMatrix = new HmdMatrix34_t();
                lastError = OVR.Overlay.GetOverlayTransformTrackedDeviceRelative(
                    Handle, ref trackedDeviceIndex, ref posMatrix
                );
                return posMatrix;
            }
            set => lastError = OVR.Overlay.SetOverlayTransformTrackedDeviceRelative(
                Handle, trackedDeviceIndex, ref value
            );
        }

        private uint trackedDeviceIndex = OpenVR.k_unTrackedDeviceIndexInvalid;
        public uint TransformTrackedDeviceRelativeIndex
        {
            get => trackedDeviceIndex;
            set => trackedDeviceIndex = value;
        }

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