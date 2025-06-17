using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class VideoPlayerEditorWindow : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;
    private EditorVideoPlayerHandler videoPlayerComponent;
    private PlayListComponent playListComponent;

    [MenuItem("Window/Video Player")]
    public static void NewWindow()
    {
        VideoPlayerEditorWindow wnd = GetWindow<VideoPlayerEditorWindow>();
        wnd.titleContent = new GUIContent("Video Player");
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        root.Add(m_VisualTreeAsset.Instantiate());


        var videoDisplay = root.Q<IMGUIContainer>("video-display");

        videoPlayerComponent = new EditorVideoPlayerHandler(videoDisplay);

        videoDisplay.onGUIHandler = videoPlayerComponent.DrawVideoFrame;

        playListComponent = new PlayListComponent(root, videoPlayerComponent);

        root.Q<ObjectField>("playlist_picker").RegisterValueChangedCallback(evt =>
        {
            SetPlaylist(evt.newValue as VideoPlaylist);
        });

        SetPlaylist(ScriptableObject.CreateInstance<VideoPlaylist>()); // Load an empty playlist initially

        //Improve Video frame rate in the editor
        EditorApplication.update += Repaint;
    }

    private void SetPlaylist(VideoPlaylist playlist)
    {
        var playlistVM = playListComponent.LoadPlaylist(playlist);
        rootVisualElement.Q<VisualElement>("root").dataSource = playlistVM;
    }


    private void OnDisable()
    {
        videoPlayerComponent.Destroy();
    }
}
