using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnSignal_Floor_Redux : MonoBehaviour 
{
	public Twister_Redux turnObj;

	[Space(10)]

	public bool autoUpdate = true;

	[Space(10)]
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

		if(initialRotation == 0)
		{
			curRotation = initialRotation = hmd.transform.rotation.eulerAngles.y;
			return;
		}

		lastRotation = curRotation;
		curRotation = hmd.transform.rotation.eulerAngles.y;

		float diff = curRotation - lastRotation;

		if(diff > 350f || diff < -350f)
			return;

		rawTurns += diff;

		turns = (int) ( ( Mathf.Abs(rawTurns) + turnTolerance ) / 360f );
		turnProgress = Mathf.Abs(rawTurns) / (float) (maxTurns * 360f);

		lastDiff = diff;
	}

	void UpdateTurnObj()
	{
		turnObj.twist = rawTurns / (maxTurns * 360f);
	}
}
