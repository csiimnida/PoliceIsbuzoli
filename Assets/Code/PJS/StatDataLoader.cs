using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using System.Text;
public class StatDataLoader : MonoBehaviour
{
    private const string fileName = "Stat";
    public Dictionary<string, StatData> stats = new Dictionary<string, StatData>();
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

    private void HandleSave(Scene scene)
    {
        writer = new StreamWriter(Application.dataPath + "/" + fileName);
        List<string> type = new List<string>();
        foreach(KeyValuePair<string, StatData> kvp in stats)
        {
            List<string> list = new List<string>() 
            { 
                kvp.Key, 
                kvp.Value.Name, 
                kvp.Value.toInt? ((int)kvp.Value.value).ToString() : kvp.Value.value.ToString(), 
                kvp.Value.toInt.ToString() 
            };
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                sb.Append(list[i]);
                sb.Append((i == list.Count - 1) ? ',' :'\0');
            }
            type.Add(sb.ToString());
        }

        foreach(string s  in type)
        {
            writer.WriteLine(s);
        }
        writer.Close();
    }

    private void HandleLoad(Scene scene, LoadSceneMode sceneMode)
    {

        reader = new StreamReader(Application.dataPath + "/" + fileName);
        reader.Close();
    }

    public bool AddStat(string name, StatData data)
    {
        if (stats.ContainsKey(name)) return false;

        stats.Add(name, data);
        return true;
    }

    public void SetAllStat(Dictionary<string, StatData> target)
    {
        stats = target;
    }
}

public struct StatData
{
    public string Name;
    public float value;
    public bool toInt;
}
