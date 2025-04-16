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