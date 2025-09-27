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
        // 테스트 용임ㅇㅇ
        private void OnValidate()
        {
            currentRate = Mathf.Clamp01(currentRate);
            Apply(currentRate);
        }

        // 이거 실행해주면 됨()
        public void UpdateStatus(float rate)
        {
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
                if (Mathf.Approximately(slider.minValue, 0f) && Mathf.Approximately(slider.maxValue, 1f))
                    slider.value = rate;
                else
                    slider.value = Mathf.Lerp(slider.minValue, slider.maxValue, rate);
            }

            if (text)
            {
                text.text = showPercentText ? Mathf.RoundToInt(rate * 100f) + "%" : rate.ToString("0.##");
            }

            onRateChanged?.Invoke(rate);
        }
    }
}
