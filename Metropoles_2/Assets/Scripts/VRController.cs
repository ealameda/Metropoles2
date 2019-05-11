using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Provider;
using UnityEngine.XR.WSA;

using Valve.VR;

// NOT ACTUALLY WORKING. 
struct XRInfo 
{
    public InputDevice device;
    public InputFeatureUsage<Vector3> position;
    public InputFeatureUsage<Quaternion> rotation;
}

public class VRController : MonoBehaviour
{
    public GameObject leftHand;
    XRInfo leftInfo;

    public GameObject rightHand;
    XRInfo rightInfo;

    XRInfo headInfo;
    // Start is called before the first frame update
    void Start()
    {
        List<InputDevice> devices = new List<InputDevice>();
        UnityEngine.XR.InputDevices.GetDevices(devices);
        Debug.Log("Printing devices");
        foreach(InputDevice device in devices)
        {
            Debug.Log("Device: "+ device.name);
            XRInfo infoToFill;
            if (device.name.Contains("HMD"))
            {
                infoToFill = headInfo;
            }
            if (device.name.Contains("Left"))
            {
                infoToFill = leftInfo;
            }
            if (device.name.Contains("Right"))
            {
                infoToFill = rightInfo;
            }
            infoToFill.device = device;

            List<InputFeatureUsage> featureUsages = new List<InputFeatureUsage>();
            if (device.TryGetFeatureUsages(featureUsages))
            {
                Debug.Log("Have features");
                foreach(InputFeatureUsage feature in featureUsages)
                {
                    Debug.Log("Feature: " + feature.name);
                    if (feature.name.Contains("Position"))
                        infoToFill.position = feature.As<Vector3>();
                    if (feature.name.Contains("Rotation"))
                        infoToFill.rotation = feature.As<Quaternion>();
                }
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (SteamVR_Input.GetStateDown("/actions/default/in/InteractUI", Valve.VR.SteamVR_Input_Sources.LeftHand))
            Debug.Log("Got a press!");
        // if (Input.GetKey("joystick14"))
        //     Debug.Log("Got a joystick!");
        
    }
}
