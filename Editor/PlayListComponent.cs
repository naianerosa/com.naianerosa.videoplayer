using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine.UIElements;

[assembly: InternalsVisibleTo("VideoPlayer.Editor.Tests")]
public class PlayListComponent
{
    private VisualElement playlistContainer;
    private IEditorVideoPlayerHandler videoPlayerComponent;
    private List<VideoClipVM> videos = new List<VideoClipVM>();
    private int currentIndex = 0;
    private TemplateContainer playlistItemTemplate;
    private VideoPlayListVM viewModel;

    public PlayListComponent(
        VisualElement root,
        IEditorVideoPlayerHandler videoPlayerComponent)
    {

        this.videoPlayerComponent = videoPlayerComponent;

        this.playlistContainer = root.Q<ScrollView>("playlist");
        this.playlistItemTemplate = playlistContainer.Q<TemplateContainer>("playlist-item");

        ConfigureButtons(root);

    }

    private void ConfigureButtons(VisualElement root)
    {
        var play = root.Q<Button>("play-button");
        play.text = "";
        play.Add(new Image
        {
            image = EditorGUIUtility.IconContent("d_PlayButton@2x").image,
        });
        play.clicked += Play;

        var pause = root.Q<Button>("pause-button");
        pause.text = "";
        pause.Add(new Image
        {
            image = EditorGUIUtility.IconContent("d_PauseButton@2x").image,
        });
        pause.clicked += Pause;

        var stop = root.Q<Button>("stop-button");
        stop.text = "";
        stop.Add(new Image
        {
            image = EditorGUIUtility.IconContent("d_StopButton@2x").image,
        });
        stop.clicked += Stop;

        var next = root.Q<Button>("next-button");
        next.text = "";
        next.Add(new Image
        {
            image = EditorGUIUtility.IconContent("Animation.NextKey").image,
        });
        next.clicked += Next;

        var prev = root.Q<Button>("prev-button");
        prev.text = "";
        prev.Add(new Image
        {
            image = EditorGUIUtility.IconContent("Animation.PrevKey").image,
        });

        prev.clicked += Previous;
    }

    public VideoPlayListVM LoadPlaylist(VideoPlaylist playlist)
    {
        videos.Clear();
        playlistContainer.Clear();
        viewModel = playlist.GetVM();
        playlistContainer.dataSource = viewModel;

        for (int i = 0; i < playlist.videoClips.Count; i++)
        {
            var videoViewModel = playlist.videoClips[i].GetVM();

            var itemRoot = playlistItemTemplate.templateSource.CloneTree();

            itemRoot.dataSource = videoViewModel;

            var playButton = itemRoot.Q<Button>("playlist-item-button-play");
            playButton.text = "";
            playButton.Add(new Image
            {
                image = EditorGUIUtility.IconContent("d_PlayButton").image,
            });

            playButton.userData = i;
            playButton.clicked += () =>
            {
                currentIndex = (int)playButton.userData;
                Play();
            };

            var pauseButton = itemRoot.Q<Button>("playlist-item-button-pause");
            pauseButton.text = "";
            pauseButton.Add(new Image
            {
                image = EditorGUIUtility.IconContent("d_PauseButton").image,
            });

            pauseButton.clicked += () =>
            {
                Pause();
            };

            playlistContainer.Add(itemRoot);
            videos.Add(videoViewModel);
        }

        currentIndex = 0;
        Play();
        return viewModel;
    }

    public void Stop()
    {
        videos[currentIndex].Pause();
        viewModel.Pause();
        videoPlayerComponent.StopVideo();
    }

    public void Pause()
    {
        videos[currentIndex].Pause();
        videoPlayerComponent.Pause();
        viewModel.Pause();
    }

    public void Next()
    {
        if (videos == null || videos.Count == 0) return;
        currentIndex = (currentIndex + 1) % videos.Count; //Return to 0 if it exceeds the count                                                                      
        Play();

    }

    public void Previous()
    {
        if (videos == null || videos.Count == 0) return;
        currentIndex = (currentIndex - 1 + videos.Count) % videos.Count; //Return to 0 if it exceeds the count        
        videos[currentIndex].Play();
        Play();
    }

    public void Play()
    {
        if (videos == null || videos.Count == 0) return;

        videos.ForEach(a => a.ResetClipState());

        var videoToPlay = videos[currentIndex];
        videoToPlay.Play();
        viewModel.CurrentVideoTitle = videoToPlay.Title;
        viewModel.Play();

        videoPlayerComponent.PlayVideo(videoToPlay.FilePath);
    }

    internal List<VideoClipVM> GetCurrentVideosViewModel()
    {
        return videos;
    }

    internal VideoPlayListVM GetPlayListViewModel()
    {
        return viewModel;
    }
}