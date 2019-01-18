using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HillDataSerialization;

namespace HillProfile
{
    public class Hill
    {
        public enum ProfileType { ICR1992, ICR1996, ICR2008 };

        public ProfileType type;
        public int gates;
        public float w;
        public float hn;

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

        public Vector2 A, B, C1, C2, CL, E1, E2, T, F, P, K, L, U;

        public Hill() { }

        public Hill(ProfileType _type, int _gates, float _w, float _hn, float _gamma, float _alpha, float _e, float _es, float _t, float _r1,
        float _betaP, float _betaK, float _betaL, float _s, float _l1, float _l2, float _rL, float _r2L, float _r2)
        {
            type = _type;
            gates = _gates;
            w = _w;
            hn = _hn;
            gamma = _gamma;
            alpha = _alpha;
            gammaR = Mathf.Deg2Rad * gamma;
            alphaR = Mathf.Deg2Rad * alpha;
            e = _e;
            es = _es;
            t = _t;
            r1 = _r1;
            betaP = _betaP;
            beta0 = betaP / 6.0f;
            betaK = _betaK;
            betaL = _betaL;
            beta0R = Mathf.Deg2Rad * beta0;
            betaPR = Mathf.Deg2Rad * betaP;
            betaKR = Mathf.Deg2Rad * betaK;
            betaLR = Mathf.Deg2Rad * betaL;
            s = _s;
            l1 = _l1;
            l2 = _l2;
            rL = _rL;
            r2L = _r2L;
            r2 = _r2;
        }

