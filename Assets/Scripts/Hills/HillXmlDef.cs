using System;
using System.Collections.Generic;
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
            return new Vector3(x, y, z);
        }

        public static Vector3X FromVector3(Vector3 item)
        {
            return new Vector3X {x = item.x, y = item.y, z = item.z};
        }

        public static Vector3X One => new Vector3X {x = 1, y = 1, z = 1};
        public static Vector3X Zero => new Vector3X {x = 0, y = 0, z = 0};
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
        [XmlElement("anchor")] public RefPointX anchor = new RefPointX();
        [XmlElement("ref-point")] public RefPointX refPoint = new RefPointX();
        [XmlElement("pos")] public Vector3X pos;
        [XmlElement("rot")] public Vector3X rot;
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

        [XmlAttribute("x")]
        public float X
        {
            get => pos.x;
            set => pos.x = value;
        }

        [XmlAttribute("y")]
        public float Y
        {
            get => pos.y;
            set => pos.y = value;
        }

        [XmlAttribute("z")]
        public float Z
        {
            get => pos.z;
            set => pos.z = value;
        }

        [XmlAttribute("rx")]
        public float Rx
        {
            get => rot.x;
            set => rot.x = value;
        }

        [XmlAttribute("ry")]
        public float Ry
        {
            get => rot.y;
            set => rot.y = value;
        }

        [XmlAttribute("rz")]
        public float Rz
        {
            get => rot.z;
            set => rot.z = value;
        }

        [XmlAttribute("sx")]
        public float Sx
        {
            get => scl.x;
            set => scl.x = value;
        }

        [XmlAttribute("sy")]
        public float Sy
        {
            get => scl.y;
            set => scl.y = value;
        }

        [XmlAttribute("sz")]
        public float Sz
        {
            get => scl.z;
            set => scl.z = value;
        }
    }

    [Serializable]
    public class RefPointX : AnonymousRefPointX
    {
        [XmlAttribute("id")] public string id = "";
        [XmlElement("aux")] public List<AnonymousRefPointX> auxiliaryRefs;
    }


    [Serializable]
    public class NodeX
    {
        [XmlIgnore] public AnonymousRefPointX target = new RefPointX();

        [XmlAttribute("ref-id")]
        public string RefId
        {
            get => target.refId;
            set => target.refId = value;
        }

        [XmlAttribute("x")]
        public float X
        {
            get => target.pos.x;
            set => target.pos.x = value;
        }

        [XmlAttribute("y")]
        public float Y
        {
            get => target.pos.y;
            set => target.pos.y = value;
        }

        [XmlAttribute("z")]
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
        [XmlAttribute("id")] public string pathId = "";
        // [XmlElement("c0")] public RefPointX c0;
        // [XmlElement("c1")] public RefPointX c1;
    }


    [Serializable]
    public class PathX
    {
        [XmlAttribute("id")] public string id;
        [XmlIgnore] public RefPointX refPoint = new RefPointX();

        [XmlElement("line", typeof(LineNodeX))]
        [XmlElement("bezier3", typeof(Bezier3NodeX))]
        [XmlElement("bezier2", typeof(Bezier2NodeX))]
        [XmlElement("arc", typeof(ArcNodeX))]
        [XmlElement("path", typeof(PathNodeX))]
        public List<NodeX> nodes = new List<NodeX>();

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

        [XmlAttribute("sx")]
        public float Sx
        {
            get => refPoint.scl.x;
            set => refPoint.scl.x = value;
        }

        [XmlAttribute("sy")]
        public float Sy
        {
            get => refPoint.scl.y;
            set => refPoint.scl.y = value;
        }

        [XmlAttribute("sz")]
        public float Sz
        {
            get => refPoint.scl.z;
            set => refPoint.scl.z = value;
        }
    }

    [Serializable]
    public class ConstructionPathX : RefPointX
    {
        // [XmlAttribute("id")] public new string id;
        [XmlAttribute("shift")] public float shift;

        // [XmlAttribute("id")]
        // public string Id
        // {
        //     get => id;
        //     set => id = value;
        // }
    }

    [Serializable]
    public class ConstructionX
    {
        [XmlAttribute("t0")] public float t0;
        [XmlAttribute("t1")] public float t1;
        [XmlAttribute("count")] public int count;
        [XmlAttribute("step")] public float step;
        [XmlAttribute("length")] public float length;
        [XmlAttribute("material")] public MaterialType material;
        [XmlElement("center-path")] public ConstructionPathX centerPath = new ConstructionPathX();
        [XmlElement("bl-path")] public ConstructionPathX bottomLeftPath = new ConstructionPathX();
        [XmlElement("br-path")] public ConstructionPathX bottomRightPath = new ConstructionPathX();
        [XmlElement("tl-path")] public ConstructionPathX topLeftPath = new ConstructionPathX();
        [XmlElement("tr-path")] public ConstructionPathX topRightPath = new ConstructionPathX();
    }

    [Serializable]
    public class StairsX
    {
        [XmlAttribute("t0")] public float t0;
        [XmlAttribute("t1")] public float t1;
        [XmlAttribute("step-length")] public float stepLength;
        [XmlAttribute("material")] public MaterialType material;

        [XmlElement("center-path")] public ConstructionPathX centerPath = new ConstructionPathX();
        [XmlElement("left-path")] public ConstructionPathX leftPath = new ConstructionPathX();
        [XmlElement("right-path")] public ConstructionPathX rightPath = new ConstructionPathX();
    }

    [Serializable]
    [XmlRoot("map")]
    public class MapX
    {
        [XmlElement("hill")] public List<HillX> hills = new List<HillX>();
        [XmlElement("ref-point")] public List<RefPointX> refPoints = new List<RefPointX>();
        [XmlElement("path")] public List<PathX> paths = new List<PathX>();

        [XmlElement("construction")] public List<ConstructionX> constructions = new List<ConstructionX>();
        [XmlElement("stairs")] public List<StairsX> stairs = new List<StairsX>();

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