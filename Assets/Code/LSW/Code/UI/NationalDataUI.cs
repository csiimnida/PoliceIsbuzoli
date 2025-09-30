using Code.MSM;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace Code.LSW.Code.UI
{
    public class NationalDataUI : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI nameText;
        
        [SerializeField] private TextMeshProUGUI infectivityText;
        [SerializeField] private TextMeshProUGUI spreadTimeText;
        [SerializeField] private TextMeshProUGUI populationDensityText;
        [SerializeField] private TextMeshProUGUI stealthText;
        
        [SerializeField] private Slider _nationProliferationSlider;
        
        public void OnSelectNation(string nationId)
        {
            var nationalData = DataConstructor.Instance.GetData<NationalData>(nationId);

            if (nationalData == null)
            {
                Debug.Log("데이터를 찾을 수 없음");
                return;
            }
            
            nameText.text = nationalData.NationalName;
            // _image = nationalData.;     도시 그림
            
            infectivityText.text = nationalData.Infectivity.ToString("전파율 : 0.##");
            spreadTimeText.text = nationalData.SpreadTime.ToString("확산 시간 : 0.##") + "초";
            populationDensityText.text = nationalData.PopulationDensity.ToString("인구 밀집도 : 0.##");
            stealthText.text = (nationalData.Stealth * 100).ToString("은밀성 : 0.##") + "%";
            
            // _nationProliferationSlider.value = nationalData.NationProliferation;
        }
    }
}