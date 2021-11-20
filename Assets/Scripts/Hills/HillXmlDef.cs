using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using OpenSkiJumping.Hills.CurvePaths;
using UnityEngine;

namespace OpenSkiJumping.Hills
{
    [Serializable]
    public class Vector3X
    {
        [XmlAttribute("x")] public float x;
        [XmlAttribute("y")] public float y;
        [XmlAttribute("z")] public float z;

        public Vector3 ToVector3()
        {
            return new(x, y, z);
        }

        public static Vector3X FromVector3(Vector3 item)
        {
            return new() {x = item.x, y = item.y, z = item.z};
        }

        public static Vector3X One => new() {x = 1, y = 1, z = 1};
        public static Vector3X Zero => new() {x = 0, y = 0, z = 0};
    }

    [Serializable]
    public class InrunDataX
    {
        [XmlAttribute("gates")] public int gates;
        [XmlAttribute("e")] public float e;
        [XmlAttribute("es")] public float es;
        [XmlAttribute("r1")] public float r1;
        [XmlAttribute("t")] public float t;
        [XmlAttribute("gamma")] public float gamma;
        [XmlAttribute("alpha")] public float alpha;
        [XmlAttribute("b1")] public float b1;
    }

    [Serializable]
    public class LandingHillX
    {
        [XmlAttribute("w")] public float w;
        [XmlAttribute("hs")] public float hs;
        [XmlAttribute("h")] public float h;
        [XmlAttribute("n")] public float n;
        [XmlAttribute("s")] public float s;
        [XmlAttribute("l1")] public float l1;
        [XmlAttribute("l2")] public float l2;
        [XmlAttribute("rL")] public float rL;
        [XmlAttribute("r2L")] public float r2L;
        [XmlAttribute("r2")] public float r2;
        [XmlAttribute("beta0")] public float beta0;
        [XmlAttribute("betaP")] public float betaP;
        [XmlAttribute("betaK")] public float betaK;
        [XmlAttribute("betaL")] public float betaL;
        [XmlAttribute("betaU")] public float betaU = 0;
        [XmlAttribute("b2")] public float b2;
        [XmlAttribute("bK")] public float bK;
        [XmlAttribute("bU")] public float bU;
    }

    [Serializable]
    public class OutrunX
    {
        [XmlAttribute("a")] public float a;
        [XmlAttribute("rA")] public float rA;
        [XmlAttribute("betaA")] public float betaA;
    }

    [Serializable]
    public class JudgesTowerX
    {
        [XmlAttribute("d")] public float d;
        [XmlAttribute("q")] public float q;
        [XmlAttribute("height")] public float g;
    }

    [Serializable]
    public class HillTerrainX
    {
        [XmlAttribute("inrun-terrain")] public float terrainSteepness;
        [XmlAttribute("terrain-offset")] public float terrainOffset;
    }

    [Serializable]
    public class HillX
    {
        [XmlAttribute("id")] public string id;
        [XmlElement("anchor")] public RefPointX anchor = new();
        [XmlIgnore] public RefPointX refPoint = new();
        [XmlElement("inrun")] public InrunDataX inrun;
        [XmlElement("landing-hill")] public LandingHillX landingHill;
        [XmlElement("outrun")] public OutrunX outrun;
        [XmlElement("judges-tower")] public JudgesTowerX judgesTower;

        [XmlAttribute("ref-id")]
        public string RefId
        {
            get => refPoint.refId;
            set => refPoint.refId = value;
        }

        [XmlAttribute("x")]
        public float X
        {
            get => refPoint.pos.x;
            set => refPoint.pos.x = value;
        }

        [XmlAttribute("y")]
        public float Y
        {
            get => refPoint.pos.y;
            set => refPoint.pos.y = value;
        }

        [XmlAttribute("z")]
        public float Z
        {
            get => refPoint.pos.z;
            set => refPoint.pos.z = value;
        }

        [XmlAttribute("rx")]
        public float Rx
        {
            get => refPoint.rot.x;
            set => refPoint.rot.x = value;
        }

        [XmlAttribute("ry")]
        public float Ry
        {
            get => refPoint.rot.y;
            set => refPoint.rot.y = value;
        }

        [XmlAttribute("rz")]
        public float Rz
        {
            get => refPoint.rot.z;
            set => refPoint.rot.z = value;
        }
    }


    [Serializable]
    public class AnonymousRefPointX
    {
        [XmlAttribute("ref-id")] public string refId = "";
        [XmlIgnore] public Vector3X pos = Vector3X.Zero;
        [XmlIgnore] public Vector3X rot = Vector3X.Zero;
        [XmlIgnore] public Vector3X scl = Vector3X.One;

        [XmlAttribute("x"), DefaultValue(0)]
        public float X
        {
            get => pos.x;
            set => pos.x = value;
        }

