using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class XRRigBodyMover : MonoBehaviour
{
[Header("Camera / Head")]
    public Transform head; // assign your Camera here

    [Header("Input Actions")]
    public InputActionProperty positionAction; // Vector3
    public InputActionProperty rotationAction; // Quaternion

    [Header("Movement Settings")]
    public float speed = 1f; // multiplier for movement
    public bool ignoreVertical = true; // only move horizontally

    private CharacterController controller;
    private Vector3 lastHeadPos;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        lastHeadPos = Vector3.zero;
    }

    void OnEnable()
    {
        positionAction.action.Enable();
        rotationAction.action.Enable();
        lastHeadPos = positionAction.action.ReadValue<Vector3>();
    }

    void OnDisable()
    {
        positionAction.action.Disable();
        rotationAction.action.Disable();
    }

    void Update()
    {
        // 1️⃣ Read current HMD position & rotation
        Vector3 currentHeadPos = positionAction.action.ReadValue<Vector3>();
        Quaternion currentHeadRot = rotationAction.action.ReadValue<Quaternion>();

        // 2️⃣ Compute HMD delta since last frame
        Vector3 delta = currentHeadPos - lastHeadPos;
        lastHeadPos = currentHeadPos;

        // 3️⃣ Ignore vertical movement if needed
        if (ignoreVertical) delta.y = 0;

        // 4️⃣ Transform delta from **HMD space to rig yaw** so forward/backward is correct
        Vector3 move = Quaternion.Euler(0, transform.eulerAngles.y, 0) * delta * speed;

        // 5️⃣ Move the capsule
        controller.Move(move);

        // 6️⃣ Apply HMD rotation to camera only
        head.localRotation = currentHeadRot;

        // 7️⃣ Keep camera inside capsule
        head.localPosition = Vector3.zero;
    }

    // void OnControllerColliderHit(ControllerColliderHit hit)
    // {
    //     Debug.Log("Collided with: " + hit.collider.name);
    // }
}
