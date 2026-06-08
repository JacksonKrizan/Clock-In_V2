#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public class PlayModeStartSceneLoader
{
    static PlayModeStartSceneLoader()
    {

        
        if (EditorBuildSettings.scenes.Length > 0)
        {
            string scenePath = EditorBuildSettings.scenes[0].path;
            SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
            
            if (sceneAsset != null)
            {
                EditorSceneManager.playModeStartScene = sceneAsset;
            }
        }
    }
}
#endif
