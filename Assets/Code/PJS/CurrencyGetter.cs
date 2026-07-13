using CSI._01_Script.System;
using UnityEngine;

public class IncomeStat : ISerializabelDatas
{
    public string Name { get => "income"; set => _ = value; }

    public float taxIncome;
    public float taxIncomeRate;
}

public class CurrencyGetter : MonoSingleton<CurrencyGetter>
{
    private static readonly string incomekey = "income";
    public float Money{get; private set;}
    public float Income { get; private set;}

    private float lastIncome;
    private void Start()
    {
        if(DataConstructor.Instance.dataTypeDict.ContainsKey(typeof(CurrencyGetter)) == false)
        {
            DataConstructor.Instance.AddData(new IncomeStat()
            {
                taxIncome = 1,
                taxIncomeRate = 1
            });
        }
        lastIncome = Time.time;
    }
    private void Update()
    {
        if (lastIncome + DataConstructor.Instance.GetData<IncomeStat>(incomekey).taxIncomeRate < Time.time)
        {
            Money += DataConstructor.Instance.GetData<IncomeStat>(incomekey).taxIncome;
            lastIncome = Time.time;
        }
    }

    public bool UseMoney(float amount)
    {
        if(Money - amount>0)
        {
            Money -= amount;
            return true;
        }
        return false;
    }
}
