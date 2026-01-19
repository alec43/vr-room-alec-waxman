using UnityEngine;
using System.Collections;
using UnityEngine.Video;
using MarksAssets.VideoPlayerWebGL;

[RequireComponent(typeof(BoxCollider))]
public class ProximityZoneMultipleObjects : MonoBehaviour
{
    [Tooltip("The target objects to measure distance to")]
    public Transform[] targetObjects;

    public VideoPlayerWebGL[] videoPlayers;

    public GameObject[] videoQuads; // The animated version

    public GameObject[] imageQuads;

    [Header("Event IDs")]
    [Tooltip("Unique ID for this proximity zone")]
    public string zoneId = "zone_1";

    [Tooltip("Unique IDs for each video in this zone (must match videoPlayers order)")]
    public string[] videoIds;

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
        SetVideoQuadsActive(false);
        SetImageQuadsActive(true);
    }

    private void SetVideoQuadsActive(bool isActive)
    {
        foreach (var videoQuad in videoQuads)
        {
            videoQuad.SetActive(isActive);
        }
    }

    private void SetImageQuadsActive(bool isActive)
    {
        foreach (var imageQuad in imageQuads)
        {
            imageQuad.SetActive(isActive);
        }
    }

    private void PlayAllVideos()
    {
        for (int i = 0; i < videoPlayers.Length; i++)
        {
            videoQuads[i].SetActive(true);
            imageQuads[i].SetActive(false);
            videoPlayers[i]?.Play();
            GalleryEventHub.Instance.Emit.VideoEvent(VideoEventType.Play, videoIds[i]);
        }
    }

    private void StopAllVideos()
    {
        for (int i = 0; i < videoPlayers.Length; i++)
        {
            videoQuads[i].SetActive(false);
            imageQuads[i].SetActive(true);
            videoPlayers[i]?.Stop();
            if (isPlaying) GalleryEventHub.Instance.Emit.VideoEvent(VideoEventType.Stop, videoIds[i]);
        }
    }

    private void Start()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        box.isTrigger = true;

        // Auto-assign VideoPlayerWebGL components if not manually set
        if (videoPlayers == null || videoPlayers.Length == 0)
        {
            videoPlayers = new VideoPlayerWebGL[videoQuads.Length];

            for (int i = 0; i < videoQuads.Length; i++)
            {
                if (videoQuads[i] != null)
                {
                    videoPlayers[i] = videoQuads[i].GetComponent<VideoPlayerWebGL>();
                }
            }
        }

        if (videoIds.Length != videoQuads.Length ||
            videoIds.Length != videoPlayers.Length ||
            videoIds.Length != imageQuads.Length)
        {
            Debug.LogError("Lengths of videoIds, videoQuads, imageQuads, and videoPlayers must all match!");
        }
        StartCoroutine(InitializeQuad());
    }



    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Object has entered");
        if (other.CompareTag("MainCamera"))
        {
            currentState = ProximityState.EnteredZone;
            GalleryEventHub.Instance.Emit.ProximityEvent(ProximityEventType.Enter, zoneId);
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
            GalleryEventHub.Instance.Emit.ProximityEvent(ProximityEventType.Exit, zoneId);
        }
    }

    // private void LogDistance(Transform playerTransform)
    // {
    //     if (targetObject == null || playerTransform == null) return;

    //     float distance = Vector3.Distance(playerTransform.position, targetObject.position);
    //     Debug.Log($"Distance to target: {distance:F2} meters");
    // }

    private bool videosReady = false;

    private int videosReadyCount = 0;

    public void OnVideoReady(int index)
    {
        videosReadyCount++;
        Debug.Log($"Video {index} ready ({videosReadyCount}/{videoPlayers.Length})");

        if (videosReadyCount >= videoPlayers.Length)
        {
            videosReady = true;
            Debug.Log("All videos READY from WebGL!");
            videosReadyCount = 0;
        }
    }


    private bool isPlaying = false;


    private void StartVideo()
    {
        if (!videosReady)
        {
            Debug.Log("Videos not ready yet…");
            return;
        }
        if (videoQuads != null && videoQuads.Length > 0)
        {
            // SetVideoQuadsActive(true);
            // SetImageQuadsActive(false);
            PlayAllVideos();
            isPlaying = true;
        }
    }

    private void StopVideo()
    {
        if (videoQuads != null && videoQuads.Length > 0)
        {
            StopAllVideos();
            // if (isPlaying)
            // {
            //     for (int i = 0; i < videoIds.Length; i++)
            //     {
            //         if (i < videoPlayers.Length)
            //         {
            //             GalleryEventHub.Instance.Emit.VideoEvent(VideoEventType.Stop, videoIds[i]);
            //         }
            //     }
            // }
            isPlaying = false;

            // SetImageQuadsActive(true);
            // SetVideoQuadsActive(false);
        }
    }


    // Define thresholds (squared)
    [Tooltip("Distance to trigger the video (in meters)")]
    public float triggerDistance = 2f;     // 2 meters
    [Tooltip("Distance to stop the video (in meters)")]
    public float exitDistance = 2f;      // 2 meters

    private float GetMinSqrDistance()
    {
        float minSqrDistance = float.MaxValue;
        foreach (var targetObject in targetObjects)
        {
            float currentSqrDistance = (Camera.main.transform.position - targetObject.position).sqrMagnitude;
            if (currentSqrDistance < minSqrDistance)
            {
                minSqrDistance = currentSqrDistance;
            }
        }
        return minSqrDistance;
    }



    private void Update()
    {
        float sqrDistance = GetMinSqrDistance();
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
