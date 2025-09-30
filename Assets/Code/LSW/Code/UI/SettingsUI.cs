using System.Collections;
using csiimnida.CSILib.SoundManager.RunTime;
using UnityEngine;
using UnityEngine.Events;

namespace Code.LSW.Code.UI
{
    public class SettingsUI : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("없을 경우 알아서 찾아옴")]
        [SerializeField] private CanvasGroup settingsUI;

        [Header("Animation")]
        [Tooltip("페이드 지속 시간(초).")]
        [SerializeField, Min(0f)] private float fadeDuration = 0.25f;
        [Tooltip("시간에 따른 알파값 변화 (X: 0~1(시간), Y: 0~1(투명도))")]
        [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Options")]
        [SerializeField] private bool startHidden = true;
        [SerializeField] private bool disableWhenHidden = true;

        [Header("Events")]
        [SerializeField] private UnityEvent onAppeared = new UnityEvent();
        [SerializeField] private UnityEvent onDisappeared = new UnityEvent();
        
        private Coroutine _fadeRoutine;
        private bool _isVisible;

        private void Awake()
        {
            if (!settingsUI)
            {
                settingsUI = GetComponent<CanvasGroup>();
                if (!settingsUI)
                {
                    settingsUI = gameObject.AddComponent<CanvasGroup>();
                }
            }
            
            if (startHidden)
            {
                ApplyInstant(0f);
                SetInteractable(false);
                _isVisible = false;
                if (disableWhenHidden) gameObject.SetActive(false);
            }
            else
            {
                if (disableWhenHidden && !gameObject.activeSelf) gameObject.SetActive(true);
                ApplyInstant(1f);
                SetInteractable(true);
                _isVisible = true;
            }
        }
        
        // 키는 거
        public void Appear()
        {
            SetVisible(true);
        }
        
        // 끄는 거
        public void Disappear()
        {
            SetVisible(false);
        }
        
        // 둘 다 혼합
        public void Toggle()
        {
            SetVisible(!_isVisible);
        }
        
        public void SetVisible(bool visible)
        {
            UIManager.Instance.PlayButtonClick();
            if (_isVisible == visible && _fadeRoutine == null)
                return;

            if (_fadeRoutine != null)
                StopCoroutine(_fadeRoutine);
            
            if (visible && disableWhenHidden && !gameObject.activeSelf)
                gameObject.SetActive(true);

            _fadeRoutine = StartCoroutine(FadeRoutine(visible));
        }

        private IEnumerator FadeRoutine(bool show)
        {
            _isVisible = show;

            if (Mathf.Approximately(fadeDuration, 0f))
            {
                ApplyInstant(show ? 1f : 0f);
                SetInteractable(show);
                PostFade(show);
                _fadeRoutine = null;
                yield break;
            }

            float startAlpha = settingsUI.alpha;
            float endAlpha = show ? 1f : 0f;
            float t = 0f;
            
            if (show)
                SetInteractable(true);

            while (t < fadeDuration)
            {
                t += Time.unscaledDeltaTime;
                float normalized = Mathf.Clamp01(t / fadeDuration);
                float evaluated = fadeCurve?.Evaluate(normalized) ?? normalized;
                settingsUI.alpha = Mathf.LerpUnclamped(startAlpha, endAlpha, evaluated);
                yield return null;
            }

            ApplyInstant(endAlpha);
            
            if (!show)
                SetInteractable(false);

            PostFade(show);
            _fadeRoutine = null;
        }

        private void PostFade(bool shown)
        {
            if (!shown && disableWhenHidden)
                gameObject.SetActive(false);

            if (shown)
                onAppeared?.Invoke();
            else
                onDisappeared?.Invoke();
        }

        private void ApplyInstant(float alpha)
        {
            settingsUI.alpha = Mathf.Clamp01(alpha);
        }

        private void SetInteractable(bool value)
        {
            settingsUI.interactable = value;
            settingsUI.blocksRaycasts = value;
        }
    }
}