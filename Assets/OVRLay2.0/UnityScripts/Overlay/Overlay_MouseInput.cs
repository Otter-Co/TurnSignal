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
    public float timeUntilHold = 0.2f;
    public bool simulateUnityInteraction = false;
    public GraphicRaycaster targetMenu;
    public Vector2 simulatedMousePosition = Vector2.zero;
    [Space(10)]
    public Vector2 rawMousePosition = Vector2.zero;
    public bool mouseDown = false;
    public bool mouseHeld = false;
    public float mouseDownTime = 0f;


    private VROverlayInputMethod lastMethod = VROverlayInputMethod.None;
    private PointerEventData lastPD = new PointerEventData(EventSystem.current);
    private List<Selectable> lastTargs = new List<Selectable>();
    private List<Selectable> currentTargs = new List<Selectable>();
    private AxisEventData axisD = new AxisEventData(EventSystem.current);


    private Overlay_Unity u_overlay;
    private OVRLay.OVRLay overlay;

    void Start()
    {
        u_overlay = GetComponent<Overlay_Unity>();
        overlay = u_overlay.overlay;

        overlay.OnMouseMove += (data) => UpdateRawMousePosition(data);
        overlay.OnMouseDown += (data) => UpdateRawMouseButton(data, true);
        overlay.OnMouseUp += (data) => UpdateRawMouseButton(data, false);
    }

    void Update()
    {
        if (overlay == null)
        {
            u_overlay = GetComponent<Overlay_Unity>();
            overlay = u_overlay.overlay;
        }

        if (overlay.Created)
            overlay.InputMethod = inputMethod;

        UpdateMouse();

        if (simulateUnityInteraction)
            UpdateMouseSimulation();
    }

    void UpdateMouse()
    {

        if (mouseDown)
            mouseDownTime += Time.deltaTime;
        else if (mouseDownTime > 0f)
            mouseDownTime = 0f;

        mouseHeld = (mouseDown && mouseDownTime > timeUntilHold);
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
        var cam = targetMenu.eventCamera;

        int ttW = cam.targetTexture.width;
        int ttH = cam.targetTexture.height;

        float mouseX = ttW * rawMousePosition.x;
        float mouseY = ttH * (1f - (rawMousePosition.y / ((float)ttH / (float)ttW)));

        simulatedMousePosition.x = mouseX;
        simulatedMousePosition.y = mouseY;

        return new PointerEventData(EventSystem.current)
        {
            position = simulatedMousePosition,
            button = PointerEventData.InputButton.Left,
            clickTime = mouseDownTime,
            dragging = mouseHeld,
            clickCount = (mouseHeld) ? 0 : 1
        };
    }

    public void UpdateMouseSimulation()
    {
        var curPD = GetCurrentPD();

        var targs = GetSelectableTargets(targetMenu, curPD);

        UIE.EnterTargets(targs, curPD);
        UIE.ExitTargets(lastTargs.FindAll(t => !targs.Contains(t)), curPD);

        lastTargs.Clear();
        lastTargs.AddRange(targs);

        if (mouseDown)
            if (!mouseHeld)
            {
                currentTargs.AddRange(targs);

                UIE.SubmitTargets(currentTargs, curPD);
                UIE.StartDragTargets(currentTargs, curPD);
                UIE.DownTargets(currentTargs, curPD);
            }
            else
            {
                var diffVec = (lastPD.position - curPD.position);
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

                UIE.MoveTargets(currentTargs, axisD);
                UIE.DragTargets(currentTargs, curPD);
                UIE.DownTargets(currentTargs, curPD);
            }
        else
        {
            UIE.UpTargets(currentTargs, curPD);
            UIE.EndDragTargets(currentTargs, curPD);
            UIE.DropTargets(currentTargs, curPD);

            currentTargs.Clear();
        }
    }

    static List<Selectable> GetSelectableTargets(GraphicRaycaster targetMenu, PointerEventData curPD)
    {
        List<RaycastResult> hits = new List<RaycastResult>();
        targetMenu.Raycast(curPD, hits);

        if (hits.Count > 0)
            curPD.pointerCurrentRaycast = curPD.pointerPressRaycast = hits[0];

        return hits.ConvertAll(h => h.gameObject.GetComponentInParent<Selectable>());
    }

    static class UIE
    {
        static public void EnterTargets(List<Selectable> t, PointerEventData pD)
        {
            foreach (Selectable b in t)
                ExecuteEvents.Execute(b.gameObject, pD, ExecuteEvents.pointerEnterHandler);
        }

        static public void ExitTargets(List<Selectable> t, PointerEventData pD)
        {
            foreach (Selectable b in t)
                ExecuteEvents.Execute(b.gameObject, pD, ExecuteEvents.pointerExitHandler);
        }

        static public void DownTargets(List<Selectable> t, PointerEventData pD)
        {
            foreach (Selectable b in t)
                ExecuteEvents.Execute(b.gameObject, pD, ExecuteEvents.pointerDownHandler);
        }

        static public void UpTargets(List<Selectable> t, PointerEventData pD)
        {
            foreach (Selectable b in t)
                ExecuteEvents.Execute(b.gameObject, pD, ExecuteEvents.pointerUpHandler);
        }

        static public void SubmitTargets(List<Selectable> t, PointerEventData pD)
        {
            foreach (Selectable b in t)
                ExecuteEvents.Execute(b.gameObject, pD, ExecuteEvents.submitHandler);
        }

        static public void StartDragTargets(List<Selectable> t, PointerEventData pD)
        {
            foreach (Selectable b in t)
                ExecuteEvents.Execute(b.gameObject, pD, ExecuteEvents.beginDragHandler);
        }

        static public void DragTargets(List<Selectable> t, PointerEventData pD)
        {
            foreach (Selectable b in t)
                ExecuteEvents.Execute(b.gameObject, pD, ExecuteEvents.dragHandler);
        }

        static public void MoveTargets(List<Selectable> t, AxisEventData aD)
        {
            foreach (Selectable b in t)
                ExecuteEvents.Execute(b.gameObject, aD, ExecuteEvents.moveHandler);
        }

        static public void EndDragTargets(List<Selectable> t, PointerEventData pD)
        {
            foreach (Selectable b in t)
                ExecuteEvents.Execute(b.gameObject, pD, ExecuteEvents.endDragHandler);
        }

        static public void DropTargets(List<Selectable> t, PointerEventData pD)
        {
            foreach (Selectable b in t)
                ExecuteEvents.Execute(b.gameObject, pD, ExecuteEvents.dropHandler);
        }
    }
}