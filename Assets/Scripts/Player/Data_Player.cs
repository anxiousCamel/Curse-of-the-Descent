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

    // === Sistema de input configurado via Player Input Actions ===
    [HideInInspector] public PlayerControls controls;

    /// <summary>
    /// Inicializa componentes e input assim que o objeto é criado.
    /// </summary>
    void Awake()
    {
        SetupInput();       // Registra eventos de input (movimento)
        CacheComponents();  // Guarda referências a componentes Unity
    }

    /// <summary>
    /// Armazena as referências dos componentes do jogador (uma vez só).
    /// </summary>
    void CacheComponents()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    /// <summary>
    /// Configura o sistema de input do jogador usando Input System.
    /// Captura input de movimento via evento (performed / canceled).
    /// </summary>
    void SetupInput()
    {
        controls = new PlayerControls();

        // Quando houver movimento, guarda o valor do direcional (teclado ou analógico)

        // Movimento
        controls.Player.Move.performed += ctx => movement.movementInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => movement.movementInput = Vector2.zero;

        // Pulo
        controls.Player.Jump.performed += ctx => state.isJumping = true;
        // Detecta se o botão de pulo está sendo mantido pressionado
        controls.Player.Jump.started += ctx => state.isJumpingHeld = true;
        controls.Player.Jump.canceled += ctx => state.isJumpingHeld = false;


        controls.Enable(); // Habilita o input
    }

    // === Agrupamento de atributos ===

    /// <summary>
    /// Dados de movimentação do jogador (velocidade, força de pulo, etc.).
    /// </summary>
    [System.Serializable]
    public class MovementStats
    {
        [HideInInspector] public Vector2 movementInput; // Direção
        [HideInInspector] public Vector3 velocity;       // Velocidade atual (usado para pulo e gravidade)

        public float moveSpeed = 5f;
        public float jumpForce = 10f;
        public float gravity = -9.81f;
        public float coyoteTimeCounter;
        public float jumpBufferCounter;
    }

    /// <summary>
    /// Configurações físicas avançadas para o movimento do jogador.
    /// Controla gravidade, coyote time, jump buffer e limitações de queda.
    /// </summary>
    [System.Serializable]
    public class AdvancedPhysicsSettings
    {
        public float gravityStrength = -9.81f;                // Gravidade base
        public float fallGravityMultiplier = 2.5f;            // Gravidade ao cair
        public float jumpHangGravityMultiplier = 0.5f;        // Gravidade ao manter pulo no ar
        public float jumpHangTimeThreshold = 0.1f;            // Tolerância para aplicar jump hang
        public float maxFallSpeed = 20f;                      // Velocidade máxima de queda
        public float coyoteTime = 0.2f;                       // Tempo para pular após sair do chão
        public float jumpBufferTime = 0.1f;                   // Tempo para pular antes de tocar o chão
    }

    /// <summary>
    /// Estados em tempo real do jogador (movimento e colisões).
    /// Atualizado por scripts como PlayerMovement e PlayerCollision.
    /// </summary>
    [System.Serializable]
    public class PlayerState
    {
        public bool isGrounded = true;       // Está no chão
        public bool isJumping = false;       // Solicitou pulo
        public bool isJumpingHeld = false; // Indica se o botão de pulo está pressionado
        public bool isRunning = false;       // Segurando correr
        public bool isCrouching = false;     // Agachado

        public bool isTouchingWall = false;  // Encostando na parede
        public bool isTouchingCeiling = false; // Encostando no teto
    }

    /// <summary>
    /// Configurações de detecção de colisão do jogador.
    /// </summary>
    [System.Serializable]
    public class CollisionSettings
    {
        public LayerMask groundLayer;
        public float groundCheckDistance = 0.2f;
        public float wallCheckDistance = 0.5f;
        public float ceilingCheckDistance = 0.2f;
    }


    /// <summary>
    /// Dados de vida e energia do jogador.
    /// </summary>
    [System.Serializable]
    public class HealthStats
    {
        public int maxHealth = 100;      // Vida máxima
        public float stamina = 100f;     // Energia do jogador
    }

    /// <summary>
    /// Lista de itens armazenados pelo jogador.
    /// </summary>
    [System.Serializable]
    public class Inventory
    {
        public List<ItemData> items = new List<ItemData>(); // Lista dinâmica de itens
    }

    // Instâncias dos grupos de dados (visíveis no Inspector)
    public MovementStats movement = new MovementStats();
    public HealthStats health = new HealthStats();
    public Inventory inventory = new Inventory();
    public PlayerState state = new PlayerState();
    public CollisionSettings collision = new CollisionSettings();
    public AdvancedPhysicsSettings physics = new AdvancedPhysicsSettings();
}
