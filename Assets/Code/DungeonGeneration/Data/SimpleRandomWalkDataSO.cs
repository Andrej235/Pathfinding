using UnityEngine;

[CreateAssetMenu(fileName = "SimpleRandomWalkParameters", menuName = "ScriptableObjects/SimpleRandomWalkParameters")]
public class SimpleRandomWalkDataSO : ScriptableObject
{
    public int iterations = 10;
    public int walkLength = 10;
    public bool startRandomlyEachIteration = true;
}
