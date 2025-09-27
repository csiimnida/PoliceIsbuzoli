using System;
using UnityEngine;

namespace Code.LSW.Code.So
{
    [CreateAssetMenu(fileName = "EventDataSO", menuName = "SO/EventDataSO", order = 0)]
    public class EventData : ScriptableObject
    {
        public string titleText;
        public string textText;
        public Sprite image;
        public Action ok;
    }
}