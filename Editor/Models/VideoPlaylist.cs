using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "VideoPlaylist", menuName = "Video Player/Video Playlist")]
public class VideoPlaylist : ScriptableObject
{
    public string title;
    public List<VideoClipEntry> videoClips = new();


    public VideoPlayListVM GetVM()
    {
        return new VideoPlayListVM
        {
            Title = title,
            PlayButtonVisibility = DisplayStyle.Flex,
            PauseButtonVisibility = DisplayStyle.None,
            CurrentVideoTitle = videoClips.Count > 0 ? videoClips[0].name : "No videos in playlist",
            NoVideosLabelVisibility = videoClips.Count == 0 ? DisplayStyle.Flex : DisplayStyle.None,
            VideoContainerVisibility = videoClips.Count > 0 ? DisplayStyle.Flex : DisplayStyle.None
        };
    }
}