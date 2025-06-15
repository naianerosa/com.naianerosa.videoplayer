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
            var playlist = evt.newValue as VideoPlaylist;
            var playlistVM = playListComponent.LoadPlaylist(playlist);
            root.Q<VisualElement>("root").dataSource = playlistVM;
        });

        //Improve Video frame rate in the editor
        EditorApplication.update += Repaint;
    }


    private void OnDisable()
    {
        videoPlayerComponent.Destroy();
    }
}
