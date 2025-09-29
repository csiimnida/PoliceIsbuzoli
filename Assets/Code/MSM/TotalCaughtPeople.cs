using System;
using CSI._01_Script.System;
using UnityEngine;

namespace Code.MSM
{
    public class TotalCaughtPeople : MonoSingleton<TotalCaughtPeople>
    {
        public int TotalCaughtPeopleValue { get; private set; } = 0;

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
        }
    }
}
