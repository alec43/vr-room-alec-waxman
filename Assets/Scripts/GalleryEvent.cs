using System;
using UnityEngine;

[Serializable]
public class GalleryEvent
{
    public GalleryEventType eventType;
    public string sourceId;
    public DateTime timestamp;
    public string sessionId;

    public GalleryEvent(GalleryEventType eventType, string sourceId, string sessionId)
    {
        this.sessionId = sessionId;
        this.timestamp = DateTime.Now;
        this.eventType = eventType;
        this.sourceId = sourceId;
    }
}

// public static class GalleryEventTypes
// {
//     public const string ProximityEnter = "ProximityZoneEnter";
//     public const string ProximityExit = "ProximityZoneExit";
//     public const string VideoPlay = "VideoPlay";
//     public const string VideoStop = "VideoStop";
// }

public enum GalleryEventType
{
    PROXIMITY_ZONE_ENTER = 0,
    PROXIMITY_ZONE_EXIT = 1,
    VIDEO_PLAY = 2,
    VIDEO_STOP = 3
}

