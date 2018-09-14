using System;
using System.Collections.Generic;
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
    public GraphicRaycaster targetMenu;
    [Space(10)]
    public Vector2 mousePosition = Vector2.zero;
    public bool mouseLeftDown = false;
    public bool mouseMiddleDown = false;
    public bool mouseRightDown = false;


    private VROverlayInputMethod lastMethod = VROverlayInputMethod.None;

    private PointerEventData curPointerD = new PointerEventData(EventSystem.current);
    private PointerEventData oldPointerD = new PointerEventData(EventSystem.current);

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

    List<Selectable> GetSelectableTargets()
    {
        var cam = targetMenu.eventCamera;
        var diffVec = (curPointerD.position - oldPointerD.position);

        float xDiff = diffVec.x, yDiff = diffVec.y;

        MoveDirection dir = (xDiff > yDiff)
            ? (xDiff > 0f)
                ? MoveDirection.Right
                : MoveDirection.Left
            : (yDiff > 0f)
                ? MoveDirection.Up
                : MoveDirection.Down;

        axisD.Reset();
        axisD.moveDir = dir;
        axisD.moveVector = diffVec;

        var ray = cam.ScreenPointToRay(curPointerD.position);
    }
}