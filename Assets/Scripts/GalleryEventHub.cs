using UnityEngine;

public class GalleryEventHub : MonoBehaviour
{
    public static GalleryEventHub Instance { get; private set; }

    [Header("Gallery Events")]
    public GalleryUnityEvent onGalleryEvent;

    private readonly string sessionId = System.Guid.NewGuid().ToString();

    // This will expose Emit.ProximityEvent() / Emit.VideoEvent()
    public EmitHelper Emit { get; private set; }

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

        // Initialize the helper
        Emit = new EmitHelper(this);
    }

    // Nested helper class for namespaced emits
    public class EmitHelper
    {
        private GalleryEventHub parent;

        public EmitHelper(GalleryEventHub parent)
        {
            this.parent = parent;
        }

        // Emit a proximity event
        public void ProximityEvent(ProximityEventType eventType, string sourceId, string zoneId = null)
        {
            var e = new ProximityEvent(eventType, sourceId, parent.sessionId, zoneId);
            parent.onGalleryEvent.Invoke(e);
        }

        // Emit a video event
        public void VideoEvent(VideoEventType eventType, string sourceId, string videoName = null)
        {
            var e = new VideoEvent(eventType, sourceId, parent.sessionId, videoName);
            parent.onGalleryEvent.Invoke(e);
        }
    }
}
