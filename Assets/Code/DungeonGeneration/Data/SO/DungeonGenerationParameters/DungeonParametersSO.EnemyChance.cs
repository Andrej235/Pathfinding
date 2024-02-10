using Assets.Code.DungeonGeneration.Models;
using Assets.Code.Utility;
using System;
using UnityEngine;

public partial class DungeonParametersSO
{
    [Serializable]
    public class EnemyChance : IChance<GameObject>
    {
        [SerializeField] private float chance;
        [SerializeField] private GameObject value;
        [SerializeField] private Room.RoomType roomType;

        public GameObject Value
        {
            get => value;
            set => this.value = value;
        }

        public float Chance
        {
            get => chance;
            set => chance = value;
        }

        public Room.RoomType RoomType
        {
            get => roomType;
            set => roomType = value;
        }
    }
}
