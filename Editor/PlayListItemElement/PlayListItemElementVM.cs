
using UnityEngine;
using UnityEngine.UIElements;


/// <summary>
/// ViewModel for the <see cref="PlayListItemElement"/>.
/// The view model contain all the properties that are binded to the UI elements 
/// and some methods to control the playlist item state.
/// This view model was created as a ScriptableObject to allow for easy experiments in the UI Builder.
/// </summary>
//[CreateAssetMenu(fileName = "VideoPlaylist", menuName = "Video Player/PlayListItemElementVM")]
public class PlayListItemElementVM : ScriptableObject
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