using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;

/// <summary>
/// Video player handler, responsible for managing the video player component and rendering video frames in the editor.
/// </summary>
public class EditorVideoPlayerHandler
{
    public EventHandler LoopPointReached;
    public double ActiveVideoTime => videoPlayer.time;

    private VideoPlayer videoPlayer;
    private IMGUIContainer videoDisplay;
    private RenderTexture renderTexture;
    private long pausedFrame = -1;
    private AudioSource audioSource;

    public EditorVideoPlayerHandler(IMGUIContainer videoDisplay)
    {
        this.videoDisplay = videoDisplay;
        renderTexture = new RenderTexture(512, 288, 0); // 16:9 aspect ratio
        var go = new GameObject(EditorVideoPlayerConstants.VideoPlayerName, typeof(VideoPlayer));
        go.hideFlags = HideFlags.HideAndDontSave;
        videoPlayer = go.GetComponent<VideoPlayer>();
        videoPlayer.playOnAwake = false;
        audioSource = videoPlayer.gameObject.AddComponent<AudioSource>();
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.controlledAudioTrackCount = 1;
        videoPlayer.SetTargetAudioSource(0, audioSource);
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = renderTexture;

        videoPlayer.loopPointReached += VideoPlayer_loopPointReached;
    }

    private void VideoPlayer_loopPointReached(VideoPlayer source)
    {
        LoopPointReached?.Invoke(this, EventArgs.Empty);
    }

    public void DrawVideoFrame()
    {
        if (renderTexture == null) return;

        //Fit the texture to the container
        var aspect = (float)renderTexture.width / renderTexture.height;
        float width = videoDisplay.contentRect.width;
        float height = width / aspect;

        var rect = GUILayoutUtility.GetRect(width, height, GUILayout.ExpandWidth(false));
        GUI.DrawTexture(rect, renderTexture);
    }

    public void Pause()
    {
        videoPlayer.Pause();
        pausedFrame = videoPlayer.frame;
    }

    public void StopVideo()
    {
        videoPlayer.Stop();
    }

    public void LoadNewVideo(string videoPath)
    {
        videoPlayer.Stop();

        videoPlayer.url = videoPath;
        videoPlayer.Prepare();
    }

    public void PlayVideo(string filePath = "")
    {
        if (string.IsNullOrEmpty(filePath))
        {
            videoPlayer.Stop();
            videoPlayer.url = "";
            videoPlayer.controlledAudioTrackCount = 1;
            return;
        }

        if (videoPlayer.url != filePath)
        {
            LoadNewVideo(filePath);
        }

        if (pausedFrame > 0)
        {
            //For some reason the video player resumes from the wrong frame when rendered on the IMGUIContainer.
            //Forcing the frame fixed this issue.
            videoPlayer.frame = pausedFrame;
            pausedFrame = -1;
        }

        videoPlayer.Play();
    }

    public void Destroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.Stop();
            GameObject.DestroyImmediate(videoPlayer.gameObject);
            videoPlayer = null;
        }

        if (renderTexture != null)
        {
            GameObject.DestroyImmediate(renderTexture);
            renderTexture = null;
        }
    }

    public void SetVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }

    internal void MuteUnmute()
    {
        if (audioSource != null)
        {
            audioSource.mute = !audioSource.mute;
        }
    }

    internal void SetPlaybackSpeed(float playbackSpeedFactor)
    {
        if (videoPlayer != null)
        {
            videoPlayer.playbackSpeed = playbackSpeedFactor;
        }
    }
}
