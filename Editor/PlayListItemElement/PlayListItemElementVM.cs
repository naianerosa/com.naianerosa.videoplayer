
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
    [SerializeField]
    private string title;
    public string Title => title;

    [SerializeField]
    private DisplayStyle playButtonVisibility;
    public DisplayStyle PlayButtonVisibility => playButtonVisibility;

    [SerializeField]
    private DisplayStyle pauseButtonVisibility;
    public DisplayStyle PauseButtonVisibility => pauseButtonVisibility;

    [SerializeField]
    private string filePath;
    public string FilePath => filePath;

    [SerializeField]
    private FontStyle titleFontStyle = FontStyle.Normal;
    public FontStyle TitleFontStyle => titleFontStyle;

    public void Initialize(string title, string filePath)
    {
        this.title = title;
        this.filePath = filePath;
        ResetClipState();
    }

    public void Pause()
    {
        playButtonVisibility = DisplayStyle.Flex;
        pauseButtonVisibility = DisplayStyle.None;
    }

    public void Play()
    {
        playButtonVisibility = DisplayStyle.None;
        pauseButtonVisibility = DisplayStyle.Flex;
        titleFontStyle = FontStyle.Bold;
    }

    public void ResetClipState()
    {
        Pause();
        titleFontStyle = FontStyle.Normal;
    }
}