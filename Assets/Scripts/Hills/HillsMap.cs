using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace OpenSkiJumping.Hills
{
    [Serializable]
    public class SerializableTransform
    {
        public Vector3 position = Vector3.zero;
        public Quaternion rotation = Quaternion.identity;
        public Vector3 EulerAngles => rotation.eulerAngles;
        public Vector3 scale = Vector3.one;

        public static SerializableTransform Identity => new()
            {position = Vector3.zero, rotation = Quaternion.identity, scale = Vector3.one};

        public static SerializableTransform Minus => new()
            {position = Vector3.zero, rotation = Quaternion.identity, scale = -Vector3.one};

        public static SerializableTransform FromTransform(Transform transform) =>
            new() {position = transform.position, rotation = transform.rotation, scale = transform.localScale};

        public SerializableTransform Inverse()
        {
            return new() {position = -position, rotation = Quaternion.Inverse(rotation), scale = -scale};
        }


        public SerializableTransform Clone()
        {
            var other = (SerializableTransform) MemberwiseClone();
            return other;
        }
    }


    [Serializable]
    public class MapHillData
    {
        public ReferencePoint anchor;
        public ReferencePoint refTransform;
        public ProfileData profileData;
    }

    public enum NodeType
    {
        Line,
        Arc,
        Bezier3,
        Bezier2,
        Path
    }

    public enum MaterialType
    {
        Planks,
        Glass,
        Concrete,
        MetalPlate,
    }

    [Serializable]
    public class ReferenceScalar
    {
        public float value;
        public string referenceId = "";
    }

    [Serializable]
    public class Length
    {
        public ReferenceScalar value = new();
        public Unit unit = Unit.Percent;

        public enum Unit
        {
            Percent,
            Meter,
            Centimeter
        }

        private static readonly Regex MatchRegex = new(@"^([+-]?\d+(?:\.\d+)?)(m|%|cm|)$", RegexOptions.Compiled);

        private void SetValuesFromString(string val)
        {
            var match = MatchRegex.Match(val);
            var valueMatch = match.Groups[1].Value;
            value.value = float.Parse(valueMatch);
            var unitMatch = match.Groups[2].Value;
            unit = unitMatch switch
            {
                "m" => Unit.Meter,
                "" => Unit.Meter,
                "%" => Unit.Percent,
                "cm" => Unit.Centimeter,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        [XmlIgnore][JsonIgnore]
        public string ValUnit
        {
            get => $"{value.value.ToString(CultureInfo.InvariantCulture)}{unit switch {Unit.Percent => "%", _ => ""}}";
            set => SetValuesFromString(value);
        }

        public static Length Percent() => new() {unit = Unit.Percent};
        public static Length Meter() => new() {unit = Unit.Meter};
    }


    [Serializable]
    public class ReferencePoint
    {
        public string id = "";
        public string referenceId = "";
        public SerializableTransform value;
        public List<ReferencePoint> auxiliaryRefs = new();

        public static ReferencePoint Neutral =>
            new() {id = "", referenceId = "", value = SerializableTransform.Identity};

        public static ReferencePoint FromPos(string id, Vector3 pos, string referenceId = "")
        {
            return new()
            {
                id = id,
                value = new SerializableTransform {position = pos},
                referenceId = referenceId
            };
        }

        public static ReferencePoint FromRefId(string referenceId = "")
        {
            return new()
            {
                id = "",
                value = SerializableTransform.Identity,
                referenceId = referenceId
            };
        }

        public static ReferencePoint FromTransform(string id, Transform transform, string referenceId = "")
        {
            return new()
            {
                id = id,
                value = SerializableTransform.FromTransform(transform),
                referenceId = referenceId
            };
        }

        public static ReferencePoint FromPosRotScale(string id, Vector3 pos, Quaternion rot, Vector3 scale,
            string referenceId = "")
        {
            return new()
            {
                id = id,
                value = new SerializableTransform {position = pos, rotation = rot, scale = scale},
                referenceId = referenceId
            };
        }

        public ReferencePoint Clone()
        {
            var other = (ReferencePoint) MemberwiseClone();
            other.id = string.Copy(id);
            other.referenceId = string.Copy(referenceId);
            other.value = value.Clone();
            other.auxiliaryRefs = auxiliaryRefs.Select(it => it.Clone()).ToList();
            return other;
        }
    }

    [Serializable]
    public class PathNode
    {
        public NodeType nodeType;
        public ReferencePoint target = ReferencePoint.Neutral;
        public ReferencePoint c0 = ReferencePoint.Neutral;
        public ReferencePoint c1 = ReferencePoint.Neutral;
        public string pathId = "";

        public static PathNode Line(ReferencePoint target)
        {
            return new() {nodeType = NodeType.Line, target = target};
        }

        public static PathNode Arc(ReferencePoint target, ReferencePoint c0)
        {
            return new() {nodeType = NodeType.Arc, target = target, c0 = c0};
        }

        public static PathNode Bezier3(ReferencePoint target, ReferencePoint c0, ReferencePoint c1)
        {
            return new() {nodeType = NodeType.Bezier3, target = target, c0 = c0, c1 = c1};
        }

        public static PathNode Bezier2(ReferencePoint target, ReferencePoint c0)
        {
            return new() {nodeType = NodeType.Bezier2, target = target, c0 = c0};
        }

        public static PathNode Path(ReferencePoint target, string id)
        {
            return new() {nodeType = NodeType.Path, target = target, pathId = id};
        }

        public PathNode Clone()
        {
            var other = (PathNode) MemberwiseClone();
            other.target = target?.Clone();
            other.c0 = c0?.Clone();
            other.c1 = c1?.Clone();
            other.pathId = string.Copy(pathId);
            return other;
        }
    }

    [Serializable]
    public class Path3D
    {
        public string id;
        public ReferencePoint refPoint;
        public List<PathNode> data;

        // public void AddNode(NodeType nodeType, string refId, Vector3 target, Vector3 c0 = default, Vector3 c1 = default)
        // {
        //     data.Add(new PathNode
        //     {
        //         nodeType = nodeType, referenceId = refId, target = SerializableVec3.FromVector3(target),
        //         c0 = SerializableVec3.FromVector3(c0), c1 = SerializableVec3.FromVector3(c1)
        //     });
        // }
    }

    [Serializable]
    public class TerrainNode
    {
        public ReferencePoint refPoint;
        public float weight;
    }

    [Serializable]
    public class Wall
    {
        public string centerPathId = "";
        public ReferencePoint refPoint;
        [FormerlySerializedAs("width")] public float w1 = 1;
        public float w2 = 1;
        [FormerlySerializedAs("height")] public float h1 = 1;
        public float h2 = 1;
        public float t0 = 0;
        public float t1 = 1;
        public int count = 1;
        public float length = 1;
        public MaterialType material;
    }

    [Serializable]
    public class ConstructionPath
    {
        public string idY = "";
        public string idZ = "";
        public float shift;
        public ReferencePoint refPoint = ReferencePoint.Neutral;
    }


    [Serializable]
    public class Construction
    {
        public ConstructionPath centerPath;
        public ConstructionPath bottomLeftPath;
        public ConstructionPath bottomRightPath;
        public ConstructionPath topLeftPath;
        public ConstructionPath topRightPath;

        public Length t0;
        public Length t1;
        public int count;
        public Length step;
        public Length length;
        public MaterialType material;
    }

    [Serializable]
    public class Stairs
    {
        public ConstructionPath centerPath;
        public ConstructionPath leftPath;
        public ConstructionPath rightPath;
        public ConstructionPath topPath;

        // public string centerPathId = "";
        // public ReferencePoint centerRefPoint = ReferencePoint.Neutral;
        //
        // public string leftPathId = "";
        // public ReferencePoint leftRefPoint = ReferencePoint.Neutral;
        //
        // public string rightPathId = "";
        // public ReferencePoint rightRefPoint = ReferencePoint.Neutral;

        public float t0;
        public float t1;
        public float stepLength;
        public MaterialType material;
    }


    [Serializable]
    public class HillsMap
    {
        public Vector3 pos;
        public List<MapHillData> profiles;
        public List<ReferencePoint> referencePoints;
        public List<Path3D> paths;
        public List<Wall> walls;
        public List<Construction> constructions;
        public List<Stairs> stairs;
        public List<TerrainNode> terrainNodes;
    }
}