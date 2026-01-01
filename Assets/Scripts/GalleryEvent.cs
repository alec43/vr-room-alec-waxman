using System;
using UnityEngine;


[Serializable]
public abstract class GalleryEvent
{
    public EventCategoryType eventCategory;  // "video" or "proximity"
    public string sourceId;
    public string sessionId;
    public DateTime timestamp;      // ISO8601 string for serialization

    protected GalleryEvent(EventCategoryType eventCategory, string sourceId, string sessionId)
    {
        this.eventCategory = eventCategory;
        this.sourceId = sourceId;
        this.sessionId = sessionId;
        this.timestamp = DateTime.Now;
    }

    public override string ToString()
    {
        return $"[{eventCategory}] Source: {sourceId} | Session: {sessionId} | Timestamp: {timestamp}";
    }
}

public enum EventCategoryType
{
    Video = 0,
    Proximity = 1
}


public enum ProximityEventType
{
    Enter = 0,
    Exit = 1,
}

[Serializable]
public class ProximityEvent : GalleryEvent
{
    public ProximityEventType eventType;
    public string zoneId; // Optional: specify which zone triggered the event

    public ProximityEvent(ProximityEventType eventType, string sourceId, string sessionId, string zoneId = null)
        : base(EventCategoryType.Proximity, sourceId, sessionId)
    {
        this.eventType = eventType;
        this.zoneId = zoneId;
    }

    public override string ToString()
    {
        return base.ToString() + $" | ProximityEventType: {eventType} | Zone: {zoneId}";
    }
}



public enum VideoEventType
{
    Play = 0,
    Stop = 1
}

[Serializable]
public class VideoEvent : GalleryEvent
{
    public VideoEventType eventType;
    public string videoName; // Optional: specify which video

    public VideoEvent(VideoEventType eventType, string sourceId, string sessionId, string videoName = null)
        : base(EventCategoryType.Video, sourceId, sessionId)
    {
        this.eventType = eventType;
        this.videoName = videoName;
    }

    public override string ToString()
    {
        return base.ToString() + $" | VideoEventType: {eventType} | Video: {videoName}";
    }
}
