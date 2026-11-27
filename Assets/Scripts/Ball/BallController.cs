using UnityEngine;

[RequireComponent(typeof(BallBody))]
public class BallController : MonoBehaviour
{
    private BallBody ballBody;
    
    private void Awake()
    {
        ballBody = GetComponent<BallBody>();
    }

    private void OnEnable()
    {
        ballBody.OnBodyCollision.AddListener(OnBallBodyCollision);
    }
    
    private void OnBallBodyCollision(CollisionBody collisionBody, RaycastHit hit)
    {
        switch (collisionBody.bodyType)
        {
            case BodyType.Wall:
                ballBody.Bounce(hit);
                break;
            case BodyType.Player:
                ballBody.IgnoreBodyThisFrame(collisionBody);
                break;
        }
    }
    
    public void HitBall(Vector3 hitDirection, int deltaSpeedLevel = 0)
    {
        ballBody.ChangeDirection(hitDirection);
        ballBody.ChangeSpeedLevel(deltaSpeedLevel);
        ballBody.Impact();
    }
    
}
