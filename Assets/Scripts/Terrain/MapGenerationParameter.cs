using System;
using UnityEngine;

namespace Terrain
{
    [Serializable]
    public struct MapGenerationParameters
    {
        [Header("General Map Settings")] public int gridRadius;
        public bool useCircularShape;
        [Range(1f, 2f)] public float circleFactor;

        [Header("Generation Seed")] [Tooltip("Seed for random generation")]
        public uint seed;

        [Header("General Noise Settings")]
        [Tooltip("Number of noise layers to combine - higher values add more detail")]
        public int octaves;

        [Tooltip("How noise frequency changes between octaves - affects detail variation")]
        public float frequency;

        [Tooltip("How amplitude changes between octaves - affects detail prominence")]
        public float lacunarity;

        [Header("Main Island Settings")]
        [Tooltip("Base scale of the main noise pattern - larger values create smoother terrain")]
        public float mainNoiseScale;

        [Tooltip("Scale of ridge noise for mountains - affects mountain spacing")]
        public float ridgeNoiseScale;

        [Range(0, 1)] [Tooltip("How much ridge noise affects terrain - higher values create more mountains")]
        public float ridgeInfluence;

        [Tooltip("How quickly terrain drops from center to edge - shapes island outline")]
        public float falloffSteepness;

        [Tooltip("Power function for elevation - higher values create more distinct levels")]
        public float heightCurve;

        [Range(0, 1)] public float deepOceanThreshold;
        [Range(0, 1)] public float oceanThreshold;
        [Range(0, 1)] public float beachThreshold;
        [Range(0, 1)] public float plainsThreshold;
        [Range(0, 1)] public float forestThreshold;
        [Range(0, 1)] public float hillsThreshold;
        [Range(0, 1)] public float mountainsThreshold;

        public static MapGenerationParameters Default()
        {
            return new MapGenerationParameters
            {
                seed = 509,
                gridRadius = 40,
                useCircularShape = false,
                circleFactor = 1.5f,
                octaves = 6,
                frequency = 15,
                lacunarity = 40,
                mainNoiseScale = 450,
                ridgeNoiseScale = 1,
                ridgeInfluence = 0,
                falloffSteepness = 1.5f,
                heightCurve = 0.7f,
                deepOceanThreshold = 0.2f,
                oceanThreshold = 0.3f,
                beachThreshold = 0.35f,
                plainsThreshold = 0.5f,
                forestThreshold = 0.85f,
                hillsThreshold = 0.86f,
                mountainsThreshold = 1f
            };
        }
    }
}