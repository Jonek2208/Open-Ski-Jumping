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
        [XmlIgnore] public AnonymousRefPointX refPoint = new AnonymousRefPointX();
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
            get => refPoint.rot.x;
            set => refPoint.rot.x = value;
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
        [XmlIgnore] public Vector3X pos = new Vector3X();
        [XmlIgnore] public Vector3X rot = new Vector3X();

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
            get => rot.x;
            set => rot.x = value;
        }

        [XmlAttribute("rz")]
        public float Rz
        {
            get => rot.z;
            set => rot.z = value;
        }
    }

    [Serializable]
    public class RefPointX : AnonymousRefPointX
    {
        [XmlAttribute("id")] public string id;
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
        [XmlElement("large-circle")] public int largeCircle;
    }


    [Serializable]
    public class PathX
    {
        [XmlAttribute("id")] public string id;
        [XmlIgnore] public RefPointX refPoint = new RefPointX();

        [XmlElement("line", typeof(LineNodeX))] [XmlElement("bezier3", typeof(Bezier3NodeX))]
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
            get => refPoint.rot.x;
            set => refPoint.rot.x = value;
        }

        [XmlAttribute("rz")]
        public float Rz
        {
            get => refPoint.rot.z;
            set => refPoint.rot.z = value;
        }
    }

    [Serializable]
    [XmlRoot("map")]
    public class MapX
    {
        [XmlElement("hill")] public List<HillX> hills = new List<HillX>();
        [XmlElement("ref-point")] public List<RefPointX> refPoints = new List<RefPointX>();
        [XmlElement("path")] public List<PathX> paths = new List<PathX>();
        [XmlIgnore] public Vector3X pos = new Vector3X();

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

    public static class HillsMapFactory
    {
        public static Path3D GetPath3D(PathX item)
        {
            var res = new Path3D
            {
                id = item.id,
                data = item.nodes.Select(GetPathNode).ToList(),
                refPoint = GetRefPoint(item.refPoint)
            };
            return res;
        }

        public static PathX ParsePath3D(Path3D item)
        {
            var res = new PathX
            {
                id = item.id,
                nodes = item.data.Select(ParsePathNode).ToList(),
                refPoint = ParseRefPoint(item.refPoint)
            };
            return res;
        }

        public static ReferencePoint GetRefPoint(RefPointX item)
        {
            var res = new ReferencePoint
            {
                id = item.id,
                referenceId = item.refId,
                value = new SerializableTransform
                {
                    position = item.pos.ToVector3(),
                    rotation = Quaternion.Euler(item.rot.ToVector3())
                }
            };
            return res;
        }

        public static RefPointX ParseRefPoint(ReferencePoint item)
        {
            var res = new RefPointX
            {
                id = item.id,
                refId = item.referenceId,
                pos = Vector3X.FromVector3(item.value.position),
                rot = Vector3X.FromVector3(item.value.rotation.eulerAngles)
            };
            return res;
        }

        public static ReferencePoint GetRefPoint(AnonymousRefPointX item)
        {
            var res = new ReferencePoint
            {
                id = "",
                referenceId = item.refId,
                value = new SerializableTransform
                {
                    position = item.pos.ToVector3(),
                    rotation = Quaternion.Euler(item.rot.ToVector3())
                }
            };
            return res;
        }

        public static PathNode GetPathNode(NodeX item)
        {
            if (item.GetType() == typeof(LineNodeX))
                return GetPathNodeHelper((LineNodeX) item);
            if (item.GetType() == typeof(Bezier3NodeX))
                return GetPathNodeHelper((Bezier3NodeX) item);
            return null;
        }

        public static NodeX ParsePathNode(PathNode item)
        {
            switch (item.nodeType)
            {
                case NodeType.Line:
                    return PathNodeToLine(item);
                case NodeType.Bezier2:
                    return PathNodeToBezier2(item);
                case NodeType.Bezier3:
                    return PathNodeToBezier3(item);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static PathNode GetPathNodeHelper(LineNodeX item)
        {
            return new PathNode {nodeType = NodeType.Line, target = GetRefPoint(item.target)};
        }

        private static PathNode GetPathNodeHelper(Bezier3NodeX item)
        {
            return new PathNode
            {
                nodeType = NodeType.Bezier3, target = GetRefPoint(item.target), c0 = GetRefPoint(item.c0),
                c1 = GetRefPoint(item.c1)
            };
        }

        private static LineNodeX PathNodeToLine(PathNode item)
        {
            return new LineNodeX {target = ParseRefPoint(item.target)};
        }

        private static Bezier3NodeX PathNodeToBezier3(PathNode item)
        {
            return new Bezier3NodeX
                {target = ParseRefPoint(item.target), c0 = ParseRefPoint(item.c0), c1 = ParseRefPoint(item.c1)};
        }

        private static Bezier2NodeX PathNodeToBezier2(PathNode item)
        {
            return new Bezier2NodeX
                {target = ParseRefPoint(item.target), c0 = ParseRefPoint(item.c0)};
        }

        public static HillsMap GetHillsMap(MapX item)
        {
            return new HillsMap
            {
                profiles = item.hills.Select(GetMapHillData).ToList(),
                referencePoints = item.refPoints.Select(GetRefPoint).ToList(),
                paths = item.paths.Select(GetPath3D).ToList(),
                pos = item.pos.ToVector3()
            };
        }

        public static MapX ParseHillsMap(HillsMap item)
        {
            return new MapX
            {
                hills = item.profiles.Select(ParseHillData).ToList(),
                refPoints = item.referencePoints.Select(ParseRefPoint).ToList(),
                paths = item.paths.Select(ParsePath3D).ToList(),
                pos = Vector3X.FromVector3(item.pos)
            };
        }

        public static MapHillData GetMapHillData(HillX item)
        {
            return new MapHillData
            {
                profileData = new ProfileData
                {
                    name = item.id,
                    gates = item.inrun.gates,
                    e = item.inrun.e,
                    es = item.inrun.es,
                    r1 = item.inrun.r1,
                    t = item.inrun.t,
                    gamma = item.inrun.gamma,
                    alpha = item.inrun.alpha,
                    b1 = item.inrun.b1,
                    w = item.landingHill.w,
                    hS = item.landingHill.hs,
                    h = item.landingHill.h,
                    n = item.landingHill.n,
                    s = item.landingHill.s,
                    l1 = item.landingHill.l1,
                    l2 = item.landingHill.l2,
                    r2 = item.landingHill.r2,
                    r2L = item.landingHill.r2L,
                    rL = item.landingHill.rL,
                    beta0 = item.landingHill.beta0,
                    betaP = item.landingHill.betaP,
                    betaK = item.landingHill.betaK,
                    betaL = item.landingHill.betaL,
                    b2 = item.landingHill.b2,
                    bK = item.landingHill.bK,
                    bU = item.landingHill.bU,
                    a = item.outrun.a,
                    rA = item.outrun.rA,
                    betaA = item.outrun.betaA,
                    d = item.judgesTower.d,
                    q = item.judgesTower.q,
                    g = item.judgesTower.g,
                },
                pos = item.pos.ToVector3(),
                azimuth = item.rot.y
            };
        }

        public static HillX ParseHillData(MapHillData item)
        {
            return new HillX
            {
                id = item.profileData.name,
                inrun =
                    new InrunDataX
                    {
                        gates = item.profileData.gates,
                        e = item.profileData.e,
                        es = item.profileData.es,
                        t = item.profileData.t,
                        r1 = item.profileData.r1,
                        gamma = item.profileData.gamma,
                        alpha = item.profileData.alpha,
                        b1 = item.profileData.b1
                    },
                landingHill =
                    new LandingHillX
                    {
                        w = item.profileData.w,
                        hs = item.profileData.hS,
                        h = item.profileData.h,
                        n = item.profileData.n,
                        s = item.profileData.s,
                        l1 = item.profileData.l1,
                        l2 = item.profileData.l2,
                        r2 = item.profileData.r2,
                        r2L = item.profileData.r2L,
                        rL = item.profileData.rL,
                        beta0 = item.profileData.beta0,
                        betaP = item.profileData.betaP,
                        betaK = item.profileData.betaK,
                        betaL = item.profileData.betaL,
                        b2 = item.profileData.b2,
                        bK = item.profileData.bK,
                        bU = item.profileData.bU
                    },
                outrun = new OutrunX {a = item.profileData.a, rA = item.profileData.rA, betaA = item.profileData.betaA},
                judgesTower = new JudgesTowerX {d = item.profileData.d, q = item.profileData.q, g = item.profileData.g},
                pos = Vector3X.FromVector3(item.pos),
                rot = new Vector3X {x = 0, y = item.azimuth, z = 0}
            };
        }
    }
}