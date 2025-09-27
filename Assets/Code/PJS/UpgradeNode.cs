using Code.MSM;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UpgradeNode : MonoBehaviour
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
        foreach (UpgradeNode node in RequestNode)
        {
            if (node.Activated == false)
            {
                requestNodeActive = false;
                break;
            }
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

        var methodInfo = typeof(DataContructor).GetMethod("GetData");
        var genericMethod = methodInfo.MakeGenericMethod(targetType);
        var result = genericMethod.Invoke(DataContructor.Instance, null);

        return result as ISerializabelDatas;
    }

    private void ApplySettings(ISerializabelDatas target)
    {
        Type dataType = target.GetType();

        foreach (var setting in settings)
        {
            FieldInfo field = dataType.GetField(setting.fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null)
            {
                Debug.LogWarning($"Field {setting.fieldName} not found in {dataType.Name}.");
                continue;
            }

            object convertedValue;
            try
            {
                convertedValue = Convert.ChangeType(setting.value, field.FieldType);
            }
            catch
            {
                Debug.LogWarning($"Cannot convert {setting.value} to {field.FieldType}");
                continue;
            }

            object currentValue = field.GetValue(target);

            object newValue = convertedValue;
            if (field.FieldType == typeof(float))
            {
                float cur = (float)currentValue;
                float val = (float)convertedValue;
                switch (setting.type)
                {
                    case ModifyType.ADD: newValue = cur + val; break;
                    case ModifyType.MULTIFLY: newValue = cur * val; break;
                    case ModifyType.SET: newValue = val; break;
                }
            }
            else if (field.FieldType == typeof(int))
            {
                int cur = (int)currentValue;
                int val = (int)convertedValue;
                switch (setting.type)
                {
                    case ModifyType.ADD: newValue = cur + val; break;
                    case ModifyType.MULTIFLY: newValue = cur * val; break;
                    case ModifyType.SET: newValue = val; break;
                }
            }
            else
            {
                // non-numeric: only SET
                newValue = convertedValue;
            }

            field.SetValue(target, newValue);
        }
    }
}

public enum ModifyType
{
    ADD,
    MULTIFLY,
    SET
}
