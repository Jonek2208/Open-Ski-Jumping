using System.Linq;
using OpenSkiJumping.Competition;
using OpenSkiJumping.Data;
using Newtonsoft.Json;
using OpenSkiJumping.Hills;

namespace OpenSkiJumping.UI.HillsMenu
{
    public class HillInfoPresenter
    {
        private readonly IHillInfoView _view;

        public HillInfoPresenter(IHillInfoView view)
        {
            this._view = view;

            InitEvents();
            SetInitValues();
        }

        private void PresentItemInfo()
        {
            var item = _view.SelectedHill;
            if (item == null)
            {
                _view.ItemInfoEnabled = false;
                return;
            }

            _view.ItemInfoEnabled = true;

            _view.Json = JsonConvert.SerializeObject(item, Formatting.Indented);
            // _view.Name = item.name;
            // _view.ProfileType = (int) item.type;
            // _view.Gates = item.gates;
            // _view.TerrainSteepness = item.terrainSteepness;
            // _view.W = item.w;
            // _view.h = item.h;
            // _view.Gamma = item.gamma;
            // _view.Alpha = item.alpha;
            // _view.E = item.e;
            // _view.Es = item.es;
            // _view.T = item.t;
            // _view.R1 = item.r1;
            // _view.BetaP = item.betaP;
            // _view.BetaK = item.betaK;
            // _view.BetaL = item.betaL;
            // _view.S = item.s;
            // _view.L1 = item.l1;
            // _view.L2 = item.l2;
            // _view.RL = item.rL;
            // _view.R2L = item.r2L;
            // _view.R2 = item.r2;
            // _view.A = item.a;
            // _view.RA = item.rA;
            // _view.BetaA = item.betaA;
            // _view.B1 = item.b1;
            // _view.B2 = item.b2;
            // _view.BK = item.bK;
            // _view.BU = item.bU;
            // _view.D = item.d;
            // _view.Q = item.q;
            // _view.GateStairsLeft = item.gateStairsLeft;
            // _view.GateStairsRight = item.gateStairsRight;
            // _view.InrunStairsLeft = item.inrunStairsLeft;
            // _view.InrunStairsRight = item.inrunStairsRight;
            // _view.InrunStairsAngle = item.inrunStairsAngle;
        }

        private void SaveItemInfo()
        {
            var item = _view.SelectedHill;
            if (item == null) return;
            
            // _view.SelectedHill = JsonConvert.DeserializeObject<ProfileData>(_view.Json);
            // item.name = _view.Name;
            // item.type = (ProfileType) _view.ProfileType;
            // item.gates = _view.Gates;
            // item.terrainSteepness = _view.TerrainSteepness;
            // item.w = _view.W;
            // item.h = _view.h;
            // item.gamma = _view.Gamma;
            // item.alpha = _view.Alpha;
            // item.e = _view.E;
            // item.es = _view.Es;
            // item.t = _view.T;
            // item.r1 = _view.R1;
            // item.betaP = _view.BetaP;
            // item.betaK = _view.BetaK;
            // item.betaL = _view.BetaL;
            // item.s = _view.S;
            // item.l1 = _view.L1;
            // item.l2 = _view.L2;
            // item.rL = _view.RL;
            // item.r2L = _view.R2L;
            // item.r2 = _view.R2;
            // item.a = _view.A;
            // item.rA = _view.RA;
            // item.betaA = _view.BetaA;
            // item.b1 = _view.B1;
            // item.b2 = _view.B2;
            // item.bK = _view.BK;
            // item.bU = _view.BU;
            // item.d = _view.D;
            // item.q = _view.Q;
            // item.gateStairsLeft = _view.GateStairsLeft;
            // item.gateStairsRight = _view.GateStairsRight;
            // item.inrunStairsLeft = _view.InrunStairsLeft;
            // item.inrunStairsRight = _view.InrunStairsRight;
            // item.inrunStairsAngle = _view.InrunStairsAngle;
        }

        private void InitEvents()
        {
            _view.OnDataSourceChanged += PresentItemInfo;
            _view.OnCurrentItemChanged += SaveItemInfo;
        }

        private void SetInitValues()
        {
            PresentItemInfo();
        }
    }
}