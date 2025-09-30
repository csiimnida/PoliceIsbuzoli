using csiimnida.CSILib.SoundManager.RunTime;
using UnityEngine;

namespace Code.LSW.Code.UI
{
    public enum InvestmentUIType
    {
        Reach, 
        Crackdown, 
        Education, 
        Cooperation
    }

    public class InvestmentUIButton : MonoBehaviour
    {
        [SerializeField] private InvestmentUI investmentUI;
        public InvestmentUIType type;
        
        public void OnClick()
        {
            UIManager.Instance.PlayButtonClick();
            investmentUI?.OnClick(type);
        }
    }
}