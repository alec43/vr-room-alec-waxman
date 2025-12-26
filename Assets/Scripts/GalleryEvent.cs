using System;
using UnityEngine;

[Serializable]
public class GalleryEvent
{
    public string eventType;
    public string sourceId;
    public float timestamp;

    public GalleryEvent(string eventType, string sourceId)
    {
        this.eventType = eventType;
        this.sourceId = sourceId;
        this.timestamp = Time.time;
    }
}

public static class GalleryEventTypes
{
    public const string ProximityEnter = "ProximityZoneEnter";
    public const string ProximityExit = "ProximityZoneExit";
    public const string VideoPlay = "VideoPlay";
    public const string VideoStop = "VideoStop";
}

