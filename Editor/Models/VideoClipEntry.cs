using UnityEngine.UIElements;

[System.Serializable]
public class VideoClipEntry
{
    public string name;
    public string filePath; // Absolute or relative path to the .webm file

    public VideoClipVM GetVM(int clipIndex)
    {
        return new VideoClipVM
        {
            Index = clipIndex,
            Title = name,
            PlayButtonVisibility = DisplayStyle.Flex,
            PauseButtonVisibility = DisplayStyle.None,
            FilePath = filePath
        };
    }
}