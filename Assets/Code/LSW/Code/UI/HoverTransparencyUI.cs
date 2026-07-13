using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Code.LSW.Code.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class HoverTransparencyUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {    
        [SerializeField] private float normalAlpha = 1f;
        [SerializeField] private float hoverAlpha = 0.6f;
        [SerializeField] private float fadeDuration = 0.1f;

        private CanvasGroup _canvasGroup;
        private Coroutine _fadeRoutine;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup != null) normalAlpha = _canvasGroup.alpha;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            SetAlpha(hoverAlpha);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            SetAlpha(normalAlpha);
        }

        private void SetAlpha(float target)
        {
            if (_canvasGroup == null) return;

            if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
            _fadeRoutine = StartCoroutine(FadeRoutine(target));
        }

        private IEnumerator FadeRoutine(float target)
        {
            float start = _canvasGroup.alpha;
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                _canvasGroup.alpha = Mathf.Lerp(start, target, Mathf.Clamp01(elapsed / fadeDuration));
                yield return null;
            }

            _canvasGroup.alpha = target;
            _fadeRoutine = null;
        }
    }
}