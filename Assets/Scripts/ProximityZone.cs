using UnityEngine;
using System.Collections;
using UnityEngine.Video;

[RequireComponent(typeof(BoxCollider))]
public class ProximityZone : MonoBehaviour
{
    [Tooltip("The target object (sphere) to measure distance to")]
    public Transform targetObject;

    // private Coroutine pulseCoroutine;

    public VideoPlayer videoPlayer;

    public GameObject videoQuad; // The animated version

    public GameObject imageQuad;
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

        if (videoQuad != null)
        {
            videoPlayer = videoQuad.GetComponent<VideoPlayer>();
            videoQuad.SetActive(false); // start hidden
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

    // private void StartPulsing()
    // {
    //     if (pulseCoroutine == null && targetObject != null)
    //     {
    //         pulseCoroutine = StartCoroutine(PulseScale(targetObject, 0.8f, 1.2f, 1f));
    //     }
    // }

    // private void StopPulsing()
    // {
    //     if (pulseCoroutine != null)
    //     {
    //         StopCoroutine(pulseCoroutine);
    //         pulseCoroutine = null;
    //         if (targetObject != null)
    //         {
    //             targetObject.localScale = Vector3.one; // Reset scale
    //         }
    //     }
    // }

    private void StartVideo()
    {
        if (videoQuad != null)
        {
            videoQuad.SetActive(true);
            imageQuad.SetActive(false);

            if (videoPlayer != null && !videoPlayer.isPlaying)
                videoPlayer.Play();
        }
    }

    private void StopVideo()
    {
        if (videoQuad != null)
        {
            if (videoPlayer != null && videoPlayer.isPlaying)
                videoPlayer.Stop();

            imageQuad.SetActive(true);
            videoQuad.SetActive(false);
        }
    }


    // Define thresholds (squared)
    float triggerDistance = 1.5f;     // 1 meter
    float exitDistance = 1.5f;      // 1 meter



    private void Update()
    {
        float sqrDistance = (Camera.main.transform.position - targetObject.position).sqrMagnitude;
        float sqrTrigger = triggerDistance * triggerDistance;
        float sqrExit = exitDistance * exitDistance;
        Debug.Log(currentState.ToString());
        switch (currentState)
        {
            case ProximityState.Idle:
                // Waiting for player to enter the zone
                // StopPulsing();
                StopVideo();
                break;

            case ProximityState.EnteredZone:
                // Player has entered the zone
                // Debug.Log("Player entered the proximity zone!");
                currentState = ProximityState.MeasuringDistanceEnter;
                break;

            case ProximityState.MeasuringDistanceEnter:
                // LogDistance(gameObject.transform);
                // Debug.Log((Camera.main.transform.position - gameObject.transform.position).magnitude.ToString());
                // StopPulsing();
                StopVideo();
                // ^ move this to measuring distance exit
                if (sqrDistance < sqrTrigger)
                {
                    // If the player is close enough to the target object, transition to Triggered
                    currentState = ProximityState.Triggered;
                }

                // Measure distance while inside the zone
                break;

            case ProximityState.Triggered:
                // Triggered state logic
                // Debug.Log("Unity event triggered!");
                // StartPulsing();
                StartVideo();
                currentState = ProximityState.MeasuringDistanceExit;
                break;

            case ProximityState.MeasuringDistanceExit:
                // LogDistance(gameObject.transform);
                // Debug.Log((Camera.main.transform.position - gameObject.transform.position).magnitude.ToString());
                if (sqrDistance > sqrExit)
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

    // private IEnumerator PulseScale(Transform obj, float minScale, float maxScale, float speed)
    // {
    //     float t = 0f;
    //     bool growing = true;

    //     while (true)
    //     {
    //         t += Time.deltaTime * speed;
    //         float scale = growing ? Mathf.Lerp(minScale, maxScale, t) : Mathf.Lerp(maxScale, minScale, t);
    //         obj.localScale = Vector3.one * scale;

    //         if (t >= 1f)
    //         {
    //             t = 0f;
    //             growing = !growing;
    //         }

    //         yield return null;
    //     }
    // }

}
