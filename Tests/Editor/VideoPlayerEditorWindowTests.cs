// using NUnit.Framework;
// using UnityEditor;
// using UnityEngine;

// public class VideoPlayerEditorWindowTests
// {
//     [Test]
//     public void CanCreateWindow()
//     {
//         var window = EditorWindow.GetWindow<VideoPlayerEditorWindow>();
//         Assert.IsNotNull(window);
//     }

//     [Test]
//     public void CanAssignPlaylist()
//     {
//         var playlist = ScriptableObject.CreateInstance<VideoPlaylist>();
//         playlist.title = "Test Playlist";
//         playlist.videoClips.Add(new VideoClipEntry { name = "TestClip", filePath = "test.webm" });
//         Assert.AreEqual(1, playlist.videoClips.Count);
//     }
// }