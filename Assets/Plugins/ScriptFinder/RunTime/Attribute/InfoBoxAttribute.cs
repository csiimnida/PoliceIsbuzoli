using System;
using UnityEditor;
using UnityEngine;

namespace MoonLib.ScriptFinder.RunTime.Attribute
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class InfoBoxAttribute : PropertyAttribute
    {
        public readonly string Message;
        public readonly MessageType Type;
        public readonly string VisibleIf;

        public InfoBoxAttribute(string message, MessageType type = MessageType.Info, string visibleIf = null)
        {
            this.Message = message;
            this.Type = type;
            this.VisibleIf = visibleIf;
        }
    }
}