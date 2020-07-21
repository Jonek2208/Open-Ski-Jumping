using System.Linq;
using UnityEngine;

namespace OpenSkiJumping.TVGraphics.SideResults
{
    public class SideResultsControllerIndividual : SideResultsController
    {
        protected override string GetNameById(int id)
        {
            return $"{competitorsList.competitors[id].lastName.ToUpper()} {competitorsList.competitors[id].firstName.FirstOrDefault()}. ";
        }

        protected override string GetCountryCodeById(int id)
        {
            return competitorsList.competitors[id].countryCode;
        }
    }
}