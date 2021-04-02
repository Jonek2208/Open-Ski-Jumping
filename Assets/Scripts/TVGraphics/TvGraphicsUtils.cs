using System.Globalization;
using OpenSkiJumping.Competition.Persistent;

namespace OpenSkiJumping.TVGraphics
{
    public static class TvGraphicsUtils
    {
        public static string JumperNameText(Competitor cmp)
        {
            return $"{cmp.firstName.ToUpper()} <b>{cmp.lastName.ToUpper()}</b>";
        }

        public static string ToBeatText(float val)
        {
            return $"<b>TO BEAT:</b> {val.ToString("F1", CultureInfo.InvariantCulture)} m";
        }

        public static string SpeedText(decimal val)
        {
            return $"{val.ToString("F1", CultureInfo.InvariantCulture)} km/h";
        }
        
        public static string DistanceText(decimal val)
        {
            return $"{val.ToString("F1", CultureInfo.InvariantCulture)} m";
        }
        
        public static string PointsText(decimal val)
        {
            return $"{val.ToString("F1", CultureInfo.InvariantCulture)}";
        }

        public static string NextAthleteText(Competitor nxt)
        {
            return $"<b>NEXT ATHLETE:</b> {nxt.firstName.ToUpper()} <b>{nxt.lastName.ToUpper()}</b>";
        }
    }
}