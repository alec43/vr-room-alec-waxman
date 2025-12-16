using UnityEngine;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(CharacterController))]
public class XRPlayerMovementWithCollision : MonoBehaviour
{
    public float speed = 2f;
    private CharacterController characterController;
    private XROrigin xrOrigin;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        xrOrigin = GetComponentInParent<XROrigin>();
        if (xrOrigin == null)
            xrOrigin = FindObjectOfType<XROrigin>();
    }

    void Update()
    {
        // Get thumbstick input
        Vector2 inputAxis = Vector2.zero;
        var inputDevice = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.LeftHand);
        inputDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out inputAxis);

        // Calculate move vector relative to camera
        Vector3 forward = xrOrigin.Camera.transform.forward;
        Vector3 right = xrOrigin.Camera.transform.right;
        Vector3 move = forward * inputAxis.y + right * inputAxis.x;
        move.y = 0; // remove vertical component to avoid vibration

        // Move the CharacterController
        characterController.Move(move * speed * Time.deltaTime);
    }

    // Called whenever the CharacterController collides with something
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Debug log for every collision
        Debug.Log($"Collided with: {hit.collider.name}");

        // Optional: stop movement when hitting walls
        // Comment out or customize tag check if needed
        if (hit.collider.CompareTag("Wall"))
        {
            Vector3 pushDir = Vector3.ProjectOnPlane(hit.moveDirection, Vector3.up);
            characterController.Move(-pushDir * hit.moveLength);
        }
    }
}
