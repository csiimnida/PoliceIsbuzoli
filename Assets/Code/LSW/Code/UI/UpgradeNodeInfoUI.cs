using CSI._01_Script.System;
using UnityEngine;

namespace Code.LSW.Code.UI
{
    public class UpgradeNodeInfoUI : MonoSingleton<UpgradeNodeInfoUI>
    {
        [Header("References")]
        [SerializeField] private Canvas rootCanvas;
        [SerializeField] private RectTransform panel;

        private bool _visible;

        private void Reset()
        {
            if (rootCanvas == null) 
                rootCanvas = GetComponentInParent<Canvas>();
            
            if (panel == null) 
                panel = GetComponent<RectTransform>();
        }

        private void Awake()
        {
            SetVisible(false);
        }

        public void Show(Vector2 screenPosition)
        {
            if (panel == null)
                return;

            SetVisible(true);
            SetScreenPosition(screenPosition);
        }

        public void Show()
        {
            Show(Input.mousePosition);
        }

        public void Hide()
        {
            SetVisible(false);
        }

        private void SetVisible(bool visible)
        {
            _visible = visible;
            if (panel != null)
            {
                panel.gameObject.SetActive(visible);
            }
        }

        private void SetScreenPosition(Vector2 screenPos)
        {
            if (panel == null)
                return;

            if (rootCanvas != null && (rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay || rootCanvas.renderMode == RenderMode.ScreenSpaceCamera))
            {
                panel.position = screenPos + new Vector2(12f, -12f);
            }
            else
            {
                Camera cam = rootCanvas != null ? rootCanvas.worldCamera : Camera.main;
                if (cam != null)
                {
                    Vector3 worldPoint = cam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, cam.nearClipPlane + 1f));
                    panel.position = worldPoint;
                }
                else
                {
                    panel.position = screenPos;
                }
            }
        }
    }
}