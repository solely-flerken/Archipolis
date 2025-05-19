using System;
using Unity.Mathematics;
using UnityEngine;

namespace Terrain
{
    [Serializable]
    public struct MapGenerationParameters
    {
        [Header("Map Settings")] 
        public int gridRadius;
        public bool useCircularShape;
        [Range(1f, 2f)] public float circleFactor;

        [Header("Chunk Settings")] 
        public int chunkSize;

        [Header("Generation Seed")] 
        public uint seed;
        public float2 offset;

        [Header("Noise Settings")] 
        public int octaves;
        public float scale;
        public float persistence;
        public float lacunarity;
        [Range(0, 1)] public float falloffInfluence;
        public float falloffSharpness;
        [Min(0)] public float terrainSharpnessExponent;
        
        [Header("Biome Thresholds")] 
        [Range(0, 1)] public float deepOcean;
        [Range(0, 1)] public float ocean;
        [Range(0, 1)] public float beach;
        [Range(0, 1)] public float plains;
        [Range(0, 1)] public float forest;
        [Range(0, 1)] public float hills;
        [Range(0, 1)] public float mountains;
        [Range(0, 1)] public float peaks;

        public static MapGenerationParameters Default()
        {
            return new MapGenerationParameters
            {
                // Map Settings
                gridRadius = 40,
                useCircularShape = false,
                circleFactor = 1.5f,
                
                // Chunk Settings
                chunkSize = 32,
                
                // Seed
                seed = 509,
                offset = new float2(0, 0),
                
                // Noise Settings
                octaves = 4,
                scale = 400,
                persistence = 0.5f,
                lacunarity = 2,
                falloffInfluence = 0.7f,
                falloffSharpness = 3,
                terrainSharpnessExponent = 1,
                
                // Biome Thresholds
                deepOcean = 0.2f,
                ocean = 0.3f,
                beach = 0.35f,
                plains = 0.5f,
                forest = 0.7f,
                hills = 0.75f,
                mountains = 0.8f,
                peaks = 1f
            };
        }
    }
}