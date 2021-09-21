using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using OpenSkiJumping.Hills;
using OpenSkiJumping.ScriptableObjects;
using UnityEngine;

namespace OpenSkiJumping.Data
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Data/HillsRuntimeXml")]
    public class HillsRuntimeXml : DatabaseObject<MapX>
    {
        [SerializeField] private HillsMap hillsMap;

        public override bool LoadData()
        {
            var filePath = Path.Combine(Application.streamingAssetsPath, path);
            var serializer = new XmlSerializer(typeof(MapX));
            var fs = new FileStream(filePath, FileMode.Open);
            data = (MapX) serializer.Deserialize(fs);
            hillsMap = HillsMapFactory.GetHillsMap(data);
            fs.Close();
            return true;
        }

        public override void SaveData()
        {
            data = HillsMapFactory.ParseHillsMap(hillsMap);
            var filePath = Path.Combine(Application.streamingAssetsPath, path);
            var serializer = new XmlSerializer(typeof(MapX));
            TextWriter writer = new StreamWriter(filePath);
            serializer.Serialize(writer, data);
            writer.Close();
        }

        public override void Reset()
        {
        }
    }
}