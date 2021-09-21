using System.Collections.Generic;
using UnityEngine;

namespace OpenSkiJumping.Hills
{
    [CreateAssetMenu(menuName = "HillElements/Hill")]
    public class Hill : ScriptableObject
    {
        public ProfileType type;
        public int gates;
        public float w;
        public float hS;
        public float h;
        public float n;

        public float gamma, alpha;
        public float gammaR, alphaR;
        public float e, es, t;
        public float r1;

        public float beta0, betaP, betaK, betaL;
        public float beta0R, betaPR, betaKR, betaLR;
        public float s, l1, l2;
        public float rL, r2L, r2;

        public float l; //Inrun
        public float u, v; //Knoll

        public float dI, cI, fI; //ICR2008 Inrun
        public float tauR, aO, bO, cO; //ICR2008 Landing Area

        public float a, rA, betaA, betaAR; //Outrun

        public float b1, b2, bK, bU;
        public float d, q, g;

        public Vector2 A, B, C1, C2, CL, CV, E1, E2, T, F, P, K, L, U, V, X;
        public Vector2[] landingAreaPoints;
        public Vector2[] inrunPoints;

        public InrunData inrunData;
        public LandingAreaData landingAreaData;

        public float terrainSteepness;

        public Hill(ProfileData profileData)
        {
            SetValues(profileData);
        }

        public void SetValues(ProfileData profileData)
        {
            type = profileData.type;
            gates = profileData.gates;
            w = profileData.w;
            hS = profileData.hS;
            h = profileData.h;
            n = profileData.n;
            gamma = profileData.gamma;
            alpha = profileData.alpha;
            gammaR = Mathf.Deg2Rad * gamma;
            alphaR = Mathf.Deg2Rad * alpha;
            e = profileData.e;
            es = profileData.es;
            t = profileData.t;
            r1 = profileData.r1;
            betaP = profileData.betaP;
            beta0 = profileData.beta0;
            betaK = profileData.betaK;
            betaL = profileData.betaL;
            beta0R = Mathf.Deg2Rad * beta0;
            betaPR = Mathf.Deg2Rad * betaP;
            betaKR = Mathf.Deg2Rad * betaK;
            betaLR = Mathf.Deg2Rad * betaL;
            s = profileData.s;
            l1 = profileData.l1;
            l2 = profileData.l2;
            rL = profileData.rL;
            r2L = profileData.r2L;
            r2 = profileData.r2;
            a = profileData.a;
            rA = profileData.rA;
            betaA = profileData.betaA;
            betaAR = Mathf.Deg2Rad * betaA;
            b1 = profileData.b1;
            b2 = profileData.b2;
            bK = profileData.bK;
            bU = profileData.bU;
            d = profileData.d;
            q = profileData.q;
            g = profileData.g;
            terrainSteepness = profileData.terrainSteepness;

            Calculate();
            inrunPoints = GenerateInrunPoints();
            landingAreaPoints = GenerateLandingAreaPoints();
            inrunData = profileData.inrunData;
            landingAreaData = profileData.landingAreaData;
        }

