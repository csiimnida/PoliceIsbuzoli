using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.MSM
{
    public class NationalStatistics : MonoBehaviour
    {
        //전염도 ->  높을수록 사람들에게 퍼질 확률 증가 (x / 5)%
        //확산시간 -> 작을수록 더 빨리 퍼짐 (x)초
        //인구 밀집도 -> 높을수록 많은 사람 사용함 (x * x)명 
        //은밀성 -> 마약복용후 들키지 않을 확률(x * 100)%
        private float _infectivity;
        private float _spreadTime;
        private float _populationDensity;
        private float _stealth;

        public float PopulationDensity => _populationDensity * _populationDensity;
        
        private int _infectedPeople;
        
        private int _totalPeople;

        private float _timer= 0f;
        
        private void Awake()
        {
            
        }

        public void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= _spreadTime)
            {
                _timer = 0f;
                float infectivityPercent = Mathf.Min(Random.Range(_infectivity / 500, 1), 1);
                int temp = _infectedPeople;
                
                _infectedPeople = (int)Mathf.Min(_infectedPeople + (PopulationDensity * infectivityPercent), _totalPeople);
                
                int newlyInfected = _infectedPeople - temp;

                float finedPeople = Mathf.Min(Random.Range(_stealth, 1f), 1f);
                
                int caughtPeople = (int)(newlyInfected * (finedPeople));
            }
        }
    }
}