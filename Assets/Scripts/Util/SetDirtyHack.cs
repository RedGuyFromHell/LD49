using UnityEngine;
using System.Collections;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEditor;

public static class SetDirtyHack
{
    public static bool CATdirtySet = false;

    public static void SetDirty(UnityEngine.Object obj)
    {
        if (obj != null)
        {
            EditorUtility.SetDirty(obj);

            if (!Application.isPlaying)
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        CATdirtySet = true;
    }
}
