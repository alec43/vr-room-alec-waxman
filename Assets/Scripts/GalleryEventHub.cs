using UnityEngine;

public class GalleryEventHub : MonoBehaviour
{
    public static GalleryEventHub Instance { get; private set; }

    [Header("Gallery Events")]
    public GalleryUnityEvent onGalleryEvent;

    private readonly string sessionId = System.Guid.NewGuid().ToString();

    private void Awake()
    {
        Debug.Log("GalleryEventHub Awake called");
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (onGalleryEvent == null)
            onGalleryEvent = new GalleryUnityEvent();
    }

    public void Emit(GalleryEventType eventType, string sourceId)
    {
        // Debug.Log($"Emitting event: {eventType} from {sourceId}");
        var e = new GalleryEvent(eventType, sourceId, sessionId);
        onGalleryEvent.Invoke(e);
    }
}
