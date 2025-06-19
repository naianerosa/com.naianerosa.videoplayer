using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// ViewModel for the <see cref="VideoPlayerEditorWindow"/>.
/// The view model contain all the properties that are binded to the UI elements 
/// This view model was created as a ScriptableObject to allow for easy experiments in the UI Builder.
/// </summary>
//[CreateAssetMenu(fileName = "VideoPlayerEditorWindowVM", menuName = "Video Player/VideoPlayerEditorWindowVM")]
public class VideoPlayerEditorWindowVM: ScriptableObject
{
    [SerializeField]
    private DisplayStyle noPlayListSelectedContainer = DisplayStyle.Flex;

    public DisplayStyle NoPlayListSelectedContainer => noPlayListSelectedContainer;

    public void SetPlaylist(VideoPlaylist playlist)
    {
        noPlayListSelectedContainer = playlist == null ? DisplayStyle.Flex : DisplayStyle.None;
    }
}