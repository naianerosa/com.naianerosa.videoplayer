using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "VideoPlaylist", menuName = "Video Player/Video Playlist")]
public class VideoPlaylist : ScriptableObject
{
    public string title;
    public List<VideoClipEntry> videoClips = new();


    public VideoPlayerEditorWindowVM GetVM()
    {
        var viewModel = ScriptableObject.CreateInstance<VideoPlayerEditorWindowVM>();

        viewModel.Title = title;
        viewModel.PlayButtonVisibility = DisplayStyle.Flex;
        viewModel.PauseButtonVisibility = DisplayStyle.None;
        viewModel.CurrentVideoTitle = videoClips.Count > 0 ? videoClips[0].name : "No videos available in this playlist";
        viewModel.NoVideosLabelVisibility = videoClips.Count == 0 ? DisplayStyle.Flex : DisplayStyle.None;
        viewModel.VideoContainerVisibility = videoClips.Count > 0 ? DisplayStyle.Flex : DisplayStyle.None;

        return viewModel;
    }
}