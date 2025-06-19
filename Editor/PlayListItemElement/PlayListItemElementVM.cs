
using UnityEngine;
using UnityEngine.UIElements;


/// <summary>
/// ViewModel for the <see cref="PlayListItemElement"/>.
/// The view model contain all the properties that are binded to the UI elements 
/// and some methods to control the playlist item state.
/// </summary>
public class PlayListItemElementVM
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

    [SerializeField]
    private string videoClipTotalTimeFormatted;
    [SerializeField]
    private double videoClipTotalTime = 0f;
    public double VideoClipTotalTime
    {
        get => videoClipTotalTime;
        set
        {
            videoClipTotalTime = value;
            int minutes = (int)(videoClipTotalTime / 60);
            int seconds = (int)(videoClipTotalTime % 60);
            videoClipTotalTimeFormatted = $"{minutes:D2}:{seconds:D2}";
        }
    }

    [SerializeField]
    private string videoClipCurrentTimeFormatted;

    [SerializeField]
    private double videoClipCurrentTime = 0f;
    public double VideoClipCurrentTime
    {
        get => videoClipCurrentTime;
        set
        {
            videoClipCurrentTime = value;
            int minutes = (int)(videoClipCurrentTime / 60);
            int seconds = (int)(videoClipCurrentTime % 60);
            videoClipCurrentTimeFormatted = $"{minutes:D2}:{seconds:D2}";
        }
    }

    public PlayListItemElementVM()
    {
        this.title = "";
        this.filePath = "";
        this.VideoClipTotalTime = 0f;
        this.VideoClipCurrentTime = 0f;
    }

    public PlayListItemElementVM(string title, string filePath, double videoTotalTime)
    {
        this.title = title;
        this.filePath = filePath;
        this.VideoClipTotalTime = videoTotalTime;
        this.VideoClipCurrentTime = 0f;
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