using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

namespace OpenSkiJumping.Hills.TerrainGenerator
{

    [XmlRoot("osm")]
    [Serializable]
    public class Osm
    {
        [XmlElement("bounds")] public Bounds bounds;
        [XmlElement("node")] public List<Node> nodes = new List<Node>();
        [XmlElement("way")] public List<Way> ways = new List<Way>();
        [XmlElement("relation")] public List<Relation> relations = new List<Relation>();
    }

    [Serializable]
    public class Bounds
    {
        [XmlAttribute("minlat")] public float minlat;
        [XmlAttribute("minlon")] public float minlon;
        [XmlAttribute("maxlat")] public float maxlat;
        [XmlAttribute("maxlon")] public float maxlon;
    }

    [Serializable]
    public class Node
    {
        [XmlAttribute("id")] public string id;
        [XmlAttribute("lat")] public float lat;
        [XmlAttribute("lon")] public float lon;
        [XmlElement("tag")] public List<Tag> tags = new List<Tag>();
    }
    
    [Serializable]

    public class Nd
    {
        [XmlAttribute("ref")] public string id;
    }

    [Serializable]
    public class Tag
    {
        [XmlAttribute("k")] public string k;
        [XmlAttribute("v")] public string v;
    }

    [Serializable]
    public class Member
    {
        [XmlAttribute("ref")] public string id;
        [XmlAttribute("type")] public string type;
        [XmlAttribute("role")] public string role;
    }

    [Serializable]
    public class Way
    {
        [XmlAttribute("id")] public string id;
        [XmlElement("nd")] public List<Nd> nodeRefs = new List<Nd>();
        [XmlElement("tag")] public List<Tag> tags = new List<Tag>();
    }

    [Serializable]
    public class Relation
    {
        [XmlAttribute("id")] public string id;
        [XmlElement("member")] public List<Member> members = new List<Member>();
        [XmlElement("tag")] public List<Tag> tags = new List<Tag>();
    }
    [CreateAssetMenu(menuName = "ScriptableObjects/OsmReader")]
    public class OsmReader : ScriptableObject
    {
        [SerializeField] private string path;
        [SerializeField] private Osm osm;

        private Dictionary<string, Node> _nodesDict;
        private Dictionary<string, Way> _waysDict;
        private Dictionary<string, Relation> _relationsDict;

        public IEnumerable<Vector2> GetWayById(string id)
        {
            return _waysDict[id].nodeRefs.Select(it => _nodesDict[it.id]).Select(x => new Vector2(x.lat, x.lon));
        }

        public void Read()
        {
            var serializer = new XmlSerializer(typeof(Osm));
            var fs = new FileStream(path, FileMode.Open);
            osm = (Osm) serializer.Deserialize(fs);
            Initialize();
        }

        private void Initialize()
        {
            _nodesDict = osm.nodes.ToDictionary(x => x.id);
            _waysDict = osm.ways.ToDictionary(x => x.id);
            _relationsDict = osm.relations.ToDictionary(x => x.id);
        }
    }
}