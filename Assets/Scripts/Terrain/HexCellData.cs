using Unity.Mathematics;

namespace Terrain
{
    public struct HexCellData
    {
        public float3 WorldPosition;
        public float Height;
        public BiomeType Biome;
    }
}