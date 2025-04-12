using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Player Data")]
    [SerializeField] private Player player;
    private PlayerStats stats;

    [Header("Camera Settings")]
    [Header("Mouse Settings")]
    [Tooltip("Sensibilidade do Rato")]
    [SerializeField] private float mouseSensitivity = 0.1f;

    // N�o funciona direito
    [Header("Controller Settings")]
    [Tooltip("Controller sensitivity multiplier.")]
    [SerializeField] private float controllerSensitivity = 5f;
    [Tooltip("Deadzone threshold for the controller stick.")]
    [SerializeField, Range(0f, 0.5f)] private float controllerDeadzone = 0.15f;
    [Tooltip("Smoothing factor for controller input (0 = max smoothing, 1 = no smoothing).")]
    [SerializeField, Range(0.01f, 1f)] private float controllerSmoothing = 0.15f;
    private Vector2 smoothedControllerInput = Vector2.zero;

    [Header("Camera Limits")]
    [Tooltip("Minimum vertical angle (in degrees).")]
    [SerializeField] private float minPitch = -60f;
    [Tooltip("Maximum vertical angle (in degrees).")]
    [SerializeField] private float maxPitch = 60f;

    private float yaw = 0f;
    private float pitch = 0f;

    private PlayerInput playerInput;
    private string currentControlScheme;

    private GameManager gameManager;

    private void Awake()
    {
        if(player == null)
        {
            Debug.LogError("PlayerController requires a Player GameObject!");
        }

        // Inicializar o CharacterController
        player.characterController = GetComponent<CharacterController>();

        // Inicializar o PlayerStats
        stats = player.stats;

        // Inicializar o PlayerInput
        playerInput = GetComponent<PlayerInput>();

        smoothedControllerInput = Vector2.zero;

        // Lock e hide do cursor do rato no jogo
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        gameManager = GameManager.instance;
    }

    // Player Input behavior t� set para 'Send Messages' (m�todos usam InputValue e n�o o CallbackContext)
    #region M�todos_InputSystem

    private void OnMove(InputValue value)
    {
        player.moveInput = value.Get<Vector2>();
    }

    private void OnJump(InputValue value)
    {
        // Resetar o coyote time counter quando o player salta
        if (value.Get<float>() > 0)
        {
            player.coyoteTimeCounter = stats.coyoteTime;
        }
    }

    private void OnLook(InputValue value)
    {
        Vector2 rawInput = value.Get<Vector2>();

        string currentScheme = playerInput.currentControlScheme;

        float deltaX = 0f;
        float deltaY = 0f;

        if (currentScheme.Contains("Mouse") || currentScheme.Contains("Keyboard"))
        {
            deltaX = rawInput.x * mouseSensitivity;
            deltaY = rawInput.y * mouseSensitivity;
        }
        // N�o funciona direito pra controller...
        else if (currentScheme.Contains("Gamepad") || currentScheme.Contains("Controller"))
        {
            if (rawInput.magnitude < controllerDeadzone)
            {
                rawInput = Vector2.zero;
            }
            else
            {
                float adjustedMagnitude = (rawInput.magnitude - controllerDeadzone) / (1f - controllerDeadzone);
                rawInput = rawInput.normalized * Mathf.Clamp01(adjustedMagnitude);
            }
            smoothedControllerInput = Vector2.Lerp(smoothedControllerInput, rawInput, controllerSmoothing);
            deltaX = smoothedControllerInput.x * controllerSensitivity;
            deltaY = smoothedControllerInput.y * controllerSensitivity;
        }
        else
        {
            // Fallback pra rato caso seja outro control scheme
            deltaX = rawInput.x * mouseSensitivity;
            deltaY = rawInput.y * mouseSensitivity;
        }
        yaw += deltaX;
        pitch -= deltaY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        transform.localRotation = Quaternion.Euler(0f, yaw, 0f);
        if (Camera.main != null)
        {
            Camera.main.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }
    }

    //public void OnSprint(InputValue value)
    //{
    //    // Mudar estado de sprinting baseado no valor do input (0 ou 1)
    //    if (value.Get<float>() > 0)
    //    {
    //        isSprinting = true;
    //        Debug.Log("Sprint Started");
    //    }
    //}

    private void OnShoot(InputValue value)
    {
        if (value.Get<float>() > 0)
        {
            player.Shoot();
        }
    }

    private void OnSprint(InputValue value)
    {
        //Debug.Log("Get(): " + value.Get());
        player.isSprinting = value.Get<float>() > 0;
    }

    private void OnTestInteraction(InputValue value)
    {
        if (value.Get<float>() > 0)
        {
            Debug.Log("Test Interaction Triggered");
            player.TestInteraction();
        }
    }
    
    private void OnReturnToMainMenu(InputValue value)
    {
        if (value.Get<float>() > 0)
        {
            gameManager.LoadScene("MainMenu");
        }
    }
    private void OnFlashlight(InputValue value)
    {
        if (value.Get<float>() > 0)
        {
            player.ToggleFlashlight();
        }
    }

    #endregion

    private void Update()
    {
        // O movimento do player est� no Update pois n�o inclui um RigidBody nem c�lculos de f�sica precisos
        // logo n�o vi a necessidade de o colocar no FixedUpdate
        // Caso problemas surjam, mudar para FixedUpdate e testar

        currentControlScheme = playerInput.currentControlScheme;

        if (player.isDead)
            return; // do nothing if the player is dead

        player.HandleMovement();
        player.HandleBunnyHopping();
        //HandleMouseLook();

        // Atualizar estado anterior do ch�o para o pr�ximo frame
        player.wasGrounded = player.characterController.isGrounded;
    }

    // Fun��o para controlar a first-person camera
    //void HandleMouseLook()
    //{
    //    // Get do movimento do rato
    //    float mouseX = Input.GetAxis("Mouse X") * mouseSensitivityX;
    //    float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivityY;

    //    // Atualizar as rota��es
    //    rotationX += mouseX;
    //    rotationY -= mouseY;
    //    rotationY = Mathf.Clamp(rotationY, minY, maxY);

    //    // Aplicar a rota��o ao corpo do jogador (somente em Y para rota��o horizontal)
    //    transform.localRotation = Quaternion.Euler(0f, rotationX, 0f);

    //    // Aplicar a rota��o � c�mera (somente em X para rota��o vertical)
    //    Camera.main.transform.localRotation = Quaternion.Euler(rotationY, 0f, 0f);
    //}
}
