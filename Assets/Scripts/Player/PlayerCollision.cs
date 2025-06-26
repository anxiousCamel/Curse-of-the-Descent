using UnityEngine;

[RequireComponent(typeof(Data_Player))]
public class PlayerCollision : MonoBehaviour
{
    private Data_Player data;

    void Awake()
    {
        data = GetComponent<Data_Player>();
    }

    void Update()
    {
        UpdateGroundCheck();
        UpdateWallCheck();
        UpdateCeilingCheck();
    }

    void UpdateGroundCheck()
    {
        Vector3 origin = transform.position;
        bool grounded = Physics.Raycast(origin, Vector3.down, data.collision.groundCheckDistance, data.collision.groundLayer);
        data.state.isGrounded = grounded;
    }

    void UpdateWallCheck()
    {
        Vector3 origin = transform.position;
        bool wall = Physics.Raycast(origin, transform.forward, data.collision.wallCheckDistance, data.collision.groundLayer);
        data.state.isTouchingWall = wall;
    }

    void UpdateCeilingCheck()
    {
        Vector3 origin = transform.position;
        bool ceiling = Physics.Raycast(origin, Vector3.up, data.collision.ceilingCheckDistance, data.collision.groundLayer);
        data.state.isTouchingCeiling = ceiling;
    }
}
