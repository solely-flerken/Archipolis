using System.Collections.Generic;
using Terrain;
using UnityEngine;

namespace Hex
{
    public class HexMapManager : MonoBehaviour
    {
        public static HexMapManager Instance { get; private set; }

        public Dictionary<HexCoordinate, HexCellData> HexMap { get; private set; } = new();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            HexMap = MapGenerator.Instance.GenerateTerrainMesh();
        }
    }
}