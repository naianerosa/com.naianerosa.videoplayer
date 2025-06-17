using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VideoPlaylist", menuName = "Video Player/Video Playlist")]
public class VideoPlaylist : ScriptableObject
{
    public string Title;
    public List<VideoClipEntry> Videos = new();

}