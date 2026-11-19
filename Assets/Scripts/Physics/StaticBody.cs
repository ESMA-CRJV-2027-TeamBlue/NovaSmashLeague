using UnityEngine;

public class StaticBody : PhysicsBody
{
    /// <summary>
    /// A static body never moves
    /// </summary>
    /// <param name="deltaTime"></param>
    public override void PhysicsTick(float deltaTime)
    {
        return;
    }
}
