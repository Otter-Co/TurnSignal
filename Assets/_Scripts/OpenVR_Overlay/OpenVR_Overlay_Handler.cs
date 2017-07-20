

using System.Collections;
using UnityEngine;

using System.Runtime.InteropServices;
using Valve.VR;

public class OpenVR_Overlay_Handler : MonoBehaviour 
{
	public OpenVR_Overlay [] overlays;

	private OpenVR_Handler handler;

	private GameObject head;
	private GameObject right;
	private GameObject left;

	void Start()
	{
		handler = OpenVR_Handler.instance;

		head = transform.Find("Head").gameObject;
		right = transform.Find("Right").gameObject;
		left = transform.Find("Left").gameObject;
	}
	
	void Update()
	{
		if(handler != null)
			handler.pose_handler.UpdatePoses();

		var pose = handler.pose_handler;

		var hmdStats = new SteamVR_Utils.RigidTransform(pose.poses[pose.hmdIndex].mDeviceToAbsoluteTracking);

		head.transform.position = hmdStats.pos;
		head.transform.rotation = hmdStats.rot;

		if(pose.rightIndex != OpenVR.k_unTrackedDeviceIndexInvalid)
		{
			var rightStats = new SteamVR_Utils.RigidTransform(pose.poses[pose.rightIndex].mDeviceToAbsoluteTracking);

			right.SetActive(true);

			right.transform.position = rightStats.pos;
			right.transform.rotation = rightStats.rot;
		}
		else if(right.activeSelf)
			right.SetActive(false);

		if(pose.leftIndex != OpenVR.k_unTrackedDeviceIndexInvalid)
		{
			var leftStats = new SteamVR_Utils.RigidTransform(pose.poses[pose.leftIndex].mDeviceToAbsoluteTracking);

			left.SetActive(true);

			left.transform.position = leftStats.pos;
			left.transform.rotation = leftStats.rot;
		}
		else if(left.activeSelf)
			left.SetActive(false);
		

		foreach(var ol in overlays)
			if(ol)
				ol.UpdateOverlay(transform, handler.pose_handler.trackingSpace);
	
	}
}

