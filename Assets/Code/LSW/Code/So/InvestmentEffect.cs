using UnityEngine;

namespace Code.LSW.Code.So
{
    [CreateAssetMenu(fileName = "InvestmentEffect", menuName = "SO/InvestmentEffect", order = 0)]
    public class InvestmentEffect : ScriptableObject
    {
        private float _infectivity;
        private float _spreadTime;
        private float _populationDensity;
        private float _stealth;
    }
}