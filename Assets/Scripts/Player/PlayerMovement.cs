using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    private Data_Player data;

    void Start()
    {
        data = GetComponent<Data_Player>();
        data.rb = GetComponent<Rigidbody>();
        data.rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void FixedUpdate()
    {
        ApplyHorizontalMovement();
        HandleJumpBuffer();
        HandleCoyoteTime();
        HandleJump();
        ApplyGravity();
    }

    void ApplyHorizontalMovement()
    {
        Vector2 input = data.movement.movementInput;

        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 move = (forward * input.y + right * input.x).normalized;
        float speed = data.movement.moveSpeed;

        if (data.state.isGrabbingLeft || data.state.isGrabbingRight)
            speed *= 0.5f; // Reduz a velocidade ao agarrar

        Vector3 desiredVelocity = move * speed;

        Vector3 velocity = data.rb.linearVelocity;
        velocity.x = desiredVelocity.x;
        velocity.z = desiredVelocity.z;

        data.rb.linearVelocity = velocity;
    }

    void HandleCoyoteTime()
    {
        if (data.state.isGrounded)
            data.movement.coyoteTimeCounter = data.physics.coyoteTime;
        else
            data.movement.coyoteTimeCounter -= Time.fixedDeltaTime;
    }

    void HandleJumpBuffer()
    {
        if (data.state.isJumping)
        {
            data.movement.jumpBufferCounter = data.physics.jumpBufferTime;
            data.state.isJumping = false;
        }
        else
        {
            data.movement.jumpBufferCounter -= Time.fixedDeltaTime;
        }
    }

    void HandleJump()
    {
        bool isAbleToJump =
            data.movement.jumpBufferCounter > 0 &&
            (data.movement.coyoteTimeCounter > 0 || data.state.isGrabbingLeft || data.state.isGrabbingRight);

        if (isAbleToJump)
        {
            Vector3 velocity = data.rb.linearVelocity;
            velocity.y = Mathf.Sqrt(data.movement.jumpForce * -2f * data.movement.gravity);
            data.rb.linearVelocity = velocity;

            data.movement.jumpBufferCounter = 0f;
            data.movement.coyoteTimeCounter = 0f;

            // Solta as mãos ao pular
            data.state.isGrabbingLeft = false;
            data.state.isGrabbingRight = false;

            // Impulso para trás ao pular do agarrão
            Vector3 pushDir = Vector3.zero;
            if (data.state.isGrabbingLeft)
                pushDir = -data.hand.handLeftOrigin.forward;
            else if (data.state.isGrabbingRight)
                pushDir = -data.hand.handRightOrigin.forward;

            if (pushDir != Vector3.zero)
                data.rb.AddForce(pushDir.normalized * data.handPhysics.releaseImpulseForce, ForceMode.VelocityChange);
        }

        // Cut jump
        if (!data.state.isJumpingHeld && data.rb.linearVelocity.y > 0)
        {
            Vector3 velocity = data.rb.linearVelocity;
            velocity.y += data.movement.gravity * data.physics.jumpHangGravityMultiplier * Time.fixedDeltaTime;
            data.rb.linearVelocity = velocity;
        }
    }

    void ApplyGravity()
    {
        float gravityMultiplier = 1f;

        if (data.rb.linearVelocity.y < 0)
            gravityMultiplier = data.physics.fallGravityMultiplier;
        else if (data.state.isJumpingHeld && data.rb.linearVelocity.y > 0)
            gravityMultiplier = data.physics.jumpHangGravityMultiplier;

        Vector3 velocity = data.rb.linearVelocity;
        velocity.y += data.movement.gravity * gravityMultiplier * Time.fixedDeltaTime;

        if (velocity.y < -data.physics.maxFallSpeed)
            velocity.y = -data.physics.maxFallSpeed;

        data.rb.linearVelocity = velocity;
    }
}
