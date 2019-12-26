using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "HillElements/LandingArea")]
public class LandingArea : ScriptableObject
{
    [SerializeField]
    private float lineWidth = 0.2f;
    [SerializeField]
    private float sideLineWidth = 0.5f;
    [SerializeField]
    private float whiteLineWidth = 3f;
    [SerializeField]
    private int maxSL = 2;

    [SerializeField]
    private Material[] materials;

    public Mesh Generate(Vector2[] landingAreaPoints, float hillW, float hillL1, float hillL2, float hillB2, float hillBK, float hillBU, Vector2 P, Vector2 K, Vector2 L, Vector2 U)
    {
        Mesh mesh = new Mesh();
        List<Vector3> verticesList = new List<Vector3>();
        List<Vector2> uvsList = new List<Vector2>();
        // 0 - green, 1 - white, 2 - blue, 3 - red
        List<(int, int, int, int)>[] facesList = new List<(int, int, int, int)>[4];
        for (int i = 0; i < 4; i++) facesList[i] = new List<(int, int, int, int)>();

        List<int> trianglesList = new List<int>();

        int[,] lines = new int[landingAreaPoints.Length, 3];
        int[,] lineColor = new int[landingAreaPoints.Length, 3];
        int[] linesMask = new int[landingAreaPoints.Length];

        int pLen = Mathf.RoundToInt(hillW - hillL1), kLen = Mathf.RoundToInt(hillW), lLen = Mathf.RoundToInt(hillW + hillL2);

        // U point line
        int uLen = 0;
        while ((landingAreaPoints[uLen + 1] - U).magnitude < (landingAreaPoints[uLen] - U).magnitude) uLen++;

        List<int> igelitLines = new List<int>();

        for (int i = Mathf.Max(5, (pLen / 5) * 5 - 10); i <= Mathf.Max(lLen + 5, (int)(1.1 * lLen)); i += 5) igelitLines.Add(i);

        // white lines
        foreach (var line in igelitLines)
        {
            linesMask[line - 1] |= (1 << 1);
            lines[line - 1, 2] |= (1 << 1);
            lines[line, 0] |= (1 << 1);
            lineColor[line, 0] |= (1 << 1);
        }

        // P point line
        linesMask[pLen - 1] |= (1 << 1);
        linesMask[pLen] |= (1 << 0);
        lineColor[pLen, 0] |= (1 << 1);
        lineColor[pLen, 1] |= (1 << 1);
        lines[pLen - 1, 2] = lines[pLen, 0] = lines[pLen, 1] = 0;
        // K point line
        linesMask[kLen - 1] |= (1 << 1);
        linesMask[kLen] |= (1 << 0);
        lineColor[kLen, 0] |= (1 << 1);
        lineColor[kLen, 1] |= (1 << 1);
        lines[kLen - 1, 2] = lines[kLen, 0] = lines[kLen, 1] = 0;
        // L point line
        linesMask[lLen - 1] |= (1 << 1);
        linesMask[lLen] |= (1 << 0);
        lineColor[lLen, 0] |= (1 << 1);
        lineColor[lLen, 1] |= (1 << 1);
        lines[lLen - 1, 2] = lines[lLen, 0] = lines[lLen, 1] = 0;
        // U point line
        linesMask[uLen - 1] |= (1 << 1);
        // linesMask[uLen] |= (1 << 0);
        lineColor[uLen, 0] |= (1 << 1);
        // lineColor[uLen, 1] |= (1 << 1);
        lines[uLen - 1, 2] = lines[uLen, 0] = lines[uLen, 1] = 0;

        // sidelines from P to L
        for (int i = 3 * (pLen - 1) + 2; i <= 3 * lLen + 1; i++) lines[(i / 3), i % 3] |= (1 << 0);
        for (int i = 3 * kLen; i <= 3 * lLen + 1; i++) lineColor[(i / 3), i % 3] |= (1 << 0);


        List<float> b = new List<float>();
        List<float> d = new List<float>();
        List<Vector2> pts = new List<Vector2>();
        List<int> sideLines = new List<int>();
        List<int> colorList = new List<int>();

        for (int i = 0; i < landingAreaPoints.Length - 1; i++)
        {
            float b0 = landingAreaPoints[i].x <= K.x ? (hillB2 / 2) + landingAreaPoints[i].x / K.x * ((hillBK - hillB2) / 2) :
                landingAreaPoints[i].x >= U.x ? (hillBU / 2) : (hillBK / 2) + (landingAreaPoints[i].x - K.x) / (U.x - K.x) * ((hillBU - hillBK) / 2);

            // |--|--------|--|--|---/
            // 0  1        2  0  1


            // level 0
            d.Add(i);
            b.Add(b0);
            pts.Add(landingAreaPoints[i]);

            sideLines.Add(lines[i, 0]);
            colorList.Add(lineColor[i, 0]);

            // break after last point
            if (i == landingAreaPoints.Length - 1) break;

            float b1 = landingAreaPoints[i + 1].x <= K.x ? (hillB2 / 2) + landingAreaPoints[i + 1].x / K.x * ((hillBK - hillB2) / 2) :
                landingAreaPoints[i + 1].x >= U.x ? (hillBU / 2) : (hillBK / 2) + (landingAreaPoints[i + 1].x - K.x) / (U.x - K.x) * ((hillBU - hillBK) / 2);

            // level 1
            if ((linesMask[i] & (1 << 0)) != 0)
            {
                d.Add(i + lineWidth);
                b.Add(Mathf.Lerp(b0, b1, lineWidth));
                pts.Add(Vector2.Lerp(landingAreaPoints[i], landingAreaPoints[i + 1], lineWidth));

                sideLines.Add(lines[i, 1]);
                colorList.Add(lineColor[i, 1]);
            }

            // level 2
            if ((linesMask[i] & (1 << 1)) != 0)
            {
                d.Add(i + 1 - lineWidth);
                b.Add(Mathf.Lerp(b0, b1, 1 - lineWidth));
                pts.Add(Vector2.Lerp(landingAreaPoints[i], landingAreaPoints[i + 1], 1 - lineWidth));

                sideLines.Add(lines[i, 2]);
                colorList.Add(lineColor[i, 2]);
            }
        }

        // vertices, uvs & triangles
        for (int i = 0; i < pts.Count; i++)
        {
            verticesList.Add(new Vector3(pts[i].x, pts[i].y, -b[i]));
            uvsList.Add(new Vector2(d[i], -b[i]));

            if ((sideLines[i] & (1 << 0)) != 0)
            {
                verticesList.Add(new Vector3(pts[i].x, pts[i].y, -b[i] + sideLineWidth));
                uvsList.Add(new Vector2(d[i], -b[i] + sideLineWidth));
            }

            if ((sideLines[i] & (1 << 1)) != 0)
            {
                verticesList.Add(new Vector3(pts[i].x, pts[i].y, -whiteLineWidth));
                uvsList.Add(new Vector2(d[i], -whiteLineWidth));
                verticesList.Add(new Vector3(pts[i].x, pts[i].y, whiteLineWidth));
                uvsList.Add(new Vector2(d[i], whiteLineWidth));
            }

            if ((sideLines[i] & (1 << 0)) != 0)
            {
                verticesList.Add(new Vector3(pts[i].x, pts[i].y, b[i] - sideLineWidth));
                uvsList.Add(new Vector2(d[i], b[i] - sideLineWidth));
            }

            verticesList.Add(new Vector3(pts[i].x, pts[i].y, b[i]));
            uvsList.Add(new Vector2(d[i], b[i]));


            if (i > 0)
            {
                int x = verticesList.Count;
                int cnt0 = 2, cnt1 = 2;
                for (int j = 0; j < maxSL; j++)
                {
                    if (((1 << j) & sideLines[i - 1]) != 0) cnt0 += 2;
                    if (((1 << j) & sideLines[i]) != 0) cnt1 += 2;
                }

                int it0 = 0, it1 = 0, l0 = 0, l1 = 0, jj = -1, listId = 0;

                for (int j = 0; j < maxSL; j++)
                {
                    if (((1 << j) & sideLines[i - 1]) != 0) it0++;
                    if (((1 << j) & sideLines[i]) != 0) it1++;

                    if (((1 << j) & sideLines[i - 1] & sideLines[i]) != 0)
                    {
                        listId = 0;
                        if (j == 0) listId = 2 + (colorList[i] & 1);
                        facesList[listId].Add(((x - cnt0 - cnt1) + l0, (x - cnt0 - cnt1) + it0, (x - cnt1) + l1, (x - cnt1) + it1));
                        facesList[listId].Add(((x - cnt1 - 1) - it0, (x - cnt1 - 1) - l0, (x - 1) - it1, (x - 1) - l1));
                        l0 = it0; l1 = it1;
                        jj = j;
                    }
                }

                listId = 0;
                if ((colorList[i] & (1 << 1)) != 0)
                {
                    if (jj == 0) listId = 2 + (colorList[i] & 1);
                    else listId = 1;
                }

                facesList[listId].Add(((x - cnt0 - cnt1) + l0, (x - cnt1 - 1) - l0, (x - cnt1) + l1, (x - 1) - l1));
            }
        }

        mesh.vertices = verticesList.ToArray();
        mesh.uv = uvsList.ToArray();
        mesh.subMeshCount = 4;

        for (int i = 0; i < 4; i++)
        {
            mesh.SetTriangles(MeshFunctions.FacesToTriangles(facesList[i]), i);
        }

        mesh.RecalculateNormals();
        return mesh;
    }

    public Material[] GetMaterials()
    {
        return this.materials;
    }

}