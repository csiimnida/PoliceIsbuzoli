using CSI._01_Script.System;
using UnityEngine;

namespace Code.LSW.Code.UI
{
    public class UIManager : MonoSingleton<UIManager>
    {
        [SerializeField] private UpgradeNodeInfoUI nodeInfoUI;
        
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

    }
}