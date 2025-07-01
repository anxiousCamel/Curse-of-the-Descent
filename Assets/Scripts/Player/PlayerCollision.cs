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
        data.state.isGrounded = Physics.Raycast(origin, Vector3.down, data.collision.groundCheckDistance, data.collision.groundLayer);
    }

    void UpdateWallCheck()
    {
        Vector3 origin = transform.position;
        data.state.isTouchingWall = Physics.Raycast(origin, transform.forward, data.collision.wallCheckDistance, data.collision.groundLayer);
    }

    void UpdateCeilingCheck()
    {
        Vector3 origin = transform.position;
        data.state.isTouchingCeiling = Physics.Raycast(origin, Vector3.up, data.collision.ceilingCheckDistance, data.collision.groundLayer);
    }

    /// <summary>
    /// Detecta se há objeto "agarrável" no centro da visão do jogador (câmera).
    /// </summary>
    public bool DetectGrabbable(out GameObject obj, out RaycastHit hit)
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(ray, out hit, data.hand.grabRange, data.hand.grabbableLayer))
        {
            if (hit.collider.CompareTag("Grabbable"))
            {
                obj = hit.collider.gameObject;
                return true;
            }
        }

        obj = null;
        return false;
    }
}
