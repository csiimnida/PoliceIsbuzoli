using System;
using System.Collections.Generic;
using System.Reflection;
using CSI._01_Script.System;
using MoonLib.ScriptFinder.RunTime.Finder.OneFinder;
using UnityEngine;

namespace Code.MSM
{
    public class NationalManager : MonoSingleton<NationalManager>
    {
        [SerializeField] private ScriptAllFinderSO _nationalsFinder;
        [SerializeField] private DataSetting dataSetting;

        private Dictionary<string, NationalStatistics> _nationalDictionary = new Dictionary<string, NationalStatistics>();
        
        private List<NationalStatistics> _nationals;
        
        [SerializeField] private string targetType;
        [SerializeField] private string peopleName;

        private void Awake()
        {
            _nationals = _nationalsFinder.GetTarget<NationalStatistics>();
            foreach (var national in _nationals)
            {
                _nationalDictionary.Add(national.NationalName, national);
            }
        }

        private void Start()
        {
            dataSetting.TrySetting();
            foreach (var national in _nationalDictionary)
            {
                national.Value.SetTotalPeople(DataContructor.Instance.GetData<NationalData>(national.Key).FirstTotalPeople);
            }
        }

        #region Change
        public void ChangeInfectivity(string nationalName, float infectivity)
        {
            ChangeNationalState(nationalName, infectivity:infectivity);
        }
        
        public void ChangeSpreadTime(string nationalName, float spreadTime)
        {
            ChangeNationalState(nationalName, spreadTime:spreadTime);
        }

        public void ChangePopulationDensity(string nationalName , float populationDensity)
        {
            ChangeNationalState(nationalName, populationDensity:populationDensity);
        }

        public void ChangeStealth(string nationalName, float stealth)
        {   
            ChangeNationalState(nationalName, stealth: stealth);
        }

        public void ChangePoint(string nationalName, float point)
        {
            ChangeNationalState(nationalName, getPoint:point);
        }
        
        public void ChangeNationalState(string nationalName,float infectivity = -1,float spreadTime = -1
            , float populationDensity = -1,float stealth = -1, float getPoint = -1)
        {
            NationalData nationalData = DataContructor.Instance.GetData<NationalData>(nationalName);
            if(!Mathf.Approximately(infectivity, -1))
                nationalData.Infectivity = infectivity;
            if (!Mathf.Approximately(spreadTime, -1))
                nationalData.SpreadTime = spreadTime;
            if (!Mathf.Approximately(populationDensity, -1))
                nationalData.PopulationDensity = populationDensity;
            if (!Mathf.Approximately(stealth, -1))
                nationalData.Stealth = stealth;
            if(!Mathf.Approximately(getPoint, -1))
                nationalData.GetPoint = getPoint;
            DataContructor.Instance.SetData(nationalData);
        }
        #endregion

        #region Get

        public NationalData GetNationalData(string nationalName)
        {
            NationalData nationalData = DataContructor.Instance.GetData<NationalData>(nationalName);
            return nationalData;
        }

        public float GetPoint(string nationalName)
        {
            return GetNationalData(nationalName).GetPoint;
        }

        public float GetInfectivity(string nationalName)
        {
            return GetNationalData(nationalName).Infectivity;
        }

        public float GetSpreadTime(string nationalName)
        {
            return GetNationalData(nationalName).SpreadTime;
        }

        public float GetPopulationDensity(string nationalName)
        {
            return GetNationalData(nationalName).PopulationDensity;
        }

        public float GetStealth(string nationalName)
        {
            return GetNationalData(nationalName).Stealth;
        }

        #endregion

        public void FailedState(string nationalName)
        {
            if (_nationalDictionary.Remove(nationalName, out var national))
            {
                //혹시 필요할까봐 남겨둠
                //_nationals.Remove(national);
            }
        }
    }
}