namespace OVRLay
{
    using UnityEngine;
    using Valve.VR;

    public class OVRLay
    {
        private static readonly uint EVENT_SIZE = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(Valve.VR.VREvent_t));

        public string Name { get; private set; } = "OpenVR Overlay";
        public string Key { get; private set; } = "openvr-overlay";
        public bool IsDashboard { get; private set; } = false;
        public ulong Handle { get; private set; } = OpenVR.k_ulOverlayHandleInvalid;
        public bool ValidHandle { get => Handle != OpenVR.k_ulOverlayHandleInvalid; }
        public ulong IconHandle { get; private set; } = OpenVR.k_ulOverlayHandleInvalid;
        public bool Created { get; private set; }
        public bool Ready { get => OVR.Overlay != null && Created; }
        public bool HasFocus { get; private set; }

        public int CurrentTextureWidth = 0;
        public int CurrentTextureHeight = 0;

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

            if (OVR.Overlay == null)
                return false;

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

        public uint GetPrimaryDashboardDevice() =>
            OVR.Overlay.GetPrimaryDashboardDevice();

        private VROverlayIntersectionParams_t pParams;
        private VROverlayIntersectionResults_t pResult;
        public RaycastHit? ComputeRayIntersection(Vector3 source, Vector3 direction)
        {
            pParams.eOrigin = ETrackingUniverseOrigin.TrackingUniverseStanding;
            pParams.vSource = source.ToHMDVector();
            pParams.vDirection = direction.ToHMDVector();

            if (OVR.Overlay.ComputeOverlayIntersection(Handle, ref pParams, ref pResult))
                return pResult.ToRayHit();
            else
                return null;
        }

        public delegate void D_OnFocusChange(VREvent_t eventT, bool hasFocus);
        public D_OnFocusChange OnFocusChange = (eventT, hasFocus) => { };
        public delegate void D_OnDashboardChange(VREvent_t eventT, bool active);
        public D_OnDashboardChange OnDashboardChange = (eventT, active) => { };
        public delegate void D_OnVisibilityChange(VREvent_t eventT, bool visible);
        public D_OnVisibilityChange OnVisibilityChange = (eventT, visible) => { };
        public delegate void D_OnKeyboardDone(VREvent_t eventT);
        public D_OnKeyboardDone OnKeyboardDone = (eventT) => { };
        public delegate void D_OnKeyboardClose(VREvent_t eventT);
        public D_OnKeyboardClose OnKeyboardClose = (eventT) => { };
        public delegate void D_OnKeyboardInput(VREvent_t eventT, string minimal, string full);
        public D_OnKeyboardInput OnKeyboardInput = (eventT, minimal, full) => { };

        public delegate void D_OnScroll(VREvent_t eventT);
        public D_OnScroll OnScroll = (eventT) => { };
        public delegate void D_OnMouseMove(VREvent_t eventT);
        public D_OnMouseMove OnMouseMove = (eventT) => { };
        public delegate void D_OnMouseDown(VREvent_t eventT);
        public D_OnMouseDown OnMouseDown = (eventT) => { };
        public delegate void D_OnMouseUp(VREvent_t eventT);
        public D_OnMouseUp OnMouseUp = (eventT) => { };

        public delegate void D_OnDualAnalogMove(VREvent_t eventT);
        public D_OnDualAnalogMove OnDualAnalogMove = (eventT) => { };
        public delegate void D_OnDualAnalogTouch(VREvent_t eventT, bool state);
        public D_OnDualAnalogTouch OnDualAnalogTouch = (eventT, state) => { };
        public delegate void D_OnDualAnalogPress(VREvent_t eventT, bool state);
        public D_OnDualAnalogPress OnDualAnalogPress = (eventT, state) => { };
        public delegate void D_OnDualAnalogModeSwitch(VREvent_t eventT, int oneOrtwo);
        public D_OnDualAnalogModeSwitch OnDualAnalogModeSwitch = (eventT, oneOrTwo) => { };
        public delegate void D_OnDualAnalogCancel(VREvent_t eventT);
        public D_OnDualAnalogCancel OnDualAnalogCancel = (eventT) => { };

        public delegate void D_OnError(string error);
        public D_OnError OnError = (error) => { };

        public delegate void D_OnOtherEvent(VREvent_t eventT);
        public D_OnOtherEvent OnOtherEvent = (eventT) => { };

