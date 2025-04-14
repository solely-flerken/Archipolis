using System;
using System.Globalization;
using UI;

namespace GameResources
{
    [Serializable]
    public class ResourceAmount
    {
        public ResourceType ResourceType { get; set; }
        public float Amount { get; set; }

        public ResourceAmount(ResourceType resourceType, int amount)
        {
            ResourceType = resourceType;
            Amount = amount;
        }
    }

    public class ViewModelResourceAmount
    {
        public readonly BindableProperty<float> Amount;
        public readonly BindableProperty<string> FormattedAmount;

        public ViewModelResourceAmount(ResourceAmount resourceAmount)
        {
            Amount = BindableProperty<float>.Bind(() => resourceAmount.Amount);
            FormattedAmount = BindableProperty<string>.Bind(() =>
                $"{resourceAmount.ResourceType.name}: {Amount.Value.ToString("0", CultureInfo.InvariantCulture)}");
        }
    }
}