using Assets.Code.DungeonGeneration.Models;
using Assets.Code.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

public partial class DungeonParametersSO
{
    [Serializable]
    public class RoomParameters : IChance<RoomParameters>
    {
        [SerializeField] private float chance;
        [SerializeField] private Room.RoomType type;

        [SerializeField] private int minimumNumberOfEnemies;
        [SerializeField] private int maximumNumberOfEnemies;

        [SerializeField] private List<PropChance> specificRoomProps = new();
        [SerializeField] private List<EnemyChance> specificRoomEnemies = new();

        public Room.RoomType Type
        {
            get => type;
            set => type = value;
        }

        public float Chance
        {
            get => chance;
            set => chance = value;
        }

        public List<PropChance> SpecificRoomPropsChance
        {
            get => specificRoomProps;
            set => specificRoomProps = value;
        }

        public int MinimumNumberOfEnemies
        {
            get => minimumNumberOfEnemies;
            set => minimumNumberOfEnemies = value;
        }

        public int MaximumNumberOfEnemies
        {
            get => maximumNumberOfEnemies;
            set => maximumNumberOfEnemies = value;
        }

        public List<EnemyChance> SpecificRoomEnemies
        {
            get => specificRoomEnemies;
            set => specificRoomEnemies = value;
        }

        public RoomParameters Value
        {
            get => this;
            set { }
        }

        public bool editor_ArePropsShown;
        public bool editor_AreEnemiesShown;
        public bool editor_IsNotCollapsed;
    }
}
