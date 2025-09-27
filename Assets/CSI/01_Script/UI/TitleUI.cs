
using NUnit.Framework.Internal.Commands;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using NotImplementedException = System.NotImplementedException;

namespace CSI._01_Script.UI
{
    public class TitleUI : MonoBehaviour
    {
        [SerializeField]private Button quitBt, startBt;
        [SerializeField] private string nextScreenName;
        private void Awake()
        {
            quitBt.onClick.AddListener(Quit);   
            startBt.onClick.AddListener(ChangeScreen);   
        }

        private void Quit()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        private void OnDestroy()
        {
            quitBt.onClick.RemoveListener(Quit);   
            startBt.onClick.RemoveListener(ChangeScreen);   
        }

        private void ChangeScreen()
        {
            SceneManager.LoadScene(nextScreenName);
        }
    }
}
