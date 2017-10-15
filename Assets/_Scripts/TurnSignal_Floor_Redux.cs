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

	public float turnProgress = 0f;


	// Methods to make UI easier;
	public void ResetRotationTracking()
	{
		rawTurns = 0f;
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

		var tp = vrHandler.poseHandler.GetRawTrackedMatrix(vrHandler.poseHandler.hmdIndex);
		Vector3 angVel = new Vector3(tp.vAngularVelocity.v0, tp.vAngularVelocity.v1, tp.vAngularVelocity.v2);

		rawTurns += -(angVel.y * Time.deltaTime);
		turns = (int) (rawTurns / (Mathf.PI * 2f));

		turnProgress = Mathf.Abs(rawTurns / ((Mathf.PI * 2f) * maxTurns));
	}

	float PI2Eul(float pi)
	{
		return -1f * ((pi / Mathf.PI) * 360f);
	}

	void UpdateTurnObj()
	{
		float raw = rawTurns / ((Mathf.PI * 2f) * maxTurns);

		if(reversed)
			raw *= -1f;

		turnObj.twist = raw;
	}
}
