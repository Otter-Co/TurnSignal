using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;

public class Overlay_MouseBaseInput : BaseInput
{
    public Overlay_MouseInput overlayMouseInput;
    private Vector2 lastMousePos;

    private bool overlayValid
    { get => (overlayMouseInput.simulateUnityInput && overlayMouseInput.overlay.Created && overlayMouseInput.overlay.Visible && overlayMouseInput.overlay.HasFocus); }
    public Overlay_MouseBaseInput SetOverlayInput(Overlay_MouseInput omi) { overlayMouseInput = omi; return this; }

    public override bool mousePresent { get => overlayValid ? overlayMouseInput.inputMethod == VROverlayInputMethod.Mouse : Input.mousePresent; }
    public override Vector2 mouseScrollDelta { get => overlayValid ? overlayMouseInput.rawScrollValue : Input.mouseScrollDelta; }

    public override Vector2 mousePosition { get => overlayValid ? overlayMouseInput.GetAdjustMousePos() : new Vector2(Input.mousePosition.x, Input.mousePosition.y); }

    public override bool GetMouseButtonUp(int button) => !GetMouseButton(button);
    public override bool GetMouseButtonDown(int button) => GetMouseButton(button);

    public override bool GetMouseButton(int button)
    {
        if (!overlayValid)
            return Input.GetMouseButtonDown(button);

        switch (button)
        {
            case 0:
                return overlayMouseInput.leftMouseDown;
            case 1:
                return overlayMouseInput.rightMouseDown;
            case 2:
                return overlayMouseInput.middleMouseDown;
            default:
                return false;
        }
    }
}