using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;

public class VideoPlayerEditorWindowTests
{
    private VideoPlayerEditorWindow window;

    [SetUp]
    public void Setup()
    {
        VideoPlayerEditorWindow.NewWindow();
        window = EditorWindow.GetWindow<VideoPlayerEditorWindow>();
    }

    [TearDown]
    public void TearDown()
    {
        if (window != null)
        {
            window.Close();
        }
    }

    [Test]
    public void NewWindow_CreatesWindowWithCorrectTitle()
    {

        Assert.That(window.titleContent.text, Is.EqualTo("Video Player"));
    }

    [Test]
    public void CreateGUI_ContainsRequiredComponents()
    {

        var root = window.rootVisualElement;
        var videoDisplay = root.Q<IMGUIContainer>("video-display");
        var playlistPicker = root.Q<ObjectField>("playlist_picker");

        Assert.That(videoDisplay, Is.Not.Null);
        Assert.That(playlistPicker, Is.Not.Null);
    }

    [Test]
    public void OnDisable_CleansUpVideoPlayerComponent()
    {
        window.Close();
        var videoPlayerGO = GameObject.Find("EditorVideoPlayer");
        Assert.That(videoPlayerGO, Is.Null, $"Video Player GameObject should be destroyed on window close.");
    }

    [Test]
    public void PlaylistPicker_LoadsPlaylistWhenValueChanged()
    {
        var root = window.rootVisualElement;
        var playlistPicker = root.Q<ObjectField>("playlist_picker");
        var rootElement = root.Q<VisualElement>("root");

        //Sets the first playlist to the picker
        var mockPlaylist1 = ScriptableObject.CreateInstance<VideoPlaylist>();
        mockPlaylist1.title = "Mock Playlist 1";
        playlistPicker.value = mockPlaylist1;

        // Verify root element has data source set and title matches
        Assert.That(rootElement.dataSource, Is.Not.Null);
        Assert.AreEqual(((VideoPlayListVM)(rootElement.dataSource)).name, mockPlaylist1.GetVM().name);

        //Sets the second playlist to the picker
        var mockPlaylist2 = ScriptableObject.CreateInstance<VideoPlaylist>();
        mockPlaylist2.title = "Mock Playlist 2";
        playlistPicker.value = mockPlaylist2;

        // Verify root element has data source set and title matches    
        Assert.That(rootElement.dataSource, Is.Not.Null);
        Assert.AreEqual(((VideoPlayListVM)(rootElement.dataSource)).name, mockPlaylist2.GetVM().name);

        UnityEngine.Object.DestroyImmediate(mockPlaylist1);
        UnityEngine.Object.DestroyImmediate(mockPlaylist2);
    }
}