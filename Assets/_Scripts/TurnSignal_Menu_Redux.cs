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
	public Slider twistRateSlider;
	public Slider petalSlider;
	public Slider heightSlider;
	public Slider followSpeedSlider;

	[Space(5)]
	
	public Toggle startWithVRToggle;
	public Toggle hideMainWindowToggle;
	public Toggle followPlayerToggle;
	public Toggle useChapColorToggle;
	public Toggle tieTwistToggle;
	public Toggle onlyShowInDashToggle;

	[Space(5)]

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
		twistRateSlider.value = prefs.TwistRate;
		petalSlider.value = prefs.Petals;
		heightSlider.value = prefs.Height;
		followSpeedSlider.value = prefs.FollowSpeed;


		startWithVRToggle.isOn = prefs.StartWithSteamVR;
		hideMainWindowToggle.isOn = prefs.HideMainWindow;

		followPlayerToggle.isOn = prefs.FollowPlayerHeadset;
		useChapColorToggle.isOn = prefs.UseChaperoneColor;
		tieTwistToggle.isOn = prefs.LinkOpacityWithTwist;

		onlyShowInDashToggle.isOn = prefs.OnlyShowInDashboard;

		disableLinkToggle.isOn = prefs.LinkDevice == TurnSignalPrefsLinkDevice.None;

		rightLinkToggle.isOn = prefs.LinkDevice == TurnSignalPrefsLinkDevice.Right;
		leftLinkToggle.isOn = prefs.LinkDevice == TurnSignalPrefsLinkDevice.Left;

		flipSidesToggle.isOn = prefs.FlipSides;
	}

	public void ResetSettings()
	{
		prefs.Reset();
		SetUIValues();
	}
}