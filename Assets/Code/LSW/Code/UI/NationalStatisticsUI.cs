using Code.MSM;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace Code.LSW.Code.UI
{
    public class NationalStatisticsUI : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI nameText;
        
        [SerializeField] private TextMeshProUGUI infectivityText;
        [SerializeField] private TextMeshProUGUI spreadTimeText;
        [SerializeField] private TextMeshProUGUI populationDensityText;
        [SerializeField] private TextMeshProUGUI stealthText;
        
        [SerializeField] private Slider _nationProliferationSlider;
        
        public void OnSelectNation(NationalStatistics nationStatistics)
        {
            // nameText.text = nationStatistics.;   이름
            // _image = nationStatistics.Image;     도시 그림
            
            // infectivityText.text = nationStatistics..ToString("0.##");
            // spreadTimeText.text = nationStatistics.SpreadTime.ToString("0.##") + "초";
            // populationDensityText.text = nationStatistics.PopulationDensity.ToString("0.##");
            // stealthText.text = (nationStatistics.Stealth * 100).ToString("0.##") + "%";
            
            // _nationProliferationSlider.value = nationStatistics.NationProliferation;
        }
    }
}