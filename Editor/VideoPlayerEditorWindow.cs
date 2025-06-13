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

    private RenderTexture renderTexture;
    private IMGUIContainer videoDisplay;
    private ScrollView playlistScrollView;

    public void CreateGUI()
    {
        var root = rootVisualElement;

        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.naianerosa.videoplayer/Editor/VideoPlayerEditorWindow.uxml");
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.naianerosa.videoplayer/Editor/VideoPlayerEditorWindow.uss");
        root.styleSheets.Add(styleSheet);
        visualTree.CloneTree(root);

        videoContainer = root.Q<VisualElement>("video-container");
        currentVideoLabel = root.Q<Label>("current-video-label");

        root.Q<Button>("play-button").clicked += Play;
        root.Q<Button>("pause-button").clicked += Pause;
        root.Q<Button>("stop-button").clicked += Stop;
        root.Q<Button>("next-button").clicked += Next;
        root.Q<Button>("prev-button").clicked += Previous;
        playlistScrollView = root.Q<ScrollView>("playlist-scroll-view");


        root.Q<ObjectField>("playlist-object-field").RegisterValueChangedCallback(evt =>
        {
            playlist = evt.newValue as VideoPlaylist;
            currentIndex = 0;
            UpdateCurrentVideo();
            PopulatePlaylistUI();
        });

        // Create RenderTexture and VideoPlayer
        renderTexture = new RenderTexture(512, 288, 0); // 16:9 aspect ratio
        player = new GameObject("EditorVideoPlayer", typeof(VideoPlayer)).GetComponent<VideoPlayer>();
        player.playOnAwake = false;
        //player.audioOutputMode = VideoAudioOutputMode.None;
        AudioSource audioSource = player.gameObject.AddComponent<AudioSource>();
        player.audioOutputMode = VideoAudioOutputMode.AudioSource;
        player.SetTargetAudioSource(0, audioSource);

        player.renderMode = VideoRenderMode.RenderTexture;
        player.targetTexture = renderTexture;
        player.controlledAudioTrackCount = 1;
        player.EnableAudioTrack(0, true);


        //player = new GameObject("EditorVideoPlayer").AddComponent<VideoPlayer>();
        //player.playOnAwake = false;
        //player.audioOutputMode = VideoAudioOutputMode.None;
        //player.renderMode = VideoRenderMode.CameraNearPlane;
        ////videoContainer. .Add(player.GetComponent<VideoPlayer>().GetComponent<VisualElement>());

        // Create IMGUI container to draw the texture
        videoDisplay = new IMGUIContainer(DrawVideoFrame);
        videoDisplay.style.height = 300;
        videoDisplay.style.backgroundColor = new Color(0.1f, 0.1f, 0.1f);
        root.Q<VisualElement>("video-container").Add(videoDisplay);

        EditorApplication.update += Repaint;
        videoDisplay.onGUIHandler = DrawVideoFrame;
    }

    private void PopulatePlaylistUI()
    {
        playlistScrollView.Clear();

        if (playlist == null || playlist.videoClips == null) return;

        for (int i = 0; i < playlist.videoClips.Count; i++)
        {
            var button = new Button();
            button.text = playlist.videoClips[i].name;
            button.userData = i;

            button.clicked += () =>
            {                
                currentIndex = (int)button.userData;
                UpdateCurrentVideo();
                Play();
                
            };

            playlistScrollView.Add(button);
        }
    }

    private void UpdateListView()
    {
        var allVideos = playlistScrollView.Query<Button>().ToList();
        foreach (var button in allVideos)
        {
            if(button.userData is int index && index == currentIndex)
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
        //if (renderTexture != null)
        //{
        //    var rect = GUILayoutUtility.GetRect(renderTexture.width, renderTexture.height, GUILayout.ExpandWidth(false));
        //    EditorGUI.DrawPreviewTexture(rect, renderTexture);
        //}

        if (renderTexture == null) return;

        var aspect = (float)renderTexture.width / renderTexture.height;
        float width = videoDisplay.contentRect.width;
        float height = width / aspect;

        var rect = GUILayoutUtility.GetRect(width, height, GUILayout.ExpandWidth(false));
        EditorGUI.DrawPreviewTexture(rect, renderTexture);
    }

    private void Play()
    {
        if (player.clip == null && !string.IsNullOrEmpty(GetCurrentFilePath()))
        {
            player.url = GetCurrentFilePath();
        }
        player.Play();
    }

    private void Pause() => player.Pause();
    private void Stop() => player.Stop();

    private void Next()
    {
        if (playlist == null || playlist.videoClips.Count == 0) return;
        currentIndex = (currentIndex + 1) % playlist.videoClips.Count;
        UpdateCurrentVideo();
    }

    private void Previous()
    {
        if (playlist == null || playlist.videoClips.Count == 0) return;
        currentIndex = (currentIndex - 1 + playlist.videoClips.Count) % playlist.videoClips.Count;
        UpdateCurrentVideo();
    }

    private string GetCurrentFilePath()
    {
        if (playlist == null || currentIndex < 0 || currentIndex >= playlist.videoClips.Count)
            return null;
        return playlist.videoClips[currentIndex].filePath;
    }

    private void UpdateCurrentVideo()
    {
        Stop();
        string path = GetCurrentFilePath();
        currentVideoLabel.text = path != null ? System.IO.Path.GetFileName(path) : "No video selected";
        if (path != null)
        {
            player.url = path;
            UpdateListView();
        }
    }

    private void OnDestroy()
    {
        DestroyImmediate(player.gameObject);
    }
}