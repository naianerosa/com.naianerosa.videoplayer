using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "VideoPlaylist", menuName = "Video Player/EditorVideoPlayerElementVM")]
public class EditorVideoPlayerElementVM : ScriptableObject
{
    public string Title;
    public DisplayStyle PlayButtonVisibility;
    public DisplayStyle PauseButtonVisibility;
    public string CurrentVideoTitle;

    public DisplayStyle NoVideosLabelVisibility = DisplayStyle.Flex;
    public DisplayStyle VideoContainerVisibility = DisplayStyle.None;

    public List<PlayListItemElementVM> Videos = new List<PlayListItemElementVM>();

    public void Pause()
    {
        PlayButtonVisibility = DisplayStyle.Flex;
        PauseButtonVisibility = DisplayStyle.None;
    }

    public void Play()
    {
        PlayButtonVisibility = DisplayStyle.None;
        PauseButtonVisibility = DisplayStyle.Flex;
    }

    public void Init(VideoPlaylist playlist)
    {
        Title = playlist.Title;
        PlayButtonVisibility = DisplayStyle.Flex;
        PauseButtonVisibility = DisplayStyle.None;
        NoVideosLabelVisibility = playlist.Videos.Count == 0 ? DisplayStyle.Flex : DisplayStyle.None;
        VideoContainerVisibility = playlist.Videos.Count > 0 ? DisplayStyle.Flex : DisplayStyle.None;

        foreach (var video in playlist.Videos)
        {
            var videoVM = ScriptableObject.CreateInstance<PlayListItemElementVM>();
            videoVM.Title = video.Name;
            videoVM.FilePath = video.FilePath;
            videoVM.ResetClipState();
            Videos.Add(videoVM);
        }

    }
}

