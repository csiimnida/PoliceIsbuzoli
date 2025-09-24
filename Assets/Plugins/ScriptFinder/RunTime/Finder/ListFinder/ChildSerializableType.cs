using System;
using MoonLib.ScriptFinder.RunTime.Serializable;

namespace MoonLib.ScriptFinder.RunTime.Finder.ListFinder
{
    [Serializable]
    public class ChildSerializableType
    {
        public SerializableType SType;
        public bool IsFindChild = false;
    }
}