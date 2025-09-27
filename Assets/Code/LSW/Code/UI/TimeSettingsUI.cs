using UnityEngine;

namespace Code.LSW.Code.UI
{
    public class TimeSettingsUI : MonoBehaviour
    {
        public void OnSetTime(float value)
        {
            Time.timeScale = value;
        }
    }
}