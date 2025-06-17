
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "VideoPlaylist", menuName = "Video Player/VideoPlayerEditorWindowVM")]
public class VideoPlayerEditorWindowVM : ScriptableObject
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
}