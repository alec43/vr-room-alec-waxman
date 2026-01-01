using UnityEngine;
using System.Collections;
using UnityEngine.Video;
using MarksAssets.VideoPlayerWebGL;

[RequireComponent(typeof(BoxCollider))]
public class ProximityZone : MonoBehaviour
{
    [Tooltip("The target object (sphere) to measure distance to")]
    public Transform targetObject;

    public VideoPlayerWebGL videoPlayer;

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

    private IEnumerator InitializeQuad()
    {
        yield return null; // let VideoPlayerWebGL.Start() run
        videoQuad.SetActive(false);
        imageQuad.SetActive(true);
    }


    private void Start()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        if (!box.isTrigger)
        {
            box.isTrigger = true;
        }

        if (videoQuad != null)
        {
            videoPlayer = videoQuad.GetComponent<VideoPlayerWebGL>();
            StartCoroutine(InitializeQuad());
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Object has entered");
        if (other.CompareTag("MainCamera"))
        {
            currentState = ProximityState.EnteredZone;
            GalleryEventHub.Instance.Emit(GalleryEventType.PROXIMITY_ZONE_ENTER, "zone_1");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            // LogDistance(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            Debug.Log("Player left the proximity zone!");
            currentState = ProximityState.ExitedZone;
            GalleryEventHub.Instance.Emit(GalleryEventType.PROXIMITY_ZONE_EXIT, "zone_1");
        }
    }

    private void LogDistance(Transform playerTransform)
    {
        if (targetObject == null || playerTransform == null) return;

        float distance = Vector3.Distance(playerTransform.position, targetObject.position);
        Debug.Log($"Distance to target: {distance:F2} meters");
    }

    private bool videoReady = false;

    public void OnVideoReady()
    {
        videoReady = true;
        Debug.Log("Video READY from WebGL!");
    }

    private bool isPlaying = false;


    private void StartVideo()
    {
        if (!videoReady)
        {
            Debug.Log("Video not ready yet…");
            return;
        }
        if (videoQuad != null)
        {
            videoQuad.SetActive(true);
            imageQuad.SetActive(false);

            videoPlayer?.Play();
            isPlaying = true;
            GalleryEventHub.Instance.Emit(GalleryEventType.VIDEO_PLAY, "video_1");
        }
    }

    private void StopVideo()
    {
        if (videoQuad != null)
        {
            videoPlayer?.Stop();
            if (isPlaying) GalleryEventHub.Instance.Emit(GalleryEventType.VIDEO_STOP, "video_1");
            isPlaying = false;

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
        // Debug.Log(currentState.ToString());
        switch (currentState)
        {
            case ProximityState.Idle:
                // Waiting for player to enter the zone
                // StopPulsing();
                // StopVideo();
                break;

            case ProximityState.EnteredZone:
                // Player has entered the zone
                currentState = ProximityState.MeasuringDistanceEnter;
                break;

            case ProximityState.MeasuringDistanceEnter:
                // StopVideo();
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
                StartVideo();
                currentState = ProximityState.MeasuringDistanceExit;
                break;

            case ProximityState.MeasuringDistanceExit:
                if (sqrDistance > sqrExit)
                {
                    // If the player is far enough from the target object, transition to ExitedZone
                    // handle triggered exit event here
                    StopVideo();
                    currentState = ProximityState.MeasuringDistanceEnter;
                }

                // Measure distance while exiting the zone
                break;

            case ProximityState.ExitedZone:
                // Player has exited the zone
                currentState = ProximityState.Idle;
                break;
        }
    }
}
