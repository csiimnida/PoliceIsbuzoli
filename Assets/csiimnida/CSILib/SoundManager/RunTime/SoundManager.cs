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

        [Header("Exposed Mixer Parameters")]
        [Tooltip("AudioMixer exposed parameter name for Master volume (in dB)")]
        [SerializeField] private string masterVolumeParam = "MasterVolume";
        [Tooltip("AudioMixer exposed parameter name for BGM volume (in dB)")]
        [SerializeField] private string bgmVolumeParam = "BGMVolume";
        [Tooltip("AudioMixer exposed parameter name for SFX volume (in dB)")]
        [SerializeField] private string sfxVolumeParam = "SFXVolume";

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

        // === Public Volume Controls (0-1 Linear) ===
        public void SetMasterVolume01(float value01) => SetVolume01(masterVolumeParam, value01);
        public void SetBGMVolume01(float value01) => SetVolume01(bgmVolumeParam, value01);
        public void SetSFXVolume01(float value01) => SetVolume01(sfxVolumeParam, value01);

        public float GetMasterVolume01() => GetVolume01(masterVolumeParam);
        public float GetBGMVolume01() => GetVolume01(bgmVolumeParam);
        public float GetSFXVolume01() => GetVolume01(sfxVolumeParam);

        private void SetVolume01(string param, float value01)
        {
            if (_mixer == null || string.IsNullOrEmpty(param)) return;
            value01 = Mathf.Clamp01(value01);
            float db = LinearToDb(value01);
            _mixer.SetFloat(param, db);
        }

        private float GetVolume01(string param)
        {
            if (_mixer == null || string.IsNullOrEmpty(param)) return 1f;
            if (_mixer.GetFloat(param, out float db))
            {
                return DbToLinear(db);
            }
            return 1f;
        }

        private static float LinearToDb(float value01)
        {
            // Treat near-zero as mute (-80 dB commonly used)
            if (value01 <= 0.0001f) return -80f;
            return Mathf.Log10(value01) * 20f;
        }

        private static float DbToLinear(float db)
        {
            if (db <= -80f) return 0f;
            return Mathf.Pow(10f, db / 20f);
        }
    }

    public enum SoundType
    {
        BGM,
        SFX
    }
}