        public void Calculate()
        {
            if (type == ProfileType.ICR1992)
            {
                betaP = betaL = betaK;
                betaPR = betaLR = betaKR;
            }

            //Inrun 
            E2 = new Vector2(-t * Mathf.Cos(alphaR), t * Mathf.Sin(alphaR));

            if (type == ProfileType.ICR1992 || type == ProfileType.ICR1996)
            {
                C1 = new Vector2(E2.x + Mathf.Sin(alphaR) * r1, E2.y + Mathf.Cos(alphaR) * r1);
                E1 = new Vector2(C1.x - Mathf.Sin(gammaR) * r1, C1.y - Mathf.Cos(gammaR) * r1);
                l = r1 * (gammaR - alphaR);
            }
            else
            {
                //ToDo
                dI = 2.0f * r1 * Mathf.Sin(gammaR - alphaR) * Mathf.Cos(gammaR - alphaR) * Mathf.Cos(gammaR - alphaR);
                cI = Mathf.Tan(gammaR - alphaR) / 3.0f / dI / dI;
                fI = Mathf.Tan(gammaR - alphaR) * dI / 3.0f;
                l = dI * (1.0f + 0.1f * Mathf.Tan(gammaR - alphaR) * Mathf.Tan(gammaR - alphaR));
                E1 = new Vector2(-(t * Mathf.Cos(alphaR) + fI * Mathf.Sin(gammaR) + dI * Mathf.Cos(gammaR)),
                    (t * Mathf.Sin(alphaR) - fI * Mathf.Cos(gammaR) + dI * Mathf.Sin(gammaR)));
            }

            A = new Vector2(E1.x - (e - l) * Mathf.Cos(gammaR), E1.y + (e - l) * Mathf.Sin(gammaR));
            B = new Vector2(A.x + es * Mathf.Cos(gammaR), A.y - es * Mathf.Sin(gammaR));

            T = new Vector2(0, 0);

            //Landing Area

            F.x = 0;
            F.y = -s;
            // K = new Vector2(Mathf.Cos(Mathf.Atan(hn)) / 1.005f, -Mathf.Sin(Mathf.Atan(hn)) / 1.005f) * w;
            K = new Vector2(n, -h);
            if (type == ProfileType.ICR1992)
            {
                P = K + new Vector2(-Mathf.Cos(betaKR), Mathf.Sin(betaKR)) * l1;
                L = K;
            }
            else
            {
                P = K - new Vector2(Mathf.Sin(betaPR) - Mathf.Sin(betaKR), Mathf.Cos(betaPR) - Mathf.Cos(betaKR)) * rL;
                L = K + new Vector2(Mathf.Sin(betaKR) - Mathf.Sin(betaLR), Mathf.Cos(betaKR) - Mathf.Cos(betaLR)) * rL;
            }

            u = -P.y + F.y - P.x * Mathf.Tan(beta0R);
            v = P.x * (Mathf.Tan(betaPR) - Mathf.Tan(beta0R));

            if (type == ProfileType.ICR1992)
            {
                C2 = K + new Vector2(Mathf.Sin(betaKR), Mathf.Cos(betaKR)) * r2;
                U = new Vector2(C2.x, C2.y - r2);
            }
            else
            {
                CL = K + new Vector2(Mathf.Sin(betaKR), Mathf.Cos(betaKR)) * rL;
                if (type == ProfileType.ICR2008)
                {
                    tauR = Mathf.Atan((Mathf.Cos(betaLR) - Mathf.Pow(r2 / r2L, 1.0f / 3.0f)) / Mathf.Sin(betaLR));
                    cO = 1.0f / (2.0f * r2 * Mathf.Cos(tauR) * Mathf.Cos(tauR) * Mathf.Cos(tauR));
                    aO = -Mathf.Tan(betaLR + tauR) / 2.0f / cO;
                    bO = -Mathf.Tan(tauR) / 2.0f / cO;

                    U = new Vector2(L.x + cO * Mathf.Sin(tauR) * (aO * aO - bO * bO) + Mathf.Cos(tauR) * (bO - aO),
                        L.y - cO * Mathf.Cos(tauR) * (aO * aO - bO * bO) + Mathf.Sin(tauR) * (bO - aO));
                }
                else
                {
                    C2 = L + new Vector2(Mathf.Sin(betaLR), Mathf.Cos(betaLR)) * r2;
                    U = new Vector2(C2.x, C2.y - r2);
                }
            }

            if (betaA > 0)
            {
                CV = U + new Vector2(0, rA);
                V = CV - rA * new Vector2(-Mathf.Sin(betaAR), Mathf.Cos(betaAR));
                var len = a - betaAR * rA;
                X = V + len * new Vector2(Mathf.Cos(betaAR), Mathf.Sin(betaAR));
            }
            else
            {
                V = U;
                X = U + new Vector2(a, 0);
            }
        }

