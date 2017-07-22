/*
	Large Chunks copied from SteamVR_Overlay.cs, All credit to Valve for creating the Base.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Valve.VR;
public class OpenVR_Overlay : MonoBehaviour 
{
	public enum OverlayType 
	{
		GameOverlay,
		DashboardOverlay
	}

	public OverlayType type = OverlayType.GameOverlay;
	
	[Space(10)]
	public string overlayName = "Unity Overlay";
	public string overlayKey = "unity_overlay";

	[Space(10)]
	public bool useWorldSpaceForOverlay = true;

	[Space(10)]
	public Vector4 uvOffset = new Vector4(0, 0, 1, 1);

	[Tooltip("Static or Render Texture to Display")]
	public Texture texture;

	[Tooltip("Thumbnail for Dashboard Overlay")]
	public Texture thumbnailIcon;

	[Space(10)]

	public bool highQualityOverlay = false;
	[Range(0, 1.0f)]
	public float opacity = 1.0f;
	public float scale = 1.0f;

	[Space(10)]
	
	public VROverlayInputMethod inputMethod = VROverlayInputMethod.None;
	public Vector2 mouseScale = new Vector2(1f, 1f);
	public bool enableScroller = true;

	[Space(10)]
	public Vector2 mousePos = new Vector2(0f, 0f);

	[Space(10)]

	public bool rightMouseDown = false;
	public bool leftMouseDown = false;
	public bool middleMouseDown = false;


	[HideInInspector()]
	public ulong handle = 0;

	[HideInInspector()]
	public ulong thumbHandle = 0;

	public bool overlayInit = false;

	private bool _enabled = false;

	private VREvent_t pEvent;

	void Start()
	{
		CreateOverlay();
	}

	void OnEnable()
	{
		_enabled = true;
	}

	void OnDisable()
	{
		_enabled = false;
	}
	void OnDestroy()
	{
		DestroyOverlay();
	}

	void DestroyOverlay()
	{
		var overlay = OpenVR.Overlay;

		if(handle == OpenVR.k_ulOverlayHandleInvalid || overlay == null)
			return;

		overlay.DestroyOverlay(handle);
		handle = OpenVR.k_ulOverlayHandleInvalid;
	}

	public void CreateOverlay()
	{
		var overlay = OpenVR.Overlay;

		if(overlay == null)
			return;
		
		EVROverlayError error = EVROverlayError.None;

		switch(type)
		{
			case OverlayType.DashboardOverlay:
				error = overlay.CreateDashboardOverlay(overlayKey, overlayName, ref handle, ref thumbHandle);
			break;

			case OverlayType.GameOverlay:
			default:
				error = overlay.CreateOverlay(overlayKey, overlayName, ref handle);
			break;
		}

		if(error != EVROverlayError.None)
		{
			Debug.Log(overlay.GetOverlayErrorNameFromEnum(error));
			enabled = false;
		}
		else
		{
			if(type == OverlayType.DashboardOverlay)
			{
				overlay.SetOverlayFromFile(thumbHandle, Application.dataPath + "\\_Res\\icon.png");
			}

			overlayInit = true;			
		}
	}

	public void UpdateOverlay(Transform origin, ETrackingUniverseOrigin trackingSpace)
	{
		var overlay = OpenVR.Overlay;

		if(overlay == null)
			return;

		if(!_enabled)
		{
			overlay.HideOverlay(handle);
			return;
		}
		else
		{
			var error = EVROverlayError.None;

			error = overlay.ShowOverlay(handle);

			if (error == EVROverlayError.InvalidHandle || error == EVROverlayError.UnknownOverlay)
			{
				if (overlay.FindOverlay(overlayKey, ref handle) != EVROverlayError.None)
					return;
			}
		}

		var tex = new Texture_t();
		tex.handle = texture.GetNativeTexturePtr();
		tex.eType = OpenVR_Handler.instance.textureType;
		tex.eColorSpace = EColorSpace.Auto;

		var textureBounds = new VRTextureBounds_t();
		textureBounds.uMin = (0 + uvOffset.x) * uvOffset.z;
		textureBounds.vMin = (1 + uvOffset.y) * uvOffset.w;
		textureBounds.uMax = (1 + uvOffset.x) * uvOffset.z;
		textureBounds.vMax = (0 + uvOffset.y) * uvOffset.w;

		var offset = new SteamVR_Utils.RigidTransform(origin, transform).ToHmdMatrix34();

		var vecMouseScale = new HmdVector2_t();
		vecMouseScale.v0 = mouseScale.x;
		vecMouseScale.v1 = mouseScale.y;

		overlay.SetOverlayTexture(handle, ref tex);
		overlay.SetOverlayTextureBounds(handle, ref textureBounds);

		if(useWorldSpaceForOverlay)
			overlay.SetOverlayTransformAbsolute(handle, trackingSpace, ref offset);
	
		overlay.SetOverlayAlpha(handle, opacity);
		overlay.SetOverlayWidthInMeters(handle, scale);

		if(highQualityOverlay)
			overlay.SetHighQualityOverlay(handle);

		overlay.SetOverlayMouseScale(handle, ref vecMouseScale);
		overlay.SetOverlayInputMethod(handle, inputMethod);

		overlay.SetOverlayFlag(handle, VROverlayFlags.ShowTouchPadScrollWheel, enableScroller);

		while(PollNextEvent(ref pEvent))
		{
			EventHandler(pEvent);
		}				
	}

	public bool PollNextEvent(ref VREvent_t pEvent)
	{
		var overlay = OpenVR.Overlay;
		if (overlay == null)
			return false;

		var size = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(Valve.VR.VREvent_t));
		return overlay.PollNextOverlayEvent(handle, ref pEvent, size);
	}

	void EventHandler(VREvent_t pEvent)
	{
		var mouseD = pEvent.data.mouse;

		switch((EVREventType) pEvent.eventType)
		{
			case EVREventType.VREvent_MouseMove:
				mousePos.x = mouseD.x;
				mousePos.y = mouseD.y;
			break;

			case EVREventType.VREvent_MouseButtonDown:

				switch((EVRMouseButton) mouseD.button)
				{
					case EVRMouseButton.Left:
						leftMouseDown = true;
					break;

					case EVRMouseButton.Right:
						rightMouseDown = true;
					break;

					case EVRMouseButton.Middle:
						middleMouseDown = true;
					break;
				}
			break;

			case EVREventType.VREvent_MouseButtonUp:
				switch((EVRMouseButton) mouseD.button)
				{
					case EVRMouseButton.Left:
						leftMouseDown = false;
					break;

					case EVRMouseButton.Right:
						rightMouseDown = false;
					break;

					case EVRMouseButton.Middle:
						middleMouseDown = false;
					break;
				}
			break;
		}
	}

	private void SetOpacity(float opacity)
	{
		if(opacity > 1f)
			opacity = 1f;

		if(opacity < 0f)
			opacity = 0f;

		this.opacity = opacity;
	}

	private void SetScale(float scale)
	{
		if(scale > 3f)
			scale = 3f;
		
		if(scale < 0f)
			scale = 0f;

		this.scale = scale;
	}

	// These Methods are for easy value manipulation using Unity UI
	// Use these as events for things.
	public void ToggleEnable()
	{
		gameObject.SetActive(!gameObject.activeSelf);
	}

	public void AddHundrethOpacity()
	{
		SetOpacity(opacity + 0.01f);
	}

	public void SubHundrethOpacity()
	{
		SetOpacity(opacity - 0.01f);
	}

	public void AddTenthScale()
	{
		SetScale(scale + 0.1f);
	}

	public void SubTenthScale()
	{
		SetScale(scale - 0.1f);
	}
}
