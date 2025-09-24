using System.Collections.Generic;
using MoonLib.ScriptFinder.RunTime.Finder.OneFinder;
using UnityEngine;

namespace Code.MSM
{
    public class NationalManager : MonoBehaviour
    {
        [SerializeField] private ScriptAllFinderSO _nationalsFinder;

        private Dictionary<string, NationalStatistics> _nationalDictionary = new Dictionary<string, NationalStatistics>();
        
        private List<NationalStatistics> _nationals;
        
        private void Awake()
        {
            _nationals = _nationalsFinder.GetTarget<NationalStatistics>();
            foreach (var national in _nationals)
            {
                _nationalDictionary.Add(national.NationalName, national);
            }
        }

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