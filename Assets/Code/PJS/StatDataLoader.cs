using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System;
public class StatDataLoader : MonoBehaviour
{
    private const string fileName = "Stat";
    public Dictionary<string, StatData> stats = new Dictionary<string, StatData>();

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
        StreamReader reader = new StreamReader(Application.dataPath + "/" + fileName);
    }

    private void HandleLoad(Scene scene, LoadSceneMode sceneMode)
    {
        
    }
}

public struct StatData
{
    public string key;
    public float value;
    public bool toInt;
}
