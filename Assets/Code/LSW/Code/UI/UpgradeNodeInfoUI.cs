using CSI._01_Script.System;
using TMPro;
using UnityEngine;


public class UpgradeNodeInfoUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Canvas rootCanvas;
    [SerializeField] private RectTransform panel;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Visual")] 
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI costText;
    
    private string _currentDataKey;
    private bool _visible;
    
    private void Awake()
    {
        if (rootCanvas == null) 
            rootCanvas = GetComponentInParent<Canvas>();
        
        if (panel == null) 
            panel = GetComponent<RectTransform>();

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null && panel != null)
            canvasGroup = panel.gameObject.AddComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            canvasGroup.ignoreParentGroups = true;
        }
        
        SetVisible(false, 0);
    }

    private void Update()
    {
        if (_visible)
        {
            SetScreenPosition(Input.mousePosition);
        }
    }

    public void Show(Vector2 screenPosition, string dataKey, float cost)
    {
        if (panel == null)
            return;

        SetVisible(true, cost, dataKey);
        SetScreenPosition(screenPosition);
    }

    public void Hide()
    {
        SetVisible(false, 0);
    }

    private void SetVisible(bool visible, float cost, string statKey = null)
    {
        _visible = visible;
        if (panel != null)
        {
            panel.gameObject.SetActive(visible);
        }

        if (!visible)
        {
            nameText.text = string.Empty;
            description.text = string.Empty;
            costText.text = string.Empty;
            _currentDataKey = string.Empty;
            return;
        }

        if (statKey != null && statKey != _currentDataKey)
        {
            _currentDataKey = statKey;
            try
            {
                var data = DataContructor.Instance.GetData<DataStructs>(statKey);
                nameText.text = data._name;
                description.text = data.Description;
                costText.text = $"Cost : {cost}";
            }
            catch (System.Exception e)
            {
                nameText.text = statKey;
                description.text = "";
                costText.text = "Cost : Error";
                
#if UNITY_EDITOR
                Debug.LogWarning($"[UpgradeNodeInfoUI] Failed to load data for key '{statKey}': {e.Message}");
#endif
            }
        }
    }

    private void SetScreenPosition(Vector2 screenPos)
    {
        if (panel == null)
            return;

        Vector2 pos = screenPos;

        if (rootCanvas != null && (rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ||
                                   rootCanvas.renderMode == RenderMode.ScreenSpaceCamera))
        {
            var screenW = Screen.width;
            var screenH = Screen.height;
            var size = panel.sizeDelta * panel.lossyScale;

            float right = screenW - size.x;
            float top = screenH;
            float left = 0f;
            float bottom = size.y;

            pos.x = Mathf.Clamp(pos.x, left, right);
            pos.y = Mathf.Clamp(pos.y, bottom, top);

            panel.position = pos;
        }
        else
        {
            Camera cam = rootCanvas != null ? rootCanvas.worldCamera : Camera.main;
            if (cam != null)
            {
                Vector3 worldPoint = cam.ScreenToWorldPoint(new Vector3(pos.x, pos.y, cam.nearClipPlane + 1f));
                panel.position = worldPoint;
            }
            else
            {
                panel.position = pos;
            }
        }
    }
}