using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private bool inputEnabled = true;

    public float speed = 5f;
    public float mouseSensitivity = 2f;
    public float jumpForce = 5f;
    public float gravity = -20f;

    // === ���� ������ ===
    public AudioClip footstepsSound;
    public float footstepsVolume = 0.4f;
    public float footstepsPitchMin = 0.9f;
    public float footstepsPitchMax = 1.1f;

    private float yRotation = 0f;
    private CharacterController controller;
    private Transform cameraTransform;
    private InputSystem_Actions actions;

    private Vector3 moveDirection;
    private float verticalVelocity = 0f;
    private AudioSource audioSource;
    private bool isWalking = false;

    void Awake()
    {
        actions = new InputSystem_Actions();
        actions.Player.Enable();
    }

    public void EnableInput()
    {
        actions.Player.Enable();
        inputEnabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void DisableInput()
    {
        actions.Player.Disable();
        inputEnabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;

        // === ������������� AUDIO SOURCE ===
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 3D ����
        audioSource.loop = true; // Зацикливаем звук шагов
        
        if (footstepsSound != null)
        {
            audioSource.clip = footstepsSound;
            audioSource.volume = footstepsVolume;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!inputEnabled) return;

        HandleLook();
        HandleMoveInput();
        HandleJump();
        ApplyGravityAndMove();
        HandleFootsteps();
    }

    void HandleMoveInput()
    {
        Vector2 input = actions.Player.Move.ReadValue<Vector2>();
        moveDirection = transform.right * input.x + transform.forward * input.y;
        moveDirection = moveDirection.normalized * speed;
    }

    void HandleLook()
    {
        Vector2 lookInput = actions.Player.Look.ReadValue<Vector2>();
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        yRotation -= mouseY;
        yRotation = Mathf.Clamp(yRotation, -80f, 80f);

        cameraTransform.localRotation = Quaternion.Euler(yRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleJump()
    {
        if (controller.isGrounded)
        {
            if (verticalVelocity < 0) verticalVelocity = 0f;

            if (actions.Player.Jump.WasPressedThisFrame())
            {
                verticalVelocity = jumpForce;
            }
        }
    }

    void ApplyGravityAndMove()
    {
        verticalVelocity += gravity * Time.deltaTime;

        Vector3 finalMove = moveDirection * Time.deltaTime;
        finalMove.y = verticalVelocity * Time.deltaTime;

        controller.Move(finalMove);
    }

    // === ���������� ������ ������ ===
    void HandleFootsteps()
    {
        bool isMoving = controller.isGrounded && moveDirection.magnitude > 0.1f;

        if (isMoving && !isWalking && footstepsSound != null)
        {
            isWalking = true;
            audioSource.Play();
        }
        else if (!isMoving && isWalking)
        {
            isWalking = false;
            audioSource.Stop();
        }
    }

    void OnDestroy()
    {
        actions?.Dispose();
    }
}