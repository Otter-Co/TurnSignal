using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using Valve.VR;

public partial class TurnSignal_Director
{
    // Methods for Easy UI.
	public void LinkOpacityWithTwist(bool linked)
	{
		twistTied = linked;
	}

	public void SetFloorOverlayDevice(int dev)
	{
		switch(dev)
		{
			case 1:
				floorOverlayDevice = Unity_Overlay.OverlayTrackedDevice.RightHand;
			break;

			case 2:
				floorOverlayDevice = Unity_Overlay.OverlayTrackedDevice.LeftHand;
			break;

			default:
				floorOverlayDevice = Unity_Overlay.OverlayTrackedDevice.None;
			break;
		}

		if(floorOverlay.deviceToTrack != floorOverlayDevice)
			floorOverlay.deviceToTrack = floorOverlayDevice;

		SetFloorOverlayScale(prefs.Scale);
		SetFloorOverlayHeight(prefs.Height);
	}

	public void SetFloorOverlayControllerSide(bool flip)
	{
		flipSides = flip;
		SetFloorOverlayHeight(prefs.Height);
	}

	public void SetFloorOverlayScale(float scale) 
	{
		if(floorOverlayDevice != Unity_Overlay.OverlayTrackedDevice.None)
			if(floorOverlay.widthInMeters != prefs.Scale * floorOverlayHandScale)
				scale *= floorOverlayHandScale;

		if(floorOverlay.widthInMeters != scale)
			floorOverlay.widthInMeters = scale;
	}

	public void SetFloorOverlayHeight(float height) 
	{
		var foT = floorOverlay.transform;

		if(floorOverlayDevice != Unity_Overlay.OverlayTrackedDevice.None)
			if(foT.position.y != floorOverlayHeight * floorOverlayHandScale)
				height *= floorOverlayHandScale;

		if(flipSides)
			height *= -1f;

		if(floorOverlayHeight != height)
			floorOverlayHeight = height;

		var heightV = new Vector3(foT.position.x, height, foT.position.z);
		
		if(foT.position != heightV)
			foT.position = heightV;
	}
}