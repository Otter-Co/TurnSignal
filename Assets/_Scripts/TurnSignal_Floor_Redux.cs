using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnSignal_Floor_Redux : MonoBehaviour 
{
	public Twister_Redux turnObj;

	[Space(10)]

	public bool autoUpdate = true;
	public bool reversed = false;

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
	}

	void UpdateHMDRotation()
	{
		if(!hmd)
			return;

		if(!vrHandler.connectedToSteam)
			return;

		var rot = vrHandler.poseHandler.GetPosRotation(vrHandler.poseHandler.hmdIndex);
		debugYRot = rot.y;

		if(initialRotation == 0)
		{
			curRotation = initialRotation = rot.y * 360f;
			return;
		}

		lastRotation = curRotation;
		curRotation = rot.y * 360f;

		float diff = curRotation - lastRotation;

		if(diff > 350f || diff < -350f)
			diff = (-1f * curRotation) - lastRotation;

		rawTurns += diff;

		turns = (int) ( ( Mathf.Abs(rawTurns) + turnTolerance ) / 720f );
		turnProgress = Mathf.Abs(rawTurns) / (float) (maxTurns * 720f);

		lastDiff = diff;
	}

	void UpdateTurnObj()
	{
		float raw = rawTurns / (maxTurns * 720f);

		if(reversed)
			raw *= -1f;

		turnObj.twist = raw;
	}
}
