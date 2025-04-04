using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Player Data")]
    [SerializeField] private Player player;

    [Header("Camera Settings")]
    [Header("Mouse Settings")]
    [Tooltip("Sensibilidade do Rato")]
    [SerializeField] private float mouseSensitivity = 0.1f;

    // Não funciona direito
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

    private void Awake()
    {
        // Inicializar o CharacterController
        player.characterController = GetComponent<CharacterController>();

        // Inicializar o PlayerInput
        playerInput = GetComponent<PlayerInput>();

        smoothedControllerInput = Vector2.zero;

        // Lock e hide do cursor do rato no jogo
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Player Input behavior tá set para 'Send Messages' (métodos usam InputValue e não o CallbackContext)
    #region Métodos_InputSystem

    private void OnMove(InputValue value)
    {
        player.moveInput = value.Get<Vector2>();
    }

    private void OnJump(InputValue value)
    {
        // Resetar o coyote time counter quando o player salta
        if (value.Get<float>() > 0)
        {
            player.coyoteTimeCounter = player.coyoteTime;
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
        // Não funciona direito pra controller...
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

    #endregion

    private void Update()
    {
        // O movimento do player está no Update pois não inclui um RigidBody nem cálculos de física precisos
        // logo não vi a necessidade de o colocar no FixedUpdate
        // Caso problemas surjam, mudar para FixedUpdate e testar

        currentControlScheme = playerInput.currentControlScheme;

        player.HandleMovement();
        player.HandleBunnyHopping();
        //HandleMouseLook();

        // Atualizar estado anterior do chão para o próximo frame
        player.wasGrounded = player.characterController.isGrounded;
    }

    // Função para controlar a first-person camera
    //void HandleMouseLook()
    //{
    //    // Get do movimento do rato
    //    float mouseX = Input.GetAxis("Mouse X") * mouseSensitivityX;
    //    float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivityY;

    //    // Atualizar as rotações
    //    rotationX += mouseX;
    //    rotationY -= mouseY;
    //    rotationY = Mathf.Clamp(rotationY, minY, maxY);

    //    // Aplicar a rotação ao corpo do jogador (somente em Y para rotação horizontal)
    //    transform.localRotation = Quaternion.Euler(0f, rotationX, 0f);

    //    // Aplicar a rotação à câmera (somente em X para rotação vertical)
    //    Camera.main.transform.localRotation = Quaternion.Euler(rotationY, 0f, 0f);
    //}
}