        public void Calculate()
        {
            if (type == ProfileType.ICR1992)
            {
                betaP = betaL = betaK;
                betaPR = betaLR = betaKR;
                beta0 = betaK / 6.0f;
                beta0R = betaKR / 6.0f;
            }
            //Inrun 
            E2 = new Vector2(-t * Mathf.Cos(alphaR), t * Mathf.Sin(alphaR));
            float l;

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

            F.x = 0; F.y = -s;
            K = new Vector2(Mathf.Cos(Mathf.Atan(hn)) / 1.005f, -Mathf.Sin(Mathf.Atan(hn)) / 1.005f) * w;
            if (type == ProfileType.ICR1992)
            {
                P = K + new Vector2(-Mathf.Cos(betaKR), Mathf.Sin(betaKR)) * l1;
                L = K;
            }
            else
            {
                P = K - new Vector2(Mathf.Sin(betaPR) - Mathf.Sin(betaKR), Mathf.Cos(betaPR) - Mathf.Cos(betaKR)) * rL;
                L = new Vector2(K.x + rL * (Mathf.Sin(betaKR) - Mathf.Sin(betaLR)), K.y - rL * (Mathf.Cos(betaLR) - Mathf.Cos(betaKR)));
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
        }

        public float Inrun(float x)
        {
            if (x >= E2.x)
            {
                return -x * Mathf.Tan(alphaR);
            }
            else if (E2.x > x && x >= E1.x)
            {
                if (type == ProfileType.ICR1992 || type == ProfileType.ICR1996)
                {
                    return -Mathf.Sqrt(r1 * r1 - (x - C1.x) * (x - C1.x)) + C1.y;
                }
                else
                {
                    float p, q, ksi;
                    p = 1.0f / (Mathf.Tan(gammaR) * 3.0f * cI);
                    q = (x + t * Mathf.Cos(alphaR) + fI * Mathf.Sin(gammaR) + dI * Mathf.Cos(gammaR)) / 2.0f / cI / Mathf.Sin(gammaR);
                    ksi = Mathf.Pow((Mathf.Sqrt(q * q + p * p * p) + q), (1.0f / 3.0f)) - Mathf.Pow((Mathf.Sqrt(q * q + p * p * p) - q), (1.0f / 3.0f));
                    return t * Mathf.Sin(alphaR) - fI * Mathf.Cos(gammaR) + dI * Mathf.Sin(gammaR) - ksi * Mathf.Sin(gammaR) + cI * ksi * ksi * ksi * Mathf.Cos(gammaR);
                }
            }
            else
            {
                return -x * Mathf.Tan(gammaR) + A.y + Mathf.Tan(gammaR) * A.x;
            }
        }

        public float LandingArea(float x)
        {
            if (x <= P.x)
            {
                return F.y - x * Mathf.Tan(beta0R) - (3 * u - v) * (x / P.x) * (x / P.x) + (2 * u - v) * (x / P.x) * (x / P.x) * (x / P.x);
            }
            else if (P.x < x && x <= L.x)
            {
                if (type == ProfileType.ICR1992)
                {
                    return Mathf.Tan(betaKR) * (K.x - x) + K.y;
                }
                return -Mathf.Sqrt(rL * rL - (x - CL.x) * (x - CL.x)) + CL.y;
            }
            else
            {
                if (type == ProfileType.ICR2008)
                {
                    float ksi;
                    ksi = (Mathf.Cos(tauR) - Mathf.Sqrt(Mathf.Cos(tauR) * Mathf.Cos(tauR) -
                        4.0f * cO * (x - L.x - cO * aO * aO * Mathf.Sin(tauR) + aO * Mathf.Cos(tauR)) * Mathf.Sin(tauR))) / 2.0f / cO / Mathf.Sin(tauR);
                    return L.y - cO * Mathf.Cos(tauR) * (aO * aO - ksi * ksi) - Mathf.Sin(tauR) * (aO - ksi);
                }
                else
                {
                    return -Mathf.Sqrt(r2 * r2 - (x - C2.x) * (x - C2.x)) + C2.y;
                }
            }
        }
        public Vector2[] LandingAreaPoints(int accuracy)
        {
            List<Vector2> points = new List<Vector2>();

            float distance = 0.0f;
            int meter = 0;

            Vector2 curr = F, last = F;

            for (int i = 0; i < accuracy * U.x; i++)
            {
                curr = new Vector2((float)i / (float)accuracy, LandingArea((float)i / (float)accuracy));

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

            Vector2[] res = new Vector2[points.Count];
            int it = 0;
            foreach (Vector2 pt in points)
            {
                res[it++] = pt;
            }

            return res;
        }

        public Vector2[] InrunPoints()
        {
            List<Vector2> points = new List<Vector2>();

            points.Add(T);

            float delta = E1.x - E2.x;
            int segments = (int)(gamma - alpha);

            for (int i = 0; i <= segments; i++)
            {
                points.Add(new Vector2(i * delta / segments + E2.x, Inrun(i * delta / segments + E2.x)));
            }

            points.Add(B);
            points.Add(A);

            Vector2[] res = new Vector2[points.Count];
            int it = 0;

            foreach (Vector2 pt in points)
            {
                res[it++] = pt;
            }

            return res;
        }

        public Vector2 GatePoint(int nr)
        {
            return (A - B) * (float)(nr - 1) / (gates - 1) + B;
        }

        public HillData GenerateHillData()
        {
            return new HillData();
        }
    }

    public class ProfileScript : MonoBehaviour
    {
        void Start()
        {
            // ProfileData pd = new ProfileData();
            // pd.type = ProfileType.ICR1996;
            // pd.w = 120;
            // pd.hn = 0.575f;
            // string json = JsonUtility.ToJson(pd);
            // Debug.Log(json);

            // ProfileData pd0 = JsonUtility.FromJson<ProfileData>(json);
            // Debug.Log(pd0.type + " " + pd0.w + " " + pd0.hn);
            // Debug.Log("Oberstdorf HS137");
            // Hill oberstdorf = new Hill(Hill.ProfileType.ICR1996, 120, 0.575f, 35, 11, 99, 23, 6.5f, 115, 37.43f, 35.5f, 32.4f, 3.38f, 11.15f, 17.42f, 321, 100, 100);
            // oberstdorf.Calculate();
            // Debug.Log(oberstdorf.A);
            // Debug.Log(oberstdorf.B);
            // Debug.Log(oberstdorf.E1);
            // Debug.Log(oberstdorf.E2);
            // Debug.Log(oberstdorf.T);
            // Debug.Log(oberstdorf.C1);
            // Debug.Log(oberstdorf.F);
            // Debug.Log(oberstdorf.P);
            // Debug.Log(oberstdorf.K);
            // Debug.Log(oberstdorf.L);
            // Debug.Log(oberstdorf.CL);
            // Debug.Log(oberstdorf.C2);
            // Debug.Log(oberstdorf.U);

            // for (int i = (int)(oberstdorf.A.x); i <= 0; i++)
            // {
            //     Debug.Log(new Vector2(i, oberstdorf.Inrun(i)));
            // }

            // Vector2[] tab = oberstdorf.LandingAreaPoints(1000);
            // for(int i = 0; i < tab.Length; i++) Debug.Log(i + " " + tab[i]);

            // for(int i = 0; i < (int) (oberstdorf.U.x); i++)
            // {
            // 	Debug.Log(new Vector2(i, oberstdorf.LandingArea(i)));
            // }

            // Debug.Log("Zakopane HS140");
            // Hill zakopane = new Hill(Hill.ProfileType.ICR2008, 125, 0.575f, 35, 11, 98.7f, 22, 6.5f, 90, 37.05f, 34.3f, 31.4f, 3.13f, 16, 15, 310, 168, 99.3f);
            // zakopane.Calculate();
            // Debug.Log(zakopane.A);
            // Debug.Log(zakopane.B);
            // Debug.Log(zakopane.E1);
            // Debug.Log(zakopane.E2);
            // Debug.Log(zakopane.T);
            // Debug.Log(zakopane.C1);
            // Debug.Log(zakopane.F);
            // Debug.Log(zakopane.P);
            // Debug.Log(zakopane.K);
            // Debug.Log(zakopane.L);
            // Debug.Log(zakopane.CL);
            // Debug.Log(zakopane.C2);
            // Debug.Log(zakopane.U);

            // Debug.Log("Hakuba HS131");
            // Hill hakuba = new Hill(Hill.ProfileType.ICR1992, 120, 0.575f, 35, 11, 95.2f, 20.5f, 6.5f, 107, 37.5f, 37.5f, 37.5f, 3, 30, 11, 0, 0, 126f);
            // hakuba.Calculate();
            // Debug.Log(hakuba.A);
            // Debug.Log(hakuba.B);
            // Debug.Log(hakuba.E1);
            // Debug.Log(hakuba.E2);
            // Debug.Log(hakuba.T);
            // Debug.Log(hakuba.C1);
            // Debug.Log(hakuba.F);
            // Debug.Log(hakuba.P);
            // Debug.Log(hakuba.K);
            // Debug.Log(hakuba.L);
            // Debug.Log(hakuba.CL);
            // Debug.Log(hakuba.C2);
            // Debug.Log(hakuba.U);
        }
    }
}