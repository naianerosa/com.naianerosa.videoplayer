using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;

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
        NoVideosLabelVisibility = playlist.Videos == null || playlist.Videos.Length == 0 ? DisplayStyle.Flex : DisplayStyle.None;
        VideoContainerVisibility = playlist.Videos != null && playlist.Videos.Length > 0 ? DisplayStyle.Flex : DisplayStyle.None;
        Videos.Clear();

        if (playlist.Videos != null)
        {
            foreach (VideoClip video in playlist.Videos)
            {
                var videoVM = ScriptableObject.CreateInstance<PlayListItemElementVM>();
                videoVM.Title = video.name;
                videoVM.FilePath = video.originalPath;
                videoVM.ResetClipState();
                Videos.Add(videoVM);
            }
        }
    }
}