        private VREvent_t event_T = new VREvent_t();
        private const int _eventLoopMaxSaftey = 100;
        public void UpdateEvents()
        {
            int currentLoops = 0;
            while (currentLoops++ <= _eventLoopMaxSaftey && OVR.Overlay.PollNextOverlayEvent(Handle, ref event_T, EVENT_SIZE))
            {
                switch ((EVREventType)event_T.eventType)
                {
                    case EVREventType.VREvent_FocusEnter:
                        HasFocus = true;
                        OnFocusChange(event_T, true);
                        break;
                    case EVREventType.VREvent_FocusLeave:
                        HasFocus = false;
                        OnFocusChange(event_T, false);
                        break;
                    case EVREventType.VREvent_DashboardActivated:
                        OnDashboardChange(event_T, true);
                        break;
                    case EVREventType.VREvent_DashboardDeactivated:
                        OnDashboardChange(event_T, false);
                        break;
                    case EVREventType.VREvent_OverlayShown:
                        OnVisibilityChange(event_T, true);
                        break;
                    case EVREventType.VREvent_OverlayHidden:
                        OnVisibilityChange(event_T, false);
                        break;
                    case EVREventType.VREvent_KeyboardDone:
                        OnKeyboardDone(event_T);
                        break;
                    case EVREventType.VREvent_KeyboardClosed:
                        OnKeyboardClose(event_T);
                        break;
                    case EVREventType.VREvent_KeyboardCharInput:
                        {
                            System.Text.StringBuilder txtB = new System.Text.StringBuilder(1024);
                            OVR.Overlay.GetKeyboardText(txtB, 1024);
                            string fullTxt = txtB.ToString();

                            OnKeyboardInput(event_T, event_T.data.keyboard.cNewInput, fullTxt);
                            break;
                        }
                    case EVREventType.VREvent_Scroll:
                        OnScroll(event_T);
                        break;

                    case EVREventType.VREvent_MouseMove:
                        OnMouseMove(event_T);
                        break;
                    case EVREventType.VREvent_MouseButtonDown:
                        OnMouseDown(event_T);
                        break;
                    case EVREventType.VREvent_MouseButtonUp:
                        OnMouseUp(event_T);
                        break;

                    case EVREventType.VREvent_DualAnalog_Move:
                        OnDualAnalogMove(event_T);
                        break;
                    case EVREventType.VREvent_DualAnalog_Touch:
                        OnDualAnalogTouch(event_T, true);
                        break;
                    case EVREventType.VREvent_DualAnalog_Untouch:
                        OnDualAnalogTouch(event_T, false);
                        break;
                    case EVREventType.VREvent_DualAnalog_Press:
                        OnDualAnalogPress(event_T, false);
                        break;
                    case EVREventType.VREvent_DualAnalog_Unpress:
                        OnDualAnalogPress(event_T, false);
                        break;
                    case EVREventType.VREvent_DualAnalog_ModeSwitch1:
                        OnDualAnalogModeSwitch(event_T, 1);
                        break;
                    case EVREventType.VREvent_DualAnalog_ModeSwitch2:
                        OnDualAnalogModeSwitch(event_T, 2);
                        break;
                    case EVREventType.VREvent_DualAnalog_Cancel:
                        OnDualAnalogCancel(event_T);
                        break;
                    default:
                        OnOtherEvent(event_T);
                        break;
                }
            }
        }

        public bool Flag_Curved
        {
            get => GetOverlayFlag(VROverlayFlags.Curved);
            set => SetOverlayFlag(VROverlayFlags.Curved, value);
        }

