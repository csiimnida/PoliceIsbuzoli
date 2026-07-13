
using csiimnida.CSILib.SoundManager.RunTime;
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
        public string titleSound = "TitleBGM";
        public string btnClickSound = "ButtonClick";
        
        private void Awake()
        {
            quitBt.onClick.AddListener(Quit);   
            startBt.onClick.AddListener(ChangeScreen);   
        }

        private void Start()
        {
            SoundManager.Instance.PlaySound(titleSound);
        }

        private void Quit()
        {
            SoundManager.Instance.PlaySound(btnClickSound);
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
            SoundManager.Instance.PlaySound(btnClickSound);
            SceneManager.LoadScene(nextScreenName);
        }
    }
}
