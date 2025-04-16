using System.Collections.Generic;
using System.Linq;
using Buildings;
using UnityEngine;

namespace GameResources
{
    public class ResourceManager : MonoBehaviour
    {
        public static ResourceManager Instance { get; private set; }

        public Dictionary<ResourceType, ResourceAmount> Resources { get; private set; } = new();

        private Dictionary<(string buildingId, string flowId), float> _resourceFlowTimers = new();

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

            // Initialize all defined resource types
            foreach (var resourceType in ResourceTypeDatabase.GetAllResources())
            {
                // TODO: Get value from save file
                Resources[resourceType] = new ResourceAmount(resourceType, 0);
            }

            // TODO: Subscribe to load/save events
        }

        private void Update()
        {
            foreach (var building in BuildingManager.Buildings)
            {
                var resourceFlow = building.ResourceFlow;
                var key = (building.buildingData.identifier, resourceFlow.identifier);

                var timer = _resourceFlowTimers.GetValueOrDefault(key, 0f);

                timer += Time.deltaTime;

                if (timer >= resourceFlow.intervalSeconds)
                {
                    ProcessResourceFlow(resourceFlow);
                    timer = 0f;
                }

                _resourceFlowTimers[key] = timer;
            }
        }

        #region Utility

        public bool HasEnoughResources(ResourceAmount resourceAmount)
        {
            if (resourceAmount == null || !resourceAmount.resourceType)
            {
                return false;
            }

            return Resources.TryGetValue(resourceAmount.resourceType, out var currentResourceAmount) &&
                   currentResourceAmount?.amount >= resourceAmount.amount;
        }

        public bool HasEnoughResources(IEnumerable<ResourceAmount> costs)
        {
            return costs.All(HasEnoughResources);
        }

        public void Consume(IEnumerable<ResourceAmount> amounts)
        {
            foreach (var amount in amounts)
            {
                if (Resources.TryGetValue(amount.resourceType, out var resource))
                {
                    resource.amount -= amount.amount;
                }
            }
        }

        public void Generate(IEnumerable<ResourceAmount> amounts)
        {
            foreach (var amount in amounts)
            {
                Resources[amount.resourceType].amount += amount.amount;
            }
        }

        public void ProcessResourceFlow(ResourceFlow flow)
        {
            var inputsConsuming = flow.inputsConsuming;
            var outputsGenerating = flow.outputsGenerating;

            if (!HasEnoughResources(inputsConsuming)) return;
            Consume(inputsConsuming);
            Generate(outputsGenerating);
        }

        #endregion
    }
}