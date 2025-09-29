using System;
using UnityEngine;

namespace Code.MSM
{
    public class TotalCaughtPeople : MonoBehaviour
    {
        public static int TotalCaughtPeopleValue { get; private set; } = 0;

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
