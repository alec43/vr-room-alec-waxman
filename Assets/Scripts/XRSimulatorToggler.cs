using UnityEngine;

public class XRSimulatorToggler : MonoBehaviour
{
    [Header("Scene References")]
    public GameObject webXRCameraSet;
    public GameObject cameraOffsetGroup;
    public GameObject xrDeviceSimulator; // 👈 add your simulator here
    public GameObject webXRManager; // optional

    private bool isInVR = false;

    private void Awake()
    {
        // Auto-find objects if not assigned
        if (!webXRCameraSet) webXRCameraSet = GameObject.Find("WebXRCameraSet");
        if (!cameraOffsetGroup) cameraOffsetGroup = GameObject.Find("Camera Offset");
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
        // WebXR-based detection
        if (webXRManager)
        {
            var manager = webXRManager.GetComponent("WebXRManager");
            if (manager != null)
            {
                var prop = manager.GetType().GetProperty("XRState");
                if (prop != null)
                {
                    var stateValue = prop.GetValue(manager, null);
                    return stateValue != null && stateValue.ToString() == "VR";
                }
            }
        }

        // Fallback XR detection
#if UNITY_XR_MANAGEMENT
        return UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.activeLoader != null;
#else
        return UnityEngine.XR.XRSettings.isDeviceActive;
#endif
    }

    private void SetCameraMode(bool inVR)
    {
        if (webXRCameraSet) webXRCameraSet.SetActive(inVR);
        if (cameraOffsetGroup) cameraOffsetGroup.SetActive(!inVR);

        // 🧠 Simulator should only be active when NOT in VR
        if (xrDeviceSimulator) xrDeviceSimulator.SetActive(!inVR);

        Debug.Log(inVR
            ? "✅ Entered VR mode — using WebXR camera, disabling simulator."
            : "💻 Exited VR — enabling simulator, using normal camera.");
    }
}
