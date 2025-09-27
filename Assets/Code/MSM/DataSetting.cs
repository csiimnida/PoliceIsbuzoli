using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Code.MSM
{
    public class DataSetting : MonoBehaviour
    {
        [SerializeField] private List<StatValuePair> settings = new List<StatValuePair>();

        [SerializeField] private string _targetType;

        [Serializable]
        private struct StatValuePair
        {
            public string nationId;
            [SerializedDictionary("DataName","Value")] public SerializedDictionary<string, string> statValues;
        }

        public void TrySetting()
        {
            Type targetType = Type.GetType(_targetType);
            if (targetType == null)
            {
                Debug.LogWarning($"type not found: {_targetType}");
                return;
            }

            ApplySettingsByCreatingAndSet(targetType);
        }

        private void ApplySettingsByCreatingAndSet(Type targetType)
        {
            MethodInfo setMethodGeneric = typeof(DataContructor).GetMethod(nameof(DataContructor.Instance.SetData));
            if (setMethodGeneric == null)
            {
                Debug.LogError("SetData method not found on DataContructor.");
                return;
            }

            int idx = 0;
            foreach (var setting in settings)
            {
                idx++;
                object instance;
                try
                {
                    instance = Activator.CreateInstance(targetType);
                    if (instance == null)
                    {
                        Debug.LogWarning($"Failed to create instance of {targetType.Name} (index {idx}).");
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Exception creating instance of {targetType.Name}: {ex}");
                    continue;
                }

                string nameForNational = setting.nationId;
                if (string.IsNullOrEmpty(nameForNational) && setting.statValues?.Count > 0)
                {
                    nameForNational = setting.statValues.Keys.First();
                }

                var nationalNameField = targetType.GetField("NationalName", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (nationalNameField != null && nameForNational != null)
                {
                    nationalNameField.SetValue(instance, nameForNational);
                }
                
                if (setting.statValues != null)
                {
                    foreach (var kv in setting.statValues)
                    {
                        string fieldName = kv.Key;
                        string stringValue = kv.Value;

                        FieldInfo field = targetType.GetField(fieldName,
                            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                        if (field == null)
                        {
                            Debug.LogWarning($"Field '{fieldName}' not found in {targetType.Name} (index {idx}).");
                            continue;
                        }

                        object converted;
                        try
                        {
                            converted = ConvertStringToType(stringValue, field.FieldType);
                        }
                        catch (Exception e)
                        {
                            Debug.LogWarning($"Conversion failed for field '{fieldName}' to {field.FieldType.Name}: {e.Message}");
                            continue;
                        }

                        try
                        {
                            field.SetValue(instance, converted);
                        }
                        catch (Exception e)
                        {
                            Debug.LogWarning($"Failed to set field '{fieldName}' on {targetType.Name}: {e.Message}");
                        }
                    }
                }

                try
                {
                    MethodInfo generic = setMethodGeneric.MakeGenericMethod(targetType);
                    generic.Invoke(DataContructor.Instance, new object[] { instance });
                }
                catch (TargetInvocationException tie)
                {
                    Debug.LogWarning($"SetData threw an exception for {targetType.Name}: {tie.InnerException?.Message ?? tie.Message}");
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Failed invoking SetData for {targetType.Name}: {e.Message}");
                }
            }
        }

        private object ConvertStringToType(string s, Type targetType)
        {
            if (targetType == typeof(string)) return s;

            if (targetType.IsEnum)
            {
                return Enum.Parse(targetType, s);
            }

            if (targetType == typeof(int))
            {
                return int.Parse(s, NumberStyles.Integer, CultureInfo.InvariantCulture);
            }

            if (targetType == typeof(float))
            {
                return float.Parse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture);
            }

            try
            {
                return Convert.ChangeType(s, targetType, CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                throw new InvalidCastException($"Cannot convert '{s}' to {targetType.Name}: {e.Message}");
            }
        }
    }
}
