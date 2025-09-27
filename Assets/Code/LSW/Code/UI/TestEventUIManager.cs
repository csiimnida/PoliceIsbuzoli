using System;
using System.Collections.Generic;
using Code.LSW.Code.So;
using CSI._01_Script.System;
using CSI._01_Script.UI;
using UnityEngine;

namespace Code.LSW.Code.UI
{
    public class TestEventUIManager : MonoSingleton<TestEventUIManager>
    {
        [SerializeField] private EventData testEventData;

        private void Awake()
        {
            EventUI.Instance.gameObject.SetActive(false);
        }

        [ContextMenu( "TestEvent" )]
        public void Test()
        {
            SetEventUI(testEventData);
        }

        public void SetEventUI(EventData eventData)
        {
            EventUI.Instance.gameObject.SetActive(true);
            EventUI.Instance.ShowEvent(eventData.titleText, eventData.textText, eventData.image, () =>
            {
                EventUI.Instance.gameObject.SetActive(false);
            });
        }
    }
}