// File: Assets/Scripts/Player/Data_Player.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Armazena dados e referências do jogador.
/// Inclui atributos de movimentação, vida, inventário, input e componentes Unity.
/// </summary>
public class Data_Player : MonoBehaviour
{
    // === Componentes Unity referenciados automaticamente ===
    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public Animator animator;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public CapsuleCollider capsuleCollider;

    [HideInInspector] public PlayerControls controls;

    void Awake()
    {
        SetupInput();
        CacheComponents();
    }

    void CacheComponents()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        cameraSettings.playerBody = transform;
        cameraSettings.cameraHolder = transform.Find("CameraHolder");

        hand.originalRightHandParent = hand.handRightOrigin.parent;
        hand.originalLeftHandParent = hand.handLeftOrigin.parent;
        hand.originalRightHandLocalPosition = hand.handRightOrigin.localPosition;
        hand.originalRightHandLocalRotation = hand.handRightOrigin.localRotation;
        hand.originalLeftHandLocalPosition = hand.handLeftOrigin.localPosition;
        hand.originalLeftHandLocalRotation = hand.handLeftOrigin.localRotation;
    }

    void SetupInput()
    {
        controls = new PlayerControls();
        controls.Player.Move.performed += ctx => movement.movementInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => movement.movementInput = Vector2.zero;

        controls.Player.Jump.performed += ctx => state.isJumping = true;
        controls.Player.Jump.started += ctx => state.isJumpingHeld = true;
        controls.Player.Jump.canceled += ctx => state.isJumpingHeld = false;

        controls.Player.Look.performed += ctx => cameraSettings.lookInput = ctx.ReadValue<Vector2>();
        controls.Player.Look.canceled += ctx => cameraSettings.lookInput = Vector2.zero;

        controls.Player.GrabLeft.started += ctx => state.isTryingGrabLeft = true;
        controls.Player.GrabLeft.canceled += ctx => state.isTryingGrabLeft = false;

        controls.Player.GrabRight.started += ctx => state.isTryingGrabRight = true;
        controls.Player.GrabRight.canceled += ctx => state.isTryingGrabRight = false;

        controls.Enable();
    }

    [System.Serializable]
    public class HandPhysicsSettings
    {
        [Header("Hand Physics Settings")]
        public float spring = 100f;
        public float damper = 5f;
        public float maxDistance = 1.5f;
        public float releaseImpulseForce = 6f;
        public SpringJoint leftJoint;
        public SpringJoint rightJoint;
    }

    [System.Serializable]
    public class HandStats
    {
        [Header("Stamina")]
        public float stamina;
        public float maxStamina = 100f;
        public float costStamina = 0.25f;

        [Header("Visual e Animação")]
        public SpriteRenderer handSprite;
        public Vector3 originalLocalPos;

        [Header("Efeito de Shake")]
        public float maxShakeIntensity = 0.05f;
        public float shakeSpeed = 25f;
    }

    [System.Serializable]
    public class Hand
    {
        [Header("Referência Inicial das Mãos")]
        public Transform originalRightHandParent;
        public Transform originalLeftHandParent;
        public Vector3 originalRightHandLocalPosition;
        public Quaternion originalRightHandLocalRotation;
        public Vector3 originalLeftHandLocalPosition;
        public Quaternion originalLeftHandLocalRotation;

        [Header("Transform da Mão Atual")]
        public Transform handRightOrigin;
        public Transform handLeftOrigin;

        [Header("Configurações de Agarrar")]
        public float grabRange = 1.5f;
        public LayerMask grabbableLayer;
        public Vector3 offset;

        [Header("Objetos Agarrados")]
        public GameObject grabbedRightObject;
        public GameObject grabbedLeftObject;
        public GameObject lastGrabbedRightObject;
        public GameObject lastGrabbedLeftObject;

        [Header("Cooldowns")]
        public float grabCooldownTimerLeft;
        public float grabCooldownTimerRight;

        public float upwardVelocityThreshold = 0.1f;
    }

    [System.Serializable]
    public class HandAnim
    {
        [Header("Animação da Mão")]
        public Animator anim;
        public AnimationState currentState;

        public enum AnimationState
        {
            HandIdle,
            HandTryGrab,
            HandGrab
        }

        public void ChangeAnimationState(AnimationState newState)
        {
            if (currentState == newState) return;
            anim.Play(newState.ToString());
            currentState = newState;
        }
    }

    [System.Serializable]
    public class MovementStats
    {
        [HideInInspector] public Vector2 movementInput;
        [HideInInspector] public Vector3 velocity;
        public float moveSpeed = 5f;
        public float jumpForce = 10f;
        public float gravity = -9.81f;
        public float coyoteTimeCounter;
        public float jumpBufferCounter;
    }

    [System.Serializable]
    public class AdvancedPhysicsSettings
    {
        [Header("Física Avançada")]
        public float gravityStrength = -9.81f;
        public float fallGravityMultiplier = 2.5f;
        public float jumpHangGravityMultiplier = 0.5f;
        public float jumpHangTimeThreshold = 0.1f;
        public float maxFallSpeed = 20f;
        public float coyoteTime = 0.2f;
        public float jumpBufferTime = 0.1f;
    }

    [System.Serializable]
    public class PlayerState
    {
        [Header("Movimento e Estado")]
        public bool isGrounded = true;
        public bool isJumping = false;
        public bool isJumpingHeld = false;
        public bool isRunning = false;
        public bool isCrouching = false;
        public bool isTouchingWall = false;
        public bool isTouchingCeiling = false;
        public bool isTryingGrabLeft = false;
        public bool isTryingGrabRight = false;
        public bool isGrabbingLeft = false;
        public bool isGrabbingRight = false;
    }

    [System.Serializable]
    public class CollisionSettings
    {
        [Header("Colisão")]
        public LayerMask groundLayer;
        public float groundCheckDistance = 0.2f;
        public float wallCheckDistance = 0.5f;
        public float ceilingCheckDistance = 0.2f;
    }

    [System.Serializable]
    public class HealthStats
    {
        [Header("Vida e Energia")]
        public int maxHealth = 100;
        public float stamina = 100f;
    }

    [System.Serializable]
    public class Inventory
    {
        [Header("Inventário")]
        public List<ItemData> items = new List<ItemData>();
    }

    [System.Serializable]
    public class CameraSettings
    {
        [HideInInspector] public Vector2 lookInput;
        [HideInInspector] public float xRotation = 0f;
        [HideInInspector] public Transform playerBody;
        [HideInInspector] public Transform cameraHolder;

        [Header("Câmera")]
        public float mouseSensitivity = 100f;
        public Camera mainCamera;
    }

    public HandPhysicsSettings handPhysics = new HandPhysicsSettings();
    public HandStats handRight = new HandStats();
    public HandStats handLeft = new HandStats();
    public Hand hand = new Hand();
    public HandAnim rightHand = new HandAnim();
    public HandAnim leftHand = new HandAnim();
    public MovementStats movement = new MovementStats();
    public HealthStats health = new HealthStats();
    public Inventory inventory = new Inventory();
    public PlayerState state = new PlayerState();
    public CollisionSettings collision = new CollisionSettings();
    public AdvancedPhysicsSettings physics = new AdvancedPhysicsSettings();
    public CameraSettings cameraSettings = new CameraSettings();
}
