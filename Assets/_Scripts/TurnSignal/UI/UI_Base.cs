using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Base : MonoBehaviour 
{
	public Selectable main;
	void Start()
	{
		main = GetComponent<Selectable>();
	}
	public virtual void OnEnter(PointerEventData pD)
	{
		
		if(main)
		{
			ExecuteEvents.Execute(main.gameObject, pD, ExecuteEvents.pointerEnterHandler);
		}
	}

	public virtual void OnExit(PointerEventData pD)
	{
		if(main)
		{
			ExecuteEvents.Execute(main.gameObject, pD, ExecuteEvents.pointerExitHandler);
		}
			
	}

	public virtual void OnSubmit(PointerEventData pD)
	{
		if(main)
		{
			ExecuteEvents.Execute(main.gameObject, pD, ExecuteEvents.submitHandler);
		}
	}

	public virtual void OnDown(PointerEventData pD)
	{
		if(main)
		{
			ExecuteEvents.Execute(main.gameObject, pD, ExecuteEvents.pointerDownHandler);
		}
			
	}

	public virtual void OnUp(PointerEventData pD)
	{
		if(main)
		{
			ExecuteEvents.Execute(main.gameObject, pD, ExecuteEvents.pointerUpHandler);
		}
	}
}
