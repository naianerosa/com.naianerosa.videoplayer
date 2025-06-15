
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "VideoPlaylist", menuName = "Video Player/VideoPlayListVM")]
public class VideoPlayListVM : ScriptableObject
{
    public string Title;
    public DisplayStyle PlayButtonVisibility;
    public DisplayStyle PauseButtonVisibility;
    public string CurrentVideoTitle;

    public DisplayStyle NoVideosLabelVisibility = DisplayStyle.Flex;
    public DisplayStyle VideoContainerVisibility = DisplayStyle.None;

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