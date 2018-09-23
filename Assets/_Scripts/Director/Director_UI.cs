using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;
using OVRLay;

public partial class Old_Director
{
    // Methods for Easy UI.
    public void OnShowWindow() => menuRig.hideMainWindowToggle.isOn = false;
    public void LinkOpacityWithTwist(bool linked) => twistTied = linked;
    public void SetFloorOverlayControllerSide(bool flip) => flipSides = flip;
    public void SetFloorOverlayFollowSpeed(float speed) => floorOverlayFollowSpeedRatio = speed;

    public void SetHideMainWindow(bool hideWin)
    {
        if (hideWin)
        {
            // menuOverlay.dontForceRenderTexCam = false;
            menuRig.menuRigCamera.enabled = false;

            winC.HideUnityWindow();
            winC.ShowTrayIcon();
        }
        else
        {
            // menuOverlay.dontForceRenderTexCam = true;
            menuRig.menuRigCamera.enabled = true;

            winC.ShowUnityWindow();
            winC.HideTrayIcon();
        }
    }

    public void SetFloorOverlayDevice(int dev)
    {
        // switch (dev)
        // {
        //     case 1:
        //         floorOverlayDevice = Unity_Overlay.OverlayTrackedDevice.RightHand;
        //         break;

        //     case 2:
        //         floorOverlayDevice = Unity_Overlay.OverlayTrackedDevice.LeftHand;
        //         break;

        //     default:
        //         floorOverlayDevice = Unity_Overlay.OverlayTrackedDevice.None;
        //         break;
        // }

        // // if (floorOverlay.deviceToTrack != floorOverlayDevice)
        // //     floorOverlay.deviceToTrack = floorOverlayDevice;

        SetFloorOverlayScale(prefs.Scale);
        SetFloorOverlayHeight(prefs.Height);
    }



    public void SetFloorOverlayScale(float scale)
    {
        // if (floorOverlayDevice != Unity_Overlay.OverlayTrackedDevice.None)
        //     if (floorOverlay.settings.WidthInMeters != prefs.Scale * floorOverlayHandScale)
        //         scale *= floorOverlayHandScale;

        if (floorOverlay.settings.WidthInMeters != scale)
            floorOverlay.settings.WidthInMeters = scale;
    }

    public void SetFloorOverlayHeight(float height)
    {
        var foT = floorOverlay.transform;

        // if (floorOverlayDevice != Unity_Overlay.OverlayTrackedDevice.None)
        //     height *= floorOverlayHandScale;

        // if (floorOverlayDevice != Unity_Overlay.OverlayTrackedDevice.None && flipSides)
        //     height *= -1f;

        var heightV = new Vector3(foT.position.x, height, foT.position.z);
        foT.position = heightV;

        floorOverlayHeight = height;
    }

    public bool GetManifestAutoLaunch() =>
        (OVR.Applications != null) ? OVR.Applications.GetApplicationAutoLaunch(appKey) : false;

    public void SetManifestAutoLaunch(bool autoLaunch)
    {
        if (OVR.Applications != null)
            OVR.Applications.SetApplicationAutoLaunch(appKey, autoLaunch);
    }
}