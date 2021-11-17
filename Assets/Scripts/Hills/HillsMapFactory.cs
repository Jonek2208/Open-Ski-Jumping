using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using OpenSkiJumping.Hills.CurvePaths;
using UnityEngine;

namespace OpenSkiJumping.Hills
{
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
                    rotation = Quaternion.Euler(item.rot.ToVector3()),
                    scale = item.scl.ToVector3()
                },
                auxiliaryRefs = item.auxiliaryRefs.Select(GetRefPoint).ToList()
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
                rot = Vector3X.FromVector3(item.value.rotation.eulerAngles),
                scl = Vector3X.FromVector3(item.value.scale),
                auxiliaryRefs = item.auxiliaryRefs.Select(ParseRefPointToAnonymous).ToList()
            };
            return res;
        }

        public static AnonymousRefPointX ParseRefPointToAnonymous(ReferencePoint item)
        {
            var res = new AnonymousRefPointX
            {
                refId = item.referenceId,
                pos = Vector3X.FromVector3(item.value.position),
                rot = Vector3X.FromVector3(item.value.rotation.eulerAngles),
                scl = Vector3X.FromVector3(item.value.scale)
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
                    rotation = Quaternion.Euler(item.rot.ToVector3()),
                    scale = item.scl.ToVector3()
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
            if (item.GetType() == typeof(Bezier2NodeX))
                return GetPathNodeHelper((Bezier2NodeX) item);
            if (item.GetType() == typeof(ArcNodeX))
                return GetPathNodeHelper((ArcNodeX) item);
            if (item.GetType() == typeof(PathNodeX))
                return GetPathNodeHelper((PathNodeX) item);
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
                case NodeType.Arc:
                    return PathNodeToArc(item);
                case NodeType.Path:
                    return PathNodeToPath(item);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static PathNode GetPathNodeHelper(LineNodeX item)
        {
            return new PathNode
            {
                nodeType = NodeType.Line, target = GetRefPoint(item.target)
            };
        }

        private static PathNode GetPathNodeHelper(Bezier2NodeX item)
        {
            return new PathNode
            {
                nodeType = NodeType.Bezier2, target = GetRefPoint(item.target), c0 = GetRefPoint(item.c0)
            };
        }

        private static PathNode GetPathNodeHelper(Bezier3NodeX item)
        {
            return new PathNode
            {
                nodeType = NodeType.Bezier3, target = GetRefPoint(item.target), c0 = GetRefPoint(item.c0),
                c1 = GetRefPoint(item.c1)
            };
        }

        private static PathNode GetPathNodeHelper(ArcNodeX item)
        {
            return new PathNode
            {
                nodeType = NodeType.Arc, target = GetRefPoint(item.target), c0 = GetRefPoint(item.c0)
            };
        }

        private static PathNode GetPathNodeHelper(PathNodeX item)
        {
            return new PathNode
            {
                nodeType = NodeType.Path, target = GetRefPoint(item.target), pathId = item.pathId
            };
        }

        private static LineNodeX PathNodeToLine(PathNode item)
        {
            return new LineNodeX {target = ParseRefPoint(item.target)};
        }

        private static Bezier3NodeX PathNodeToBezier3(PathNode item)
        {
            return new Bezier3NodeX
            {
                target = ParseRefPoint(item.target), c0 = ParseRefPoint(item.c0), c1 = ParseRefPoint(item.c1)
            };
        }

        private static Bezier2NodeX PathNodeToBezier2(PathNode item)
        {
            return new Bezier2NodeX
            {
                target = ParseRefPoint(item.target), c0 = ParseRefPoint(item.c0)
            };
        }

        private static ArcNodeX PathNodeToArc(PathNode item)
        {
            return new ArcNodeX
            {
                target = ParseRefPoint(item.target), c0 = ParseRefPoint(item.c0)
            };
        }

        private static PathNodeX PathNodeToPath(PathNode item)
        {
            return new PathNodeX
            {
                target = ParseRefPoint(item.target), pathId = item.pathId
            };
        }

        public static HillsMap GetHillsMap(MapX item)
        {
            return new HillsMap
            {
                profiles = item.hills.Select(GetMapHillData).ToList(),
                referencePoints = item.refPoints.Select(GetRefPoint).ToList(),
                paths = item.paths.Select(GetPath3D).ToList(),
                constructions = item.constructions.Select(GetConstruction).ToList(),
                stairs = item.stairs.Select(GetStairs).ToList(),
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
                constructions = item.constructions.Select(ParseConstruction).ToList(),
                stairs = item.stairs.Select(ParseStairs).ToList(),
                pos = Vector3X.FromVector3(item.pos)
            };
        }

        public static ConstructionPath GetConstructionPath(ConstructionPathX item)
        {
            var newIdY = string.IsNullOrEmpty(item.id) ? item.idY : item.id;
            var newIdZ = string.IsNullOrEmpty(item.id) ? item.idZ : item.id;

            return new ConstructionPath
            {
                idY = newIdY, idZ = newIdZ, shift = item.shift, refPoint = GetRefPoint(item)
            };
        }

        public static ConstructionPathX ParseConstructionPath(ConstructionPath item)
        {
            var refPoint = ParseRefPoint(item.refPoint);
            var newId = item.idY == item.idZ ? item.idY : "";
            var newIdY = item.idY == item.idZ ? "" : item.idY;
            var newIdZ = item.idY == item.idZ ? "" : item.idZ;

            return new ConstructionPathX
            {
                id = newId,
                idY = newIdY,
                idZ = newIdZ,
                shift = item.shift,
                refId = refPoint.refId,
                pos = refPoint.pos,
                rot = refPoint.rot,
                scl = refPoint.scl
            };
        }

        public static Construction GetConstruction(ConstructionX item)
        {
            return new Construction
            {
                centerPath = GetConstructionPath(item.centerPath),
                bottomLeftPath = GetConstructionPath(item.bottomLeftPath),
                bottomRightPath = GetConstructionPath(item.bottomRightPath),
                topLeftPath = GetConstructionPath(item.topLeftPath),
                topRightPath = GetConstructionPath(item.topRightPath),
                t0 = item.t0,
                t1 = item.t1,
                count = item.count,
                step = item.step,
                length = item.length,
                material = item.material
            };
        }

        public static ConstructionX ParseConstruction(Construction item)
        {
            return new ConstructionX
            {
                centerPath = ParseConstructionPath(item.centerPath),
                bottomLeftPath = ParseConstructionPath(item.bottomLeftPath),
                bottomRightPath = ParseConstructionPath(item.bottomRightPath),
                topLeftPath = ParseConstructionPath(item.topLeftPath),
                topRightPath = ParseConstructionPath(item.topRightPath),
                t0 = item.t0,
                t1 = item.t1,
                count = item.count,
                step = item.step,
                length = item.length,
                material = item.material
            };
        }

        public static Stairs GetStairs(StairsX item)
        {
            return new Stairs
            {
                centerPath = GetConstructionPath(item.centerPath),
                leftPath = GetConstructionPath(item.leftPath),
                rightPath = GetConstructionPath(item.rightPath),
                t0 = item.t0,
                t1 = item.t1,
                stepLength = item.stepLength,
                material = item.material
            };
        }

        public static StairsX ParseStairs(Stairs item)
        {
            return new StairsX
            {
                centerPath = ParseConstructionPath(item.centerPath),
                leftPath = ParseConstructionPath(item.leftPath),
                rightPath = ParseConstructionPath(item.rightPath),
                t0 = item.t0,
                t1 = item.t1,
                stepLength = item.stepLength,
                material = item.material
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
                refTransform = GetRefPoint(item.refPoint),
                anchor = GetRefPoint(item.anchor)
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
                outrun = new OutrunX
                    {a = item.profileData.a, rA = item.profileData.rA, betaA = item.profileData.betaA},
                judgesTower = new JudgesTowerX
                    {d = item.profileData.d, q = item.profileData.q, g = item.profileData.g},
                refPoint = ParseRefPoint(item.refTransform),
                anchor = ParseRefPoint(item.anchor)
            };
        }
    }
}