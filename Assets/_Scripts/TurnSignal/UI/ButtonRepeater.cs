using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class ButtonRepeater : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	public bool shouldRepeat { get { return _pointerData != null; } }
	public Button button = null;
	public float rateLimit = 0.1f;
	private PointerEventData _pointerData = null;
	private float lastFire = 0f;

	private void Update()
	{
		if(shouldRepeat && (lastFire += Time.deltaTime) > rateLimit)
		{
			button.OnSubmit(_pointerData);
			lastFire = 0f;
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		_pointerData = eventData;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		_pointerData = null;
	}
}