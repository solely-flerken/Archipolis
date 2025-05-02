#if UNITY_EDITOR
namespace Terrain
{
    using UnityEditor.Callbacks;
    using UnityEngine;

    public static class ChunkPoolCleanup
    {
        [DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            // Find all GameObjects in the scene (including hidden ones)
            foreach (var go in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (go.name.StartsWith("Chunk"))
                {
                    Object.DestroyImmediate(go);
                }
            }
        }
    }
}
#endif