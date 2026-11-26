using UnityEngine;

public class CollisionBody : MonoBehaviour
{
    public BodyType bodyType;
}

public enum BodyType
{
    Wall,
    Player,
    Ball
}