using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class EditorVideoPlayerElementTests
{
    private EditorVideoPlayerElement videoPlayerElement;
    //private IEditorVideoPlayerHandler videoPlayer;
    private VideoPlaylist mockPlayList;

    [SetUp]
    public void Setup()
    {
        // Setup root element
        var template = EditorGUIUtility.Load("Packages/com.naianerosa.videoplayer/Editor/EditorVideoPlayerElement.uxml") as VisualTreeAsset;

        var root = new VisualElement();

        template.CloneTree(root);

        videoPlayerElement = root.Q<EditorVideoPlayerElement>();
        videoPlayerElement.Init();

        // Setup mock video player
        //videoPlayer = new MockVideoPlayerHandler();

        // Create mock playlist
        mockPlayList = ScriptableObject.CreateInstance<VideoPlaylist>();
        mockPlayList.Videos = new List<VideoClipEntry>
        {
            new VideoClipEntry { Name = "Test Video 1", FilePath = "path/to/video1.mp4" },
            new VideoClipEntry { Name = "Test Video 2", FilePath = "path/to/video2.mp4" },
            new VideoClipEntry { Name = "Test Video 3", FilePath = "path/to/video3.mp4" }
        };
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(mockPlayList);
        //videoPlayer.Destroy();
    }

    [Test]
    public void LoadPlaylist_HasCorrectNumberOfVideosAndState()
    {
        videoPlayerElement.LoadPlayList(mockPlayList);

        //Check items and buttons states
        Assert.IsNotNull(videoPlayerElement.viewModel);
        Assert.That(videoPlayerElement.viewModel.Videos.Count, Is.EqualTo(mockPlayList.Videos.Count));
        AssertButtonStates(mainButtonPlaying: true, indexOfPlayingVideo: 0);

        //Check containers visibility
        Assert.AreEqual(DisplayStyle.Flex, videoPlayerElement.viewModel.VideoContainerVisibility, "Videos container should be visible");
        Assert.AreEqual(DisplayStyle.None, videoPlayerElement.viewModel.NoVideosLabelVisibility, "No videos label should NOT be visible");

        //Change Playlist
        var newPlayList = ScriptableObject.CreateInstance<VideoPlaylist>();
        newPlayList.Videos = new List<VideoClipEntry>
        {
            new VideoClipEntry { Name = "New Test Video 1", FilePath = "path/to/video1.mp4" }
        };

        videoPlayerElement.LoadPlayList(newPlayList);

        //Check items and buttons states
        Assert.IsNotNull(videoPlayerElement.viewModel);
        Assert.That(videoPlayerElement.viewModel.Videos.Count, Is.EqualTo(newPlayList.Videos.Count));
        AssertButtonStates(mainButtonPlaying: true, indexOfPlayingVideo: 0);

        //Check containers visibility
        Assert.AreEqual(DisplayStyle.Flex, videoPlayerElement.viewModel.VideoContainerVisibility, "Videos container should be visible");
        Assert.AreEqual(DisplayStyle.None, videoPlayerElement.viewModel.NoVideosLabelVisibility, "No videos label should NOT be visible");

        Object.DestroyImmediate(newPlayList);
    }

    [Test]
    public void LoadPlaylist_WithEmptyPlaylist()
    {
        var emptyPlaylist = ScriptableObject.CreateInstance<VideoPlaylist>();

        videoPlayerElement.LoadPlayList(emptyPlaylist);

        Assert.IsNotNull(videoPlayerElement.viewModel, "ViewModel should not be null for an empty playlist.");
        Assert.AreEqual(DisplayStyle.Flex, videoPlayerElement.viewModel.NoVideosLabelVisibility, "No videos label should be visible");
        Assert.AreEqual(DisplayStyle.None, videoPlayerElement.viewModel.VideoContainerVisibility, "Videos container should not be visible");

        Object.DestroyImmediate(emptyPlaylist);
    }

    [Test]
    public void Play_ButtonsAreOnCorrectState()
    {
        videoPlayerElement.LoadPlayList(mockPlayList);
        videoPlayerElement.Pause();
        videoPlayerElement.Play();
        AssertButtonStates(mainButtonPlaying: true, indexOfPlayingVideo: 0);
    }

    [Test]
    public void Next_ButtonsAreOnCorrectState()
    {
        videoPlayerElement.LoadPlayList(mockPlayList);
        videoPlayerElement.Next();

        AssertButtonStates( mainButtonPlaying: true, indexOfPlayingVideo: 1);
    }

    [Test]
    public void Next_MoreTimesThanVideos_GoesBackToFirst()
    {
        videoPlayerElement.LoadPlayList(mockPlayList);
        for (int i = 1; i < mockPlayList.Videos.Count; i++)
        {
            videoPlayerElement.Next();
        }
        videoPlayerElement.Next(); //Once more, should go back to first video

        AssertButtonStates(mainButtonPlaying: true, indexOfPlayingVideo: 0);
    }

    [Test]
    public void Previous_ButtonsAreOnCorrectState()
    {
        videoPlayerElement.LoadPlayList(mockPlayList);
        videoPlayerElement.Next(); // Move to second video
        videoPlayerElement.Previous(); // Move back to first

        AssertButtonStates(mainButtonPlaying: true, indexOfPlayingVideo: 0);
    }

    [Test]
    public void Previous_OnFirstVideo_GoesBackToLast()
    {
        videoPlayerElement.LoadPlayList(mockPlayList);
        videoPlayerElement.Previous(); // Move to last video

        AssertButtonStates(mainButtonPlaying: true, indexOfPlayingVideo: mockPlayList.Videos.Count - 1);
    }

    [Test]
    public void Stop_ButtonsAreOnCorrectState()
    {
        videoPlayerElement.LoadPlayList(mockPlayList);
        videoPlayerElement.Stop();

        AssertButtonStates(mainButtonPlaying: false, indexOfPlayingVideo: -1);
    }

    [Test]
    public void Pause_ButtonsAreOnCorrectState()
    {
        videoPlayerElement.LoadPlayList(mockPlayList);
        videoPlayerElement.Pause();

        AssertButtonStates(mainButtonPlaying: false, indexOfPlayingVideo: -1);
    }

    private void AssertButtonStates(bool mainButtonPlaying, int indexOfPlayingVideo)
    {
        var playlist = videoPlayerElement.viewModel;
        var currentVideos = playlist.Videos;

        DisplayStyle expectedMainButtonPlay = mainButtonPlaying ? DisplayStyle.None : DisplayStyle.Flex;
        DisplayStyle expectedMainButtonPause = mainButtonPlaying ? DisplayStyle.Flex : DisplayStyle.None;

        Assert.AreEqual(expectedMainButtonPlay, playlist.PlayButtonVisibility);
        Assert.AreEqual(expectedMainButtonPause, playlist.PauseButtonVisibility);

        for (int i = 0; i < currentVideos.Count; i++)
        {
            DisplayStyle expectedPlay = (i == indexOfPlayingVideo) ? DisplayStyle.None : DisplayStyle.Flex;
            DisplayStyle expectedPause = (i == indexOfPlayingVideo) ? DisplayStyle.Flex : DisplayStyle.None;
            Assert.AreEqual(expectedPlay, currentVideos[i].PlayButtonVisibility, $"Wrong play visibility on video {i}");
            Assert.AreEqual(expectedPause, currentVideos[i].PauseButtonVisibility, $"Wrong pause visibility on video {i}");
        }
    }
}