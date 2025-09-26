using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System;
using System.Text;
using CSI._01_Script.System;
public class DataContructor : MonoSingleton<DataContructor>
{
    private const string fileName = "Stat.txt";
    public Dictionary<Type, Dictionary<string, object>> dataTypeDict = new();
    StreamReader reader;
    StreamWriter writer;

    private void Awake()
    {
        SceneManager.sceneLoaded += HandleLoad;
        SceneManager.sceneUnloaded += HandleSave;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= HandleLoad;
        SceneManager.sceneUnloaded -= HandleSave;
    }

    [ContextMenu("saveTest")]
    public void HandleSave(Scene scene)
    {
        string path = Application.dataPath + "/Resources/" + fileName;
        using (writer = new StreamWriter(path, false, Encoding.UTF8))
        {
            foreach(var kvp in dataTypeDict)
            {
                foreach(var data in kvp.Value)
                {
                    object obj = data.Value; //obj �� ����ü
                    Type t = obj.GetType(); //������Ʈ�� Ÿ��
                    List<string> fields = new List<string>(); //�ʵ带 ��Ʈ������ ���� ����

                    fields.Add(t.Name); //�ʵ��� ù��° : Ÿ��

                    foreach (var field in t.GetFields()) //�ʵ� ���ο� ���ؼ�
                    {
                        fields.Add(field.GetValue(obj)?.ToString() ?? ""); //���� �ʵ忡�� �����ͼ� ��Ʈ������ ����� �ֱ�
                    }

                    string line = string.Join("\0", fields); //�ʵ带 �ϳ��� ���ڿ��� ��ħ
                    writer.WriteLine(line);//����
                }
            }
        }
    }

    // �ε�
    private void HandleLoad(Scene scene, LoadSceneMode sceneMode)
    {
        string path = Application.dataPath + "/" + fileName;
        dataTypeDict.Clear();
        if (!File.Exists(path)) return;

        using (reader = new StreamReader(path))
        {
            string line;
            while((line = reader.ReadLine())!=null)
            {
                string[] tokens = line.Split(new char[] { '\0' }, StringSplitOptions.None);

                if (tokens.Length < 2) 
                    continue;

                string typeName = tokens[0];
                string keyName = tokens[1];
                string[] values = new string[tokens.Length - 1];

                Array.Copy(tokens, 1, values, 0, values.Length);

                Type t = Type.GetType(typeName);
                if (t == null)
                {
                    Debug.LogWarning($"{typeName} is not a valid type.");
                    continue;
                }

                object obj = Activator.CreateInstance(t);

                var fields = t.GetFields();
                for (int i = 0; i < fields.Length && i < values.Length; i++)
                {
                    object converted = Convert.ChangeType(values[i], fields[i].FieldType);
                    fields[i].SetValue(obj, converted);
                }

                //insert data to dict
                if(dataTypeDict.TryGetValue(t, out var dataPair))
                {
                    if(dataPair.ContainsKey(keyName) == false)
                    {
                        dataPair.Add(keyName, obj);
                    }
                    else
                    {
                        throw new InvalidOperationException(
            $"Duplicate key detected! Type: {t.Name}, Key: {keyName}");
                    }
                }
                else
                {
                    dataTypeDict.Add(t, new Dictionary<string, object>()
                    {
                        {keyName, obj }
                    });
                }
            }
        }
    }

    public void AddData<T>(T data) where T : ISerializabelDatas
    {
        Type t = typeof(T);

        if(!dataTypeDict.ContainsKey(t))
        {
            dataTypeDict.Add(t, new Dictionary<string, object>());
        }    
        dataTypeDict[t].Add(data.Name, data);
    }

    public void SetData<T>(T data) where T : ISerializabelDatas
    {
        Type t = typeof(T);

        if (!dataTypeDict.ContainsKey(t))
        {
            AddData(data);
            return;
        }
        dataTypeDict[t][data.Name] = data;
    }

    public T GetData<T>(string key) where T : ISerializabelDatas
    {
        Type t = typeof(T);
        if(dataTypeDict.TryGetValue(t, out var dict))
        {
            if(dict.TryGetValue(key, out var data))
            {
                return (T)data;
            }
            throw new KeyNotFoundException($"{key} is not a object Dictionary key");
        }
        throw new KeyNotFoundException($"{t.Name} is not a TypeDictionary key");
    }
}
