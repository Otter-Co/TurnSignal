using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Valve.VR;

public class TurnSignal_Floor_Redux : MonoBehaviour 
{
	public Camera floorRigCamera;
	
	[Space(10)]
	
	public Twister_Redux turnObj;

	[Space(10)]

	public bool autoUpdate = true;
	public bool reversed = false;

	[Space(10)]

	public Unity_Overlay overlay;

	[Space(10)]

	public Unity_SteamVR_Handler vrHandler;
	public GameObject hmd;

	[Space(10)]

	public float turnTolerance = 20f;
	
	[Space(10)]

	public int maxTurns = 10;

	[Space(10)]

	public int turns = 0;
	public float rawTurns = 0f;


	[Space(10)]
	public float lastRotation = 360f;
	public float lastDiff = 0f;


	public float initialRotation = 0f;
	public float curRotation = 0f;

	public float turnProgress = 0f;

	[Space(10)]

	public float debugYRot = 0f;


	// Methods to make UI easier;
	public void ResetRotationTracking()
	{
		rawTurns = 0f;
		lastDiff = 0f;
		initialRotation = 0f;
	}

	public void AddMaxTurn()
	{
		maxTurns += 1;
	}

	public void SubMaxTurn()
	{
		if(maxTurns - 1 > 0)
			maxTurns -= 1;
	}

	public void SetMaxTurns(float t)
	{
		int turns = (int) t;
		maxTurns = turns;
	}

	void Update()
	{
		if(autoUpdate)
			UpdateFloor();
	}

	public void UpdateFloor()
	{
		UpdateHMDRotation();
		UpdateTurnObj();

		if(overlay)
			overlay.UpdateOverlay();
	}

	void UpdateHMDRotation()
	{
		if(!hmd || !vrHandler || !vrHandler.connectedToSteam)
			return;

		HmdMatrix34_t pos = vrHandler.poseHandler.GetRawTrackedMatrix(vrHandler.poseHandler.hmdIndex);

		// Assuming pos[0][2] and pos[2][2] are [((0 * 4) + 2)](2) and [((2 * 4) + 2)](10)
		// This gibberish is referencing HmdMatrix34_t being a float[3][4] in C++, but not Here!
		float y = Mathf.Atan2(pos.m2, pos.m10);

		debugYRot = y;

		if(initialRotation == 0)
		{
			curRotation = initialRotation = PI2Eul(y);

			// Skip the initial frame, saves a bunch of error checking stuff.
			return;
		}

		lastRotation = curRotation;
		curRotation = PI2Eul(y);

		float diff = curRotation - lastRotation;

		if(diff > 350f || diff < -350f)
			diff = (-1f * curRotation) - lastRotation;

		rawTurns += diff;

		turns = (int) ( ( Mathf.Abs(rawTurns) + turnTolerance ) / 720f );
		turnProgress = Mathf.Abs(rawTurns) / (float) (maxTurns * 720f);

		lastDiff = diff;
	}

	float PI2Eul(float pi)
	{
		return -1f * ((pi / Mathf.PI) * 360f);
	}

	void UpdateTurnObj()
	{
		float raw = rawTurns / (maxTurns * 720f);

		if(reversed)
			raw *= -1f;

		turnObj.twist = raw;
	}
}
