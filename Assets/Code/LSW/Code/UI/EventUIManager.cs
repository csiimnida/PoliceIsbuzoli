using System.Collections.Generic;
using Code.LSW.Code.So;
using CSI._01_Script.System;
using CSI._01_Script.UI;
using UnityEngine;

namespace Code.LSW.Code.UI
{
    public class EventUIManager : MonoSingleton<EventUIManager>
    {
        [SerializeField] private EventUI eventUIPrefab;
        [SerializeField] private Transform eventUIParent;
        private List<EventUI> eventUIPool = new List<EventUI>();
        private int poolSize = 10;
        
        private void Awake()
        {
            for (int i = 0; i < poolSize; i++)
            {
                EventUI ui = Instantiate(eventUIPrefab, eventUIParent);
                ui.gameObject.SetActive(false);
                eventUIPool.Add(ui);
            }
        }

        [SerializeField] private EventData testEventData;
        [ContextMenu( "TestEvent" )]
        public void Test()
        {
            SetEventUI(testEventData);
        }
        
        private EventUI GetPooledEventUI()
        {
            foreach (var ui in eventUIPool)
            {
                if (!ui.gameObject.activeInHierarchy)
                    return ui;
            }
            
            EventUI newUI = Instantiate(eventUIPrefab, eventUIParent);
            newUI.gameObject.SetActive(false);
            eventUIPool.Add(newUI);
            return newUI;
        }

        public void SetEventUI(EventData eventData)
        {
            EventUI ui = GetPooledEventUI();
            ui.gameObject.SetActive(true);
            ui.ShowEvent(eventData.titleText, eventData.textText, eventData.image, eventData.ok);
        }
    }
}