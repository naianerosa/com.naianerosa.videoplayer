using UnityEngine;

internal class MockVideoPlayerHandler : IEditorVideoPlayerHandler
{
    public void Destroy()
    {
        Debug.Log("MockVideoPlayerHandler Destroy called");
    }

    public void DrawVideoFrame()
    {
        Debug.Log("MockVideoPlayerHandler DrawVideoFrame called");
    }

    public void LoadNewVideo(string videoPath)
    {
        Debug.Log("MockVideoPlayerHandler LoadNewVideo called");
    }

    public void Pause()
    {
        Debug.Log("MockVideoPlayerHandler Pause called");
    }

    public void PlayVideo(string filePath = "")
    {
        Debug.Log("MockVideoPlayerHandler PlayVideo called");
    }

    public void StopVideo()
    {
        Debug.Log("MockVideoPlayerHandler StopVideo called");
    }
}

