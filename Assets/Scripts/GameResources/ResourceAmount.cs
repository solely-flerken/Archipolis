using System;
using System.Globalization;
using UI;

namespace GameResources
{
    [Serializable]
    public class ResourceAmount
    {
        public ResourceType resourceType;
        public float amount;

        public ResourceAmount(ResourceType resourceType, float amount)
        {
            this.resourceType = resourceType;
            this.amount = amount;
        }
        
        public ResourceAmount Clone()
        {
            return new ResourceAmount(resourceType, amount);
        }
    }
    
    /// <summary>
    /// Serializable data transfer object (DTO) used to save and load resource amounts,
    /// storing only the resource type's identifier instead of a direct reference and all its data.
    /// This avoids serialization issues with ScriptableObject references (e.g. ResourceType),
    /// which Unity's built-in serializers cannot handle.
    /// </summary>
    [Serializable]
    public class ResourceAmountDto
    {
        public string resourceTypeId;
        public float amount;
    }
    
    public class ViewModelResourceAmount
    {
        public readonly BindableProperty<float> Amount;
        public readonly BindableProperty<string> FormattedAmount;

        public ViewModelResourceAmount(ResourceAmount resourceAmount)
        {
            Amount = BindableProperty<float>.Bind(() => resourceAmount.amount);
            FormattedAmount = BindableProperty<string>.Bind(() =>
                $"{resourceAmount.resourceType.name}: {Amount.Value.ToString("0", CultureInfo.InvariantCulture)}");
        }
    }
}