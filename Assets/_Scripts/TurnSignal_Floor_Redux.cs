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

	public float trackerDist = 0.2f;

	public float maxXBeforeSwitch = 70f;
	public float precision = 0.9f;

	[Space(10)]

	public int maxTurns = 10;

	public float turnProgress = 0f;
	public float rawTurnProgress = 0f;

	[Space(10)]

	public int turns = 0;
	public float rawCombinedTurns = 0f;

	[Space(10)]

	public int angVelTurns = 0;
	public int angPosTurns = 0;

	[Space(5)]

	public float rawAngVelTurns = 0f;
	public float rawAngPosTurns = 0f;

	[Space(10)]

	public float debugAngPos = 0f;
	public float debugAngVel = 0f;

	[Space(10)]

	public float lastDot = 0f;


	private float lastAngPos = 0f;

	private Vector3 lastForward;
	private Quaternion lastRot;


	// Methods to make UI easier;
	public void ResetRotationTracking()
	{
		rawCombinedTurns = 0f;
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

		if(!tp.bPoseIsValid)
			return;

		Vector3 angVelVec = new Vector3(tp.vAngularVelocity.v0, tp.vAngularVelocity.v1, tp.vAngularVelocity.v2);

		Quaternion hmdQ = hmd.transform.rotation;
		Vector3 hmdR = hmd.transform.eulerAngles;

		if(lastRot == null)
			lastRot = hmdQ;

		hmdQ *= Quaternion.Euler(0, hmdR.y, 0);

		lastDot = Quaternion.Dot(hmdQ, lastRot);
		lastRot = hmdQ;

		float angPosY = (-Mathf.Atan2(tp.mDeviceToAbsoluteTracking.m2, tp.mDeviceToAbsoluteTracking.m10) / (Mathf.PI) ) * 180f;


		if(lastAngPos == 0)
			lastAngPos = angPosY;

		var angPosDiff = angPosY - lastAngPos;
		lastAngPos = angPosY;		

		if(Mathf.Abs(angPosDiff) >= 340f)
			angPosDiff += angPosDiff > 0f ? -360f : 360f;

		float angPos = angPosDiff;
		float angVel = ((angVelVec.y * Time.deltaTime) / (Mathf.PI * 2f)) * -360f;

		rawAngPosTurns += (angPosDiff);
		rawAngVelTurns += (angVel);

		angPosTurns = (int) (rawAngPosTurns / 360f);
		angVelTurns = (int) (rawAngVelTurns / 360f);

		if(hmdR.y > maxXBeforeSwitch || hmdR.y < -maxXBeforeSwitch)
			rawCombinedTurns += angVel;
		else
			rawCombinedTurns += angPos;

		rawTurnProgress = (rawCombinedTurns / (360f)) / maxTurns;

		turnProgress = Mathf.Abs(rawTurnProgress);

		turns = (int) ( ( rawCombinedTurns ) / (360f) );

		debugAngPos = angPosDiff;
		debugAngVel = (angVelVec.y);
	}

	float PI2Eul(float pi)
	{
		return ((pi / (Mathf.PI)) * 180f);
	}

	void UpdateTurnObj()
	{
		float raw = rawCombinedTurns / (360f * maxTurns);

		if(reversed)
			raw *= -1f;

		turnObj.twist = raw;
	}
}
