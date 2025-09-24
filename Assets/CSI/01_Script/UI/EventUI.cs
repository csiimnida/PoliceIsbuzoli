using System;
using CSI._01_Script.System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace CSI._01_Script.UI
{
    public class EventUI : MonoSingleton<EventUI>
    {
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Image image;
        [SerializeField]private RectTransform obj;
        [SerializeField] private Button okBt;
        private void Awake()
        {
            Hide();
        }

        private void Hide()
        {
            obj.gameObject.SetActive(false);
        }

        public void ShowEvent(string titleText ,string textText,Sprite image,Action ok)
        {
            title.SetText(titleText);
            text.SetText(textText);
            this.image.sprite = image;
            okBt.onClick.AddListener(() =>
            {
                ok.Invoke();
                Debug.Log("Click");
                okBt.onClick.RemoveAllListeners();
            });
            
        }
    }
}