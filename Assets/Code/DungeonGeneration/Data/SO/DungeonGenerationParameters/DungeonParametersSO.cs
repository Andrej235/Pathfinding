using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "DungeonParameters", menuName = "Scriptable objects/Dungeon parameters")]
public partial class DungeonParametersSO : ScriptableObject
{
    public TileBase floorTile;
    public TileBase wallTop;
    public TileBase wallSideRight;
    public TileBase wallSideLeft;
    public TileBase wallBottom;
    public TileBase wallFull;
    public TileBase wallInnerCornerDownLeft;
    public TileBase wallInnerCornerDownRight;
    public TileBase wallDiagonalCornerDownRight;
    public TileBase wallDiagonalCornerDownLeft;
    public TileBase wallDiagonalCornerUpRight;
    public TileBase wallDiagonalCornerUpLeft;

    public double chanceToSpawnAProp;
    public List<PropChance> propsChance;
    public List<EnemyChance> enemiesChance;
    public List<RoomParameters> roomParameters;

    public bool editor_AreTilesShown;
    public bool editor_ArePropsShown;
    public bool editor_AreEnemiesShown;
    public bool editor_AreRoomsShown;
}
