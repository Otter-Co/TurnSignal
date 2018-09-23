using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Valve.VR;
using OVRLay;

[RequireComponent(typeof(Overlay_Unity))]
[RequireComponent(typeof(Overlay_Texture))]
public class Overlay_MouseInput : MonoBehaviour
{
    public VROverlayInputMethod inputMethod = VROverlayInputMethod.None;
    [Space(10)]
    public Vector2 rawMousePosition = Vector2.zero;
    [Space(10)]
    public bool mouseDown = false;
    public float mouseDownTime = 0f;
    [Space(10)]
    public bool simulateUnityInteraction = false;
    public GraphicRaycaster targetMenu;
    public Vector2 simulatedMousePosition = Vector2.zero;

    private HashSet<Selectable> lastTargs = new HashSet<Selectable>();
    private HashSet<Selectable> currentTargs = new HashSet<Selectable>();

    private PointerEventData pointerData = new PointerEventData(EventSystem.current);
    private AxisEventData axisData = new AxisEventData(EventSystem.current);

    private HmdVector2_t mouseScale = new HmdVector2_t() { v0 = 1f, v1 = 1f };

    private float reverseAspect = 0f;
    private int widthMulti = 0;
    private int heightMulti = 0;

    private bool pastFirstClick = false;


    private Overlay_Unity u_overlay;
    private Overlay_Texture u_tex;
    private OVRLay.OVRLay overlay;

    void Update()
    {
        if (overlay == null)
        {
            u_overlay = GetComponent<Overlay_Unity>();
            u_tex = GetComponent<Overlay_Texture>();

            if (u_overlay.overlay != null)
                overlay = u_overlay.overlay;
            else
                return;

            overlay.OnMouseMove += (data) => UpdateRawMousePosition(data);
            overlay.OnMouseDown += (data) => UpdateRawMouseButton(data, true);
            overlay.OnMouseUp += (data) => UpdateRawMouseButton(data, false);

            return;
        }

        if (overlay.Created)
        {
            overlay.InputMethod = inputMethod;

            if (mouseScale.v1 != reverseAspect)
            {
                mouseScale.v1 = reverseAspect;
                overlay.MouseScale = mouseScale;
            }

            UpdateMouse();

            if (simulateUnityInteraction)
                UpdateMouseSimulation();
        }
    }

    void UpdateMouse()
    {
        if (u_tex?.currentTexture == null)
            return;

        int ttW = widthMulti = u_tex.currentTexture.width,
            ttH = heightMulti = u_tex.currentTexture.height;

        reverseAspect = (float)ttH / (float)ttW;

        if (mouseDown)
            mouseDownTime += Time.deltaTime;
        else if (mouseDownTime > 0f)
            mouseDownTime = 0f;
    }

    void UpdateRawMousePosition(VREvent_Mouse_t mD)
    {
        rawMousePosition.x = mD.x;
        rawMousePosition.y = mD.y;
    }

    void UpdateRawMouseButton(VREvent_Mouse_t mD, bool state)
    {
        UpdateRawMousePosition(mD);

        switch ((EVRMouseButton)mD.button)
        {
            case EVRMouseButton.Left:
                mouseDown = state;
                break;
        }
    }

    PointerEventData GetCurrentPD()
    {
        float mouseX = widthMulti * rawMousePosition.x;
        float mouseY = heightMulti * (1f - (rawMousePosition.y / reverseAspect));

        simulatedMousePosition.x = mouseX;
        simulatedMousePosition.y = mouseY;

        return new PointerEventData(EventSystem.current)
        {
            position = simulatedMousePosition,
            button = PointerEventData.InputButton.Left,
            clickTime = mouseDownTime,
            dragging = mouseDown,
            clickCount = 1
        };
    }

    AxisEventData GetCurrentAD(Vector3 diffVec)
    {
        float xDiff = diffVec.x, yDiff = diffVec.y;

        MoveDirection dir = (xDiff > yDiff)
            ? (xDiff > 0f)
                ? MoveDirection.Right
                : MoveDirection.Left
            : (yDiff > 0f)
                ? MoveDirection.Up
                : MoveDirection.Down;

        return new AxisEventData(EventSystem.current)
        {
            moveDir = dir,
            moveVector = diffVec
        };
    }

    public void UpdateMouseSimulation()
    {
        var curPD = GetCurrentPD();
        var curAD = GetCurrentAD((pointerData.position - curPD.position));

        var targs = GetSelectableTargets(targetMenu, curPD);

        UIE.FireEvent<IPointerEnterHandler>(ExecuteEvents.pointerEnterHandler, targs, curPD);

        foreach (Selectable t in targs)
            lastTargs.Remove(t);

        UIE.FireEvent<IPointerExitHandler>(ExecuteEvents.pointerExitHandler, lastTargs, curPD);

        lastTargs = targs;

        if (mouseDown)
            if (!pastFirstClick)
            {
                foreach (Selectable t in targs)
                    currentTargs.Add(t);

                UIE.FireEvent<ISubmitHandler>(ExecuteEvents.submitHandler, targs, curPD);
                UIE.FireEvent<IBeginDragHandler>(ExecuteEvents.beginDragHandler, targs, curPD);
                UIE.FireEvent<IPointerDownHandler>(ExecuteEvents.pointerDownHandler, targs, curPD);

                pastFirstClick = true;
            }
            else
            {
                UIE.FireEvent<IMoveHandler>(ExecuteEvents.moveHandler, currentTargs, axisData);
                UIE.FireEvent<IDragHandler>(ExecuteEvents.dragHandler, targs, curPD);
                UIE.FireEvent<IPointerDownHandler>(ExecuteEvents.pointerDownHandler, targs, curPD);
            }
        else
        {
            UIE.FireEvent<IPointerUpHandler>(ExecuteEvents.pointerUpHandler, targs, curPD);
            UIE.FireEvent<IEndDragHandler>(ExecuteEvents.endDragHandler, targs, curPD);
            UIE.FireEvent<IDropHandler>(ExecuteEvents.dropHandler, targs, curPD);

            currentTargs.Clear();
            pastFirstClick = false;
        }

        pointerData = curPD;
        axisData = curAD;
    }

    static HashSet<Selectable> GetSelectableTargets(GraphicRaycaster targetMenu, PointerEventData curPD)
    {
        List<RaycastResult> hits = new List<RaycastResult>();
        targetMenu.Raycast(curPD, hits);

        if (hits.Count > 0)
            curPD.pointerCurrentRaycast = curPD.pointerPressRaycast = hits[0];

        HashSet<Selectable> ret = new HashSet<Selectable>();
        foreach (RaycastResult hit in hits)
        {
            var u = hit.gameObject.GetComponentInParent<Selectable>();

            if (u)
                ret.Add(u);
        }

        return ret;
    }

    static class UIE
    {
        static public void FireEvent<T>(
            ExecuteEvents.EventFunction<T> eventF,
            HashSet<Selectable> t,
            PointerEventData pD
            ) where T : IEventSystemHandler
        {
            foreach (Selectable b in t)
                ExecuteEvents.Execute<T>(b.gameObject, pD, eventF);
        }

        static public void FireEvent<T>(
            ExecuteEvents.EventFunction<T> eventF,
            HashSet<Selectable> t,
            AxisEventData aD
            ) where T : IEventSystemHandler
        {
            foreach (Selectable b in t)
                ExecuteEvents.Execute<T>(b.gameObject, aD, eventF);
        }
    }
}