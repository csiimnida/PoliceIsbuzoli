using System;
using System.Collections.Generic;
using Code.LSW.Code.So;
using CSI._01_Script.System;
using CSI._01_Script.UI;
using csiimnida.CSILib.SoundManager.RunTime;
using TMPro;
using UnityEngine;

namespace Code.LSW.Code.UI
{
    public class UIManager : MonoSingleton<UIManager>
    {
        [SerializeField] private UpgradeNodeInfoUI nodeInfoUI;
        [SerializeField] private GameObject gameOverUI;
        [SerializeField] private GameObject gameClearUI;
        [SerializeField] private TextMeshProUGUI coinsUI;
        
        [Header("Sound")]
        public string inGameBGM = "InGameBGM";
        public string btnClickSound = "ButtonClick";

        private void Start()
        {
            SoundManager.Instance.PlaySound(inGameBGM);
        }
        
        public void PlayButtonClick() =>
            SoundManager.Instance.PlaySound(btnClickSound);

        private void Update()
        {
            coinsUI.text = $"Coins : {CurrencyGetter.Instance.Money}";
        }

        public void ShowNodeUI(Vector2 mousePos, string key, float cost)
        {
            nodeInfoUI.gameObject.SetActive(true);
            nodeInfoUI.Show(mousePos, key, cost);
        }
        
        public void ShowGameEndUI(bool isWins)
        {
            Time.timeScale = 0f;
            if (isWins)
                gameClearUI.SetActive(true);
            else
                gameOverUI.SetActive(true);
        }
        
        public void HideNodeUI()
        {
            nodeInfoUI.Hide();
            nodeInfoUI.gameObject.SetActive(false);
        }
    }

    [Serializable]
    public class EventDataClass
    {
        public string name;
        public EventData eventData;
    }
}