        public float Inrun(float x)
        {
            if (x >= E2.x)
            {
                return -x * Mathf.Tan(alphaR);
            }

            if (E2.x > x && x >= E1.x)
            {
                if (type == ProfileType.ICR1992 || type == ProfileType.ICR1996)
                {
                    return -Mathf.Sqrt(r1 * r1 - (x - C1.x) * (x - C1.x)) + C1.y;
                }

                var p = 1.0f / (Mathf.Tan(gammaR) * 3.0f * cI);
                var q = (x + t * Mathf.Cos(alphaR) + fI * Mathf.Sin(gammaR) + dI * Mathf.Cos(gammaR)) / 2.0f / cI /
                        Mathf.Sin(gammaR);
                var ksi = Mathf.Pow((Mathf.Sqrt(q * q + p * p * p) + q), (1.0f / 3.0f)) -
                          Mathf.Pow((Mathf.Sqrt(q * q + p * p * p) - q), (1.0f / 3.0f));
                return t * Mathf.Sin(alphaR) - fI * Mathf.Cos(gammaR) + dI * Mathf.Sin(gammaR) -
                    ksi * Mathf.Sin(gammaR) + cI * ksi * ksi * ksi * Mathf.Cos(gammaR);
            }

            return -x * Mathf.Tan(gammaR) + A.y + Mathf.Tan(gammaR) * A.x;
        }

        public float LandingArea(float x)
        {
            if (x <= P.x)
            {
                return F.y - x * Mathf.Tan(beta0R) - (3 * u - v) * (x / P.x) * (x / P.x) +
                       (2 * u - v) * (x / P.x) * (x / P.x) * (x / P.x);
            }

            if (P.x < x && x <= L.x)
            {
                if (type == ProfileType.ICR1992)
                {
                    return Mathf.Tan(betaKR) * (K.x - x) + K.y;
                }

                return -Mathf.Sqrt(rL * rL - (x - CL.x) * (x - CL.x)) + CL.y;
            }

            if (L.x <= x && x <= U.x)
            {
                if (type == ProfileType.ICR2008)
                {
                    var ksi = (Mathf.Cos(tauR) - Mathf.Sqrt(Mathf.Cos(tauR) * Mathf.Cos(tauR) -
                                                            4.0f * cO * (x - L.x - cO * aO * aO * Mathf.Sin(tauR) +
                                                                         aO * Mathf.Cos(tauR)) * Mathf.Sin(tauR))) /
                              2.0f / cO / Mathf.Sin(tauR);
                    return L.y - cO * Mathf.Cos(tauR) * (aO * aO - ksi * ksi) - Mathf.Sin(tauR) * (aO - ksi);
                }

                return -Mathf.Sqrt(r2 * r2 - (x - C2.x) * (x - C2.x)) + C2.y;
            }

            if (U.x < x && x < V.x)
            {
                return -Mathf.Sqrt(rA * rA - (x - CV.x) * (x - CV.x)) + CV.y;
            }

            return V.y + Mathf.Sin(betaAR) * (x - V.x);
        }

        public Vector2[] GenerateLandingAreaPoints(int accuracy = 1000)
        {
            var points = new List<Vector2>();

            var distance = 0.0f;
            var meter = 0;

            var last = F;

            for (var i = 0; i < accuracy * (X.x); i++)
            {
                var curr = new Vector2(i / (float) accuracy, LandingArea(i / (float) accuracy));

                if (distance + (curr - last).magnitude >= meter)
                {
                    if (meter - distance < distance + (curr - last).magnitude - meter)
                    {
                        points.Add(curr);
                    }
                    else
                    {
                        points.Add(last);
                    }

                    meter++;
                }

                distance += (curr - last).magnitude;
                last = curr;
            }

            return points.ToArray();
        }

        public Vector2[] GenerateInrunPoints()
        {
            var points = new List<Vector2> {T};

            //add points between E2 and E1 (inclusive)
            var delta = E1.x - E2.x;
            var segments = (int) (l);
            for (var i = 0; i <= segments; i++)
            {
                points.Add(new Vector2(i * delta / segments + E2.x, Inrun(i * delta / segments + E2.x)));
            }

            points.Add(B);
            points.Add(A);
            return points.ToArray();
        }

        public Vector2 GatePoint(int nr)
        {
            return (A - B) * (nr - 1) / (gates - 1) + B;
        }
    }
}