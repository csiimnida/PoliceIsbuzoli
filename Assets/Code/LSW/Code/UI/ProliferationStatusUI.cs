using Code.MSM;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Code.LSW.Code.UI
{
    public class ProliferationStatusUI : MonoBehaviour
    {
        [Header("State")]
        [Tooltip("0과 1 사이의 비율")]
        [SerializeField, Range(0f, 1f)] private float currentRate = 0f;
        [Tooltip("전체 인구")]
        public int totalPeople = 50990000;

        [Header("Optional UI References")] 
        [SerializeField] private Image fillImage;
        [SerializeField] private Slider slider;
        [SerializeField] private TextMeshProUGUI text;
        [Tooltip("비율 텍스트 보이게 할 건지")]
        [SerializeField] private bool showPercentText = true;

        [Header("Events")]
        [Tooltip("바뀔 때 이벤트 실행")]
        [SerializeField] private UnityEvent<float> onRateChanged = new UnityEvent<float>();

        private void Awake()
        {
            Apply(currentRate);
        }
        
        public void Update()
        {
            int caught = TotalCaughtPeople.Instance.TotalCaughtPeopleValue;
            float rate = 0f;
            if (totalPeople > 0)
            {
                rate = (float)caught / (float)totalPeople;
            }

            currentRate = Mathf.Clamp01(rate);
            Apply(currentRate);
        }

        private void Apply(float rate)
        {
            if (fillImage)
            {
                fillImage.fillAmount = rate;
            }

            if (slider)
            {
                slider.normalizedValue = rate;
            }

            if (text)
            {
                text.text = showPercentText ? Mathf.RoundToInt(rate * 100f) + "%" : rate.ToString("0.##");
            }

            onRateChanged?.Invoke(rate);
        }
    }
}
