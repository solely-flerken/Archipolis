using System;
using System.Collections.Generic;
using System.Linq;

namespace GameResources
{
    [Serializable]
    public class ResourceFlowTimerEntry
    {
        public string buildingId;
        public float timer;

        public ResourceFlowTimerEntry(string buildingId, float timer)
        {
            this.buildingId = buildingId;
            this.timer = timer;
        }
    }

    public static class ResourceFlowTimerMapper
    {
        public static Dictionary<string, float> DeserializeToDictionary(
            this List<ResourceFlowTimerEntry> list)
        {
            var dictionary = new Dictionary<string, float>();

            if (list == null)
            {
                return dictionary;
            }

            foreach (var entry in list)
            {
                dictionary[entry.buildingId] = entry.timer;
            }

            return dictionary;
        }

        public static List<ResourceFlowTimerEntry> ToSerializableList(
            this Dictionary<string, float> dictionary)
        {
            var list = new List<ResourceFlowTimerEntry>();

            if (dictionary == null)
            {
                return list;
            }

            list.AddRange(dictionary.Select(kvp =>
                new ResourceFlowTimerEntry(kvp.Key,kvp.Value)));

            return list;
        }
    }
}