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
        bool requestNodeActive = true;
        SoundManager.Instance.PlaySound(completeUnlockSound);
        foreach (UpgradeNode node in RequestNode)
        {
            if (node.Activated == false)
            {
                requestNodeActive = false;
                break;
            }
        }

        if(CurrencyGetter.Instance.UseMoney(coast) == false)
        {
            requestNodeActive = false;
        }

        if (requestNodeActive)
        {
            ApplySettings(GetTargetData(targetType));
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

    private void ApplySettings(ISerializabelDatas target)
    {
        Type dataType = target.GetType();

        foreach (var setting in settings)
        {
            FieldInfo field = dataType.GetField(setting.fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            object convertedValue = setting.value;
            if (field == null)
            {
                // Debug.LogWarning($"Field {setting.fieldName} not found in {dataType.Name}.");
                // continue;
            }
            else
            {
                try
                {
                    convertedValue = Convert.ChangeType(setting.value, field.FieldType);
                }
                catch
                {
                    
                }
            }

            if (field != null && field.FieldType == typeof(float))
            {
                float val = (float)convertedValue;
                NationalManager.Instance.Upgrade(setting.type, val, setting.fieldName);
            }
            else if (field != null && field.FieldType == typeof(int))
            {
                int val = (int)convertedValue;
                NationalManager.Instance.Upgrade(setting.type, val);
            }
            else
            {
                // non-numeric: only SET
                float val = 0;
                try
                {
                    val = (float)Convert.ChangeType(setting.value, typeof(float));
                }
                catch (Exception e)
                {
                    Debug.LogError("what is this???");
                    continue;
                }
                NationalManager.Instance.Upgrade(setting.type, val, setting.fieldName);
            }
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
