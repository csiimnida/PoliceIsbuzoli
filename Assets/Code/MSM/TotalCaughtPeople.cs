using System;
using CSI._01_Script.System;
using UnityEngine;

namespace Code.MSM
{
    public class TotalCaughtPeople : MonoSingleton<TotalCaughtPeople>
    {
        [Range(0, 50990000)]
        public int TotalCaughtPeopleValue { get; private set; } = 0;
        
        [Tooltip("전체 인구")]
        public int totalPeople = 50990000;

        [Header("State")]
        [Tooltip("0과 1 사이의 비율")]
        [Range(0f, 1f)] public float currentRate = 0f;
        
        private void Start()
        {
            NationalManager.Instance.TotalCaughtChangeEvent += SetTotalCaughtPeople;
        }

        private void OnDestroy()
        {
            NationalManager.Instance.TotalCaughtChangeEvent -= SetTotalCaughtPeople;
        }

        private void SetTotalCaughtPeople(int value)
        {
            TotalCaughtPeopleValue = value;
            
            float rate = 0f;
            if (totalPeople > 0)
            {
                rate = (float)TotalCaughtPeopleValue / (float)totalPeople;
            }

            currentRate = Mathf.Clamp01(rate);
        }
    }
}
