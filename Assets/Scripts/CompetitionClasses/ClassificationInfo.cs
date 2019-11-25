using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CompCal
{

    [Serializable]
    public class ClassificationInfo
    {
        public string name;
        [JsonConverter(typeof(StringEnumConverter))]
        public ClassificationType classificationType;
        public List<int> events;

        public ClassificationInfo(string _name, ClassificationType _classificationType)
        {
            name = _name;
            classificationType = _classificationType;
            events = new List<int>();
        }
    }


}
