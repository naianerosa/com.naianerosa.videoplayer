using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;

/// <summary>
/// ViewModel for the <see cref="EditorVideoPlayerElement"/>.
/// The view model contain all the properties that are binded to the UI elements 
/// and some methods to control the video player state.
/// This view model was created as a ScriptableObject to allow for easy experiments in the UI Builder.
/// </summary>
//[CreateAssetMenu(fileName = "VideoPlaylist", menuName = "Video Player/EditorVideoPlayerElementVM")]
public class EditorVideoPlayerElementVM : ScriptableObject
{
    [SerializeField]
    private string title;
    public string Title => title;

    [SerializeField]
    private DisplayStyle playButtonVisibility;
    public DisplayStyle PlayButtonVisibility => playButtonVisibility;

    [SerializeField]
    private DisplayStyle pauseButtonVisibility;
    public DisplayStyle PauseButtonVisibility => pauseButtonVisibility;

    [SerializeField]
    private DisplayStyle noVideosLabelVisibility = DisplayStyle.None;
    public DisplayStyle NoVideosLabelVisibility => noVideosLabelVisibility;

    [SerializeField]
    private DisplayStyle videoContainerVisibility = DisplayStyle.None;
    public DisplayStyle VideoContainerVisibility => videoContainerVisibility;

    [SerializeField]
    public List<PlayListItemElementVM> videos = new List<PlayListItemElementVM>();

    public List<PlayListItemElementVM> Videos => videos;

    [SerializeField]
    private PlayListItemElementVM activeVideo = null;

    public PlayListItemElementVM ActiveVideo
    {
        get => activeVideo;
        set => activeVideo = value;
    }

    [SerializeField]
    private float volume = 1f;
    public float Volume => volume;

    [SerializeField]
    private DisplayStyle volumeButtonVisibility = DisplayStyle.Flex;
    public DisplayStyle VolumeButtonVisibility => volumeButtonVisibility;

    [SerializeField]
    private DisplayStyle muteButtonVisibility = DisplayStyle.None;
    public DisplayStyle MuteButtonVisibility => muteButtonVisibility;

    [SerializeField]
    private int playbackSpeedIndex = 1;
    public int PlaybackSpeedIndex => playbackSpeedIndex;

    public void Pause()
    {
        playButtonVisibility = DisplayStyle.Flex;
        pauseButtonVisibility = DisplayStyle.None;
    }

    public void Play()
    {
        playButtonVisibility = DisplayStyle.None;
        pauseButtonVisibility = DisplayStyle.Flex;
    }

    public void MuteUnMute()
    {
        if (MuteButtonVisibility == DisplayStyle.Flex)
        {
            muteButtonVisibility = DisplayStyle.None;
            volumeButtonVisibility = DisplayStyle.Flex;
        }
        else
        {
            muteButtonVisibility = DisplayStyle.Flex;
            volumeButtonVisibility = DisplayStyle.None;
        }
    }

    public void OnEnable()
    {
        activeVideo = ScriptableObject.CreateInstance<PlayListItemElementVM>();
    }

    public void Init(VideoPlaylist playlist)
    {
        videos.Clear();
        if (playlist == null)
        {
            title = "No Playlist Loaded";
            playButtonVisibility = DisplayStyle.None;
            pauseButtonVisibility = DisplayStyle.None;
            noVideosLabelVisibility = DisplayStyle.None;
            videoContainerVisibility = DisplayStyle.None;
        }
        else
        {
            title = playlist.Title;
            playButtonVisibility = DisplayStyle.Flex;
            pauseButtonVisibility = DisplayStyle.None;
            noVideosLabelVisibility = playlist.Videos == null || playlist.Videos.Length == 0 ? DisplayStyle.Flex : DisplayStyle.None;
            videoContainerVisibility = playlist.Videos != null && playlist.Videos.Length > 0 ? DisplayStyle.Flex : DisplayStyle.None;

            if (playlist.Videos != null)
            {
                foreach (VideoClip video in playlist.Videos)
                {
                    var videoVM = ScriptableObject.CreateInstance<PlayListItemElementVM>();
                    videoVM.Initialize(video.name, video.originalPath, video.length);
                    videos.Add(videoVM);
                }
            }
        }

        ActiveVideo = Videos.Count > 0 ? Videos[0] : null;
    }
}

