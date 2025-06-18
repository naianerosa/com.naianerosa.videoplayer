using UnityEditor;
using UnityEngine.UIElements;

/// <summary>
/// The UI Element that represents a single item in the video playlist.
/// </summary>
[UxmlElement]
public partial class PlayListItemElement : VisualElement
{
    public delegate void ItemButtonClickHandler(object sender, int itemIndex);

    public event ItemButtonClickHandler PlayClicked;

    public event ItemButtonClickHandler PauseClicked;

    private Button playButton => this.Q<Button>("playlist-item-button-play");
    private Button pauseButton => this.Q<Button>("playlist-item-button-pause");

    public void Init(PlayListItemElementVM viewModel, int index)
    {
        this.dataSource = viewModel;

        playButton.text = "";
        playButton.Add(new Image
        {
            image = EditorGUIUtility.IconContent(EditorVideoPlayerConstants.PlayButtonIcon).image,
        });
        playButton.clicked += () =>
        {
            PlayClicked?.Invoke(this, index);
        };

        pauseButton.text = "";
        pauseButton.Add(new Image
        {
            image = EditorGUIUtility.IconContent(EditorVideoPlayerConstants.PlayButtonIcon).image,
        });
        pauseButton.clicked += () =>
        {
            PauseClicked?.Invoke(this, index);
        };
    }

    public PlayListItemElement() { }

}