using System;
using Code.MSM;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Code.LSW.Code.UI
{
    public class ProliferationStatusUI : MonoBehaviour
    {    
        [Header("Optional UI References")] 
        [SerializeField] private Image fillImage;
        [SerializeField] private Slider slider;
        [SerializeField] private TextMeshProUGUI text;
        [Tooltip("비율 텍스트 보이게 할 건지")]
        [SerializeField] private bool showPercentText = true;
        
        [Header("Shader")]
        [SerializeField] private Material spreadMaterial;
        public string impulse = "";

        private void Start()
        {
            spreadMaterial.SetFloat(impulse, 0f);
        }

        public void Update()
        {
            Apply(TotalCaughtPeople.Instance.currentRate);
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
                text.SetText(showPercentText ? Mathf.RoundToInt(rate * 100f) + "%" : rate.ToString("0.##"));
            }
            
            spreadMaterial.SetFloat(impulse, rate);
            
            if(Mathf.Approximately(rate, 1f))  
                UIManager.Instance.ShowGameEndUI(false);
        }
    }
}
