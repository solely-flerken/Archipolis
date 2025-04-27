using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;
using UnityEngine;
using System;

namespace Terrain
{
    public struct IslandBiomeGenerator : IJobParallelFor
    {
        [ReadOnly] private NativeArray<float3> _positions;
        [WriteOnly] private NativeArray<BiomeType> _biomeMap;
        [WriteOnly] private NativeArray<float> _heightMap;

        private readonly IslandGenerationParameters _parameters;
        private readonly float2 _mapCenter;
        private readonly float _mapRadius;

        public IslandBiomeGenerator(IslandGenerationParameters parameters, NativeArray<float3> positions,
            NativeArray<BiomeType> biomeMap, NativeArray<float> heightMap,
            float2 mapCenter, float mapRadius)
        {
            _positions = positions;
            _biomeMap = biomeMap;
            _heightMap = heightMap;
            _parameters = parameters;
            _mapCenter = mapCenter;
            _mapRadius = mapRadius;
        }

        public void Execute(int index)
        {
            var worldPos = _positions[index];
            var pos = new float2(worldPos.x, worldPos.z);

            // Generate height value
            var height = GenerateHeight(pos);
            _heightMap[index] = height;

            // Determine biome based on height
            var biome = DetermineBiomeFromHeight(height);
            _biomeMap[index] = biome;
        }

        private float GenerateHeight(float2 pos)
        {
            // Generate base continent shape with falloff from center
            var falloff = FalloffMap(pos - _mapCenter, _mapRadius);

            // Generate main noise
            var mainNoise = OctavedSimplexNoise(pos, _parameters.mainNoiseScale);

            // Generate ridge noise for mountains
            var ridgeNoise = OctavedRidgeNoise(pos, _parameters.ridgeNoiseScale);

            // Blend noise types
            var blendedNoise = math.lerp(mainNoise, ridgeNoise, _parameters.ridgeInfluence);

            // Generate small islands noise with different scale
            var islandNoise = 0f;
            if (_parameters.enableSmallIslands)
            {
                // Use different scale for smaller islands
                islandNoise = OctavedSimplexNoise(pos, _parameters.islandNoiseScale);

                // Apply threshold to create distinct small islands
                islandNoise = islandNoise > _parameters.islandThreshold
                    ? (islandNoise - _parameters.islandThreshold) / (1 - _parameters.islandThreshold)
                    : 0;
            }

            // Combine main island and small islands
            var combinedNoise = math.max(blendedNoise * falloff, islandNoise * _parameters.islandStrength);

            // Create more distinct elevation levels
            combinedNoise = math.pow(combinedNoise, _parameters.heightCurve);

            return combinedNoise;
        }

        private float OctavedSimplexNoise(float2 pos, float noiseScale)
        {
            float noiseVal = 0;
            float amplitude = 1;
            float normalization = 0;
            var currentFreq = noiseScale;

            for (var o = 0; o < _parameters.octaves; o++)
            {
                // Get simplex noise and normalize to 0-1
                var n = (noise.snoise(pos / currentFreq) + 1) * 0.5f;

                // Track normalization
                normalization += amplitude;

                // Add to total
                noiseVal += n * amplitude;

                // Update frequency and amplitude for next octave
                currentFreq /= _parameters.frequency;
                amplitude /= _parameters.lacunarity;
            }

            // Normalize result to 0-1 range
            return noiseVal / normalization;
        }

        private float OctavedRidgeNoise(float2 pos, float noiseScale)
        {
            float noiseVal = 0;
            float amplitude = 1;
            float weight = 1;
            float normalization = 0;
            var currentFreq = noiseScale;

            for (var o = 0; o < _parameters.octaves; o++)
            {
                // Get ridge noise
                var n = 1 - math.abs(noise.snoise(pos / currentFreq));
                n *= n; // Square for sharper ridges
                n *= weight;

                // Track normalization
                normalization += amplitude;

                // Add to total
                noiseVal += n * amplitude;

                // Update weight for next octave
                weight = math.clamp(n * 2, 0, 1);

                // Update frequency and amplitude for next octave
                currentFreq /= _parameters.frequency;
                amplitude /= _parameters.lacunarity;
            }

            // Normalize result to 0-1 range
            return noiseVal / normalization;
        }

        private float FalloffMap(float2 distanceFromCenter, float radius)
        {
            // Calculate normalized distance from center (0-1 range)
            var distanceNormalized = math.length(distanceFromCenter) / radius;

            // Apply falloff curve - more land near center, falls off toward edges
            if (distanceNormalized > 1)
            {
                return 0;
            }

            // Apply falloff curve with plateau in center
            var falloffValue = 1 - math.pow(distanceNormalized, _parameters.falloffSteepness);

            // Optional flatter center
            if (_parameters.enableCentralPlateau && distanceNormalized < _parameters.plateauSize)
            {
                return 1.0f;
            }

            return falloffValue;
        }

        private BiomeType DetermineBiomeFromHeight(float height)
        {
            // Determine biome based solely on height
            if (height < _parameters.deepOceanThreshold)
                return BiomeType.DeepOcean;
            if (height < _parameters.oceanThreshold)
                return BiomeType.ShallowOcean;
            if (height < _parameters.beachThreshold)
                return BiomeType.Beach;
            if (height < _parameters.plainsThreshold)
                return BiomeType.Plains;
            if (height < _parameters.forestThreshold)
                return BiomeType.Forest;
            if (height < _parameters.hillsThreshold)
                return BiomeType.Hills;
            if (height < _parameters.mountainsThreshold)
                return BiomeType.Mountains;
            return BiomeType.Peaks;
        }
    }

    [Serializable]
    public struct IslandGenerationParameters
    {
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

        [Header("Small Islands Settings")] [Tooltip("Toggle to enable/disable generation of additional small islands")]
        public bool enableSmallIslands;

        [Tooltip("Scale of noise for small islands - affects island size and distribution")]
        public float islandNoiseScale;

        [Range(0, 1)] [Tooltip("Minimum noise value for small islands - higher means fewer islands")]
        public float islandThreshold;

        [Range(0, 1)] [Tooltip("How prominent small islands are relative to main island")]
        public float islandStrength;

        [Header("Island Shape")] [Tooltip("Toggle to create a flat area in the center of the island")]
        public bool enableCentralPlateau;

        [Range(0, 1)] [Tooltip("Radius of central plateau as fraction of total island size")]
        public float plateauSize;

        [Tooltip("Power function for elevation - higher values create more distinct levels")]
        public float heightCurve;

        [Range(0, 1)] public float deepOceanThreshold;
        [Range(0, 1)] public float oceanThreshold;
        [Range(0, 1)] public float beachThreshold;
        [Range(0, 1)] public float plainsThreshold;
        [Range(0, 1)] public float forestThreshold;
        [Range(0, 1)] public float hillsThreshold;
        [Range(0, 1)] public float mountainsThreshold;
    }
}