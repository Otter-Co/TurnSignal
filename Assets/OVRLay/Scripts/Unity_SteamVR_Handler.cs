using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Unity_SteamVR_Handler : MonoBehaviour 
{
	public float steamVRPollTime = 0.05f;

	[Space(10)]

	public GameObject hmdObject;
	public GameObject rightTrackerObj;
	public GameObject leftTrackerObj;

	[Space(10)]

	public UnityEvent onSteamVRConnect = new UnityEvent();
	public UnityEvent onSteamVRDisconnect = new UnityEvent();


	private OVR_Handler ovrHandler = OVR_Handler.instance;

	private OVR_Overlay_Handler overlayHandler { get { return ovrHandler.overlayHandler; } }
	private OVR_Pose_Handler poseHandler { get { return ovrHandler.poseHandler; } }

	private float lastSteamVRPollTime = 0f;

	void Start()
	{

	}

	void Update () 
	{
		if(!SteamVRStartup())
			return;

		ovrHandler.UpdateAll();

		if(hmdObject)
			poseHandler.SetTransformToTrackedDevice(hmdObject.transform, poseHandler.hmdIndex);

		if(poseHandler.rightActive && rightTrackerObj)
		{
			rightTrackerObj.SetActive(true);
			poseHandler.SetTransformToTrackedDevice(rightTrackerObj.transform, poseHandler.rightIndex);
		}
		else if(rightTrackerObj)
			rightTrackerObj.SetActive(false);
		
		if(poseHandler.leftActive && leftTrackerObj)
		{
			leftTrackerObj.SetActive(true);
			poseHandler.SetTransformToTrackedDevice(leftTrackerObj.transform, poseHandler.leftIndex);
		}
		else if(leftTrackerObj)
			leftTrackerObj.SetActive(false);
	}

	bool SteamVRStartup()
	{
		lastSteamVRPollTime += Time.deltaTime;

		if(ovrHandler.OpenVRConnected)
			return true;
		else if(lastSteamVRPollTime >= steamVRPollTime)
		{
			lastSteamVRPollTime = 0f;

			Debug.Log("Starting Up SteamVR Connection...");

			if( !ovrHandler.StartupOpenVR() )
			{
				Debug.Log("Connection Failed :( !");
				return false;
			}
			else
			{
				Debug.Log("Connected to SteamVR!");
				
				onSteamVRConnect.Invoke();

				return true;
			}		
		}
		else
			return false;
	}
	void OnApplicationQuit()
	{
		if(ovrHandler.OpenVRConnected)
			ovrHandler.ShutDownOpenVR();
	}
}
