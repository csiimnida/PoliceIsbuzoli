public class NationalData : ISerializabelDatas
{
    string ISerializabelDatas.Name
    {
        get => NationalName;
    }

    public string NationalName;
    public float Infectivity;
    public float SpreadTime;
    public float PopulationDensity;
    public float Stealth;
    public float GetPoint;
    public int TotalPeople;

    public int FirstTotalPeople;
}