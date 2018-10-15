using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;

[RequireComponent(typeof(Overlay_Unity))]
public class Overlay_MouseInput : BaseInput
{
    [Header("Overlay Mouse Input Settings")]
    public VROverlayInputMethod inputMethod = VROverlayInputMethod.None;
    [Space(10)]
    public bool simulateUnityInput = false;
    public EventSystem targetEventSystem;
    [Space(10)]
    public Vector2 lastAdjustedMousePos;

    [Header("Overlay Mouse Values")]
    public Vector2 rawScrollValue;
    [Space(10)]
    public Vector2 rawMousePosition;
    [Space(10)]
    public bool leftMouseDown = false;
    public bool middleMouseDown = false;
    public bool rightMouseDown = false;
    [Space(10)]
    public Vector2 dualAnalogRightPosition;
    public Vector2 dualAnalogLeftPosition;
    [Space(10)]
    public bool dualAnalogRightTouch = false;
    public bool dualAnalogLeftTouch = false;
    [Space(10)]
    public bool dualAnalogRightPress = false;
    public bool dualAnalogLeftPress = false;

    private HmdVector2_t mouseScale = new HmdVector2_t() { v0 = 1f, v1 = 1f };
    public OVRLay.OVRLay overlay;
    private bool overlayValid { get => (overlay != null && overlay.Ready && overlay.Visible && overlay.HasFocus); }

    void Update()
    {
        if (overlay == null)
        {
            if ((overlay = GetComponent<Overlay_Unity>()?.overlay) == null)
                return;

            overlay.OnScroll += (data) => UpdateScroll(data);

            overlay.OnMouseMove += (data) => UpdateRawMousePosition(data);
            overlay.OnMouseDown += (data) => UpdateRawMouseButton(data, true);
            overlay.OnMouseUp += (data) => UpdateRawMouseButton(data, false);

            overlay.OnDualAnalogMove += (data) => UpdateDAPosition(data);
            overlay.OnDualAnalogTouch += (data, state) => UpdateDATouch(data, state);
            overlay.OnDualAnalogPress += (data, state) => UpdateDAPress(data, state);
        }

        if (overlay.Ready)
        {
            mouseScale.v1 = overlay.CurrentTextureWidth != 0f ? (float)((float)overlay.CurrentTextureHeight / (float)overlay.CurrentTextureWidth) : 1f;

            if (!overlay.MouseScale.Equals(mouseScale))
                overlay.MouseScale = mouseScale;

            if (overlay.InputMethod != inputMethod)
                overlay.InputMethod = inputMethod;

            if (simulateUnityInput)
            {
                if (targetEventSystem.currentInputModule.inputOverride != this)
                    targetEventSystem.currentInputModule.inputOverride = this;

                if (!targetEventSystem.isFocused)
                    targetEventSystem.SendMessage("OnApplicationFocus", true);
            }
            else if (targetEventSystem.currentInputModule.inputOverride == this)
                targetEventSystem.currentInputModule.inputOverride = null;
        }
    }

    public Vector2 GetCurrentAdjustedMousePos()
    {
        if (!overlayValid)
            return Vector2.zero;

        float mouseX = overlay.CurrentTextureWidth * rawMousePosition.x;
        float mouseY = overlay.CurrentTextureHeight * (1f - (
            rawMousePosition.y / mouseScale.v1
        ));

        mouseY = !float.IsNaN(mouseY) ? mouseY : 0;

        return new Vector2(mouseX, mouseY);
    }

    #region Event Handlers
    void UpdateScroll(VREvent_t eventT)
    {
        rawScrollValue.x = eventT.data.scroll.xdelta;
        rawScrollValue.y = eventT.data.scroll.ydelta;
        // rawScrollValue.z = eventT.data.scroll.repeatCount;
    }

    void UpdateRawMousePosition(VREvent_t eventT)
    {
        rawMousePosition.x = eventT.data.mouse.x;
        rawMousePosition.y = eventT.data.mouse.y;
    }

    void UpdateRawMouseButton(VREvent_t eventT, bool state)
    {
        switch ((EVRMouseButton)eventT.data.mouse.button)
        {
            case EVRMouseButton.Left:
                leftMouseDown = state;
                break;
            case EVRMouseButton.Middle:
                middleMouseDown = state;
                break;
            case EVRMouseButton.Right:
                rightMouseDown = state;
                break;
        }
    }

    void UpdateDAPosition(VREvent_t eventT)
    {
        var pos = new Vector2(
            eventT.data.dualAnalog.x,
            eventT.data.dualAnalog.y
        );

        switch (eventT.data.dualAnalog.which)
        {
            case EDualAnalogWhich.k_EDualAnalog_Right:
                dualAnalogRightPosition = pos;
                break;
            case EDualAnalogWhich.k_EDualAnalog_Left:
                dualAnalogLeftPosition = pos;
                break;
        }
    }
    void UpdateDATouch(VREvent_t eventT, bool state)
    {
        switch (eventT.data.dualAnalog.which)
        {
            case EDualAnalogWhich.k_EDualAnalog_Right:
                dualAnalogRightTouch = state;
                break;
            case EDualAnalogWhich.k_EDualAnalog_Left:
                dualAnalogLeftTouch = state;
                break;
        }
    }

    void UpdateDAPress(VREvent_t eventT, bool state)
    {
        switch (eventT.data.dualAnalog.which)
        {
            case EDualAnalogWhich.k_EDualAnalog_Right:
                dualAnalogRightPress = state;
                break;
            case EDualAnalogWhich.k_EDualAnalog_Left:
                dualAnalogLeftPress = state;
                break;
        }
    }
    #endregion

    #region BaseInput Overrides
    public override bool mousePresent { get => overlayValid ? inputMethod == VROverlayInputMethod.Mouse : base.mousePresent; }
    public override Vector2 mouseScrollDelta { get => overlayValid ? rawScrollValue : base.mouseScrollDelta; }

    public override Vector2 mousePosition { get => overlayValid ? GetCurrentAdjustedMousePos() : base.mousePosition; }

    public override bool GetMouseButtonUp(int button) => !GetMouseButton(button);
    public override bool GetMouseButtonDown(int button) => GetMouseButton(button);

    public override bool GetMouseButton(int button)
    {
        if (!overlayValid)
            return base.GetMouseButtonDown(button);

        switch (button)
        {
            case 0:
                return leftMouseDown;
            case 1:
                return rightMouseDown;
            case 2:
                return middleMouseDown;
            default:
                return false;
        }
    }
    #endregion
}

