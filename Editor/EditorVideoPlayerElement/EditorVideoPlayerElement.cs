using System;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// UI Element that controls the main video player actions in the UI
/// </summary>
[assembly: InternalsVisibleTo("VideoPlayer.Editor.Tests")]
[UxmlElement]
public partial class EditorVideoPlayerElement : VisualElement
{
    public delegate void PlayButtonClickHandler(object sender, string filePath);
    public event PlayButtonClickHandler PlayClicked;

    public delegate void ButtonClickHandler(object sender);

    public event ButtonClickHandler PauseClicked;

    public event ButtonClickHandler StopClicked;

    public event ButtonClickHandler MuteUnmuteClicked;

    public delegate void VolumeChangedHandler(object sender, float volume);
    public event VolumeChangedHandler VolumeChanged;

    public delegate void PlaybackSpeedChangedHandler(object sender, float playbackSpeedFactor);
    public event PlaybackSpeedChangedHandler PlaybackSpeedChanged;

    public IMGUIContainer videoDisplay => this.Q<IMGUIContainer>("video-display");

    internal EditorVideoPlayerElementVM ViewModel => this.dataSource as EditorVideoPlayerElementVM;

    private Button play => this.Q<Button>("play-button");
    private Button pause => this.Q<Button>("pause-button");
    private Button stop => this.Q<Button>("stop-button");
    private Button next => this.Q<Button>("next-button");
    private Button prev => this.Q<Button>("prev-button");
    private Button volume => this.Q<Button>("volume-button");
    private Button mute => this.Q<Button>("mute-button");

    private Slider volumeSlider => this.Q<Slider>("volume-slider");

    private ScrollView playListVideosContainer => this.Q<ScrollView>("playlist");

    private TemplateContainer playlistItemTemplate;
    private int currentIndex = 0;

    private DropdownField playbackSpeed => this.Q<DropdownField>("video-speed-dropdown");
    public EditorVideoPlayerElement() { }

    public void Init()
    {
        this.dataSource = new EditorVideoPlayerElementVM();

        this.playlistItemTemplate = this.Q<TemplateContainer>("playlist-item").templateSource.CloneTree();
        this.playListVideosContainer.Clear();

        play.text = "";
        play.Add(new Image
        {
            image = EditorGUIUtility.IconContent(EditorVideoPlayerConstants.PlayButtonIcon).image,
        });
        play.clicked += Play;

        pause.text = "";
        pause.Add(new Image
        {
            image = EditorGUIUtility.IconContent(EditorVideoPlayerConstants.PauseButtonIcon).image,
        });
        pause.clicked += Pause;

        stop.text = "";
        stop.Add(new Image
        {
            image = EditorGUIUtility.IconContent(EditorVideoPlayerConstants.StopButtonIcon).image,
        });
        stop.clicked += Stop;

        next.text = "";
        next.Add(new Image
        {
            image = EditorGUIUtility.IconContent(EditorVideoPlayerConstants.NextButtonIcon).image,
        });
        next.clicked += Next;

        prev.text = "";
        prev.Add(new Image
        {
            image = EditorGUIUtility.IconContent(EditorVideoPlayerConstants.PrevButtonIcon).image,
        });

        prev.clicked += Previous;

        volume.text = "";
        volume.Add(new Image
        {
            image = EditorGUIUtility.IconContent(EditorVideoPlayerConstants.VolumeButtonIcon).image,
        });
        volume.clicked += MuteUnmute;

        mute.text = "";
        mute.Add(new Image
        {
            image = EditorGUIUtility.IconContent(EditorVideoPlayerConstants.MuteButtonIcon).image,
        });
        mute.clicked += MuteUnmute;

        volumeSlider.RegisterValueChangedCallback<float>(VolumeSlider_ValueChanged);

        playbackSpeed.RegisterValueChangedCallback<string>(PlaybackSpeed_ValueChanged);
    }

    private void PlaybackSpeed_ValueChanged(ChangeEvent<string> evt)
    {
        if (float.TryParse(evt.newValue.Replace("x", ""), out float playbackSpeedFactor))
        {
            PlaybackSpeedChanged?.Invoke(this, playbackSpeedFactor);
        }
        else
        {
            Debug.LogError($"Invalid playback speed value: {evt.newValue}");
            return;
        }

    }

    private void VolumeSlider_ValueChanged(ChangeEvent<float> evt)
    {
        VolumeChanged?.Invoke(this, evt.newValue);
    }

    public void LoadPlayList(VideoPlaylist videoPlaylist)
    {        
        this.playListVideosContainer.Clear();

        ViewModel.Init(videoPlaylist);

        if (videoPlaylist != null)
        {
            if (this.playlistItemTemplate == null)
            {
                Debug.LogError("Playlist item template is null, cannot load playlist.");
                return;
            }

            for (int i = 0; i < ViewModel.Videos.Count; i++)
            {
                var videoViewModel = ViewModel.Videos[i];

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
        ViewModel.Videos[currentIndex].Pause();
        ViewModel.Pause();
        StopClicked?.Invoke(this);
    }

    public void Pause()
    {
        ViewModel.Videos[currentIndex].Pause();
        ViewModel.Pause();
        PauseClicked?.Invoke(this);
    }

    public void Next()
    {
        if (ViewModel.Videos == null || ViewModel.Videos.Count == 0) return;
        currentIndex = (currentIndex + 1) % ViewModel.Videos.Count; //Return to 0 if it exceeds the count                                                                      
        Play();

    }

    public void Previous()
    {
        if (ViewModel.Videos == null || ViewModel.Videos.Count == 0) return;
        currentIndex = (currentIndex - 1 + ViewModel.Videos.Count) % ViewModel.Videos.Count; //Return to 0 if it exceeds the count        
        ViewModel.Videos[currentIndex].Play();
        Play();
    }

    public void Play()
    {
        if (ViewModel.Videos == null || ViewModel.Videos.Count == 0)
        {
            PlayClicked?.Invoke(this, "");
        }
        else
        {
            ViewModel.Videos.ForEach(a => a.ResetClipState());

            var videoToPlay = ViewModel.Videos[currentIndex];
            videoToPlay.Play();
            ViewModel.ActiveVideo = videoToPlay;
            ViewModel.Play();
            PlayClicked?.Invoke(this, videoToPlay.FilePath);
        }

    }

    public void MuteUnmute()
    {
        ViewModel.MuteUnMute();
        MuteUnmuteClicked?.Invoke(this);
    }

    public void UpdateActiveVideoTime(double currentTime)
    {
        ViewModel.ActiveVideo.VideoClipCurrentTime = currentTime;
    }


}