// using System;
// using System.Collections;
// using System.Collections.Generic;
// using Sirenix.OdinInspector;
// using UnityEditor;
// using UnityEngine;
//
//
// [InitializeOnLoad]
// public class MouseReleaseInScene
// {
//     static MouseReleaseInScene()
//     {
//         SceneView.duringSceneGui += OnSceneGUI;
//     }
//
//     private static void OnSceneGUI(SceneView sceneView)
//     {
//         Event e = Event.current;
//
//         if (e.type == EventType.MouseUp)
//         {
//             Debug.Log($"Position.Mouse: {e.mousePosition}");
//         }
//     }
//
//
// }
