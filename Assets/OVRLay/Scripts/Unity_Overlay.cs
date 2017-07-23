using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unity_Overlay : MonoBehaviour 
{
	[Space(10)]
	public string overlayName = "Unity Overlay";
	public string overlayKey = "unity_overlay";

	[Space(10)]

	public Texture overlayTexture;

	[Space(10)]
	public float widthInMeters = 1.0f;
	[Range(0f, 1f)]
	public float opacity = 1.0f;


	

	[Space(10)]

	private OVR_Overlay overlay;

	void Start () 
	{
		overlay = new OVR_Overlay();

		overlay.overlayName = overlayName;
		overlay.overlayKey = overlayKey;
	}

	void OnEnable()
	{
		overlay.HideOverlay();
	}

	void OnDisable()
	{
		overlay.ShowOverlay();
	}
	
	
	void Update() 
	{
		
	}
}
