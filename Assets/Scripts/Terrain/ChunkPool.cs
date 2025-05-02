using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace Terrain
{
    public class ChunkPool
    {
        private readonly List<GameObject> _activeChunks = new();
        private readonly Queue<GameObject> _inactiveChunks = new();

        private readonly Material _material;
        private readonly Transform _parent;

        private readonly int _expandAmount;

        public ChunkPool(Material material, Transform parent, int poolSize, int expandAmount)
        {
            _material = material;
            _parent = parent;
            _expandAmount = expandAmount;

            for (var i = 0; i < poolSize; i++)
            {
                CreateNewChunk();
            }
        }

        public GameObject GetChunk(Mesh mesh)
        {
            var chunk = GetOrCreateChunk();

            chunk.SetActive(true);
            chunk.GetComponent<MeshFilter>().mesh = mesh;

            _activeChunks.Add(chunk);

            return chunk;
        }

        public void ReleaseChunk(GameObject chunk)
        {
            if (chunk == null)
            {
                return;
            }

            if (!_activeChunks.Contains(chunk))
            {
                return;
            }

            chunk.SetActive(false);
            chunk.GetComponent<MeshFilter>().mesh = null;

            _activeChunks.Remove(chunk);
            _inactiveChunks.Enqueue(chunk);
        }

        public void ReleaseAllChunks()
        {
            var chunksToRelease = _activeChunks.ToArray();

            foreach (var chunk in chunksToRelease)
            {
                ReleaseChunk(chunk);
            }
        }

        public void ClearInactiveChunks()
        {
            foreach (var chunk in _inactiveChunks.Where(chunk => chunk != null))
            {
                DestroyChunkGameObject(chunk);
            }

            _inactiveChunks.Clear();
        }

        public void ClearPool()
        {
            foreach (var chunk in _activeChunks.Where(chunk => chunk != null))
            {
                DestroyChunkGameObject(chunk);
            }

            _activeChunks.Clear();

            ClearInactiveChunks();
        }

        private GameObject GetOrCreateChunk()
        {
            if (_inactiveChunks.Count == 0)
            {
                ExpandChunkPool();
            }

            return _inactiveChunks.Dequeue();
        }

        private void ExpandChunkPool()
        {
            for (var i = 0; i < math.max(1, _expandAmount); i++)
            {
                CreateNewChunk();
            }
        }

        private void CreateNewChunk()
        {
            var totalChunks = _activeChunks.Count + _inactiveChunks.Count;

            var chunkObject = new GameObject("Chunk-" + totalChunks);
            chunkObject.transform.SetParent(_parent, false);
            chunkObject.AddComponent<MeshFilter>();
            chunkObject.SetActive(false);
            chunkObject.hideFlags = HideFlags.DontSave;

            var meshRenderer = chunkObject.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = _material;

            _inactiveChunks.Enqueue(chunkObject);
        }

        private static void DestroyChunkGameObject(GameObject obj)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Object.DestroyImmediate(obj);
                return;
            } 
#endif
            Object.Destroy(obj);
        }
    }
}