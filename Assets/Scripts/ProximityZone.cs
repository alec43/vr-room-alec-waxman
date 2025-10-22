using UnityEngine;

public class ProximityZone : MonoBehaviour
{
    [Tooltip("The target object to measure distance to")]
    public Transform targetObject;

    private Transform player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
            Debug.Log("Player entered the zone!");
            CalculateDistance();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && player != null)
        {
            CalculateDistance();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = null;
            Debug.Log("Player left the zone!");
        }
    }

    private void CalculateDistance()
    {
        if (targetObject == null || player == null) return;

        float distance = Vector3.Distance(player.position, targetObject.position);
        Debug.Log($"Distance to target: {distance:F2} meters");
    }
}
