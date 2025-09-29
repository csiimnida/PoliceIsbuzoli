using UnityEngine;

public class IncomeStat : ISerializabelDatas
{
    public string Name { get => "income"; set => _ = value; }

    public float taxIncome;
    public float taxIncomeRate;
}
public class CurrencyGetter : MonoBehaviour
{
    public float Money{get; private set;}

    [SerializeField] private float earnRate;
    public float Income { get; private set;}

    private float lastIncome;

    private void Start()
    {
        lastIncome = Time.time;
    }
    private void Update()
    {
        if (lastIncome + earnRate < Time.time)
        {
            Money += Income;
            lastIncome = Time.time;
        }
    }
}
