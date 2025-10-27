using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ProximityZone : MonoBehaviour
{
    [Tooltip("The target object (sphere) to measure distance to")]
    public Transform targetObject;

    private void Start()
    {
        // Ensure the BoxCollider is set as a trigger
        BoxCollider box = GetComponent<BoxCollider>();
        if (!box.isTrigger)
        {
            Debug.LogWarning("BoxCollider is not set as a trigger. Setting isTrigger = true.");
            box.isTrigger = true;
        }

        if (targetObject == null)
        {
            Debug.LogError("Target object is not assigned!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the proximity zone!");
            LogDistance(other.transform);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LogDistance(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player left the proximity zone!");
        }
    }

    private void LogDistance(Transform playerTransform)
    {
        if (targetObject == null || playerTransform == null) return;

        float distance = Vector3.Distance(playerTransform.position, targetObject.position);
        Debug.Log($"Distance to target: {distance:F2} meters");
    }
}
