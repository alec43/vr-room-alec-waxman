using UnityEngine;

public class GalleryEventObserver : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log("GalleryEventObserver Awake");
    }

    private void Start()
    {
        if (GalleryEventHub.Instance == null)
        {
            // Debug.LogError("GalleryEventHub.Instance is null!");
            return;
        }

        GalleryEventHub.Instance.onGalleryEvent.AddListener(HandleEvent);
        // Debug.Log("GalleryEventObserver subscribed to GalleryEventHub");
    }

    private void OnDisable()
    {
        GalleryEventHub.Instance.onGalleryEvent.RemoveListener(HandleEvent);
        Debug.Log("GalleryEventObserver unsubscribed from GalleryEventHub");
    }

    public void HandleEvent(GalleryEvent e)
    {
        Debug.Log(
            $"GalleryEventObserver received event: {e.ToString()}"
        );
    }
}
