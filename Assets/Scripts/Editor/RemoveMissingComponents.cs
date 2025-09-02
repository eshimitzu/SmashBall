using UnityEditor;
using UnityEngine;

public class RemoveMissingComponents
{
    [MenuItem("Tools/Remove Missing Components")]
    public static void FixPrefab()
    {
        var gameObject = Selection.activeGameObject;
        var allObjects = gameObject.GetComponentsInChildren<Transform>(true);

        foreach (var obj in allObjects)
        {
            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(obj.gameObject);
        }
        
        EditorUtility.SetDirty(gameObject);
    }
}
