using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "VideoPlaylist", menuName = "Video Player/Video Playlist")]
public class VideoPlaylist : ScriptableObject
{
    public string title;
    public List<VideoClipEntry> videoClips = new();


    public VideoPlayListVM GetVM()
    {
        return new VideoPlayListVM
        {
            Title = title,
            PlayButtonVisibility = DisplayStyle.Flex,
            PauseButtonVisibility = DisplayStyle.None,
            CurrentVideoTitle = videoClips.Count > 0 ? videoClips[0].name : "No videos in playlist"
        };
    }
}

[System.Serializable]
public class VideoClipEntry
{
    public string name;
    public string filePath; // Absolute or relative path to the .webm file

    public VideoClipVM GetVM()
    {
        return new VideoClipVM
        {
            Title = name,
            PlayButtonVisibility = DisplayStyle.Flex,
            PauseButtonVisibility = DisplayStyle.None,
            FilePath = filePath
        };
    }
}

[CreateAssetMenu(fileName = "VideoPlaylist", menuName = "Video Player/VideoClipVM")]
public class VideoClipVM:ScriptableObject
{
    public string Title;
    public DisplayStyle PlayButtonVisibility;
    public DisplayStyle PauseButtonVisibility;
    public string FilePath;

    public void Pause()
    {
        PlayButtonVisibility = DisplayStyle.Flex;
        PauseButtonVisibility = DisplayStyle.None;
    }

    public void Play()
    {
        PlayButtonVisibility = DisplayStyle.None;
        PauseButtonVisibility = DisplayStyle.Flex;
    }
}

[CreateAssetMenu(fileName = "VideoPlaylist", menuName = "Video Player/VideoPlayListVM")]
public class VideoPlayListVM:ScriptableObject
{
    public string Title;
    public DisplayStyle PlayButtonVisibility;
    public DisplayStyle PauseButtonVisibility;
    public string CurrentVideoTitle;

    public void Pause()
    {
        PlayButtonVisibility = DisplayStyle.Flex;
        PauseButtonVisibility = DisplayStyle.None;
    }

    public void Play()
    {
        PlayButtonVisibility = DisplayStyle.None;
        PauseButtonVisibility = DisplayStyle.Flex;
    }
}

public class VideoPlayerComponent
{
    private VideoPlayer videoPlayer;
    private VideoPlayListVM playlistVM;
    public VideoPlayListVM PlayListVM => playlistVM;

    private List<VideoClipVM> videoClips = new List<VideoClipVM>();

    public List<VideoClipVM> VideoClips => videoClips;

    private VideoClipVM currentVideoClipVM;

    private long pausedFrame = -1;

    public void Load(VideoPlaylist playlist, VideoPlayer videoPlayer)
    {
        this.videoPlayer = videoPlayer;

        playlistVM = playlist.GetVM();
        videoClips.Clear();
        foreach (var clip in playlist.videoClips)
        {
            videoClips.Add(clip.GetVM());
        }

        if (videoClips.Count > 0)
        {
            StartVideo(0);
        }
    }

    public void StartVideo(int index)
    {
        if (index < 0 || index >= videoClips.Count) return;

        videoPlayer.Stop();

        foreach (var item in videoClips)
        {
            item.Pause();
        }

        currentVideoClipVM = videoClips[index];

        videoPlayer.url = currentVideoClipVM.FilePath;
        videoPlayer.Prepare();
        videoPlayer.Play();

        currentVideoClipVM.Play();

        playlistVM.CurrentVideoTitle = currentVideoClipVM.Title;
        playlistVM.Play();
    }

    public void PauseVideo(int index = -1)
    {
        videoPlayer.Pause();
        pausedFrame = videoPlayer.frame;

        if (index == -1)
        {
            currentVideoClipVM?.Pause();
        }
        else
        {
            videoClips[index].Pause();
        }
        playlistVM.Pause();
    }

    public void PlayVideo(int index = -1)
    {
        if (pausedFrame > 0)
        {
            videoPlayer.frame = pausedFrame;
            pausedFrame = -1;
        }

        videoPlayer.Play();

        if (index == -1)
        {
            currentVideoClipVM?.Play();
        }
        else
        {
            if (currentVideoClipVM != videoClips[index])
            {
                StartVideo(index);
            }
            else
            {
                currentVideoClipVM.Play();
            }
        }
        playlistVM.Play();
    }
}