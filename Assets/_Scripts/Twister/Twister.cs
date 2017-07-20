using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode()]
public class Twister : MonoBehaviour 
{
	public GameObject armBase;
	public int armCount = 5;

	[Space(10)]
	public float outerDiameter = 1f;
	public float innerDiameter = 0.2f;

	[Space(10)]

	public float twistOffset = 0.01f;
	
	[Space(10)]
	public float twist = 0f;

	[Space(10)]
	public Twister_Arm [] arms = new Twister_Arm[0];


	private int lastArmCount = 5;
	private float lastOuterDia = 0f;
	private float lastInnerDia = 0f;
	private float lastTwistOff = 0f;

	private float lastTwist = 0f;
	
	void Update () 
	{
		if(armCount < 1)
			armCount = 1;
			
		if(lastArmCount != armCount)
		{
			CreateArms();
			lastArmCount = armCount;
		}

		if( 
			lastTwist != twist || 
			lastOuterDia != outerDiameter || 
			lastInnerDia != innerDiameter || 
			lastTwistOff != twistOffset 
		  )
		{
			
			lastOuterDia = outerDiameter;
			lastInnerDia = innerDiameter;
			lastTwistOff = twistOffset;
			lastTwist = twist;
		}

		UpdateArms();
	}

	void CreateArms()
	{
		var Pchild = transform.GetChild(0);
		Pchild.transform.parent = null;

		while(transform.childCount != 0)
		{
			var child = transform.GetChild(0);
			if(child)
				DestroyImmediate(child.gameObject);
		}

		arms = new Twister_Arm[armCount];

		Pchild.transform.parent = transform;
		Pchild.name = "child_arm";
		arms[0] = Pchild.gameObject.GetComponent<Twister_Arm>();

		for(int i = 1; i < armCount; i++)
		{
			var arm = Instantiate(Pchild, transform.position, transform.rotation);
			arms[i] = arm.GetComponent<Twister_Arm>();

			arm.transform.parent = transform;
			arm.name = "child_arm";
		}
	}

	void UpdateArms()
	{
		for(int i = 0; i < arms.Length; i++)
		{
			Twister_Arm tArm = arms[i];
			Transform arm = arms[i].gameObject.transform;

			float prog = ((float) i) / (float) arms.Length;
			
			arm.localPosition = GetPosOnCircle(Vector3.zero, outerDiameter, prog);
			arm.LookAt(transform);

			GameObject[] pN = tArm.pointObjs;

			pN[0].transform.localPosition = Vector3.zero;
			pN[pN.Length - 1].transform.position = transform.position;

			for(int i2 = 1; i2 < pN.Length - 1; i2++)
			{
				GameObject p = pN[i2];

				if(p == pN[0])
					continue;

				float pProg = ((float) i2) / (float) pN.Length;

				float rad = ((outerDiameter - innerDiameter) * (1f - pProg) ) + innerDiameter;
				rad += (rad * (twistOffset * twist)) * twist;

				float prg = prog + (pProg * twist);

				p.transform.position = GetPosOnCircle(transform.position, rad, prg);
			}

			tArm.UpdatePoints();
		}
	}

	Vector3 GetPosOnCircle(Vector3 origin, float radius, float progress)
	{
		float prog = progress * (Mathf.PI * 2f);

		float cx = origin.x,
			  cy = origin.z;
		
		float x = cx + Mathf.Sin(prog) * radius,
			  y = cy + Mathf.Cos(prog) * radius;

		return new Vector3(x, 0f, y);
	}
}
