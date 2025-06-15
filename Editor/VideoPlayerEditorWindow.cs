using Codice.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;

public class VideoPlayerEditorWindow : EditorWindow
{
    [MenuItem("Window/Video Player")]
    public static void NewInstance()
    {
        VideoPlayerEditorWindow wnd = GetWindow<VideoPlayerEditorWindow>();
        wnd.titleContent = new GUIContent("Video Player");
    }

    private EditorVideoPlayerHandler videoPlayerComponent;
    private PlayListComponent playListComponent;

    public void CreateGUI()
    {
        var root = rootVisualElement;

        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.naianerosa.videoplayer/Editor/VideoPlayerEditorWindow_v2.uxml");
        visualTree.CloneTree(root);

        var videoDisplay = root.Q<IMGUIContainer>("video-display");

        videoPlayerComponent = new EditorVideoPlayerHandler(videoDisplay);

        videoDisplay.onGUIHandler = videoPlayerComponent.DrawVideoFrame;

        playListComponent = new PlayListComponent(root, videoPlayerComponent);

        root.Q<ObjectField>("playlist_picker").RegisterValueChangedCallback(evt =>
        {
            var playlist = evt.newValue as VideoPlaylist;
            var playlistVM = playListComponent.LoadPlaylist(playlist);
            root.Q<VisualElement>("root").dataSource = playlistVM;
        });

        //Improve Video frame rate in the editor
        EditorApplication.update += Repaint;
    }

    private void OnDisable()
    {
        videoPlayerComponent.Destroy();
    }
}