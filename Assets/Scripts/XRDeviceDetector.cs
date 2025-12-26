using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;


public class XRDeviceDetector : MonoBehaviour
{
    void Start()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevices(devices);


        foreach (var device in devices)
        {
            Debug.Log($"Device found: {device.name} with characteristics: {device.characteristics}");
        }
    }
}
