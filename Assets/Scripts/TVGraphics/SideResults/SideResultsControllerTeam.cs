using UnityEngine;

namespace OpenSkiJumping.TVGraphics.SideResults
{
    public class SideResultsControllerTeam : SideResultsController
    {
        protected override string GetNameById(int id)
        {
            return flagsData.GetEnglishName(competitorsList.teams[id].countryCode).ToUpper();
        }

        protected override string GetCountryCodeById(int id)
        {
            return competitorsList.teams[id].countryCode;
        }
    }
}