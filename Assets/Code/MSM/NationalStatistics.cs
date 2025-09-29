using System;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Code.MSM
{
    public class NationalStatistics : MonoBehaviour
    {
        [field:SerializeField] public string NationalName { get; private set; }
        //전염도 ->  높을수록 사람들에게 퍼질 확률 증가 (x / 5)%
        //확산시간 -> 작을수록 더 빨리 퍼짐 (x)초
        //인구 밀집도 -> 높을수록 많은 사람 사용함 (x * x)명 
        //은밀성 -> 마약복용후 들키지 않을 확률(x * 100)%
        private float _infectivity;
        private float _spreadTime;
        private float _populationDensity;
        private float _stealth;
        
        public UnityEvent<int,int> totalAndCaughtPeopleEvent;

        public float PopulationDensity => _populationDensity * _populationDensity;
        
        private int _infectedPeople;
        
        private int _totalPeople;
        
        private int _totalCaughtPeople = 0;

        private float _timer= 0f;

        public void SetTotalPeople(int value)
        {
            _totalPeople = value;
        }

        public int GetTotalPeople() => _totalPeople;

        public void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= _spreadTime)
            {
                _timer = 0f;
                
                NationalData nationalData = DataConstructor.Instance.GetData<NationalData>(NationalName);
                _infectivity = nationalData.Infectivity;
                _spreadTime = nationalData.SpreadTime;
                _populationDensity = nationalData.PopulationDensity;
                _stealth = nationalData.Stealth;
                
                float infectivityPercent = Mathf.Min(Random.Range(_infectivity / 500, 1), 1);//전염 확률 계산
                int temp = _infectedPeople;
                
                _infectedPeople = (int)Mathf.Min(_infectedPeople + (PopulationDensity * infectivityPercent), _totalPeople);//전염 된사람
                
                int newlyInfected = _infectedPeople - temp;//새로 전염된 사람

                float finedPeople = Mathf.Min(Random.Range(_stealth, 1f), 1f);//들킨 사람 비율

                int caughtPeople = (int)((newlyInfected * (finedPeople)) + (float)(_totalPeople - _totalCaughtPeople) / 5 * finedPeople); //들킨 사람 수
                
                _totalCaughtPeople += caughtPeople;
                
                totalAndCaughtPeopleEvent?.Invoke(_totalPeople, caughtPeople);
            }
        }

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(NationalName))
            {
                gameObject.name = "NationalStatistics";
                return;
            }
            gameObject.name = NationalName;
        }
    }
}