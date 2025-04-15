using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameResources
{
    public class ResourceManager : MonoBehaviour
    {
        public static ResourceManager Instance { get; private set; }

        public Dictionary<ResourceType, ResourceAmount> Resources { get; private set; } = new();

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

            // TODO: Subscribe to load/save events
        }

        private void Start()
        {
            foreach (var resourceType in ResourceTypeDatabase.GetAllResources())
            {
                // TODO: Get value from save file
                Resources.Add(new ResourceAmount(resourceType, 0));
            }
        }

        private void Update()
        {
            // TODO: Remove this update, is only for testing
            foreach (var resourceAmount in Resources.Values)
            {
                Resources[resourceAmount.resourceType].amount += 1 * Time.deltaTime;
            }
        }
    }
}