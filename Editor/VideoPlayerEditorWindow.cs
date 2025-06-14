using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;

public class VideoPlayerEditorWindow : EditorWindow
{
    [MenuItem("Window/Video Player")]
    public static void ShowExample()
    {
        VideoPlayerEditorWindow wnd = GetWindow<VideoPlayerEditorWindow>();
        wnd.titleContent = new GUIContent("Video Player");
    }

    private VideoPlaylist playlist;
    private int currentIndex = -1;
    private VideoPlayer player;
    private VisualElement videoContainer;
    private Label currentVideoLabel;
    private Button play;
    private RenderTexture renderTexture;
    private IMGUIContainer videoDisplay;
    private ScrollView playlistContainer;
    private long pausedFrame = -1;
    private Texture playImage;
    private TemplateContainer playlistItemTemplate;
    private VideoPlayerComponent videoPlayerComponent;

    public void CreateGUI()
    {
        var root = rootVisualElement;

        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.naianerosa.videoplayer/Editor/VideoPlayerEditorWindow_v2.uxml");
        //var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.naianerosa.videoplayer/Editor/VideoPlayerEditorWindow.uss");
        //root.styleSheets.Add(styleSheet);
        visualTree.CloneTree(root);

        videoContainer = root.Q<VisualElement>("video-container");
        //currentVideoLabel = root.Q<Label>("current-video-label");

        play = root.Q<Button>("play-button");
        //var playButton = root.Q<Button>("play-button");
        play.text = ""; // Hide text
        //var button = new Button();
        playImage = EditorGUIUtility.IconContent("d_PlayButton@2x").image;
        play.Add(new Image
        {
            image = playImage,
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
        stop.text = ""; // Hide text
        stop.Add(new Image
        {
            image = EditorGUIUtility.IconContent("d_StopButton@2x").image,
        });
        stop.clicked += Stop;

        var next = root.Q<Button>("next-button");
        next.text = ""; // Hide text]
        next.Add(new Image
        {
            image = EditorGUIUtility.IconContent("Animation.NextKey").image,
        });
        next.clicked += Next;

        var prev = root.Q<Button>("prev-button");
        prev.text = ""; // Hide text]
        prev.Add(new Image
        {
            image = EditorGUIUtility.IconContent("Animation.PrevKey").image,
        });

        prev.clicked += Previous;
        playlistContainer = root.Q<ScrollView>("playlist");

        //videoContainer.

       

        // Create RenderTexture and VideoPlayer
        renderTexture = new RenderTexture(512, 288, 0); // 16:9 aspect ratio
        var go = new GameObject("EditorVideoPlayer", typeof(VideoPlayer), typeof(AudioSource));
        go.hideFlags = HideFlags.HideAndDontSave;
        player = go.GetComponent<VideoPlayer>();
        player.playOnAwake = false;
        //player.timeReference = VideoTimeReference.InternalTime;
        //player.audioOutputMode = VideoAudioOutputMode.None;
        AudioSource audioSource = player.gameObject.AddComponent<AudioSource>();
        player.audioOutputMode = VideoAudioOutputMode.AudioSource;
        player.SetTargetAudioSource(0, audioSource);

        player.renderMode = VideoRenderMode.RenderTexture;
        player.targetTexture = renderTexture;
        //player.controlledAudioTrackCount = 1;
        //player.EnableAudioTrack(0, true);
        //player.frameReady += (vp, frameIdx) =>
        //{
        //    //Debug.Log($"Frame Ready: {frameIdx}");
        //    // This is where you could handle frame updates if needed
        //    DrawVideoFrame();
        //};


        //player = new GameObject("EditorVideoPlayer").AddComponent<VideoPlayer>();
        //player.playOnAwake = false;
        //player.audioOutputMode = VideoAudioOutputMode.None;
        //player.renderMode = VideoRenderMode.CameraNearPlane;
        ////videoContainer. .Add(player.GetComponent<VideoPlayer>().GetComponent<VisualElement>());

        // Create IMGUI container to draw the texture
        videoDisplay = root.Q<IMGUIContainer>("video-display"); // new IMGUIContainer(DrawVideoFrame);
        //videoDisplay.style.height = 300;
        //videoDisplay.style.backgroundColor = new Color(0.1f, 0.1f, 0.1f);
        ////var videoContainer = root.Q<VisualElement>("video-container");
        //videoContainer.Add(videoDisplay);

        EditorApplication.update += Repaint;
        videoDisplay.onGUIHandler = DrawVideoFrame;

        root.Q<ObjectField>("playlist_picker").RegisterValueChangedCallback(evt =>
        {
            playlist = evt.newValue as VideoPlaylist;

            videoPlayerComponent.Load(playlist, player);
            videoContainer.dataSource = videoPlayerComponent.PlayListVM;

            currentIndex = 0;
            videoPlayerComponent.StartVideo(currentIndex);
            //UpdateCurrentVideo();
            PopulatePlaylistUI();
        });

        playlistItemTemplate = root.Q<TemplateContainer>("playlist-item");

        videoPlayerComponent = new VideoPlayerComponent();
    }

    private void PopulatePlaylistUI()
    {
        playlistContainer.Clear();

        if (playlist == null || playlist.videoClips == null) return;

        for (int i = 0; i < playlist.videoClips.Count; i++)
        {
            var item = playlistItemTemplate.templateSource.CloneTree();
            item.dataSource = videoPlayerComponent.VideoClips[i];

            var playButton = item.Q<Button>("playlist-item-button-play");
            playButton.text = "";
            playButton.Add(new Image
            {
                image = EditorGUIUtility.IconContent("d_PlayButton").image,
            });
            playButton.userData = i;
            playButton.clicked += () =>
            {
                currentIndex = (int)playButton.userData;
                videoPlayerComponent.PlayVideo(currentIndex);
            };

            var pauseButton = item.Q<Button>("playlist-item-button-pause");
            pauseButton.text = "";
            pauseButton.Add(new Image
            {
                image = EditorGUIUtility.IconContent("d_PauseButton").image,
            });
            pauseButton.userData = i;
            pauseButton.clicked += () =>
            {
                currentIndex = (int)playButton.userData;
                videoPlayerComponent.PauseVideo(currentIndex);
            };

            playlistContainer.Add(item);
        }
    }

    private void UpdateListView()
    {
        var allVideos = playlistContainer.Query<Button>().ToList();
        foreach (var button in allVideos)
        {
            if (button.userData is int index && index == currentIndex)
            {
                button.AddToClassList("current-video-highlight");
            }
            else
            {
                button.RemoveFromClassList("current-video-highlight");
            }
        }
    }

    private void DrawVideoFrame()
    {
        if (renderTexture == null) return;

        //Fit the texture to the container
        var aspect = (float)renderTexture.width / renderTexture.height;
        Debug.Log($"videoDisplay.contentRect.width:{videoDisplay.contentRect.width}");
        float width = videoDisplay.contentRect.width;
        float height = width / aspect;
        //videoContainer.style.height = height;
        //videoContainer.parent.style.height = height;
        Debug.Log($"videoDisplay.contentRect.width:{videoDisplay.contentRect.width}");
        Debug.Log($"height:{height}");


        var rect = GUILayoutUtility.GetRect(width, height, GUILayout.ExpandWidth(false));
        GUI.DrawTexture(rect, renderTexture);
    }

    private void PlayPause()
    {
        if (!player.isPlaying || player.isPaused)
        {

            play.Q<Image>().image = EditorGUIUtility.IconContent("d_PauseButton@2x").image;
            Play();
        }
        else
        {

            play.Q<Image>().image = EditorGUIUtility.IconContent("d_PlayButton@2x").image;
            Pause();
        }
    }

    private void Play()
    {
        //if (player.url == null && !string.IsNullOrEmpty(GetCurrentFilePath()))
        //{

        //    player.url = GetCurrentFilePath();
        //    player.Prepare();
        //}

        videoPlayerComponent.PlayVideo(currentIndex);

        //if (pausedFrame > 0)
        //{
        //    player.frame = pausedFrame;
        //    pausedFrame = -1;
        //}

        //player.Play();
    }

    private void Pause()
    {
        //player.Pause();
        //pausedFrame = player.frame;
        videoPlayerComponent.PauseVideo(currentIndex);
    }
    private void Stop() => videoPlayerComponent.StopVideo(currentIndex);

    private void Next()
    {
        if (playlist == null || playlist.videoClips.Count == 0) return;
        currentIndex = (currentIndex + 1) % playlist.videoClips.Count;
       // UpdateCurrentVideo();
        videoPlayerComponent.StartVideo(currentIndex);
    }

    private void Previous()
    {
        if (playlist == null || playlist.videoClips.Count == 0) return;
        currentIndex = (currentIndex - 1 + playlist.videoClips.Count) % playlist.videoClips.Count;
        //UpdateCurrentVideo();
        videoPlayerComponent.StartVideo(currentIndex);
    }

    private string GetCurrentFilePath()
    {
        if (playlist == null || currentIndex < 0 || currentIndex >= playlist.videoClips.Count)
            return null;
        return playlist.videoClips[currentIndex].filePath;
    }

    //private void UpdateCurrentVideo()
    //{
    //    Stop();
    //    string path = GetCurrentFilePath();
    //    //currentVideoLabel.text = path != null ? System.IO.Path.GetFileName(path) : "No video selected";
    //    if (path != null)
    //    {
    //        player.url = path;
    //        UpdateListView();
    //    }
    //}

    private void OnDisable()
    {
        DestroyImmediate(player?.gameObject);
    }
}