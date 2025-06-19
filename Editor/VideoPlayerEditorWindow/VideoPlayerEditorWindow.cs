using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Main editor window for the Video Player package.
/// </summary>
[assembly: InternalsVisibleTo("VideoPlayer.Editor.Tests")]
public class VideoPlayerEditorWindow : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset visualTreeAsset = default;
    private EditorVideoPlayerHandler videoPlayerHandler;


    private EditorVideoPlayerElement editorVideoPlayerElement;
    internal EditorVideoPlayerElement EditorVideoPlayerElement => editorVideoPlayerElement;

    internal VideoPlayerEditorWindowVM ViewModel
    {
        get => rootVisualElement.Q<VisualElement>("root").dataSource as VideoPlayerEditorWindowVM;
        set => rootVisualElement.Q<VisualElement>("root").dataSource = value;
    }

    [MenuItem("Window/Video Player")]
    public static void NewWindow()
    {
        VideoPlayerEditorWindow wnd = GetWindow<VideoPlayerEditorWindow>();
        wnd.titleContent = new GUIContent("Video Player");
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        root.Add(visualTreeAsset.Instantiate());

        ViewModel = ScriptableObject.CreateInstance<VideoPlayerEditorWindowVM>();

        this.editorVideoPlayerElement = root.Q<EditorVideoPlayerElement>();
        editorVideoPlayerElement.Init();

        videoPlayerHandler = new EditorVideoPlayerHandler(editorVideoPlayerElement.videoDisplay);

        editorVideoPlayerElement.videoDisplay.onGUIHandler = videoPlayerHandler.DrawVideoFrame;

        root.Q<ObjectField>("playlist_picker").RegisterCallback<ChangeEvent<Object>>((EventCallback<ChangeEvent<Object>>)((evt) =>
        {
            var playlist = evt.newValue as VideoPlaylist;
            this.ViewModel.SetPlaylist(playlist);
            editorVideoPlayerElement.LoadPlayList(playlist);
        }));

        videoPlayerHandler.LoopPointReached += VideoPlayerHandler_LoopPointReached;

        editorVideoPlayerElement.PlayClicked += EditorVideoPlayerElement_PlayClicked;
        editorVideoPlayerElement.PauseClicked += EditorVideoPlayerElement_PauseClicked;
        editorVideoPlayerElement.StopClicked += EditorVideoPlayerElement_StopClicked;

        //Improve Video frame rate in the editor
        EditorApplication.update += Repaint;
    }

    private void VideoPlayerHandler_LoopPointReached(object sender, System.EventArgs e)
    {
        editorVideoPlayerElement.Pause();
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
        videoPlayerHandler.LoopPointReached -= VideoPlayerHandler_LoopPointReached;
        videoPlayerHandler.Destroy();
    }
}
