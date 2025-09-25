using UnityEngine;
using UnityEngine.UI;
using csiimnida.CSILib.SoundManager.RunTime;

namespace Code.LSW.Code.UI
{
    public class SoundSettingsUI : MonoBehaviour
    {
        [Header("Sliders (0-1)")]
        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Slider sfxSlider;

        [Header("PlayerPrefs Keys")]
        [SerializeField] private string masterKey = "MasterVolume01";
        [SerializeField] private string bgmKey = "BGMVolume01";
        [SerializeField] private string sfxKey = "SFXVolume01";

        private SoundManager Sm => SoundManager.Instance;

        private void Awake()
        {
            InitSlider(masterSlider);
            InitSlider(bgmSlider);
            InitSlider(sfxSlider);
        }

        private void OnEnable()
        {
            float master = PlayerPrefs.GetFloat(masterKey, 1f);
            float bgm = PlayerPrefs.GetFloat(bgmKey, 1f);
            float sfx = PlayerPrefs.GetFloat(sfxKey, 1f);

            ApplyVolumes(master, bgm, sfx, applyToUI: true, savePrefs: false);

            if (masterSlider) 
                masterSlider.onValueChanged.AddListener(OnMasterChanged);
            if (bgmSlider) 
                bgmSlider.onValueChanged.AddListener(OnBgmChanged);
            if (sfxSlider) 
                sfxSlider.onValueChanged.AddListener(OnSfxChanged);
        }

        private void OnDisable()
        {
            if (masterSlider) 
                masterSlider.onValueChanged.RemoveListener(OnMasterChanged);
            if (bgmSlider) 
                bgmSlider.onValueChanged.RemoveListener(OnBgmChanged);
            if (sfxSlider) 
                sfxSlider.onValueChanged.RemoveListener(OnSfxChanged);
        }

        private void InitSlider(Slider slider)
        {
            if (!slider) return;
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.wholeNumbers = false;
        }
        
        public void OnMasterChanged(float value01)
        {
            if (Sm != null) 
                Sm.SetMasterVolume01(value01);
            PlayerPrefs.SetFloat(masterKey, Mathf.Clamp01(value01));
        }
        public void OnBgmChanged(float value01)
        {
            if (Sm != null) 
                Sm.SetBGMVolume01(value01);
            PlayerPrefs.SetFloat(bgmKey, Mathf.Clamp01(value01));
        }
        public void OnSfxChanged(float value01)
        {
            if (Sm != null) 
                Sm.SetSFXVolume01(value01);
            PlayerPrefs.SetFloat(sfxKey, Mathf.Clamp01(value01));
        }

        public void LoadFromMixerToUI()
        {
            if (Sm == null) return;
            if (masterSlider) masterSlider.SetValueWithoutNotify(Sm.GetMasterVolume01());
            if (bgmSlider) bgmSlider.SetValueWithoutNotify(Sm.GetBGMVolume01());
            if (sfxSlider) sfxSlider.SetValueWithoutNotify(Sm.GetSFXVolume01());
        }

        private void ApplyVolumes(float master, float bgm, float sfx, bool applyToUI, bool savePrefs)
        {
            if (Sm != null)
            {
                Sm.SetMasterVolume01(master);
                Sm.SetBGMVolume01(bgm);
                Sm.SetSFXVolume01(sfx);
            }

            if (applyToUI)
            {
                if (masterSlider) 
                    masterSlider.SetValueWithoutNotify(master);
                
                if (bgmSlider) 
                    bgmSlider.SetValueWithoutNotify(bgm);
                
                if (sfxSlider) 
                    sfxSlider.SetValueWithoutNotify(sfx);
            }

            if (savePrefs)
            {
                PlayerPrefs.SetFloat(masterKey, Mathf.Clamp01(master));
                PlayerPrefs.SetFloat(bgmKey, Mathf.Clamp01(bgm));
                PlayerPrefs.SetFloat(sfxKey, Mathf.Clamp01(sfx));
            }
        }
    }
}