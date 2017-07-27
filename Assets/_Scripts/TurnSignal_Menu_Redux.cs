using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TurnSignal_Menu_Redux : MonoBehaviour 
{
	public TurnSignal_Prefs_Handler prefs;

	[Space(10)]

	public Slider scaleSlider;
	public Slider opacitySlider;
	public Slider twistRateSlider;
	public Slider pedalSlider;

	[Space(5)]

	public Toggle startWithVRToggle;
	public Toggle useChapColorToggle;
	public Toggle tieTwistToggle;

	[Space(10)]

	public FloatChangeEvent onScaleChange = new FloatChangeEvent();
	public FloatChangeEvent onOpacityChange = new FloatChangeEvent();
	public FloatChangeEvent onTwistRateChange = new FloatChangeEvent();
	public FloatChangeEvent onPedalsChange = new FloatChangeEvent();

	public BoolChangeEvent onStartWithSteamVRChange = new BoolChangeEvent();
	public BoolChangeEvent onUseChapColorChange = new BoolChangeEvent();
	public BoolChangeEvent onLinkTwistChange = new BoolChangeEvent();

	bool settingValues = false;

	void Start () 
	{
		SetUIValues();
		ForceChange();
	}

	void GetUIValues(bool save = false)
	{
		prefs.Scale = scaleSlider.value;
		prefs.Opacity = opacitySlider.value;
		prefs.TwistRate = (int) twistRateSlider.value;
		prefs.Pedals = (int) pedalSlider.value;

		prefs.Save();			
	}
	void SetUIValues()
	{	
		settingValues = true;

		scaleSlider.value = prefs.Scale;
		opacitySlider.value = prefs.Opacity;
		twistRateSlider.value = prefs.TwistRate;
		pedalSlider.value = prefs.Pedals;

		startWithVRToggle.isOn = prefs.StartWithSteamVR;
		useChapColorToggle.isOn = prefs.UseChaperoneColor;
		tieTwistToggle.isOn = prefs.LinkOpacityWithTwist;

		settingValues = false;
	}

	void CheckChange()
	{
		if(scaleSlider.value != prefs.Scale)
			onScaleChange.Invoke(prefs.Scale);
		
		if(opacitySlider.value != prefs.Opacity)
			onOpacityChange.Invoke(prefs.Opacity);	

		if(twistRateSlider.value != prefs.TwistRate)
			onTwistRateChange.Invoke(prefs.TwistRate);

		if(pedalSlider.value != prefs.Pedals)
			onPedalsChange.Invoke(prefs.Pedals);
	}

	void ForceChange()
	{
		onScaleChange.Invoke(prefs.Scale);
		onOpacityChange.Invoke(prefs.Opacity);
		onTwistRateChange.Invoke(prefs.TwistRate);
		onPedalsChange.Invoke(prefs.Pedals);
	}

	public void OnChange()
	{
		if(settingValues)
			return;

		CheckChange();
		GetUIValues();
	}

	public void ResetSettings()
	{
		prefs.Reset();
		SetUIValues();
		ForceChange();
	}
}

[System.Serializable]
public class FloatChangeEvent : UnityEvent <float>{}


[System.Serializable]
public class BoolChangeEvent : UnityEvent <bool>{}