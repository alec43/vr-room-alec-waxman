using UnityEngine;
using UnityEngine.InputSystem;

public class XRHeadPose : MonoBehaviour
{
    public InputActionProperty positionAction;
    public InputActionProperty rotationAction;

    void OnEnable()
    {
        positionAction.action.Enable();
        rotationAction.action.Enable();
    }

    void OnDisable()
    {
        positionAction.action.Disable();
        rotationAction.action.Disable();
    }

    void LateUpdate()
    {
        transform.localPosition = positionAction.action.ReadValue<Vector3>();
        transform.localRotation = rotationAction.action.ReadValue<Quaternion>();
    }
}
