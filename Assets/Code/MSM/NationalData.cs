namespace Code.MSM
{
    public class NationalData : ISerializabelDatas
    {
        string ISerializabelDatas.Name { get => NationalName; set => NationalName = value; }
        
        public string NationalName;
        public float Infectivity;
        public float SpreadTime;
        public float PopulationDensity;
        public float Stealth;
    }
}