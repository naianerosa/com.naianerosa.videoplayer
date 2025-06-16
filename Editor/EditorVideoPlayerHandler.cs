using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;

public class EditorVideoPlayerHandler
{
    private VideoPlayer videoPlayer;
    private IMGUIContainer videoDisplay;
    private RenderTexture renderTexture;
    private long pausedFrame = -1;

    public EditorVideoPlayerHandler(IMGUIContainer videoDisplay)
    {
        this.videoDisplay = videoDisplay;
        renderTexture = new RenderTexture(512, 288, 0); // 16:9 aspect ratio
        var go = new GameObject($"EditorVideoPlayer", typeof(VideoPlayer));       
        go.hideFlags = HideFlags.HideAndDontSave;
        videoPlayer = go.GetComponent<VideoPlayer>();
        videoPlayer.playOnAwake = false;
        AudioSource audioSource = videoPlayer.gameObject.AddComponent<AudioSource>();
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.SetTargetAudioSource(0, audioSource);
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = renderTexture;
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
        if (filePath != "" && videoPlayer.url != filePath)
        {
            LoadNewVideo(filePath);
        }

        if (pausedFrame > 0)
        {
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
}
