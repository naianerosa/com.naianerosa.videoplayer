using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// Video Playlist object be used in the Video Player editor window.
/// </summary>
[CreateAssetMenu(fileName = "VideoPlaylist", menuName = "Video Player/Video Playlist")]
public class VideoPlaylist : ScriptableObject
{
    [SerializeField]
    private string title;
    public string Title => title;

    [SerializeField]
    private VideoClip[] videos;
    public VideoClip[] Videos => videos;
}