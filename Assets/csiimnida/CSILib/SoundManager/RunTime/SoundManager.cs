using System.Collections;
using CSILib.SoundManager.RunTime;
using UnityEngine;
using UnityEngine.Audio;

namespace csiimnida.CSILib.SoundManager.RunTime
{
    public class SoundManager : MonoSingleton<SoundManager>
    {
        [Header("Assets")]
        [SerializeField] private SoundListSo _soundListSo;
        [SerializeField] private AudioMixer _mixer;

        [Header("AudioMixer Exposed Parameters")]
        [Tooltip("AudioMixer의 Exposed Parameter 이름 (Master)")]
        [SerializeField] private string _masterVolumeParam = "MasterVolume";
        [Tooltip("AudioMixer의 Exposed Parameter 이름 (BGM)")]
        [SerializeField] private string _bgmVolumeParam = "BGMVolume";
        [Tooltip("AudioMixer의 Exposed Parameter 이름 (SFX)")]
        [SerializeField] private string _sfxVolumeParam = "SFXVolume";

        private void Awake()
        {
            if (_soundListSo == null)
            {
                Debug.Assert(_soundListSo != null,$"SoundListSo asset is null");
            }
            if (_mixer == null)
            {
                Debug.LogError("AudioMixer가 할당되지 않았습니다. SoundManager를 사용하기 전에 할당해주세요.");
            }
            DontDestroyOnLoad(gameObject);
        }

        public void PlaySound(string soundName)
        {
            GameObject obj = new GameObject();
            obj.name = soundName + " Sound";
            AudioSource source = obj.AddComponent<AudioSource>();
            SoundSo so = _soundListSo.SoundsDictionary[soundName];
            if (_mixer == null)
            {
                Debug.LogWarning("Mixer가 할당되지 않았습니다. SoundManager를 사용하기 전에 할당해주세요.");
                SetAudio(source,so);
                return;
            }
            if(so.soundType == SoundType.SFX)
                source.outputAudioMixerGroup = _mixer.FindMatchingGroups("SFX")[0];
            else if(so.soundType == SoundType.BGM)
            {
                source.outputAudioMixerGroup = _mixer.FindMatchingGroups("BGM")[0];
            }
            else
            {
                Debug.LogWarning("Type이 없습니다");
                source.outputAudioMixerGroup = _mixer.FindMatchingGroups("Master")[0];

            }
            SetAudio(source,so);
        
        }

        private void SetAudio(AudioSource source,SoundSo sounds)
        {
            source.clip = sounds.clip;
            source.loop = sounds.loop;
            source.priority = sounds.Priority;
            source.volume = sounds.volume;
            source.pitch = sounds.pitch;
            source.panStereo = sounds.stereoPan;
            source.spatialBlend = sounds.SpatialBlend;
            if (sounds.RandomPitch)
            {
                sounds.pitch = Random.Range(sounds.MinPitch, sounds.MaxPitch);
            }
            if (sounds.pitch < 0)
            {
                source.time = 1;
            }
            source.Play();
            if (!sounds.loop) { StartCoroutine(DestroyCo(source.clip.length,source.gameObject)); }

        }

        IEnumerator DestroyCo(float endTime,GameObject obj)
        {
            yield return new WaitForSecondsRealtime(endTime);
            Destroy(obj);
        }

        // ========================
        // Volume Controls (0~1)
        // ========================
        public void SetMasterVolume(float normalized)
        {
            SetVolumeInternal(_masterVolumeParam, normalized);
        }

        public void SetBGMVolume(float normalized)
        {
            SetVolumeInternal(_bgmVolumeParam, normalized);
        }

        public void SetSFXVolume(float normalized)
        {
            SetVolumeInternal(_sfxVolumeParam, normalized);
        }

        public void SetVolume(float normalized, SoundType type)
        {
            switch (type)
            {
                case SoundType.BGM:
                    SetBGMVolume(normalized);
                    break;
                case SoundType.SFX:
                    SetSFXVolume(normalized);
                    break;
                default:
                    SetMasterVolume(normalized);
                    break;
            }
        }

        private void SetVolumeInternal(string paramName, float normalized)
        {
            if (_mixer == null)
            {
                Debug.LogWarning("AudioMixer가 없어 볼륨을 설정할 수 없습니다.");
                return;
            }
            if (string.IsNullOrEmpty(paramName))
            {
                Debug.LogWarning("AudioMixer Exposed Parameter 이름이 비어 있습니다.");
                return;
            }
            normalized = Mathf.Clamp01(normalized);
            float dB = Mathf.Approximately(normalized, 0f) ? -80f : Mathf.Log10(normalized) * 20f;
            bool ok = _mixer.SetFloat(paramName, dB);
            if (!ok)
            {
                Debug.LogWarning($"AudioMixer에 '{paramName}' Exposed Parameter가 없습니다. AudioMixer에서 노출하고 같은 이름으로 설정해주세요.");
            }
        }
    }

    public enum SoundType
    {
        BGM,
        SFX
    }
}