using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class VideoPlayerEditorWindow : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;
    private EditorVideoPlayerHandler videoPlayerHandler;
    public EditorVideoPlayerElement editorVideoPlayerElement;

    //private PlayListComponent playListComponent;

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

        this.editorVideoPlayerElement = root.Q<EditorVideoPlayerElement>();
        editorVideoPlayerElement.Init();

        videoPlayerHandler = new EditorVideoPlayerHandler(editorVideoPlayerElement.videoDisplay);

        editorVideoPlayerElement.videoDisplay.onGUIHandler = videoPlayerHandler.DrawVideoFrame;

        root.Q<ObjectField>("playlist_picker").RegisterCallback<ChangeEvent<Object>>((evt) =>
        {
            editorVideoPlayerElement.LoadPlayList(evt.newValue as VideoPlaylist);
        });

        editorVideoPlayerElement.PlayClicked += EditorVideoPlayerElement_PlayClicked;
        editorVideoPlayerElement.PauseClicked += EditorVideoPlayerElement_PauseClicked;
        editorVideoPlayerElement.StopClicked += EditorVideoPlayerElement_StopClicked;

        //Improve Video frame rate in the editor
        EditorApplication.update += Repaint;
    }

    private void EditorVideoPlayerElement_PlayClicked(object sender, string filePath)
    {
        videoPlayerHandler.PlayVideo(filePath);
    }

    private void EditorVideoPlayerElement_PauseClicked(object sender)
    {
        videoPlayerHandler.Pause();
    }

    private void EditorVideoPlayerElement_StopClicked(object sender)
    {
        videoPlayerHandler.StopVideo();
    }

    private void OnDisable()
    {
        editorVideoPlayerElement.PlayClicked -= EditorVideoPlayerElement_PlayClicked;
        editorVideoPlayerElement.PauseClicked -= EditorVideoPlayerElement_PauseClicked;
        editorVideoPlayerElement.StopClicked -= EditorVideoPlayerElement_StopClicked;
        videoPlayerHandler.Destroy();
    }
}
