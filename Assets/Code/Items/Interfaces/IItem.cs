using UnityEngine;

public interface IItem
{
    int Id { get; }
    string Name { get; protected set; }
    int MaxStack { get; protected set; }
    Texture Icon { get; protected set; }
}
