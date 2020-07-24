using System.Linq;
using OpenSkiJumping.Competition;
using OpenSkiJumping.Data;
using OpenSkiJumping.Hills;

namespace OpenSkiJumping.UI.HillsMenu
{
    public class HillInfoPresenter
    {
        private readonly IHillInfoView view;

        public HillInfoPresenter(IHillInfoView view)
        {
            this.view = view;

            InitEvents();
            SetInitValues();
        }

        private void PresentItemInfo()
        {
            var item = view.SelectedHill;
            if (item == null)
            {
                view.ItemInfoEnabled = false;
                return;
            }

            view.ItemInfoEnabled = true;

            view.Name = item.name;
            view.ProfileType = (int) item.type;
            view.Gates = item.gates;
            view.TerrainSteepness = item.terrainSteepness;
            view.W = item.w;
            view.Hn = item.hn;
            view.Gamma = item.gamma;
            view.Alpha = item.alpha;
            view.E = item.e;
            view.Es = item.es;
            view.T = item.t;
            view.R1 = item.r1;
            view.BetaP = item.betaP;
            view.BetaK = item.betaK;
            view.BetaL = item.betaL;
            view.S = item.s;
            view.L1 = item.l1;
            view.L2 = item.l2;
            view.RL = item.rL;
            view.R2L = item.r2L;
            view.R2 = item.r2;
            view.A = item.a;
            view.RA = item.rA;
            view.BetaA = item.betaA;
            view.B1 = item.b1;
            view.B2 = item.b2;
            view.BK = item.bK;
            view.BU = item.bU;
            view.D = item.d;
            view.Q = item.q;
            view.GateStairsLeft = item.gateStairsLeft;
            view.GateStairsRight = item.gateStairsRight;
            view.InrunStairsLeft = item.inrunStairsLeft;
            view.InrunStairsRight = item.inrunStairsRight;
            view.InrunStairsAngle = item.inrunStairsAngle;
        }

        private void SaveItemInfo()
        {
            var item = view.SelectedHill;
            if (item == null) return;

            item.name = view.Name;
            item.type = (ProfileType) view.ProfileType;
            item.gates = view.Gates;
            item.terrainSteepness = view.TerrainSteepness;
            item.w = view.W;
            item.hn = view.Hn;
            item.gamma = view.Gamma;
            item.alpha = view.Alpha;
            item.e = view.E;
            item.es = view.Es;
            item.t = view.T;
            item.r1 = view.R1;
            item.betaP = view.BetaP;
            item.betaK = view.BetaK;
            item.betaL = view.BetaL;
            item.s = view.S;
            item.l1 = view.L1;
            item.l2 = view.L2;
            item.rL = view.RL;
            item.r2L = view.R2L;
            item.r2 = view.R2;
            item.a = view.A;
            item.rA = view.RA;
            item.betaA = view.BetaA;
            item.b1 = view.B1;
            item.b2 = view.B2;
            item.bK = view.BK;
            item.bU = view.BU;
            item.d = view.D;
            item.q = view.Q;
            item.gateStairsLeft = view.GateStairsLeft;
            item.gateStairsRight = view.GateStairsRight;
            item.inrunStairsLeft = view.InrunStairsLeft;
            item.inrunStairsRight = view.InrunStairsRight;
            item.inrunStairsAngle = view.InrunStairsAngle;
        }

        private void InitEvents()
        {
            view.OnDataSourceChanged += PresentItemInfo;
            view.OnCurrentItemChanged += SaveItemInfo;
        }

        private void SetInitValues()
        {
            PresentItemInfo();
        }
    }
}