using System.Collections.Generic;
using UnityEngine;

namespace GameResources
{
    public static class ResourceTypeDatabase
    {
        private static readonly Dictionary<string, ResourceType> ResourceMap = new();
        
        static ResourceTypeDatabase()
        {
            Initialize();
        }
        
        private static void Initialize()
        {
            foreach (var resource in Resources.LoadAll<ResourceType>("ResourceTypes"))
            {
                if (resource == null)
                {
                    Debug.LogError($"Invalid resource blueprint found.");
                    continue;
                }

                if (!ResourceMap.TryAdd(resource.identifier, resource))
                {
                    Debug.LogWarning($"Duplicate resource identifier: {resource.identifier}");
                }
            }
        }

        public static ResourceType GetResourceByID(string id)
        {
            ResourceMap.TryGetValue(id, out var resource);
            return resource;
        }

        public static IReadOnlyCollection<ResourceType> GetAllResources()
        {
            return ResourceMap.Values;
        }
    }
}