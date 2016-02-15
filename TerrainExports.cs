using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using System.Collections.Generic;
using System;

public class TerrainExports : MonoBehaviour {
    private static bool SceneRegistered(EditorBuildSettingsScene[] scenes, string scenePath)
    {
        foreach (EditorBuildSettingsScene scene in scenes)
        {
            if (scene.path == scenePath)
            {
                return true;
            }
        }
        return false;
    }

    [MenuItem("Tools/Terrain to scenes")]
    private static void TerrainExport()
    {
        Terrain[] objects = (Terrain[])Resources.FindObjectsOfTypeAll(typeof(Terrain));
        List<string> terrainNames = new List<string>();
        Dictionary<string, Vector3> positions = new Dictionary<string, Vector3>();

        foreach (Terrain obj in objects)
        {
            terrainNames.Add(obj.name);
        }

        string baseScenePath = EditorSceneManager.GetActiveScene().path;
        string baseSceneDirectory = Path.GetDirectoryName(baseScenePath);
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());

        foreach (string name in terrainNames)
        {
            string subscenePrefabPath = baseSceneDirectory + "/" + name + ".prefab";
            Debug.Log(name + " - making prefab");
            GameObject terrain = GameObject.Find(name);
            positions[name] = terrain.transform.position;
            Selection.activeGameObject = terrain;
            UnityEngine.Object prefab = PrefabUtility.CreateEmptyPrefab(subscenePrefabPath);
            PrefabUtility.ReplacePrefab(terrain, prefab);
        }
        AssetDatabase.Refresh();

        foreach (string name in terrainNames)
        {
            Debug.Log(name + " - making subscene");
            string subscenePath = baseSceneDirectory + "/" + name + ".unity";
            string subscenePrefabPath = baseSceneDirectory + "/" + name + ".prefab";
            EditorSceneManager.NewScene(new NewSceneSetup());            
            UnityEngine.Object prefabResource = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(subscenePrefabPath);
            UnityEngine.Object prefab = PrefabUtility.InstantiatePrefab(prefabResource);
            ((GameObject)prefab).transform.position = positions[name];
            EditorApplication.ExecuteMenuItem("GameObject/Camera");
            Camera camera = (Camera)UnityEngine.Object.FindObjectOfType(typeof(Camera));
            camera.name = "SERVICE";
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), subscenePath);
            
            EditorBuildSettingsScene[] original = EditorBuildSettings.scenes;
            if (! SceneRegistered(original, subscenePath))
            {
                EditorBuildSettingsScene[] scenes = new EditorBuildSettingsScene[original.Length + 1];
                Array.Copy(original, scenes, original.Length);
                scenes[scenes.Length - 1] = new EditorBuildSettingsScene(subscenePath, true);
                EditorBuildSettings.scenes = scenes;
            }
        }
        EditorSceneManager.OpenScene(baseScenePath);
    }
}
