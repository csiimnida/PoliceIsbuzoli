using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Code.LSW.Code.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class HoverTransparencyUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Alpha Settings")]
        [Range(0f, 1f)] [SerializeField] private float normalAlpha = 1f;
        [Range(0f, 1f)] [SerializeField] private float hoverAlpha = 0.6f;

        [Header("Animation")]
        [Min(0f)] [SerializeField] private float fadeDuration = 0.1f;
        [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Options")] 
        [SerializeField] private bool ignoreWhenNotInteractable = true;

        private CanvasGroup _canvasGroup;
        private Coroutine _fadeRoutine;
        private bool _isHovered;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup != null)
            {
                if (Mathf.Approximately(normalAlpha, 1f))
                {
                    normalAlpha = _canvasGroup.alpha;
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (ignoreWhenNotInteractable && _canvasGroup && !_canvasGroup.interactable)
                return;

            _isHovered = true;
            SetAlpha(hoverAlpha);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (ignoreWhenNotInteractable && _canvasGroup && !_canvasGroup.interactable)
                return;

            _isHovered = false;
            SetAlpha(normalAlpha);
        }

        private void SetAlpha(float target)
        {
            if (!_canvasGroup) return;

            if (_fadeRoutine != null)
                StopCoroutine(_fadeRoutine);

            if (Mathf.Approximately(fadeDuration, 0f))
            {
                _canvasGroup.alpha = Mathf.Clamp01(target);
                return;
            }

            _fadeRoutine = StartCoroutine(FadeRoutine(target));
        }

        private IEnumerator FadeRoutine(float target)
        {
            if (!_canvasGroup)
                yield break;

            float start = _canvasGroup.alpha;
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / fadeDuration);
                float e = fadeCurve?.Evaluate(t) ?? t;
                _canvasGroup.alpha = Mathf.LerpUnclamped(start, target, e);
                yield return null;
            }

            _canvasGroup.alpha = Mathf.Clamp01(target);
            _fadeRoutine = null;
        }
    }
}