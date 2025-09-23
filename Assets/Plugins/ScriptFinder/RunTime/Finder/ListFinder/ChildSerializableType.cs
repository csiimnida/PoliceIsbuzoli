using System;
using Plugins.ScriptFinder.RunTime.Serializable;

namespace Plugins.ScriptFinder.RunTime.Finder.ListFinder
{
    [Serializable]
    public class ChildSerializableType
    {
        public SerializableType SType;
        public bool IsFindChild = false;
    }
}