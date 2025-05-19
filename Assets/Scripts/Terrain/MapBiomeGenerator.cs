using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;

namespace Terrain
{
    public struct MapBiomeGenerator : IJobParallelFor
    {
        [ReadOnly] private NativeArray<float3> _positions;
        [ReadOnly] private NativeArray<float2> _octaveOffsets;
        [WriteOnly] private NativeArray<BiomeType> _biomeMap;
        [WriteOnly] private NativeArray<float> _heightMap;

        private readonly MapGenerationParameters _parameters;
        private readonly float2 _mapCenter;
        private readonly float _mapRadius;
        private readonly float _offsetSpeed;

        public MapBiomeGenerator(MapGenerationParameters parameters, NativeArray<float3> positions,
            NativeArray<float2> octaveOffsets,
            NativeArray<BiomeType> biomeMap, NativeArray<float> heightMap,
            float2 mapCenter, float mapRadius)
        {
            _positions = positions;
            _octaveOffsets = octaveOffsets;
            _biomeMap = biomeMap;
            _heightMap = heightMap;
            _parameters = parameters;
            _mapCenter = mapCenter;
            _mapRadius = mapRadius;

            // a unit equals an offset of 1/10 the map radius
            _offsetSpeed = _mapRadius / 10;
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
            var n = 0f;
            var frequency = 1f;
            var amplitude = 1f;
            var totalAmplitude = 0f;
            var scale = _parameters.scale;

            // Main noise generation
            for (var i = 0; i < _parameters.octaves; i++)
            {
                // Calculate noise at this octave
                n += amplitude * noise.snoise((pos + _parameters.offset * _offsetSpeed) / scale * frequency +
                                              _octaveOffsets[i]);

                // Keep track of total amplitude for normalization
                totalAmplitude += amplitude;

                amplitude *= _parameters.persistence;
                frequency *= _parameters.lacunarity;
            }

            n /= totalAmplitude;

            // Normalize to [0,1]
            n = n * 0.5f + 0.5f;

            // Falloff
            const float a = 3f;
            var b = _parameters.falloffSharpness;
            // Normalized distance
            var d = math.length(pos - _mapCenter) / _mapRadius;
            var falloff = math.pow(d, a) / (math.pow(d, a) + math.pow(b - b * d, a));
            n = math.lerp(n, n - falloff, _parameters.falloffInfluence);
            n = math.saturate(n);

            // terrainSharpnessExponent > 1 -> sharpens (emphasizes peaks, flattens valleys).
            // terrainSharpnessExponent < 1 -> flattens (lifts valleys, smooths peaks).
            n = math.pow(n, _parameters.terrainSharpnessExponent);

            return n;
        }

        private BiomeType DetermineBiomeFromHeight(float height)
        {
            // Determine biome based solely on height
            if (height < 0)
                return BiomeType.Unknown;
            if (height <= _parameters.deepOcean)
                return BiomeType.DeepOcean;
            if (height <= _parameters.ocean)
                return BiomeType.ShallowOcean;
            if (height <= _parameters.beach)
                return BiomeType.Beach;
            if (height <= _parameters.plains)
                return BiomeType.Plains;
            if (height <= _parameters.forest)
                return BiomeType.Forest;
            if (height <= _parameters.hills)
                return BiomeType.Hills;
            if (height <= _parameters.mountains)
                return BiomeType.Mountains;
            if (height <= _parameters.peaks)
                return BiomeType.Peaks;
            return BiomeType.Unknown;
        }
    }
}