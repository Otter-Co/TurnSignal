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
	public Slider petalSlider;

	[Space(5)]

	public Toggle startWithVRToggle;
	public Toggle useChapColorToggle;
	public Toggle tieTwistToggle;
	public Toggle onlyShowInDashToggle;
	
	void Start () 
	{
		SetUIValues();
	}

	public void SetUIValues()
	{	
		scaleSlider.value = prefs.Scale;
		opacitySlider.value = prefs.Opacity;
		twistRateSlider.value = prefs.TwistRate;
		petalSlider.value = prefs.Petals;

		startWithVRToggle.isOn = prefs.StartWithSteamVR;
		useChapColorToggle.isOn = prefs.UseChaperoneColor;
		tieTwistToggle.isOn = prefs.LinkOpacityWithTwist;
		onlyShowInDashToggle.isOn = prefs.OnlyShowInDashboard;
	}

	public void ResetSettings()
	{
		prefs.Reset();
		SetUIValues();
	}
}