        [XmlAttribute("y"), DefaultValue(0)]
        public float Y
        {
            get => pos.y;
            set => pos.y = value;
        }

        [XmlAttribute("z"), DefaultValue(0)]
        public float Z
        {
            get => pos.z;
            set => pos.z = value;
        }

        [XmlAttribute("rx"), DefaultValue(0)]
        public float Rx
        {
            get => rot.x;
            set => rot.x = value;
        }

        [XmlAttribute("ry"), DefaultValue(0)]
        public float Ry
        {
            get => rot.y;
            set => rot.y = value;
        }

        [XmlAttribute("rz"), DefaultValue(0)]
        public float Rz
        {
            get => rot.z;
            set => rot.z = value;
        }

        [XmlAttribute("sx"), DefaultValue(1)]
        public float Sx
        {
            get => scl.x;
            set => scl.x = value;
        }

        [XmlAttribute("sy"), DefaultValue(1)]
        public float Sy
        {
            get => scl.y;
            set => scl.y = value;
        }

        [XmlAttribute("sz"), DefaultValue(1)]
        public float Sz
        {
            get => scl.z;
            set => scl.z = value;
        }
    }

    [Serializable]
    public class RefPointX : AnonymousRefPointX
    {
        [XmlIgnore, DefaultValue("")] public string id = "";

        [XmlAttribute("id"), DefaultValue("")]
        public string Id
        {
            get => id;
            set => id = value;
        }

        [XmlElement("aux")] public List<AnonymousRefPointX> auxiliaryRefs = new();
    }


    [Serializable]
    public class NodeX
    {
        [XmlIgnore] public AnonymousRefPointX target = new RefPointX();

        [XmlAttribute("ref-id"), DefaultValue("")]
        public string RefId
        {
            get => target.refId;
            set => target.refId = value;
        }

        [XmlAttribute("x"), DefaultValue(0)]
        public float X
        {
            get => target.pos.x;
            set => target.pos.x = value;
        }

        [XmlAttribute("y"), DefaultValue(0)]
        public float Y
        {
            get => target.pos.y;
            set => target.pos.y = value;
        }

        [XmlAttribute("z"), DefaultValue(0)]
        public float Z
        {
            get => target.pos.z;
            set => target.pos.z = value;
        }
    }

    [Serializable]
    public class LineNodeX : NodeX
    {
    }

    [Serializable]
    public class Bezier2NodeX : NodeX
    {
        [XmlElement("c0")] public RefPointX c0;
    }

    [Serializable]
    public class Bezier3NodeX : NodeX
    {
        [XmlElement("c0")] public RefPointX c0;
        [XmlElement("c1")] public RefPointX c1;
    }

    [Serializable]
    public class ArcNodeX : NodeX
    {
        [XmlElement("c0")] public RefPointX c0;
        // [XmlElement("large-circle")] public int largeCircle;
    }

    [Serializable]
    public class PathNodeX : NodeX
    {
        [XmlAttribute("id")] public string pathId;
        // [XmlElement("c0")] public RefPointX c0;
        // [XmlElement("c1")] public RefPointX c1;
    }


    [Serializable]
    public class PathX
    {
        [XmlAttribute("id")] public string id;
        [XmlIgnore] public RefPointX refPoint = new();

        [XmlElement("line", typeof(LineNodeX))]
        [XmlElement("bezier3", typeof(Bezier3NodeX))]
        [XmlElement("bezier2", typeof(Bezier2NodeX))]
        [XmlElement("arc", typeof(ArcNodeX))]
        [XmlElement("path", typeof(PathNodeX))]
        public List<NodeX> nodes = new();

        [XmlAttribute("x"), DefaultValue(0)]
        public float X
        {
            get => refPoint.pos.x;
            set => refPoint.pos.x = value;
        }

        [XmlAttribute("y"), DefaultValue(0)]
        public float Y
        {
            get => refPoint.pos.y;
            set => refPoint.pos.y = value;
        }

        [XmlAttribute("z"), DefaultValue(0)]
        public float Z
        {
            get => refPoint.pos.z;
            set => refPoint.pos.z = value;
        }

        [XmlAttribute("rx"), DefaultValue(0)]
        public float Rx
        {
            get => refPoint.rot.x;
            set => refPoint.rot.x = value;
        }

        [XmlAttribute("ry"), DefaultValue(0)]
        public float Ry
        {
            get => refPoint.rot.y;
            set => refPoint.rot.y = value;
        }

        [XmlAttribute("rz"), DefaultValue(0)]
        public float Rz
        {
            get => refPoint.rot.z;
            set => refPoint.rot.z = value;
        }

