using Plugins.ScriptFinder.RunTime.Attribute;
using Plugins.ScriptFinder.RunTime.Serializable;
using UnityEditor;
using UnityEngine;

namespace Plugins.ScriptFinder.RunTime.Finder.OneFinder
{
    public class ScriptOneFinderBase : ScriptFinderBase
    {
        
#if UNITY_EDITOR
        [field:InfoBox("Types must be not null", MessageType.Warning, nameof(_isWarning))]
#endif
        [field:Header("MonoBehaviour or Interface type to find in the scene")]
        [field: SerializeField]public SerializableType KeyType { get; protected set; }
        
        [SerializeField, HideInInspector]
        protected bool _isWarning = false;
        
        public override void Initialize() { }
        
        private void OnValidate()
        {
            _isWarning = KeyType.Type == null;
            if(KeyType.Type == null) return;
            if (KeyType.Type.IsAbstract || KeyType.Type.IsInterface)
            {
                IsFindChild = true;
            }
        }
    }
}