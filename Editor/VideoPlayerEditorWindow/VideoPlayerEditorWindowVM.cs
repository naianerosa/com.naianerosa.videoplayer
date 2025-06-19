using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// ViewModel for the <see cref="VideoPlayerEditorWindow"/>.
/// The view model contain all the properties that are binded to the UI elements 
/// </summary>
public class VideoPlayerEditorWindowVM
{
    [SerializeField]
    private DisplayStyle noPlayListSelectedContainer = DisplayStyle.Flex;

    public DisplayStyle NoPlayListSelectedContainer => noPlayListSelectedContainer;

    public void SetPlaylist(VideoPlaylist playlist)
    {
        noPlayListSelectedContainer = playlist == null ? DisplayStyle.Flex : DisplayStyle.None;
    }
}