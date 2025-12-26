using UnityEngine;
using UnityEngine.InputSystem.XR;

[RequireComponent(typeof(CharacterController))]
public class TrackedPoseDriverWithMove : TrackedPoseDriver
{
    private CharacterController controller;
    private Vector3 lastPosition;
    private bool hasLast;

    protected override void Awake()
    {
        base.Awake();
        controller = GetComponent<CharacterController>();
    }

    protected override void SetLocalTransform(Vector3 newPosition, Quaternion newRotation)
    {
        // --- ROTATION ---
        if (trackingType != TrackingType.PositionOnly)
        {
            transform.localRotation = newRotation;
        }

        // --- POSITION ---
        if (trackingType == TrackingType.RotationOnly)
            return;

        if (!hasLast)
        {
            lastPosition = newPosition;
            hasLast = true;
            return;
        }

        Vector3 delta = newPosition - lastPosition;
        lastPosition = newPosition;

        controller.Move(delta);
    }

    void OnCharacterControllerColliderHit(ControllerColliderHit hit)
    {
        Debug.Log($"Collided with: {hit.collider.name}");
    }
}
