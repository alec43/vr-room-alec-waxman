using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class HeadDrivenBodyMover : MonoBehaviour
{
    public Transform head; // assign Main Camera here

    private CharacterController controller;
    private Vector3 lastHeadLocalPos;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        lastHeadLocalPos = head.localPosition;
    }

    void LateUpdate()
    {
        Vector3 deltaLocal = head.localPosition - lastHeadLocalPos;
        lastHeadLocalPos = head.localPosition;

        // Convert to world space
        Vector3 deltaWorld = transform.TransformVector(deltaLocal);

        controller.Move(deltaWorld);
    }
}
