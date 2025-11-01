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
        Debug.Log("Object has entered");
        if (other.CompareTag("MainCamera"))
        {
            // Debug.Log("Player entered the proximity zone!");
            // LogDistance(other.transform);
            currentState = ProximityState.EnteredZone;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            LogDistance(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            Debug.Log("Player left the proximity zone!");
            currentState = ProximityState.ExitedZone;
        }
    }

    private void LogDistance(Transform playerTransform)
    {
        if (targetObject == null || playerTransform == null) return;

        float distance = Vector3.Distance(playerTransform.position, targetObject.position);
        Debug.Log($"Distance to target: {distance:F2} meters");
    }

    public enum ProximityState
    {
        Idle,
        EnteredZone,
        MeasuringDistanceEnter,
        Triggered,
        MeasuringDistanceExit,
        ExitedZone
    }

    public ProximityState currentState = ProximityState.Idle;

    private void Update()
    {
        Debug.Log(currentState.ToString());
        switch (currentState)
        {
            case ProximityState.Idle:
                // Waiting for player to enter the zone
                break;

            case ProximityState.EnteredZone:
                // Player has entered the zone
                // Debug.Log("Player entered the proximity zone!");
                currentState = ProximityState.MeasuringDistanceEnter;
                break;

            case ProximityState.MeasuringDistanceEnter:
                // LogDistance(gameObject.transform);
                // Debug.Log((Camera.main.transform.position - gameObject.transform.position).magnitude.ToString());
                if ((Camera.main.transform.position - gameObject.transform.position).magnitude < 1f)
                {
                    // If the player is close enough to the target object, transition to Triggered
                    currentState = ProximityState.Triggered;
                }

                // Measure distance while inside the zone
                break;

            case ProximityState.Triggered:
                // Triggered state logic
                // Debug.Log("Unity event triggered!");
                currentState = ProximityState.MeasuringDistanceExit;
                break;

            case ProximityState.MeasuringDistanceExit:
                // LogDistance(gameObject.transform);
                // Debug.Log((Camera.main.transform.position - gameObject.transform.position).magnitude.ToString());
                if ((Camera.main.transform.position - gameObject.transform.position).magnitude > 1f)
                {
                    // If the player is far enough from the target object, transition to ExitedZone
                    // handle triggered exit event here
                    currentState = ProximityState.MeasuringDistanceEnter;
                }

                // Measure distance while exiting the zone
                break;

            case ProximityState.ExitedZone:
                // Player has exited the zone
                // Debug.Log("Player left the inner zone!");
                currentState = ProximityState.Idle;
                break;
        }
        // This method can be used for additional logic if needed
    }

}
