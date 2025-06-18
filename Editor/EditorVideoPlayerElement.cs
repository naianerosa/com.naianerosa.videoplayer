using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// UI Element that controls the main video player actions in the UI
/// </summary>
[UxmlElement]
public partial class EditorVideoPlayerElement : VisualElement
{
    public delegate void PlayButtonClickHandler(object sender, string filePath);
    public event PlayButtonClickHandler PlayClicked;

    public delegate void ButtonClickHandler(object sender);

    public event ButtonClickHandler PauseClicked;

    public event ButtonClickHandler StopClicked;

    public IMGUIContainer videoDisplay => this.Q<IMGUIContainer>("video-display");

    public EditorVideoPlayerElementVM viewModel => this.dataSource as EditorVideoPlayerElementVM;

    private Button play => this.Q<Button>("play-button");
    private Button pause => this.Q<Button>("pause-button");
    private Button stop => this.Q<Button>("stop-button");
    private Button next => this.Q<Button>("next-button");
    private Button prev => this.Q<Button>("prev-button");

    private ScrollView playListVideosContainer => this.Q<ScrollView>("playlist");

    private TemplateContainer playlistItemTemplate;
    private int currentIndex = 0;

    public EditorVideoPlayerElement() { }

    public void Init()
    {
        this.dataSource = ScriptableObject.CreateInstance<EditorVideoPlayerElementVM>();

        this.playlistItemTemplate = this.Q<TemplateContainer>("playlist-item").templateSource.CloneTree();

        play.text = "";
        play.Add(new Image
        {
            image = EditorGUIUtility.IconContent("d_PlayButton@2x").image,
        });
        play.clicked += Play;

        pause.text = "";
        pause.Add(new Image
        {
            image = EditorGUIUtility.IconContent("d_PauseButton@2x").image,
        });
        pause.clicked += Pause;

        stop.text = "";
        stop.Add(new Image
        {
            image = EditorGUIUtility.IconContent("d_StopButton@2x").image,
        });
        stop.clicked += Stop;

        next.text = "";
        next.Add(new Image
        {
            image = EditorGUIUtility.IconContent("Animation.NextKey").image,
        });
        next.clicked += Next;

        prev.text = "";
        prev.Add(new Image
        {
            image = EditorGUIUtility.IconContent("Animation.PrevKey").image,
        });

        prev.clicked += Previous;
    }

    public void LoadPlayList(VideoPlaylist videoPlaylist)
    {
        this.dataSource = ScriptableObject.CreateInstance<EditorVideoPlayerElementVM>();
        this.playListVideosContainer.Clear();

        if (videoPlaylist != null)
        {
            viewModel.Init(videoPlaylist);

            if (this.playlistItemTemplate == null)
            {
                Debug.LogError("Playlist item template is null, cannot load playlist.");
                return;
            }

            for (int i = 0; i < viewModel.Videos.Count; i++)
            {
                var videoViewModel = viewModel.Videos[i];

                var itemRoot = playlistItemTemplate.templateSource.CloneTree();
                var playListItemElement = itemRoot.Q<PlayListItemElement>();
                playListItemElement.Init(videoViewModel, i);
                playListItemElement.PlayClicked += (sender, itemIndex) =>
                {
                    currentIndex = itemIndex;
                    Play();
                };

                playListItemElement.PauseClicked += (sender, itemIndex) =>
                {
                    Pause();
                };

                playListVideosContainer.Add(itemRoot);
            }
        }
        currentIndex = 0;
        Play();
    }

    public void Stop()
    {
        viewModel.Videos[currentIndex].Pause();
        viewModel.Pause();
        StopClicked?.Invoke(this);
    }

    public void Pause()
    {
        viewModel.Videos[currentIndex].Pause();
        viewModel.Pause();
        PauseClicked?.Invoke(this);
    }

    public void Next()
    {
        if (viewModel.Videos == null || viewModel.Videos.Count == 0) return;
        currentIndex = (currentIndex + 1) % viewModel.Videos.Count; //Return to 0 if it exceeds the count                                                                      
        Play();

    }

    public void Previous()
    {
        if (viewModel.Videos == null || viewModel.Videos.Count == 0) return;
        currentIndex = (currentIndex - 1 + viewModel.Videos.Count) % viewModel.Videos.Count; //Return to 0 if it exceeds the count        
        viewModel.Videos[currentIndex].Play();
        Play();
    }

    public void Play()
    {
        if (viewModel.Videos == null || viewModel.Videos.Count == 0)
        {
            PlayClicked?.Invoke(this, "");
        }
        else
        {
            viewModel.Videos.ForEach(a => a.ResetClipState());

            var videoToPlay = viewModel.Videos[currentIndex];
            videoToPlay.Play();
            viewModel.CurrentVideoTitle = videoToPlay.Title;
            viewModel.Play();
            PlayClicked?.Invoke(this, videoToPlay.FilePath);
        }

    }
}