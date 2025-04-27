using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;

namespace Terrain
{
    public struct MapBiomeGenerator : IJobParallelFor
    {
        [ReadOnly] private NativeArray<float3> _positions;
        [WriteOnly] private NativeArray<BiomeType> _biomeMap;
        [WriteOnly] private NativeArray<float> _heightMap;

        private readonly MapGenerationParameters _parameters;
        private readonly float2 _mapCenter;
        private readonly float _mapRadius;
        private readonly float2 _seedOffset;

        public MapBiomeGenerator(MapGenerationParameters parameters, NativeArray<float3> positions,
            NativeArray<BiomeType> biomeMap, NativeArray<float> heightMap,
            float2 mapCenter, float mapRadius)
        {
            _positions = positions;
            _biomeMap = biomeMap;
            _heightMap = heightMap;
            _parameters = parameters;
            _mapCenter = mapCenter;
            _mapRadius = mapRadius;

            _seedOffset = new float2(
                math.sin(_parameters.seed * 12.9898f) * 43758.5453f % 1f,
                math.sin(_parameters.seed * 78.233f) * 43758.5453f % 1f
            ) * 10000f;
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

            // Create more distinct elevation levels
            var combinedNoise = math.pow(blendedNoise * falloff, _parameters.heightCurve);

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
                var n = (noise.snoise((pos + _seedOffset) / currentFreq) + 1) * 0.5f;

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
                var n = 1 - math.abs(noise.snoise((pos + _seedOffset) / currentFreq));
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

            // Apply falloff curve
            return 1 - math.pow(distanceNormalized, _parameters.falloffSteepness);
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
}