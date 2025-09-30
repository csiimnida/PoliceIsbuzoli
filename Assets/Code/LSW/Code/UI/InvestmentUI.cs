using UnityEngine;

namespace Code.LSW.Code.UI
{
    public class InvestmentUI : MonoBehaviour
    {
        [SerializeField] private GameObject reachTabUI;
        [SerializeField] private GameObject crackdownTabUI;
        [SerializeField] private GameObject educationTabUI;
        [SerializeField] private GameObject incomeTabUI;
        
        public void OnClick(InvestmentUIType type)
        {
            UIManager.Instance.PlayButtonClick();
            switch (type)
            {
                case InvestmentUIType.Reach:
                    reachTabUI.SetActive(true);
                    crackdownTabUI.SetActive(false);
                    educationTabUI.SetActive(false);
                    incomeTabUI.SetActive(false);
                    break;
                case InvestmentUIType.Crackdown:
                    reachTabUI.SetActive(false);
                    crackdownTabUI.SetActive(true);
                    educationTabUI.SetActive(false);
                    incomeTabUI.SetActive(false);
                    break;
                case InvestmentUIType.Education:
                    reachTabUI.SetActive(false);
                    crackdownTabUI.SetActive(false);
                    educationTabUI.SetActive(true);
                    incomeTabUI.SetActive(false);
                    break;
                case InvestmentUIType.Cooperation:
                    reachTabUI.SetActive(false);
                    crackdownTabUI.SetActive(false);
                    educationTabUI.SetActive(false);
                    incomeTabUI.SetActive(true);
                    break;
                default:
                    Debug.Log("없음");
                    break;   
            }
        }
    }
}