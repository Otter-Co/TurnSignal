using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode()]
[RequireComponent(typeof(LineRenderer))]
public class Twister_Arm : MonoBehaviour 
{
	public bool manualMode = false;
	public int pCount = 3;
	public int resolution = 10;

	[Range(0f, 1f)]
	public float progress = 1.0f;
	public bool reverse = false;

	private LineRenderer line;
	private Vector3 [] points = new Vector3[0];
	private Vector3 [] pN = new Vector3[0];

	public GameObject[] pointObjs = new GameObject[0];

	private float lastProgress = 1.0f;
	private bool lastReverse = true;

	void Start() 
	{
		line = GetComponent<LineRenderer>();
	}

	void Update()
	{
		if(line == null)
		{	
			line = GetComponent<LineRenderer>();
			return;
		}

		if(pointObjs.Length != pCount)
		{
			pointObjs = new GameObject[pCount];
			GetPoints();
			lastProgress = -1f;
		}

		if( manualMode && ( points.Length != resolution || 
			lastProgress != progress || 
			lastReverse != reverse ||
			PointMoved()) )
		{
			lastProgress = progress;
			lastReverse = reverse;

			UpdatePoints();
		}
	}

	public bool PointMoved()
	{	
		for(int i = 0; i < pointObjs.Length; i++)
			if(pN[i] != pointObjs[i].transform.localPosition)
				return true;
	
		return false;
	}

	void GetPoints()
	{
		for(int i = 0; i < pCount; i++)
		{
			var gOT = transform.Find("p" + i);
			if(gOT != null)
				pointObjs[i] = gOT.gameObject;
			else
			{
				var gO = new GameObject("p" + i);
				gO.transform.parent = transform;
				gO.transform.localPosition = Vector3.zero;
				gO.transform.localRotation = Quaternion.Euler(Vector3.zero);

				pointObjs[i] = gO;
			}
		}
	}

	public void UpdatePoints(bool skip = false)
	{
		points = new Vector3[resolution];
		pN = new Vector3[pCount];

		for(int i = 0; i < pCount; i++)
			pN[i] = pointObjs[i].transform.localPosition;

		for(int i = 0; i < points.Length; i++)
		{
			float prog = ((float) i) / (float) (points.Length - 1);
			var pnt = BezierN(pN, prog);

			if(i == points.Length - 1)
				pnt = Vector3.Lerp(pN[0], pnt, progress);
			else
				pnt *= progress;

			if(reverse) pnt *= -1;

			points[i] = pnt;
		}
		if(!line)
			line = GetComponent<LineRenderer>();
			
		line.positionCount = resolution;
		line.SetPositions(points);
	}

	Vector3 BezierN(Vector3 [] pN, float prog)
	{
		if(pN.Length == 1)
			return pN[0];

		Vector3[] nextP = new Vector3[pN.Length - 1];

		for(int i = 0; i < pN.Length - 1; i++)
			nextP[i] = Vector3.Lerp(pN[i], pN[i + 1], prog);
		
		return BezierN(nextP, prog);
	}
}
