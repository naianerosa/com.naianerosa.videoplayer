using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayListComponentTest
{
    private PlayListComponent playListComponent;
    private IEditorVideoPlayerHandler videoPlayer;
    private VideoPlaylist mockPlayList;

    [SetUp]
    public void Setup()
    {
        // Setup root element
        var template = EditorGUIUtility.Load("Packages/com.naianerosa.videoplayer/Editor/VideoPlayerEditorWindow.uxml") as VisualTreeAsset;

        var root = new VisualElement();

        template.CloneTree(root);

        // Setup mock video player
        videoPlayer = new MockVideoPlayerHandler();

        // Create mock playlist
        mockPlayList = ScriptableObject.CreateInstance<VideoPlaylist>();
        mockPlayList.videoClips = new List<VideoClipEntry>
        {
            new VideoClipEntry { name = "Test Video 1", filePath = "path/to/video1.mp4" },
            new VideoClipEntry { name = "Test Video 2", filePath = "path/to/video2.mp4" },
            new VideoClipEntry { name = "Test Video 3", filePath = "path/to/video3.mp4" }
        };

        // Create component
        playListComponent = new PlayListComponent(root, videoPlayer);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(mockPlayList);
        videoPlayer.Destroy();
    }

    [Test]
    public void LoadPlaylist_HasCorrectNumberOfVideosAndState()
    {
        var viewModel = playListComponent.LoadPlaylist(mockPlayList);

        //Check items and buttons states
        Assert.IsNotNull(viewModel);
        Assert.That(viewModel.Videos.Count, Is.EqualTo(mockPlayList.videoClips.Count));
        AssertButtonStates(viewModel, mainButtonPlaying: true, indexOfPlayingVideo: 0);

        //Check containers visibility
        Assert.AreEqual(DisplayStyle.Flex, viewModel.VideoContainerVisibility, "Videos container should be visible");
        Assert.AreEqual(DisplayStyle.None, viewModel.NoVideosLabelVisibility, "No videos label should NOT be visible");

        //Change Playlist
        var newPlayList = ScriptableObject.CreateInstance<VideoPlaylist>();
        newPlayList.videoClips = new List<VideoClipEntry>
        {
            new VideoClipEntry { name = "New Test Video 1", filePath = "path/to/video1.mp4" }
        };

        viewModel = playListComponent.LoadPlaylist(newPlayList);

        //Check items and buttons states
        Assert.IsNotNull(viewModel);
        Assert.That(viewModel.Videos.Count, Is.EqualTo(newPlayList.videoClips.Count));
        AssertButtonStates(viewModel, mainButtonPlaying: true, indexOfPlayingVideo: 0);

        //Check containers visibility
        Assert.AreEqual(DisplayStyle.Flex, viewModel.VideoContainerVisibility, "Videos container should be visible");
        Assert.AreEqual(DisplayStyle.None, viewModel.NoVideosLabelVisibility, "No videos label should NOT be visible");

        Object.DestroyImmediate(newPlayList);
    }

    [Test]
    public void LoadPlaylist_WithEmptyPlaylist()
    {
        var emptyPlaylist = ScriptableObject.CreateInstance<VideoPlaylist>();

        VideoPlayerEditorWindowVM viewModel = playListComponent.LoadPlaylist(emptyPlaylist);

        Assert.IsNotNull(viewModel, "ViewModel should not be null for an empty playlist.");
        Assert.AreEqual(DisplayStyle.Flex, viewModel.NoVideosLabelVisibility, "No videos label should be visible");
        Assert.AreEqual(DisplayStyle.None, viewModel.VideoContainerVisibility, "Videos container should not be visible");

        Object.DestroyImmediate(emptyPlaylist);
    }

    [Test]
    public void Play_ButtonsAreOnCorrectState()
    {
        var vm = playListComponent.LoadPlaylist(mockPlayList);
        playListComponent.Pause();
        playListComponent.Play();
        AssertButtonStates(vm, mainButtonPlaying: true, indexOfPlayingVideo: 0);
    }

    [Test]
    public void Next_ButtonsAreOnCorrectState()
    {
        var vm = playListComponent.LoadPlaylist(mockPlayList);
        playListComponent.Next();

        AssertButtonStates(vm, mainButtonPlaying: true, indexOfPlayingVideo: 1);
    }

    [Test]
    public void Next_MoreTimesThanVideos_GoesBackToFirst()
    {
        var vm = playListComponent.LoadPlaylist(mockPlayList);
        for (int i = 1; i < mockPlayList.videoClips.Count; i++)
        {
            playListComponent.Next();
        }
        playListComponent.Next(); //Once more, should go back to first video

        AssertButtonStates(vm, mainButtonPlaying: true, indexOfPlayingVideo: 0);
    }

    [Test]
    public void Previous_ButtonsAreOnCorrectState()
    {
        var vm = playListComponent.LoadPlaylist(mockPlayList);
        playListComponent.Next(); // Move to second video
        playListComponent.Previous(); // Move back to first

        AssertButtonStates(vm, mainButtonPlaying: true, indexOfPlayingVideo: 0);
    }

    [Test]
    public void Previous_OnFirstVideo_GoesBackToLast()
    {
        var vm = playListComponent.LoadPlaylist(mockPlayList);
        playListComponent.Previous(); // Move to last video

        AssertButtonStates(vm, mainButtonPlaying: true, indexOfPlayingVideo: mockPlayList.videoClips.Count - 1);
    }

    [Test]
    public void Stop_ButtonsAreOnCorrectState()
    {
        var vm = playListComponent.LoadPlaylist(mockPlayList);
        playListComponent.Stop();

        AssertButtonStates(vm, mainButtonPlaying: false, indexOfPlayingVideo: -1);
    }

    [Test]
    public void Pause_ButtonsAreOnCorrectState()
    {
        var vm = playListComponent.LoadPlaylist(mockPlayList);
        playListComponent.Pause();

        AssertButtonStates(vm, mainButtonPlaying: false, indexOfPlayingVideo: -1);
    }

    private void AssertButtonStates(VideoPlayerEditorWindowVM playlist, bool mainButtonPlaying, int indexOfPlayingVideo)
    {
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