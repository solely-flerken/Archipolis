using System;
using UnityEngine;

namespace GameResources
{
    [CreateAssetMenu(fileName = "Resource", menuName = "Game/ResourceType")]
    public class ResourceType : ScriptableObject
    {
        public string identifier = Guid.NewGuid().ToString();
        public string resourceName;
        public ResourceCategory category;
        public Sprite icon;
    }
}