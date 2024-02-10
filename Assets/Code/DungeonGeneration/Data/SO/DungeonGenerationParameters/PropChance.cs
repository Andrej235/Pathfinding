using Assets.Code.Utility;
using System;
using UnityEngine;

public partial class DungeonParametersSO
{
    [Serializable]
    public class PropChance : IChance<PropSO>
    {
        [SerializeField] private float chance;
        [SerializeField] private PropSO value;

        public PropSO Value
        {
            get => value;
            set => this.value = value;
        }

        public float Chance
        {
            get => chance;
            set => chance = value;
        }
    }
}
