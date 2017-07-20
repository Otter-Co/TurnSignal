using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode()]
[RequireComponent(typeof(LineRenderer))]
public class TurnArrow : MonoBehaviour {

	public int resolution = 10;
	private LineRenderer line;

	private Vector3 [] points = new Vector3[0];
	void Start () 
	{
		line = GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(!line)
			line = GetComponent<LineRenderer>();

		if(points.Length != resolution)
		{
			UpdatePoints();
			
			Debug.Log(points.Length);
			line.SetVertexCount(resolution);
			line.SetPositions(points);
			
		}
	}

	void UpdatePoints()
	{
		points = new Vector3[resolution];

		float lx, ly, lz;

		lx = transform.localScale.x;
		ly = transform.localScale.y;
		lz = transform.localScale.z;

		float dia, rad;

		dia = 1f;
		rad = dia * 0.5f;

		float xInset = dia * 0.05f;
		float yInset = rad * (4.0f / 3.0f);

		xInset *= lx;
		yInset *= ly;

		Vector3[] pN = {
			new Vector3(0f, 0f, 0f),
			new Vector3(xInset, 0f, yInset),
			new Vector3(dia - xInset, 0f, yInset),
			new Vector3(dia, 0f)
		};

		for(int i = 0; i < points.Length; i++)
		{
			float prog = ((float) i) / (float) (points.Length - 1);
			points[i] = BezierN(pN, prog);
		}
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
