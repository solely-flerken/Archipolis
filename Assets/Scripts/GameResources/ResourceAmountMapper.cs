namespace GameResources
{
    public static class ResourceAmountMapper
    {
        public static ResourceAmount ToEntity(this ResourceAmountDto dto)
        {
            var resourceType = ResourceTypeDatabase.GetResourceByID(dto.resourceTypeId);
            return new ResourceAmount(resourceType, dto.amount);
        }

        public static ResourceAmountDto ToDto(this ResourceAmount entity)
        {
            return new ResourceAmountDto
            {
                resourceTypeId = entity.resourceType.identifier,
                amount = entity.amount
            };
        }
    }
}