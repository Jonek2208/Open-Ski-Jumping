using System;
using System.Collections.Generic;
using OpenSkiJumping.Competition.Persistent;
using OpenSkiJumping.Hills;

namespace OpenSkiJumping.UI.HillsMenu
{
    public interface IHillInfoView
    {
        ProfileData SelectedHill { get;set; }
        string Json { get; set; }
        string Name { get; set; }
        int ProfileType { get; set; }
        int Gates { get; set; }
        float TerrainSteepness { get; set; }

        float W { get; set; }
        float h { get; set; }
        float Gamma { get; set; }
        float Alpha { get; set; }
        float E { get; set; }
        float Es { get; set; }
        float T { get; set; }
        float R1 { get; set; }
        float BetaP { get; set; }
        float BetaK { get; set; }
        float BetaL { get; set; }
        float S { get; set; }
        float L1 { get; set; }
        float L2 { get; set; }
        float RL { get; set; }
        float R2L { get; set; }
        float R2 { get; set; }

        float A { get; set; }
        float RA { get; set; }
        float BetaA { get; set; }
        float B1 { get; set; }
        float B2 { get; set; }
        float BK { get; set; }
        float BU { get; set; }
        float D { get; set; }
        float Q { get; set; }

        bool GateStairsLeft { get; set; }
        bool GateStairsRight { get; set; }
        bool InrunStairsLeft { get; set; }
        bool InrunStairsRight { get; set; }
        float InrunStairsAngle { get; set; }


        event Action OnCurrentItemChanged;
        bool ItemInfoEnabled { set; }
        event Action OnDataSourceChanged;
    }
}