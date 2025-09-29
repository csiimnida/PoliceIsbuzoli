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

        public event Action<int> TotalCaughtChangeEvent;

        private void Awake()
        {
            _nationals = _nationalsFinder.GetTarget<NationalStatistics>();
            foreach (var national in _nationals)
            {
                _nationalDictionary.Add(national.NationalName, national);
                national.CaughtPeopleEvent += TotalCaughtChange;
            }
        }

        private void OnDestroy()
        {
            foreach (var nationalDictionaryValue in _nationalDictionary.Values)
            {
                nationalDictionaryValue.CaughtPeopleEvent -= TotalCaughtChange;
            }
        }

        private void TotalCaughtChange()
        {
            int total = 0;
            
            _nationals.ForEach(n => total += n.GetTotalCaught());
            
            TotalCaughtChangeEvent?.Invoke(total);
        }

        private void Start()
        {
            dataSetting.TrySetting();
            foreach (var national in _nationalDictionary)
            {
                national.Value.SetTotalPeople(DataContructor.Instance.GetData<NationalData>(national.Key).FirstTotalPeople);
            }
        }
        
        

        public void UpgradeSet(float infectivity = -1,float spreadTime = -1
            , float populationDensity = -1,float stealth = -1, float getPoint = -1 , int totalPeople = -1)
        {
            foreach (var keyValuePair in _nationalDictionary)
            {
                ChangeNationalState(keyValuePair.Key, infectivity, spreadTime, populationDensity, stealth, getPoint,
                    totalPeople);
            }
        }

        public void UpgradePlus(float infectivity = -1,float spreadTime = -1
            , float populationDensity = -1,float stealth = -1, float getPoint = -1 , int totalPeople = -1)
        {
            foreach (var keyValuePair in _nationalDictionary)
            {
                PlusNationalState(keyValuePair.Key, infectivity, spreadTime, populationDensity, stealth, getPoint,
                    totalPeople);
            }
        }

        public void UpgradeMultiply(float infectivity = -1,float spreadTime = -1
            , float populationDensity = -1,float stealth = -1, float getPoint = -1 , int totalPeople = -1)
        {
            foreach (var keyValuePair in _nationalDictionary)
            {
                MultiplyNationalState(keyValuePair.Key, infectivity, spreadTime, populationDensity, stealth, getPoint,
                    totalPeople);
            }
        }

        private void Set(float data, string fieldName)
        {
            switch (fieldName)
            {
                case "Infectivity":
                    UpgradeSet(infectivity: data);
                    break;
                case "SpreadTime":
                    UpgradeSet(spreadTime: data);
                    break;
                case "PopulationDensity":
                    UpgradeSet(populationDensity: data);
                    break;
                case "Stealth":
                    UpgradeSet(stealth: data);
                    break;
                case "GetPoint":
                    UpgradeSet(getPoint: data);
                    break;
            }
        }
        
        private void Add(float data, string fieldName)
        {
            switch (fieldName)
            {
                case "Infectivity":
                    UpgradePlus(infectivity: data);
                    break;
                case "SpreadTime":
                    UpgradePlus(spreadTime: data);
                    break;
                case "PopulationDensity":
                    UpgradePlus(populationDensity: data);
                    break;
                case "Stealth":
                    UpgradePlus(stealth: data);
                    break;
                case "GetPoint":
                    UpgradePlus(getPoint: data);
                    break;
            }
        }
        
        private void Multiply(float data, string fieldName)
        {
            switch (fieldName)
            {
                case "Infectivity":
                    UpgradeMultiply(infectivity: data);
                    break;
                case "SpreadTime":
                    UpgradeMultiply(spreadTime: data);
                    break;
                case "PopulationDensity":
                    UpgradeMultiply(populationDensity: data);
                    break;
                case "Stealth":
                    UpgradeMultiply(stealth: data);
                    break;
                case "GetPoint":
                    UpgradeMultiply(getPoint: data);
                    break;
            }
        }

        public void Upgrade(ModifyType type, float data, string fieldName)
        {
            switch (type)
            {
                case ModifyType.ADD:
                    Add(data, fieldName);
                    break;
                case ModifyType.SET:
                    Set(data, fieldName);
                    break;
                case ModifyType.MULTIFLY:
                    Multiply(data, fieldName);
                    break;
            }
        }

        public void Upgrade(ModifyType type, int data)
        {
            switch (type)
            {
                case ModifyType.ADD:
                    UpgradePlus(totalPeople: data);
                    break;
                case ModifyType.SET:
                    UpgradeSet(totalPeople: data);
                    break;
                case ModifyType.MULTIFLY:
                    UpgradeMultiply(totalPeople: data);
                    break;
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

        public void ChangePeople(string nationalName, int people)
        {
            ChangeNationalState(nationalName, totalPeople:people);
        }

        public void ChangeNationalState(string nationalName,float infectivity = -1,float spreadTime = -1
            , float populationDensity = -1,float stealth = -1, float getPoint = -1 , int totalPeople = -1)
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
            if(totalPeople != -1)
                nationalData.TotalPeople = totalPeople;
            DataContructor.Instance.SetData(nationalData);
        }
        #endregion

        #region Plus
        public void PlusInfectivity(string nationalName, float infectivity)
        {
            PlusNationalState(nationalName, infectivity:infectivity);
        }

        public void PlusSpreadTime(string nationalName, float spreadTime)
        {
            PlusNationalState(nationalName, spreadTime:spreadTime);
        }

        public void PlusPopulationDensity(string nationalName, float populationDensity)
        {
            PlusNationalState(nationalName, populationDensity:populationDensity);
        }

        public void PlusStealth(string nationalName, float stealth)
        {
            PlusNationalState(nationalName, stealth:stealth);
        }

        public void PlusPoint(string nationalName, float point)
        {
            PlusNationalState(nationalName, getPoint:point);
        }

        public void PlusPeople(string nationalName, int people)
        {
            PlusNationalState(nationalName, totalPeople:people);
        }

        public void PlusNationalState(string nationalName, float infectivity = 0, float spreadTime = 0
            , float populationDensity = 0, float stealth = 0, float getPoint = 0, int totalPeople = 0)
        {
            NationalData nationalData = DataContructor.Instance.GetData<NationalData>(nationalName);
            if(!Mathf.Approximately(infectivity, 0))
                nationalData.Infectivity += infectivity;
            if (!Mathf.Approximately(spreadTime, 0))
                nationalData.SpreadTime += spreadTime;
            if (!Mathf.Approximately(populationDensity, 0))
                nationalData.PopulationDensity += populationDensity;
            if (!Mathf.Approximately(stealth, 0))
                nationalData.Stealth += stealth;
            if(!Mathf.Approximately(getPoint, 0))
                nationalData.GetPoint += getPoint;
            if(totalPeople != 0)
                nationalData.TotalPeople += totalPeople;
            DataContructor.Instance.SetData(nationalData);
        }
        #endregion

        #region Multiply
        public void MultiplyInfectivity(string nationalName, float infectivity)
        {
            MultiplyNationalState(nationalName, infectivity:infectivity);
        }

        public void MultiplySpreadTime(string nationalName, float spreadTime)
        {
            MultiplyNationalState(nationalName, spreadTime:spreadTime);
        }

        public void MultiplyPopulationDensity(string nationalName, float populationDensity)
        {
            MultiplyNationalState(nationalName, populationDensity:populationDensity);
        }

        public void MultiplyStealth(string nationalName, float stealth)
        {
            MultiplyNationalState(nationalName, stealth:stealth);
        }

        public void MultiplyPoint(string nationalName, float point)
        {
            MultiplyNationalState(nationalName, getPoint:point);
        }

        public void MultiplyPeople(string nationalName, int people)
        {
            MultiplyNationalState(nationalName, totalPeople:people);
        }

        public void MultiplyNationalState(string nationalName, float infectivity = 1, float spreadTime = 1
            , float populationDensity = 1, float stealth = 1, float getPoint = 1, int totalPeople = 1)
        {
            NationalData nationalData = DataContructor.Instance.GetData<NationalData>(nationalName);
            if(!Mathf.Approximately(infectivity, 1))
                nationalData.Infectivity *= infectivity;
            if (!Mathf.Approximately(spreadTime, 1))
                nationalData.SpreadTime *= spreadTime;
            if (!Mathf.Approximately(populationDensity, 1))
                nationalData.PopulationDensity *= populationDensity;
            if (!Mathf.Approximately(stealth, 1))
                nationalData.Stealth *= stealth;
            if(!Mathf.Approximately(getPoint, 1))
                nationalData.GetPoint *= getPoint;
            if(totalPeople != 1)
                nationalData.TotalPeople = Mathf.RoundToInt(nationalData.TotalPeople * totalPeople);
            DataContructor.Instance.SetData(nationalData);
        }
        #endregion

        #region Get
        public NationalData GetNationalData(string nationalName)
        {
            NationalData nationalData = DataContructor.Instance.GetData<NationalData>(nationalName);
            return nationalData;
        }

        public int GetTotalPeople(string nationalName)
        {
            return GetNationalData(nationalName).TotalPeople;
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
                national.CaughtPeopleEvent -= TotalCaughtChange;

            }
        }
    }
}