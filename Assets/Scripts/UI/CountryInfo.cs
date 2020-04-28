using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OpenSkiJumping.UI
{
    public class CountryInfo : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text countryName;
        [SerializeField]
        private Image flagImage;

        public TMP_Text CountryName { get => countryName; set => countryName = value; }
        public Image FlagImage { get => flagImage; set => flagImage = value; }
    }
}