        public bool Flag_RGSS4X
        {
            get => GetOverlayFlag(VROverlayFlags.RGSS4X);
            set => SetOverlayFlag(VROverlayFlags.RGSS4X, value);
        }
        public bool Flag_NoDashboardTab
        {
            get => GetOverlayFlag(VROverlayFlags.NoDashboardTab);
            set => SetOverlayFlag(VROverlayFlags.NoDashboardTab, value);
        }
        public bool Flag_AcceptsGamepadEvents
        {
            get => GetOverlayFlag(VROverlayFlags.AcceptsGamepadEvents);
            set => SetOverlayFlag(VROverlayFlags.AcceptsGamepadEvents, value);
        }
        public bool Flag_ShowGamepadFocus
        {
            get => GetOverlayFlag(VROverlayFlags.ShowGamepadFocus);
            set => SetOverlayFlag(VROverlayFlags.ShowGamepadFocus, value);
        }
        public bool Flag_SendVRScrollEvents
        {
            get => GetOverlayFlag(VROverlayFlags.SendVRScrollEvents);
            set => SetOverlayFlag(VROverlayFlags.SendVRScrollEvents, value);
        }
        public bool Flag_SendVRTouchpadEvents
        {
            get => GetOverlayFlag(VROverlayFlags.SendVRTouchpadEvents);
            set => SetOverlayFlag(VROverlayFlags.SendVRTouchpadEvents, value);
        }
        public bool Flag_ShowTouchPadScrollWheel
        {
            get => GetOverlayFlag(VROverlayFlags.ShowTouchPadScrollWheel);
            set => SetOverlayFlag(VROverlayFlags.ShowTouchPadScrollWheel, value);
        }
        public bool Flag_TransferOwnershipToInternalProcess
        {
            get => GetOverlayFlag(VROverlayFlags.TransferOwnershipToInternalProcess);
            set => SetOverlayFlag(VROverlayFlags.TransferOwnershipToInternalProcess, value);
        }
        public bool Flag_SideBySide_Parallel
        {
            get => GetOverlayFlag(VROverlayFlags.SideBySide_Parallel);
            set => SetOverlayFlag(VROverlayFlags.SideBySide_Parallel, value);
        }
        public bool Flag_SideBySide_Crossed
        {
            get => GetOverlayFlag(VROverlayFlags.SideBySide_Crossed);
            set => SetOverlayFlag(VROverlayFlags.SideBySide_Crossed, value);
        }
        public bool Flag_Panorama
        {
            get => GetOverlayFlag(VROverlayFlags.Panorama);
            set => SetOverlayFlag(VROverlayFlags.Panorama, value);
        }
        public bool Flag_StereoPanorama
        {
            get => GetOverlayFlag(VROverlayFlags.StereoPanorama);
            set => SetOverlayFlag(VROverlayFlags.StereoPanorama, value);
        }
        public bool Flag_SortWithNonSceneOverlays
        {
            get => GetOverlayFlag(VROverlayFlags.SortWithNonSceneOverlays);
            set => SetOverlayFlag(VROverlayFlags.SortWithNonSceneOverlays, value);
        }
        public bool Flag_VisibleInDashboard
        {
            get => GetOverlayFlag(VROverlayFlags.VisibleInDashboard);
            set => SetOverlayFlag(VROverlayFlags.VisibleInDashboard, value);
        }

        public void SetOverlayFlag(VROverlayFlags flag, bool toggle) { lastError = OVR.Overlay.SetOverlayFlag(Handle, flag, toggle); }
        public bool GetOverlayFlag(VROverlayFlags flag)
        {
            bool ret = false;
            lastError = OVR.Overlay.GetOverlayFlag(Handle, VROverlayFlags.Curved, ref ret);
            return ret;
        }

        private float pubFloat = 0f;
        private Color pubColor = Color.white;
        private Vector2 pubVec2 = Vector2.zero;

        public float WidthInMeters
        {
            get
            {
                ErrorCheck(lastError = OVR.Overlay.GetOverlayWidthInMeters(Handle, ref pubFloat));
                return pubFloat;
            }
            set => ErrorCheck(lastError = OVR.Overlay.SetOverlayWidthInMeters(Handle, value));
        }

        public Color Color
        {
            get
            {
                ErrorCheck(lastError = OVR.Overlay.GetOverlayColor(Handle, ref pubColor.r, ref pubColor.g, ref pubColor.b));
                return pubColor;
            }
            set => ErrorCheck(lastError = OVR.Overlay.SetOverlayColor(Handle, value.r, value.g, value.b));
        }

        public float Alpha
        {
            get
            {
                ErrorCheck(lastError = OVR.Overlay.GetOverlayAlpha(Handle, ref pubFloat));
                return pubFloat;
            }
            set => ErrorCheck(lastError = OVR.Overlay.SetOverlayAlpha(Handle, value));
        }

