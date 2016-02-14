using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using System.Collections.Generic;

public class TerrainExports : MonoBehaviour {
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
            Object prefab = PrefabUtility.CreateEmptyPrefab(subscenePrefabPath);
            PrefabUtility.ReplacePrefab(terrain, prefab);
        }
        AssetDatabase.Refresh();

        foreach (string name in terrainNames)
        {
            Debug.Log(name + " - making subscene");
            string subscenePath = baseSceneDirectory + "/" + name + ".unity";
            string subscenePrefabPath = baseSceneDirectory + "/" + name + ".prefab";
            EditorSceneManager.NewScene(new NewSceneSetup());            
            Object prefabResource = AssetDatabase.LoadAssetAtPath<Object>(subscenePrefabPath);
            Object prefab = PrefabUtility.InstantiatePrefab(prefabResource);
            ((GameObject)prefab).transform.position = positions[name];
            Debug.Log(prefabResource);
            Debug.Log(subscenePrefabPath);
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), subscenePath);
        }
        EditorSceneManager.OpenScene(baseScenePath);
    }
}
