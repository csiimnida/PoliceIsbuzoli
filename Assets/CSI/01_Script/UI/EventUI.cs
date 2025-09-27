using System;
using CSI._01_Script.System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace CSI._01_Script.UI
{
    public class EventUI : MonoSingleton<EventUI>
    {
        [Header("UI Refs")] 
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Image image;
        [SerializeField] private RectTransform obj; // root to animate
        [SerializeField] private Button okBt;
        [SerializeField] private CanvasGroup canvasGroup; // for fade/interaction

        [Header("Tween Settings")] 
        [SerializeField] private float durationIn = 0.25f;
        [SerializeField] private float durationOut = 0.2f;
        [SerializeField] private Ease easeIn = Ease.OutBack;
        [SerializeField] private Ease easeOut = Ease.InBack;
        [SerializeField] private float startScale = 0.9f;

        private Tween _inTween;
        private Tween _outTween;

         private void Awake()
        {
            EnsureCanvasGroup();
            HideImmediate();
        }

        private void OnDestroy()
        {
            KillTweens();
        }

        private void EnsureCanvasGroup()
        {
            if (obj == null)
            {
                Debug.LogWarning("EventUI: RectTransform 'obj' is not assigned.");
                obj = GetComponent<RectTransform>();
            }

            if (canvasGroup == null)
            {
                canvasGroup = obj != null ? obj.GetComponent<CanvasGroup>() : null;
                if (canvasGroup == null && obj != null)
                {
                    canvasGroup = obj.gameObject.AddComponent<CanvasGroup>();
                }
            }
        }

        private void KillTweens()
        {
            _inTween?.Kill();
            _outTween?.Kill();
            _inTween = null;
            _outTween = null;
        }

        private void HideImmediate()
        {
            if (obj == null) return;
            KillTweens();
            obj.localScale = Vector3.one * startScale;
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
            obj.gameObject.SetActive(false);
        }

        private void ShowAnimated()
        {
            if (obj == null) return;
            KillTweens();

            obj.gameObject.SetActive(true);
            obj.localScale = Vector3.one * startScale;
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = true; // allow raycasts during anim so button feels responsive after
            }

            // Parallel fade + scale
            var scaleTween = obj.DOScale(1f, durationIn).SetEase(easeIn);
            var alphaTween = canvasGroup != null ? canvasGroup.DOFade(1f, durationIn) : null;

            _inTween = DOTween.Sequence()
                .Join(scaleTween)
                .Join(alphaTween)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    if (canvasGroup != null)
                    {
                        canvasGroup.interactable = true;
                        canvasGroup.blocksRaycasts = true;
                    }
                });
        }

        private void HideAnimated(Action onComplete)
        {
            if (obj == null)
            {
                onComplete?.Invoke();
                return;
            }
            KillTweens();
            Time.timeScale = 1;

            if (canvasGroup != null)
            {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }

            var scaleTween = obj.DOScale(startScale, durationOut).SetEase(easeOut);
            var alphaTween = canvasGroup != null ? canvasGroup.DOFade(0f, durationOut) : null;

            _outTween = DOTween.Sequence()
                .Join(scaleTween)
                .Join(alphaTween)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    obj.gameObject.SetActive(false);
                    onComplete?.Invoke();
                });
        }

        public void ShowEvent(string titleText, string textText, Sprite image, Action ok)
        {
            title?.SetText(titleText);
            text?.SetText(textText);
            if (this.image != null) this.image.sprite = image;
            Time.timeScale = 0;
            // Reset listeners to avoid stacking
            okBt.onClick.RemoveAllListeners();
            okBt.onClick.AddListener(() =>
            {
                okBt.interactable = false; // prevent double click
                HideAnimated(() =>
                {
                    try
                    {
                        ok?.Invoke();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                    finally
                    {
                        okBt.onClick.RemoveAllListeners();
                        okBt.interactable = true;
                    }
                });
                Debug.Log("Click");
            });

            ShowAnimated();
        }
        /*private void Update(){
            if (Keyboard.current.yKey.wasPressedThisFrame)
            {
                ShowEvent("1Ti","123",null, () => { });
                
            }

            if (Keyboard.current.hKey.wasPressedThisFrame)
            {
                HideAnimated(() => { });
            }
        }*/
    }
}