        public float TexelAspect
        {
            get
            {
                ErrorCheck(lastError = OVR.Overlay.GetOverlayTexelAspect(Handle, ref pubFloat));
                return pubFloat;
            }
            set => ErrorCheck(lastError = OVR.Overlay.SetOverlayTexelAspect(Handle, value));
        }

        public bool Visible
        {
            get => OVR.Overlay.IsOverlayVisible(Handle);
            set
            {
                if (value) ErrorCheck(lastError = OVR.Overlay.ShowOverlay(Handle));
                else ErrorCheck(lastError = OVR.Overlay.HideOverlay(Handle));
            }
        }

        public bool HighQuality
        {
            get => OVR.Overlay.GetHighQualityOverlay() == Handle;
            set { if (value) ErrorCheck(lastError = OVR.Overlay.SetHighQualityOverlay(Handle)); }
        }

        public Vector2 Size
        {
            get
            {
                uint w = 0, h = 0;
                ErrorCheck(lastError = OVR.Overlay.GetOverlayImageData(Handle, System.IntPtr.Zero, 0, ref w, ref h));
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

                CurrentTextureWidth = value.width;
                CurrentTextureHeight = value.height;

                ErrorCheck(lastError = OVR.Overlay.SetOverlayTexture(Handle, ref t));
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

                ErrorCheck(lastError = OVR.Overlay.SetOverlayTexture(IconHandle, ref t));
            }
        }

        public VRTextureBounds_t TextureBounds
        {
            get
            {
                VRTextureBounds_t t = new VRTextureBounds_t();
                ErrorCheck(lastError = OVR.Overlay.GetOverlayTextureBounds(Handle, ref t));
                return t;
            }
            set => ErrorCheck(lastError = OVR.Overlay.SetOverlayTextureBounds(Handle, ref value));
        }

        public VRTextureBounds_t IconTextureBounds
        {
            get
            {
                VRTextureBounds_t t = new VRTextureBounds_t();
                ErrorCheck(lastError = OVR.Overlay.GetOverlayTextureBounds(IconHandle, ref t));
                return t;
            }
            set => ErrorCheck(lastError = OVR.Overlay.SetOverlayTextureBounds(IconHandle, ref value));
        }

        public ETextureType TextureType { get; set; } = ETextureType.DirectX;
        public ETextureType IconTextureType { get; set; } = ETextureType.DirectX;

        public HmdMatrix34_t TransformAbsolute
        {
            get
            {
                HmdMatrix34_t posMatrix = new HmdMatrix34_t();
                ErrorCheck(lastError = OVR.Overlay.GetOverlayTransformAbsolute(
                    Handle, ref transformAbsoluteTrackingOrigin, ref posMatrix
                ));
                return posMatrix;
            }
            set => ErrorCheck(lastError = OVR.Overlay.SetOverlayTransformAbsolute(
                Handle, transformAbsoluteTrackingOrigin, ref value
            ));
        }

        public HmdMatrix34_t TransformTrackedDeviceRelative
        {
            get
            {
                HmdMatrix34_t posMatrix = new HmdMatrix34_t();
                ErrorCheck(lastError = OVR.Overlay.GetOverlayTransformTrackedDeviceRelative(
                    Handle, ref trackedDeviceIndex, ref posMatrix
                ));
                return posMatrix;
            }
            set => ErrorCheck(lastError = OVR.Overlay.SetOverlayTransformTrackedDeviceRelative(
                Handle, trackedDeviceIndex, ref value
            ));
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
                ErrorCheck(lastError = OVR.Overlay.GetOverlayInputMethod(Handle, ref inputMethod));
                return inputMethod;
            }
            set => ErrorCheck(lastError = OVR.Overlay.SetOverlayInputMethod(Handle, inputMethod = value));
        }

        private HmdVector2_t mouseScale = new HmdVector2_t();
        public HmdVector2_t MouseScale
        {
            get
            {
                ErrorCheck(lastError = OVR.Overlay.GetOverlayMouseScale(Handle, ref mouseScale));
                return mouseScale;
            }
            set
            {
                mouseScale = value;
                ErrorCheck(lastError = OVR.Overlay.SetOverlayMouseScale(Handle, ref mouseScale));
            }
        }
        public void ErrorCheck(EVROverlayError error)
        {
            if (error != EVROverlayError.None)
                OnError(Name + " : " + OVR.Overlay.GetOverlayErrorNameFromEnum(error));
        }
    }
}