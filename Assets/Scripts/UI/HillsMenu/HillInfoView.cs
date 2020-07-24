using System;
using System.Globalization;
using OpenSkiJumping.Hills;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace OpenSkiJumping.UI.HillsMenu
{
    public class HillInfoView : MonoBehaviour, IHillInfoView
    {
        private HillInfoPresenter presenter;
        private ProfileData selectedHill;

        [SerializeField] private GameObject itemInfoObj;
        [SerializeField] private SegmentedControl profileTypeSelect;

        [SerializeField] private TMP_InputField nameInput;
        [SerializeField] private TMP_InputField gatesInput;
        [SerializeField] private TMP_InputField terrainSteepnessInput;
        [SerializeField] private TMP_InputField wInput;
        [SerializeField] private TMP_InputField hnInput;
        [SerializeField] private TMP_InputField gammaInput;
        [SerializeField] private TMP_InputField alphaInput;
        [SerializeField] private TMP_InputField eInput;
        [SerializeField] private TMP_InputField esInput;
        [SerializeField] private TMP_InputField tInput;
        [SerializeField] private TMP_InputField r1Input;
        [SerializeField] private TMP_InputField betaPInput;
        [SerializeField] private TMP_InputField betaKInput;
        [SerializeField] private TMP_InputField betaLInput;
        [SerializeField] private TMP_InputField sInput;
        [SerializeField] private TMP_InputField l1Input;
        [SerializeField] private TMP_InputField l2Input;
        [SerializeField] private TMP_InputField rLInput;
        [SerializeField] private TMP_InputField r2LInput;
        [SerializeField] private TMP_InputField r2Input;
        [SerializeField] private TMP_InputField aInput;
        [SerializeField] private TMP_InputField rAInput;
        [SerializeField] private TMP_InputField betaAInput;
        [SerializeField] private TMP_InputField b1Input;
        [SerializeField] private TMP_InputField b2Input;
        [SerializeField] private TMP_InputField bKInput;
        [SerializeField] private TMP_InputField bUInput;
        [SerializeField] private TMP_InputField dInput;
        [SerializeField] private TMP_InputField qInput;
        [SerializeField] private TMP_InputField inrunStairsAngleInput;
        [SerializeField] private Toggle gateStairsLeftToggle;
        [SerializeField] private Toggle gateStairsRightToggle;
        [SerializeField] private Toggle inrunStairsLeftToggle;
        [SerializeField] private Toggle inrunStairsRightToggle;

        public ProfileData SelectedHill
        {
            get => selectedHill;
            set
            {
                selectedHill = value;
                OnDataSourceChanged?.Invoke();
            }
        }

        public event Action OnCurrentItemChanged;
        public event Action OnDataSourceChanged;


        public bool ItemInfoEnabled
        {
            set => itemInfoObj.SetActive(value);
        }


        public void Initialize()
        {
            RegisterCallbacks();
            presenter = new HillInfoPresenter(this);
        }


        private void RegisterCallbacks()
        {
            RegisterInputFieldCallbacks(nameInput);
            RegisterInputFieldCallbacks(gatesInput);
            RegisterInputFieldCallbacks(gatesInput);
            RegisterInputFieldCallbacks(terrainSteepnessInput);
            RegisterInputFieldCallbacks(wInput);
            RegisterInputFieldCallbacks(hnInput);
            RegisterInputFieldCallbacks(gammaInput);
            RegisterInputFieldCallbacks(alphaInput);
            RegisterInputFieldCallbacks(eInput);
            RegisterInputFieldCallbacks(esInput);
            RegisterInputFieldCallbacks(tInput);
            RegisterInputFieldCallbacks(r1Input);
            RegisterInputFieldCallbacks(betaPInput);
            RegisterInputFieldCallbacks(betaKInput);
            RegisterInputFieldCallbacks(betaLInput);
            RegisterInputFieldCallbacks(sInput);
            RegisterInputFieldCallbacks(l1Input);
            RegisterInputFieldCallbacks(l2Input);
            RegisterInputFieldCallbacks(rLInput);
            RegisterInputFieldCallbacks(r2LInput);
            RegisterInputFieldCallbacks(r2Input);
            RegisterInputFieldCallbacks(aInput);
            RegisterInputFieldCallbacks(rAInput);
            RegisterInputFieldCallbacks(betaAInput);
            RegisterInputFieldCallbacks(b1Input);
            RegisterInputFieldCallbacks(b2Input);
            RegisterInputFieldCallbacks(bKInput);
            RegisterInputFieldCallbacks(bUInput);
            RegisterInputFieldCallbacks(dInput);
            RegisterInputFieldCallbacks(qInput);
            RegisterInputFieldCallbacks(inrunStairsAngleInput);
            RegisterToggleCallbacks(gateStairsLeftToggle);
            RegisterToggleCallbacks(gateStairsRightToggle);
            RegisterToggleCallbacks(inrunStairsLeftToggle);
            RegisterToggleCallbacks(inrunStairsRightToggle);

            profileTypeSelect.onValueChanged.AddListener(x => OnValueChanged());
        }

        private void RegisterInputFieldCallbacks(TMP_InputField inputField)
        {
            inputField.onEndEdit.AddListener(x => OnValueChanged());
        }

        private void RegisterToggleCallbacks(Toggle toggle)
        {
            toggle.onValueChanged.AddListener(x => OnValueChanged());
        }


        private void OnValueChanged()
        {
            OnCurrentItemChanged?.Invoke();
        }


        public int ProfileType
        {
            get => profileTypeSelect.selectedSegmentIndex;
            set => profileTypeSelect.SetSelectedSegmentWithoutNotify(value);
        }

        public string Name
        {
            get => nameInput.text;
            set => nameInput.SetTextWithoutNotify(value);
        }


        public int Gates
        {
            get => int.Parse(gatesInput.text);
            set => gatesInput.SetTextWithoutNotify(value.ToString());
        }

        public float TerrainSteepness
        {
            get => float.Parse(terrainSteepnessInput.text);
            set => terrainSteepnessInput.SetTextWithoutNotify(value.ToString(CultureInfo.InvariantCulture));
        }

        public float W
        {
            get => float.Parse(wInput.text);
            set => wInput.SetTextWithoutNotify(value.ToString(CultureInfo.InvariantCulture));
        }

        public float Hn
        {
            get => float.Parse(hnInput.text);
            set => hnInput.SetTextWithoutNotify(value.ToString(CultureInfo.InvariantCulture));
        }

        public float Gamma
        {
            get => float.Parse(gammaInput.text);
            set => gammaInput.SetTextWithoutNotify(value.ToString(CultureInfo.InvariantCulture));
        }

        public float Alpha
        {
            get => float.Parse(alphaInput.text);
            set => alphaInput.SetTextWithoutNotify(value.ToString(CultureInfo.InvariantCulture));
        }

        public float E
        {
            get => float.Parse(eInput.text);
            set => eInput.SetTextWithoutNotify(value.ToString(CultureInfo.InvariantCulture));
        }

        public float Es
        {
            get => float.Parse(esInput.text);
            set => esInput.SetTextWithoutNotify(value.ToString(CultureInfo.InvariantCulture));
        }

        public float T
        {
            get => float.Parse(tInput.text);
            set => tInput.SetTextWithoutNotify(value.ToString(CultureInfo.InvariantCulture));
        }

        public float R1
        {
            get => float.Parse(r1Input.text);
            set => r1Input.SetTextWithoutNotify(value.ToString(CultureInfo.InvariantCulture));
        }

        public float BetaP
        {
            get => float.Parse(betaPInput.text);
            set => betaPInput.SetTextWithoutNotify(value.ToString(CultureInfo.InvariantCulture));
        }

        public float BetaK
        {
            get => float.Parse(betaKInput.text);
            set => betaKInput.SetTextWithoutNotify(value.ToString(CultureInfo.InvariantCulture));
        }

        public float BetaL
        {
            get => float.Parse(betaLInput.text);
            set => betaLInput.SetTextWithoutNotify(value.ToString(CultureInfo.InvariantCulture));
        }

        public float S
        {
            get => float.Parse(sInput.text);
            set => sInput.SetTextWithoutNotify(value.ToString(CultureInfo.InvariantCulture));
        }

        public float L1
        {
            get => float.Parse(l1Input.text);
            set => l1Input.SetTextWithoutNotify(value.ToString(CultureInfo.InvariantCulture));
        }

        public float L2
        {
            get => float.Parse(l2Input.text);
            set => l2Input.SetTextWithoutNotify(value.ToString(CultureInfo.InvariantCulture));
        }

        public float RL
        {
            get => float.Parse(rLInput.text);
            set => rLInput.SetTextWithoutNotify(value.ToString(CultureInfo.InvariantCulture));
        }

        public float R2L
        {
            get => float.Parse(r2LInput.text);
            set => r2LInput.SetTextWithoutNotify(value.ToString(CultureInfo.InvariantCulture));
        }

        public float R2
        {
            get => float.Parse(r2Input.text);
            set => r2Input.SetTextWithoutNotify(value.ToString(CultureInfo.InvariantCulture));
        }

        public float A
        {
            get => float.Parse(aInput.text);
            set => aInput.SetTextWithoutNotify(value.ToString(CultureInfo.InvariantCulture));
        }

        public float RA
        {
            get => float.Parse(rAInput.text);
            set => rAInput.SetTextWithoutNotify(value.ToString(CultureInfo.InvariantCulture));
        }

        public float BetaA
        {
            get => float.Parse(betaAInput.text);
            set => betaAInput.SetTextWithoutNotify(value.ToString(CultureInfo.InvariantCulture));
        }

        public float B1
        {
            get => float.Parse(b1Input.text);
            set => b1Input.SetTextWithoutNotify(value.ToString(CultureInfo.InvariantCulture));
        }

        public float B2
        {
            get => float.Parse(b2Input.text);
            set => b2Input.SetTextWithoutNotify(value.ToString(CultureInfo.InvariantCulture));
        }

        public float BK
        {
            get => float.Parse(bKInput.text);
            set => bKInput.SetTextWithoutNotify(value.ToString(CultureInfo.InvariantCulture));
        }

        public float BU
        {
            get => float.Parse(bUInput.text);
            set => bUInput.SetTextWithoutNotify(value.ToString(CultureInfo.InvariantCulture));
        }

        public float D
        {
            get => float.Parse(dInput.text);
            set => dInput.SetTextWithoutNotify(value.ToString(CultureInfo.InvariantCulture));
        }

        public float Q
        {
            get => float.Parse(qInput.text);
            set => qInput.SetTextWithoutNotify(value.ToString(CultureInfo.InvariantCulture));
        }

        public float InrunStairsAngle
        {
            get => float.Parse(inrunStairsAngleInput.text);
            set => inrunStairsAngleInput.SetTextWithoutNotify(value.ToString(CultureInfo.InvariantCulture));
        }

        public bool GateStairsLeft
        {
            get => gateStairsLeftToggle.isOn;
            set => gateStairsLeftToggle.SetIsOnWithoutNotify(value);
        }

        public bool GateStairsRight
        {
            get => gateStairsRightToggle.isOn;
            set => gateStairsRightToggle.SetIsOnWithoutNotify(value);
        }

        public bool InrunStairsLeft
        {
            get => inrunStairsLeftToggle.isOn;
            set => inrunStairsLeftToggle.SetIsOnWithoutNotify(value);
        }

        public bool InrunStairsRight
        {
            get => inrunStairsRightToggle.isOn;
            set => inrunStairsRightToggle.SetIsOnWithoutNotify(value);
        }
    }
}