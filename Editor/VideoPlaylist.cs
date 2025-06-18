using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// Video Playlist object be used in the Video Player editor window.
/// </summary>
[CreateAssetMenu(fileName = "VideoPlaylist", menuName = "Video Player/Video Playlist")]
public class VideoPlaylist : ScriptableObject
{
    public string Title;
    public VideoClip[] Videos;
}