        [XmlAttribute("sx"), DefaultValue(1)]
        public float Sx
        {
            get => refPoint.scl.x;
            set => refPoint.scl.x = value;
        }

        [XmlAttribute("sy"), DefaultValue(1)]
        public float Sy
        {
            get => refPoint.scl.y;
            set => refPoint.scl.y = value;
        }

        [XmlAttribute("sz"), DefaultValue(1)]
        public float Sz
        {
            get => refPoint.scl.z;
            set => refPoint.scl.z = value;
        }
        
        [XmlAttribute("ref-id"), DefaultValue("")]
        public string RefId
        {
            get => refPoint.refId;
            set => refPoint.refId = value;
        }
    }

    [Serializable]
    public class ConstructionPathX : RefPointX
    {
        [XmlIgnore] public string idY = "";
        [XmlIgnore] public string idZ = "";

        [XmlAttribute("shift"), DefaultValue(0)]
        public float shift;
        
        [XmlAttribute("id"), DefaultValue("")]
        public new string Id
        {
            get => id;
            set
            {
                id = value;
                idY = "";
                idZ = "";
            }
        }

        [XmlAttribute("id-y"), DefaultValue("")]
        public string IdY
        {
            get => idY;
            set
            {
                idY = value;
                if (string.IsNullOrEmpty(id)) return;
                idZ = id;
                id = "";
            }
        }

        [XmlAttribute("id-z"), DefaultValue("")]
        public string IdZ
        {
            get => idZ;
            set
            {
                idZ = value;
                if (string.IsNullOrEmpty(id)) return;
                idY = id;
                id = "";
            }
        }
    }

    [Serializable]
    public class ConstructionX
    {
        [XmlIgnore] public Length t0 = Length.Percent();
        [XmlIgnore] public Length t1 = Length.Percent();
        [XmlAttribute("count")] public int count;
        [XmlIgnore] public Length step = Length.Meter();
        [XmlIgnore] public Length length = Length.Meter();
        [XmlAttribute("material")] public MaterialType material;
        [XmlElement("center-path")] public ConstructionPathX centerPath = new();
        [XmlElement("bl-path")] public ConstructionPathX bottomLeftPath = new();
        [XmlElement("br-path")] public ConstructionPathX bottomRightPath = new();
        [XmlElement("tl-path")] public ConstructionPathX topLeftPath = new();
        [XmlElement("tr-path")] public ConstructionPathX topRightPath = new();

        [XmlAttribute("t0")]
        public string T0Val
        {
            get => t0.ValUnit;
            set => t0.ValUnit = value;
        }

        [XmlAttribute("t0-ref")][DefaultValue("")]
        public string T0Ref
        {
            get => t0.value.referenceId;
            set => t0.value.referenceId = value;
        }

        [XmlAttribute("t1")]
        public string T1Val
        {
            get => t1.ValUnit;
            set => t1.ValUnit = value;
        }

        [XmlAttribute("t1-ref")][DefaultValue("")]
        public string T1Ref
        {
            get => t1.value.referenceId;
            set => t1.value.referenceId = value;
        }

        [XmlAttribute("length")]
        public string LengthVal
        {
            get => length.ValUnit;
            set => length.ValUnit = value;
        }

        [XmlAttribute("length-ref")][DefaultValue("")]
        public string LengthRef
        {
            get => length.value.referenceId;
            set => length.value.referenceId = value;
        }
    }

    [Serializable]
    public class StairsX
    {
        [XmlAttribute("t0")] public float t0;
        [XmlAttribute("t1")] public float t1;
        [XmlAttribute("step-length")] public float stepLength;
        [XmlAttribute("material")] public MaterialType material;

        [XmlElement("center-path")] public ConstructionPathX centerPath = new();
        [XmlElement("l-path")] public ConstructionPathX leftPath = new();
        [XmlElement("r-path")] public ConstructionPathX rightPath = new();
        [XmlElement("t-path")] public ConstructionPathX topPath = new();
    }

    [Serializable]
    [XmlRoot("map")]
    public class MapX
    {
        [XmlElement("hill")] public List<HillX> hills = new();
        [XmlElement("ref-point")] public List<RefPointX> refPoints = new();
        [XmlElement("path")] public List<PathX> paths = new();

        [XmlElement("construction")] public List<ConstructionX> constructions = new();
        [XmlElement("stairs")] public List<StairsX> stairs = new();

        [XmlIgnore] public Vector3X pos = Vector3X.Zero;

        [XmlAttribute("lat")]
        public float Lat
        {
            get => pos.x;
            set => pos.x = value;
        }

        [XmlAttribute("lon")]
        public float Lon
        {
            get => pos.y;
            set => pos.y = value;
        }

        [XmlAttribute("alt")]
        public float Alt
        {
            get => pos.z;
            set => pos.z = value;
        }
    }
}