using Code.MSM;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tester : MonoBehaviour
{
    [ContextMenu("saveTest")]
    public void Test() //TestComplete, removeThis
    {
        DataContructor.Instance.AddData<NationalData>(new NationalData()
        {
            NationalName = "°æ±âµµ",
            Infectivity = 1.2f,
            SpreadTime = 1f,
            PopulationDensity = 1f,
            Stealth = 1f,
            GetPoint = 4
        });
    }
}
