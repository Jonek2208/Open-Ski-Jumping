using OpenSkiJumping.Competition.Runtime;
using OpenSkiJumping.ScriptableObjects.Variables;
using UnityEngine;

namespace OpenSkiJumping.Hills
{
    public class ToBeatLineController : MonoBehaviour
    {
        public Hill hill;
        public RuntimeResultsManager resultsManager;
        public LineRenderer lineRenderer;
        [SerializeField] private FloatVariable toBeatDistFV;

        public void SetLine()
        {
            var k = hill.w;

            var judges = resultsManager.Value.EventInfo.roundInfos[resultsManager.Value.RoundIndex].disableJudgesMarks
                ? 0
                : 54.0f;

            float p = 60f, q = 4.8f;
            if (k < 25)
            {
                q = 4.8f;
            }
            else if (k < 30)
            {
                q = 4.4f;
            }
            else if (k < 35)
            {
                q = 4.0f;
            }
            else if (k < 40)
            {
                q = 3.6f;
            }
            else if (k < 50)
            {
                q = 3.2f;
            }
            else if (k < 60)
            {
                q = 2.8f;
            }
            else if (k < 70)
            {
                q = 2.4f;
            }
            else if (k < 80)
            {
                q = 2.2f;
            }
            else if (k < 100)
            {
                q = 2.0f;
            }
            else if (k < 165)
            {
                q = 1.8f;
            }
            else
            {
                q = 1.2f;
                p = 120f;
            }


            float leaderPoints = 0;
            float currentJumperPoints = 0;
            if (resultsManager.Value.RoundIndex > 0 || resultsManager.Value.SubroundIndex > 0 ||
                resultsManager.Value.StartListIndex > 0)
            {
                leaderPoints = (float) resultsManager.Value.GetResultByRank(0).TotalPoints;
            }

            if (resultsManager.Value.RoundIndex > 0 || resultsManager.Value.SubroundIndex > 0)
            {
                var competitorId = resultsManager.Value.StartList[resultsManager.Value.StartListIndex];
                currentJumperPoints = (float) resultsManager.Value.Results[competitorId].TotalPoints;
            }

            var toBeatDist = (leaderPoints - currentJumperPoints - judges - p) / q + k;

            var toBeatDistDoubled = Mathf.Min(2 * (hill.landingAreaPoints.Length - 1),
                Mathf.RoundToInt(Mathf.Ceil(2 * toBeatDist)));
            // Debug.Log(p + " " + q + " " + toBeatDist + " " + toBeatDistDoubled + " " + (toBeatDistDoubled / 2));
            var point = hill.landingAreaPoints[toBeatDistDoubled / 2];
            var delta = hill.landingAreaPoints[toBeatDistDoubled / 2 + 1] - point;
            if (toBeatDistDoubled % 2 == 1)
            {
                point += delta / 2;
            }

            var b = point.x <= hill.K.x ? (hill.b2 / 2) + point.x / hill.K.x * ((hill.bK - hill.b2) / 2) :
                point.x >= hill.U.x ? (hill.bU / 2) :
                (hill.bK / 2) + (point.x - hill.K.x) / (hill.U.x - hill.K.x) * ((hill.bU - hill.bK) / 2);

            lineRenderer.SetPositions(new[] {new Vector3(point.x, point.y, -b), new Vector3(point.x, point.y, b)});

            toBeatDistFV.Value = toBeatDistDoubled / 2;
        }
    }
}