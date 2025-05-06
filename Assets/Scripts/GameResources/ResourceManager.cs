using System.Collections.Generic;
using System.Linq;
using Buildings;
using Events;
using Save;
using UnityEngine;

namespace GameResources
{
    public class ResourceManager : MonoBehaviour
    {
        public static ResourceManager Instance { get; private set; }

        public static readonly Dictionary<ResourceType, ResourceAmount> Resources = new();

        public static Dictionary<string, float> ResourceFlowTimers = new();

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

            // We need to initialize here to successfully bind the UI
            foreach (var resourceType in ResourceTypeDatabase.GetAllResources())
            {
                Resources[resourceType] = new ResourceAmount(resourceType, 0f);
            }
        }

        private void Start()
        {
            EventSystem.Instance.OnLoadGame += HandleGameLoad;
        }

        private void Update()
        {
            foreach (var building in BuildingManager.Buildings)
            {
                var resourceFlow = building.ResourceFlow;
                var key = building.buildingData.identifier;

                var timer = ResourceFlowTimers.GetValueOrDefault(key, 0f);

                timer += Time.deltaTime;

                if (timer >= resourceFlow.intervalSeconds)
                {
                    ProcessResourceFlow(resourceFlow);
                    timer = 0f;
                }

                ResourceFlowTimers[key] = timer;
            }
        }

        private void OnDestroy()
        {
            EventSystem.Instance.OnLoadGame -= HandleGameLoad;
        }

        #region Events

        private static void HandleGameLoad(BaseSaveData saveData)
        {
            foreach (var resource in saveData.resources.Select(r => r.ToEntity()))
            {
                Resources[resource.resourceType].amount = resource.amount;
            }

            ResourceFlowTimers = saveData.resourceFlowTimers.DeserializeToDictionary();
        }

        #endregion

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