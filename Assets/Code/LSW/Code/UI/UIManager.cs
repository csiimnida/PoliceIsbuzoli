using System;
using System.Collections.Generic;
using Code.LSW.Code.So;
using CSI._01_Script.System;
using CSI._01_Script.UI;
using UnityEngine;

namespace Code.LSW.Code.UI
{
    public class UIManager : MonoSingleton<UIManager>
    {
        [SerializeField] private UpgradeNodeInfoUI nodeInfoUI;
        [SerializeField] private List<EventDataClass> eventDatas = new List<EventDataClass>();
        
        public string startEventName = "Start";
        
        private void Awake()
        {
            EventUI.Instance.gameObject.SetActive(false);
        }

        private void Start()
        {
            SetEventUI(startEventName);
        }

        public void ShowNodeUI(Vector2 mousePos, string key, float cost)
        {
            nodeInfoUI.gameObject.SetActive(true);
            nodeInfoUI.Show(mousePos, key, cost);
        }
        
        public void HideNodeUI()
        {
            nodeInfoUI.Hide();
            nodeInfoUI.gameObject.SetActive(false);
        }
        
        public void SetEventUI(string eventName)
        {
            EventData eventData = eventDatas.Find(a => eventName == a.name).eventData;
            if (eventData == null)
            {
                Debug.LogWarning($"Can't find eventData : + {eventName}");
                return;
            }
            EventUI.Instance.gameObject.SetActive(true);
            EventUI.Instance.ShowEvent(eventData.titleText, eventData.textText, eventData.image, () =>
            {
                EventUI.Instance.gameObject.SetActive(false);
            });
        }
    }

    [Serializable]
    public class EventDataClass
    {
        public string name;
        public EventData eventData;
    }
}