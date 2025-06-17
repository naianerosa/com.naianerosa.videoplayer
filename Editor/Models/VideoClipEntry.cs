using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class VideoClipEntry
{
    public string name;
    public string filePath; // Absolute or relative path to the .webm file

    public PlayListItemElementVM GetVM()
    {
        var viewModel = ScriptableObject.CreateInstance<PlayListItemElementVM>();

        viewModel.Title = name;
        viewModel.PlayButtonVisibility = DisplayStyle.Flex;
        viewModel.PauseButtonVisibility = DisplayStyle.None;
        viewModel.FilePath = filePath;
        return viewModel;
    }
}