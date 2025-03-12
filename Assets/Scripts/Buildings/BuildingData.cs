using System;
using UnityEngine;

namespace Buildings
{
    public class BuildingData
    {
        public string ID { get; }
        public string Name { get; }
        public GameObject Prefab { get; }

        public BuildingData(string name, GameObject prefab)
        {
            ID = Guid.NewGuid().ToString();
            Name = name;
            Prefab = prefab;
        }
    }
}