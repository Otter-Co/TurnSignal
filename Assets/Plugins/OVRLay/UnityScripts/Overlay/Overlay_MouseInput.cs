using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;

[RequireComponent(typeof(Overlay_Unity))]
public class Overlay_MouseInput : MonoBehaviour
{
    [Header("Overlay Mouse Input Settings")]
    public VROverlayInputMethod inputMethod = VROverlayInputMethod.None;

    [Space(10)]
    public bool simulateUnityInput = false;
    public EventSystem targetEventSystem;
    public Vector2 mouseScreenPixelSize;
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
    private float reverseAspect = 0f;

    private int widthMulti = 0;
    private int heightMulti = 0;

    private Overlay_Unity u_overlay;
    public OVRLay.OVRLay overlay;

    private Overlay_MouseBaseInput inputOverride;

    public Vector2 GetAdjustMousePos()
    {
        float mouseX = widthMulti * rawMousePosition.x;
        float mouseY = heightMulti * (1f - (rawMousePosition.y / reverseAspect));

        mouseY = !float.IsNaN(mouseY) ? mouseY : 0;

        return new Vector2(mouseX, mouseY);
    }


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

    void Update()
    {
        if (overlay == null)
        {
            u_overlay = GetComponent<Overlay_Unity>();

            if (u_overlay.overlay != null)
                overlay = u_overlay.overlay;
            else
                return;

            overlay.OnScroll += (data) => UpdateScroll(data);

            overlay.OnMouseMove += (data) => UpdateRawMousePosition(data);
            overlay.OnMouseDown += (data) => UpdateRawMouseButton(data, true);
            overlay.OnMouseUp += (data) => UpdateRawMouseButton(data, false);

            overlay.OnDualAnalogMove += (data) => UpdateDAPosition(data);
            overlay.OnDualAnalogTouch += (data, state) => UpdateDATouch(data, state);
            overlay.OnDualAnalogPress += (data, state) => UpdateDAPress(data, state);


            inputOverride = gameObject.AddComponent<Overlay_MouseBaseInput>().SetOverlayInput(this);
            targetEventSystem.currentInputModule.inputOverride = inputOverride;

            return;
        }

        if (overlay.Created)
        {
            int ttW = widthMulti = (int)mouseScreenPixelSize.x,
                ttH = heightMulti = (int)mouseScreenPixelSize.y;

            reverseAspect = (float)ttH / (float)ttW;
            mouseScale.v1 = reverseAspect;

            overlay.InputMethod = inputMethod;
            overlay.MouseScale = mouseScale;

            lastAdjustedMousePos = GetAdjustMousePos();

            if (simulateUnityInput && !targetEventSystem.isFocused)
                targetEventSystem.SendMessage("OnApplicationFocus", true);
        }
    }
}

