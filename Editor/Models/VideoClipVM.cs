
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "VideoPlaylist", menuName = "Video Player/VideoClipVM")]
public class VideoClipVM : ScriptableObject
{    
    public string Title;
    public DisplayStyle PlayButtonVisibility;
    public DisplayStyle PauseButtonVisibility;
    public string FilePath;
    public FontStyle TitleFontStyle = FontStyle.Normal;

    public void Pause()
    {
        PlayButtonVisibility = DisplayStyle.Flex;
        PauseButtonVisibility = DisplayStyle.None;
    }

    public void Play()
    {
        PlayButtonVisibility = DisplayStyle.None;
        PauseButtonVisibility = DisplayStyle.Flex;
        TitleFontStyle = FontStyle.Bold;
    }

    public void ResetClipState()
    {
        Pause();
        TitleFontStyle = FontStyle.Normal;
    }
}