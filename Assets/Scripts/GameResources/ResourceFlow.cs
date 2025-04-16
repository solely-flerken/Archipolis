using System;
using System.Collections.Generic;

namespace GameResources
{
    [Serializable]
    public class ResourceFlow
    {
        public string identifier = Guid.NewGuid().ToString();
        public List<ResourceAmount> inputsConsuming;
        public List<ResourceAmount> outputsGenerating;
        public float intervalSeconds = 60f;

        public ResourceFlow Clone()
        {
            var clone = new ResourceFlow
            {
                identifier = Guid.NewGuid().ToString(),
                intervalSeconds = intervalSeconds,
                inputsConsuming = new List<ResourceAmount>(),
                outputsGenerating = new List<ResourceAmount>()
            };

            // Deep copy
            foreach (var input in inputsConsuming)
            {
                clone.inputsConsuming.Add(input.Clone()); 
            }

            // Deep copy
            foreach (var output in outputsGenerating)
            {
                clone.outputsGenerating.Add(output.Clone());
            }
            
            return clone;
        }
    }
}