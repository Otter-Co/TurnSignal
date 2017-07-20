using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnSignal_Floor : MonoBehaviour 
{
	public Twister turnObj;

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

	void Update()
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

		lastDiff = diff;
	}

	void UpdateTurnObj()
	{
		turnObj.twist = rawTurns / (maxTurns * 360f);
	}

	public void ResetRotationTracking()
	{
		rawTurns = 0f;
		lastDiff = 0f;
		initialRotation = 0f;

		Debug.Log("test!");
	}
}
