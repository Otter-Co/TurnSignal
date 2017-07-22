using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TurnSignal_Menu : MonoBehaviour 
{
	public Camera cam;
	public OpenVR_Overlay overlay;
	public GraphicRaycaster gRay;
	
	[Space(10)]
	public OpenVR_Overlay floorOverlay;
	public TurnSignal_Floor floorRig;


	[Space(10)]
	public Text opacityText;
	public Text scaleText;
	public Text turnText;


	private Vector2 lastMouse = Vector2.zero;
	private Vector3 mouseScreenCoords = new Vector3(0f, 0f, 0f);
	private PointerEventData pD;

	private HashSet<UI_Base> enterTargets = new HashSet<UI_Base>();
	private HashSet<UI_Base> downTargets = new HashSet<UI_Base>();

	private float leftMouseDragTime = 0f;
	private bool inDrag = false;

	private float lastOpat = 0f;
	private float lastScale = 0f;

	private int lastMaxTurns = 0;

	void Start () 
	{
		pD = new PointerEventData(EventSystem.current);

		float opacity = PlayerPrefs.GetFloat("opacity", 0.03f);
		float scale = PlayerPrefs.GetFloat("scale", 2f);
		int maxTurns = PlayerPrefs.GetInt("maxturns", 10);

		floorOverlay.opacity = opacity;
		floorOverlay.scale = scale;
		floorRig.maxTurns = maxTurns;
	}
	void OnApplicationQuit()
	{
		PlayerPrefs.SetFloat("opacity", floorOverlay.opacity);
		PlayerPrefs.SetFloat("scale", floorOverlay.scale);
		PlayerPrefs.SetInt("maxturns", floorRig.maxTurns);
	}
	
	void Update()
	{
		if(lastOpat != floorOverlay.opacity)
		{
			opacityText.text = (int) (floorOverlay.opacity * 100f) + "%";
			lastOpat = floorOverlay.opacity;
		}
		
		if(lastScale != floorOverlay.scale)
		{
			scaleText.text = (floorOverlay.scale).ToString("N1");
			lastScale = floorOverlay.scale;
		}

		if(lastMaxTurns != floorRig.maxTurns)
		{
			turnText.text = "" + floorRig.maxTurns;
			lastMaxTurns = floorRig.maxTurns;
		}

		if(lastMouse != overlay.mousePos)
		{
			Vector2 m = overlay.mousePos;

			float mx = m.x * cam.pixelWidth;
			float my = (1f - m.y) * cam.pixelHeight;

			mouseScreenCoords.x = mx;
			mouseScreenCoords.y = my;

			lastMouse = overlay.mousePos;
		}

		if( overlay.mousePos.x < 0f || 
			overlay.mousePos.x > 1 || 
			overlay.mousePos.y < 0f || 
			overlay.mousePos.y > 1 )
		{
			return;
		}

		// Reuse pointer event data object, no need for new mem every frame.
		pD.Reset();
		// Ignore this in lew (lou? liu?) of above
		pD.position = new Vector2(mouseScreenCoords.x, mouseScreenCoords.y);		
		pD.button = PointerEventData.InputButton.Left;
		pD.clickCount = 1;
		pD.dragging = inDrag;

		var nTargs = GetUITargets();

		EnterTargets(nTargs);

		foreach(UI_Base ub in nTargs)
			if(enterTargets.Contains(ub))
				enterTargets.Remove(ub);

		ExitTargets(enterTargets);
		enterTargets = nTargs;
		
		if(overlay.leftMouseDown)
		{
			leftMouseDragTime += Time.deltaTime;

			if(!inDrag)
			{
				foreach(UI_Base ub in nTargs)
					downTargets.Add(ub);

				SubmitTargets(downTargets);
				DownTargets(downTargets);
			}
			else
				DownTargets(downTargets);
	
			inDrag = true;
		}
		else 
		{
			UpTargets(downTargets);
			downTargets.Clear();

			if(inDrag)
				inDrag = false;

			if(leftMouseDragTime > 0f)
				leftMouseDragTime = 0f;
		}
	}

	HashSet<UI_Base> GetUITargets()
	{
		var ray = cam.ScreenPointToRay(mouseScreenCoords);
		Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);

		List<RaycastResult> hits = new List<RaycastResult>();

		gRay.Raycast(pD, hits);

		HashSet<UI_Base> uibT = new HashSet<UI_Base>();

		for(int i = 0; i < hits.Count; i++)
		{
			var go = hits[i].gameObject;

			UI_Base u = GOGetter(go);
			
			uibT.Add(u);
		}
		
		return uibT;
	}
	UI_Base GOGetter(GameObject go, bool tryPar = false)
	{
		UI_Base u = go.GetComponent<UI_Base>();
		Selectable sel = go.GetComponent<Selectable>();

		if(!sel)
			if(!tryPar)
			{
				UI_Base t = GOGetter(go.transform.parent.gameObject, true);
				if(t)
					return t;
			}

		if(!u)
		{
			u = go.AddComponent<UI_Base>();
			u.main = sel;
		}

		return u;
	}

	void EnterTargets(HashSet<UI_Base> t)
	{
		foreach(UI_Base b in t)
			b.OnEnter(pD);
	}

	void ExitTargets(HashSet<UI_Base> t)
	{
		foreach(UI_Base b in t)
			b.OnExit(pD);
	}

	void SubmitTargets(HashSet<UI_Base> t)
	{
		foreach(UI_Base b in t) 
			b.OnSubmit(pD);
	}

	void DownTargets(HashSet<UI_Base> t)
	{
		foreach(UI_Base b in t) 
			b.OnDown(pD);
	}

	void UpTargets(HashSet<UI_Base> t)
	{
		foreach(UI_Base b in t)
			b.OnUp(pD);
	}
}

