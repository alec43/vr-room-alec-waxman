using UnityEngine;

public class XRSimulatorToggler : MonoBehaviour
{
    [Header("Scene References")]
    public GameObject webXRCameraSet;
    public GameObject desktopCamera;   // Your Camera Offset Main Camera
    public GameObject xrDeviceSimulator;
    public GameObject webXRManager;

    private bool isInVR = false;

    private void Awake()
    {
        // Auto-find
        if (!webXRCameraSet) webXRCameraSet = GameObject.Find("WebXRCameraSet");
        if (!desktopCamera) desktopCamera = GameObject.Find("Main Camera");
        if (!xrDeviceSimulator) xrDeviceSimulator = GameObject.Find("XR Device Simulator");
        if (!webXRManager) webXRManager = GameObject.Find("WebXRManager");

        SetCameraMode(false);
    }

    private void Update()
    {
        bool currentlyInVR = IsInVRSession();

        if (currentlyInVR != isInVR)
        {
            isInVR = currentlyInVR;
            SetCameraMode(isInVR);
        }
    }

    private bool IsInVRSession()
    {
        if (webXRManager)
        {
            var manager = webXRManager.GetComponent("WebXRManager");
            if (manager != null)
            {
                var prop = manager.GetType().GetProperty("XRState");
                if (prop != null)
                {
                    var value = prop.GetValue(manager, null);
                    return value != null && value.ToString() == "VR";
                }
            }
        }

        return false;
    }

    private void SetCameraMode(bool inVR)
    {
        // CameraOffset ALWAYS stays active

        webXRCameraSet.SetActive(inVR);
        desktopCamera.SetActive(!inVR);

        if (xrDeviceSimulator) xrDeviceSimulator.SetActive(!inVR);

        Debug.Log(inVR
            ? "Entered VR mode — WebXR camera active."
            : "Desktop mode — simulator and desktop camera active.");
    }
}
