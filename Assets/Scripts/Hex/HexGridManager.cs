using Terrain;
using UnityEngine;

namespace Hex
{
    public class HexGridManager : MonoBehaviour
    {
        public static HexGridManager Instance { get; private set; }

        public HexGrid HexGrid { get; private set; }

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

            HexGrid = new HexGrid();
        }

        private void Start()
        {
            HexGrid.HexMap = MapGenerator.Instance.GenerateTerrainMesh();
        }
    }
}