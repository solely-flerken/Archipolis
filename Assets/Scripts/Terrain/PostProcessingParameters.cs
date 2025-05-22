using System;
using UnityEngine;

namespace Terrain
{
    [Serializable]
    public struct PostProcessingParameters
    {
        [Header("Post-Processing Settings")] 
        [Min(0)] public int minBiomeTissueSize;

        public static PostProcessingParameters Default()
        {
            return new PostProcessingParameters
            {
                minBiomeTissueSize = 4
            };
        }
    }
}