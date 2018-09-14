using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Valve.VR;
using OVRLay;

[RequireComponent(typeof(Overlay_Unity))]
public class Overlay_MouseInput : MonoBehaviour
{
    public VROverlayInputMethod inputMethod = VROverlayInputMethod.None;
    [Space(10)]
    public bool simulateUnityInteraction = false;
    public Camera targetCamera;
    public GraphicRaycaster targetMenu;
    [Space(10)]
    public Vector2 mousePosition = Vector2.zero;
    public bool mouseLeftDown = false;
    public bool mouseMiddleDown = false;
    public bool mouseRightDown = false;


    private VROverlayInputMethod lastMethod = VROverlayInputMethod.None;
    private PointerEventData pointerD = new PointerEventData(EventSystem.current);
    private AxisEventData axisD = new AxisEventData(EventSystem.current);


    private Overlay_Unity u_overlay;
    private OVRLay.OVRLay overlay;

    void Start()
    {
        u_overlay = GetComponent<Overlay_Unity>();
        overlay = u_overlay.overlay;

        overlay.OnMouseMove += (data) => UpdateMousePosition(data);
        overlay.OnMouseDown += (data) => UpdateMouseButton(data, true);
        overlay.OnMouseUp += (data) => UpdateMouseButton(data, false);
    }

    void Update()
    {
        if (overlay.Created && lastMethod != inputMethod)
        {
            overlay.InputMethod = inputMethod;
            lastMethod = inputMethod;
        }
    }

    void UpdateMousePosition(VREvent_Mouse_t mD)
    {
        mousePosition.x = mD.x;
        mousePosition.y = mD.y;
    }

    void UpdateMouseButton(VREvent_Mouse_t mD, bool state)
    {
        UpdateMousePosition(mD);

        switch ((EVRMouseButton)mD.button)
        {
            case EVRMouseButton.Left:
                mouseLeftDown = state;
                break;
            case EVRMouseButton.Middle:
                mouseMiddleDown = state;
                break;
            case EVRMouseButton.Right:
                mouseRightDown = state;
                break;
        }
    }
}