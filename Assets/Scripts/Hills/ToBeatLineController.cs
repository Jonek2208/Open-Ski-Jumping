using Competition.Runtime;
using ScriptableObjects.Variables;
using UnityEngine;

namespace Hills
{
    public class ToBeatLineController : MonoBehaviour
    {
        public Hill hill;
        public RuntimeResultsManager resultsManager;
        public LineRenderer lineRenderer;
        [SerializeField] private FloatVariable toBeatDistFV;

        public void SetLine()
        {
            float k = hill.w;
            float judges = 54.0f;

            float p = 60f, q = 4.8f;
            if (k < 25) { q = 4.8f; }
            else if (k < 30) { q = 4.4f; }
            else if (k < 35) { q = 4.0f; }
            else if (k < 40) { q = 3.6f; }
            else if (k < 50) { q = 3.2f; }
            else if (k < 60) { q = 2.8f; }
            else if (k < 70) { q = 2.4f; }
            else if (k < 80) { q = 2.2f; }
            else if (k < 100) { q = 2.0f; }
            else if (k < 165) { q = 1.8f; }
            else
            {
                q = 1.2f;
                p = 120f;
            }


            float leaderPoints = 0;
            float currentJumperPoints = 0;
            if (resultsManager.roundIndex > 0 || resultsManager.subroundIndex > 0 || resultsManager.startListIndex > 0)
            {
                leaderPoints = (float)resultsManager.AllroundResults.Keys[0].Item1;
            }
            if (resultsManager.roundIndex > 0 || resultsManager.subroundIndex > 0)
            {
                int competitorId = resultsManager.currentStartList[resultsManager.startListIndex];
                currentJumperPoints = (float)resultsManager.results[competitorId].TotalPoints;
            }

            float toBeatDist = (leaderPoints - currentJumperPoints - judges - p) / q + k;

            int toBeatDistDoubled = Mathf.Min(2 * (hill.landingAreaPoints.Length - 1), Mathf.RoundToInt(Mathf.Ceil(2 * toBeatDist)));
            // Debug.Log(p + " " + q + " " + toBeatDist + " " + toBeatDistDoubled + " " + (toBeatDistDoubled / 2));
            Vector2 point = hill.landingAreaPoints[toBeatDistDoubled / 2];
            Vector2 delta = hill.landingAreaPoints[toBeatDistDoubled / 2 + 1] - point;
            if (toBeatDistDoubled % 2 == 1) { point += delta / 2; }

            float b = point.x <= hill.K.x ? (hill.b2 / 2) + point.x / hill.K.x * ((hill.bK - hill.b2) / 2) :
                point.x >= hill.U.x ? (hill.bU / 2) : (hill.bK / 2) + (point.x - hill.K.x) / (hill.U.x - hill.K.x) * ((hill.bU - hill.bK) / 2);

            lineRenderer.SetPositions(new[] { new Vector3(point.x, point.y, -b), new Vector3(point.x, point.y, b) });

            toBeatDistFV.Value = toBeatDistDoubled / 2;
        }
    }
}