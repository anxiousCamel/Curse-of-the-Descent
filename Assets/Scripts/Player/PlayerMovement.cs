using UnityEngine;

/// <summary>
/// Responsável por movimentar o jogador no espaço 3D,
/// com mecânicas avançadas de física como Coyote Time, Jump Buffer e Cut Jump.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private Data_Player data;

    void Start()
    {
        data = GetComponent<Data_Player>();
    }

    void Update()
    {
        ApplyHorizontalMovement();
        HandleJumpBuffer();
        HandleCoyoteTime();
        HandleJump();
        ApplyGravity();

        // Move personagem no total (horizontal + vertical)
        data.characterController.Move(data.movement.velocity * Time.deltaTime);
    }

    void ApplyHorizontalMovement()
    {
        Vector2 input = data.movement.movementInput;

        // Movimento baseado na direção da câmera
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 move = forward * input.y + right * input.x;
        move *= data.movement.moveSpeed;

        data.movement.velocity.x = move.x;
        data.movement.velocity.z = move.z;
    }


    void HandleCoyoteTime()
    {
        if (data.state.isGrounded)
            data.movement.coyoteTimeCounter = data.physics.coyoteTime;
        else
            data.movement.coyoteTimeCounter -= Time.deltaTime;
    }

    void HandleJumpBuffer()
    {
        if (data.state.isJumping)
        {
            data.movement.jumpBufferCounter = data.physics.jumpBufferTime;
            data.state.isJumping = false; // Consome o input
        }
        else
        {
            data.movement.jumpBufferCounter -= Time.deltaTime;
        }
    }

    void HandleJump()
    {
        if (data.movement.jumpBufferCounter > 0 && data.movement.coyoteTimeCounter > 0)
        {
            data.movement.velocity.y = Mathf.Sqrt(data.movement.jumpForce * -2f * data.movement.gravity);
            data.movement.jumpBufferCounter = 0f;
            data.movement.coyoteTimeCounter = 0f;
        }

        // Cut jump: se soltou botão e está subindo
        if (!data.state.isJumpingHeld && data.movement.velocity.y > 0)
        {
            data.movement.velocity.y += data.movement.gravity * data.physics.jumpHangGravityMultiplier * Time.deltaTime;
        }
    }

    void ApplyGravity()
    {
        float gravityMultiplier = 1f;

        if (data.movement.velocity.y < 0)
        {
            gravityMultiplier = data.physics.fallGravityMultiplier;
        }
        else if (data.state.isJumpingHeld && data.movement.velocity.y > 0)
        {
            gravityMultiplier = data.physics.jumpHangGravityMultiplier;
        }

        data.movement.velocity.y += data.movement.gravity * gravityMultiplier * Time.deltaTime;

        // Limita velocidade de queda
        if (data.movement.velocity.y < -data.physics.maxFallSpeed)
        {
            data.movement.velocity.y = -data.physics.maxFallSpeed;
        }
    }

}
