using UnityEngine;

[CreateAssetMenu(fileName = "RandomWalkParameters", menuName = "Scriptable objects/Random walk parameters")]
public class RandomWalkParametersSO : ScriptableObject
{
    public int iterations = 10;
    public int walkLength = 10;
    public bool startRandomlyEachIteration = true;
}
