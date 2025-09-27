using UnityEngine;

namespace Code.LSW.Code.UI
{
    public class InvestmentUI : MonoBehaviour
    {
        [SerializeField] private GameObject reachTabUI;
        [SerializeField] private GameObject crackdownTabUI;
        [SerializeField] private GameObject educationTabUI;
        [SerializeField] private GameObject cooperationTabUI;
        
        public void OnClick(InvestmentUIType type)
        {
            switch (type)
            {
                case InvestmentUIType.Reach:
                    reachTabUI.SetActive(true);
                    crackdownTabUI.SetActive(false);
                    educationTabUI.SetActive(false);
                    cooperationTabUI.SetActive(false);
                    break;
                case InvestmentUIType.Crackdown:
                    reachTabUI.SetActive(false);
                    crackdownTabUI.SetActive(true);
                    educationTabUI.SetActive(false);
                    cooperationTabUI.SetActive(false);
                    break;
                case InvestmentUIType.Education:
                    reachTabUI.SetActive(false);
                    crackdownTabUI.SetActive(false);
                    educationTabUI.SetActive(true);
                    cooperationTabUI.SetActive(false);
                    break;
                case InvestmentUIType.Cooperation:
                    reachTabUI.SetActive(false);
                    crackdownTabUI.SetActive(false);
                    educationTabUI.SetActive(false);
                    cooperationTabUI.SetActive(true);
                    break;
                default:
                    Debug.Log("없음");
                    break;   
            }
        }
    }
}