using Code.MSM;
using System;
using System.Collections.Generic;
using System.Reflection;
using Code.LSW.Code.UI;
using csiimnida.CSILib.SoundManager.RunTime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private UpgradeNode[] RequestNode;
    [SerializeField] private UpgradeNode[] NextNode;
    [SerializeField] private Image icon;
    
    [SerializeField] private float coast;

    [SerializeField] private List<StatValuePair> settings = new List<StatValuePair>();
    [SerializeField] private string targetType;
    [SerializeField] private string _key;

    private Button btn;

    private bool _activated = false;
    public bool Activated { get { return _activated; } }
    public string completeUnlockSound = "CompleteUnlock";

    private void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(TryActiveNode);
    }

    private void OnDestroy()
    {
        btn.onClick.RemoveAllListeners();
    }

    [Serializable]
    private struct StatValuePair
    {
        public string fieldName;   // field name in NationalData
        public string value; // store as string in inspector
        public ModifyType type;
    }

    public void TryActiveNode()
    {
        if(_activated)
            return;
        
        bool requestNodeActive = true;
        SoundManager.Instance.PlaySound(completeUnlockSound);
        foreach (UpgradeNode node in RequestNode)
        {
            if (node.Activated == false)
            {
                requestNodeActive = false;
                return;
            }
        }
        if(CurrencyGetter.Instance.UseMoney(coast) == false)
        {
            requestNodeActive = false;
        }
Debug.Log(requestNodeActive);
        if (requestNodeActive)
        {
            ApplySettings();
            Color color = icon.color;
            color.a = 0.5f;
            icon.color = color;
            _activated = true;
        }
    }
    private ISerializabelDatas GetTargetData(string typeName)
    {
        Type targetType = Type.GetType(typeName);
        if (targetType == null)
        {
            Debug.LogWarning($"type not found: {typeName}");
            return null;
        }
        var method = typeof(DataConstructor).GetMethod("GetData");
        var genericMethod = method.MakeGenericMethod(targetType);
        var result = genericMethod.Invoke(DataConstructor.Instance, new object[] { _key });

        return result as ISerializabelDatas;
    }

    private void ApplySettings()
    {

        foreach (var setting in settings)
        {
            float val;
            val = (float)Convert.ChangeType(setting.value, typeof(float));
                
            NationalManager.Instance.Upgrade(setting.type, val, setting.fieldName);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
        => UIManager.Instance.ShowNodeUI(eventData.position, _key, coast);

    public void OnPointerExit(PointerEventData eventData)
        => UIManager.Instance.HideNodeUI();
}

public enum ModifyType
{
    ADD,
    MULTIFLY,
    SET
}
