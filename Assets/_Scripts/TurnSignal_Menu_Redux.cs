using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TurnSignal_Menu_Redux : MonoBehaviour 
{
	public Camera menuRigCamera;

	[Space(10)]
	
	public TurnSignal_Prefs_Handler prefs;

	[Space(10)]

	public Slider scaleSlider;
	public Slider opacitySlider;
	public Slider heightSlider;
	public Slider twistRateSlider;
	public Slider petalSlider;

	[Space(5)]
	
	public Toggle startWithVRToggle;
	public Toggle hideMainWindowToggle;
	public Toggle useChapColorToggle;
	public Toggle tieTwistToggle;
	public Toggle onlyShowInDashToggle;

	[Space(10)]

	public Toggle disableLinkToggle;
	public Toggle rightLinkToggle;
	public Toggle leftLinkToggle;
	public Toggle flipSidesToggle;

	public void SteamStart()
	{

		if(!prefs.Load())			
			Debug.Log("Bad Settings Load!");

		SetUIValues();
	}

	public void SetUIValues()
	{	
		scaleSlider.value = prefs.Scale;
		opacitySlider.value = prefs.Opacity;
		heightSlider.value = prefs.Height;
		twistRateSlider.value = prefs.TwistRate;
		petalSlider.value = prefs.Petals;

		hideMainWindowToggle.isOn = prefs.HideMainWindow;

		startWithVRToggle.isOn = prefs.StartWithSteamVR;
		useChapColorToggle.isOn = prefs.UseChaperoneColor;
		tieTwistToggle.isOn = prefs.LinkOpacityWithTwist;
		onlyShowInDashToggle.isOn = prefs.OnlyShowInDashboard;

		disableLinkToggle.isOn = prefs.LinkDevice != 1 && prefs.LinkDevice != 2;

		rightLinkToggle.isOn = prefs.LinkDevice == 1;
		leftLinkToggle.isOn = prefs.LinkDevice == 2;	

		flipSidesToggle.isOn = prefs.FlipSides;
	}

	public void ResetSettings()
	{
		prefs.Reset();
		SetUIValues();
	}
}