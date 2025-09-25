using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Code.LSW.Code.UI
{
    public class SliderAnimationUI : MonoBehaviour
    {
        [SerializeField] private RectTransform panel;
        
        [Header("Animation")] 
        [SerializeField] private bool useSlideFromBottom = true;
        [SerializeField, Min(0f)] private float slideDuration = 0.25f;
        [SerializeField] private AnimationCurve slideCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private bool autoHiddenOffset = true;
        [SerializeField] private float extraHiddenOffset = 40f;
        
        [Header("Options")]
        [SerializeField] private bool startHidden = true;
        [SerializeField] private bool disableWhenHidden = true;

        [Header("Events")]
        [SerializeField] private UnityEvent onAppeared = new UnityEvent();
        [SerializeField] private UnityEvent onDisappeared = new UnityEvent();
        
        private Vector2 _shownPos;
        private Vector2 _hiddenPos;
        private bool _isVisible;
        private Coroutine _slideRoutine;

        private void Awake()
        {
            if (!panel)
            {
                panel = transform as RectTransform;
            }
            
            if (panel)
            {
                _shownPos = panel.anchoredPosition;
                float offset = autoHiddenOffset ? (panel.rect.height + extraHiddenOffset) : extraHiddenOffset;
                _hiddenPos = _shownPos + Vector2.down * Mathf.Max(0f, offset);
            }
            
            if (startHidden)
            {
                ApplySlideInstant(false);
                _isVisible = false;
                if (disableWhenHidden) gameObject.SetActive(false);
            }
            else
            {
                if (disableWhenHidden && !gameObject.activeSelf) gameObject.SetActive(true);
                ApplySlideInstant(true);
                _isVisible = true;
            }
        }

        // 표시
        public void Appear() => SetVisible(true);
        // 숨김
        public void Disappear() => SetVisible(false);
        // 혼합
        public void Toggle() => SetVisible(!_isVisible);

        public void SetVisible(bool visible)
        {
            if (_isVisible == visible && _slideRoutine == null)
                return;

            if (_slideRoutine != null)
                StopCoroutine(_slideRoutine);

            if (visible && disableWhenHidden && !gameObject.activeSelf)
                gameObject.SetActive(true);

            _slideRoutine = StartCoroutine(AnimateSlideRoutine(visible));
        }

        private IEnumerator AnimateSlideRoutine(bool show)
        {
            _isVisible = show;

            bool instant = !useSlideFromBottom || panel == null || Mathf.Approximately(slideDuration, 0f);
            if (instant)
            {
                ApplySlideInstant(show);
                PostSlide(show);
                _slideRoutine = null;
                yield break;
            }

            Vector2 startPos = panel.anchoredPosition;
            Vector2 endPos = show ? _shownPos : _hiddenPos;
            float elapsed = 0f;

            while (elapsed < slideDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / slideDuration);
                float e = slideCurve?.Evaluate(t) ?? t;
                panel.anchoredPosition = Vector2.LerpUnclamped(startPos, endPos, e);
                yield return null;
            }

            ApplySlideInstant(show);
            PostSlide(show);
            _slideRoutine = null;
        }

        private void ApplySlideInstant(bool show)
        {
            if (!panel) return;
            if (!useSlideFromBottom)
            {
                panel.anchoredPosition = _shownPos;
                return;
            }
            panel.anchoredPosition = show ? _shownPos : _hiddenPos;
        }

        private void PostSlide(bool shown)
        {
            if (!shown && disableWhenHidden)
                gameObject.SetActive(false);

            if (shown)
                onAppeared?.Invoke();
            else
                onDisappeared?.Invoke();
        }
    }
}