using System.Collections.Generic;
using Plugins.ScriptFinder.RunTime.Finder.OneFinder;
using UnityEngine;

namespace Code.MSM
{
    public class NationalManager : MonoBehaviour
    {
        [SerializeField] private ScriptAllFinderSO _nationalsFinder;

        private List<NationalStatistics> _nationals;
        
        private void Awake()
        {
            _nationals = _nationalsFinder.GetTarget<NationalStatistics>();
        }
    }
}