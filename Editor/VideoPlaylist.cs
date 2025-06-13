using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VideoPlaylist", menuName = "Video Player/Video Playlist")]
public class VideoPlaylist : ScriptableObject
{
    public string title;
    public List<VideoClipEntry> videoClips = new();
}

[System.Serializable]
public class VideoClipEntry
{
    public string name;
    public string filePath; // Absolute or relative path to the .webm file
}
