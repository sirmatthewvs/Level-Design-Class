namespace Gamekit3D
{
    using UnityEngine;
    using Cinemachine;
    using UnityEngine.InputSystem;

    /// <summary>
    /// Camera control modes for different player skill levels
    /// </summary>
    public enum CameraMode
    {
        Manual,     // Player controls camera freely (traditional)
        Auto        // Camera auto-follows player movement direction (beginner-friendly)
    }

    /// <summary>
    /// Bridges the new Input System to Cinemachine cameras with multiple control modes
    /// Supports both Manual (full control) and Auto (beginner-friendly) camera modes
    /// </summary>
    [RequireComponent(typeof(CinemachineFreeLook))]
    public class CinemachineInputProvider : MonoBehaviour
    {
        private CinemachineFreeLook freeLookCamera;
        private InputSystem_Actions inputActions;
        private Transform playerTransform;
        private CharacterController playerCharacterController;
        private Vector3 lastPlayerPosition;
        readonly private float targetXAxisValue;

        [Header("Camera Mode")]
        [Tooltip("Manual: Player controls camera freely\nAuto: Camera follows player movement direction")]
        [SerializeField] private CameraMode cameraMode = CameraMode.Manual;

        [Header("Manual Mode Settings")]
        [SerializeField] private float mouseSensitivity = 1f;
        [SerializeField] private float controllerSensitivity = 200f;

        [Header("Auto Mode Settings")]
        [Tooltip("How smoothly the camera rotates when moving")]
        [SerializeField] private float autoRotationSpeed = 2f;
        
        [Tooltip("Smoothing applied to camera rotation (higher = smoother, less noise)")]
        [SerializeField] private float rotationDamping = 1f;
        
        [Tooltip("Minimum input to consider player is moving")]
        [SerializeField] private float inputDeadzone = 0.2f;
        
        [Tooltip("Camera gently aligns behind character when idle")]
        [SerializeField] private bool alignWhenIdle = true;
        
        [Tooltip("How slow the idle alignment is (higher = slower, gentler)")]
        [SerializeField] private float idleAlignDamping = 3f;
        
        [Tooltip("How long to wait before starting idle alignment")]
        [SerializeField] private float idleAlignDelay = 1f;
        
        [Tooltip("Right-click to temporarily enable manual control in Auto mode")]
        [SerializeField] private bool rightClickForManualControl = true;

        private float timeSinceLastMovement;
        private float currentRotationVelocity;
        private Vector2 smoothedMoveInput;
        private Vector2 moveInputVelocity;
        private bool wasManualLastFrame;

        void Awake()
        {
            freeLookCamera = GetComponent<CinemachineFreeLook>();
            
            // Disable Cinemachine's built-in input
            freeLookCamera.m_XAxis.m_InputAxisName = "";
            freeLookCamera.m_YAxis.m_InputAxisName = "";

            inputActions = new InputSystem_Actions();

            // Find player and character controller
            PlayerController player = FindFirstObjectByType<PlayerController>();
            if (player != null)
            {
                playerTransform = player.transform;
                playerCharacterController = player.GetComponent<CharacterController>();
                lastPlayerPosition = playerTransform.position;
                
                if (playerCharacterController == null)
                {
                    Debug.LogWarning("CinemachineInputProvider: CharacterController not found on player. Auto camera will use position-based movement detection.");
                }
            }
            else
            {
                Debug.LogWarning("CinemachineInputProvider: PlayerController not found. Auto camera mode may not work correctly.");
            }
        }

        void OnEnable()
        {
            if (inputActions != null)
                inputActions.Enable();
        }

        void OnDisable()
        {
            if (inputActions != null)
                inputActions.Disable();
        }

        void OnDestroy()
        {
            if (inputActions != null)
                inputActions.Dispose();
        }

        void Update()
        {
            if (inputActions == null || freeLookCamera == null) return;

            switch (cameraMode)
            {
                case CameraMode.Manual:
                    UpdateManualMode();
                    break;
                case CameraMode.Auto:
                    UpdateAutoMode();
                    break;
            }
        }

        /// <summary>
        /// Traditional camera control - player has full control
        /// </summary>
        private void UpdateManualMode()
        {
            Vector2 lookInput = inputActions.Player.Look.ReadValue<Vector2>();

            // Apply sensitivity based on input device
            float sensitivity = Mathf.Approximately(lookInput.magnitude, 0f) ? 
                controllerSensitivity : mouseSensitivity;

            // Feed the input to Cinemachine
            freeLookCamera.m_XAxis.m_InputAxisValue = lookInput.x * sensitivity;
            freeLookCamera.m_YAxis.m_InputAxisValue = lookInput.y * sensitivity;
        }

        /// <summary>
        /// Auto-follow camera - rotates to keep behind player's input direction
        /// This feels natural because camera follows where you want to go, not where character faces
        /// </summary>
        private void UpdateAutoMode()
        {
            // Check for manual override (right-click held)
            bool rightClickHeld = false;
            if (rightClickForManualControl && Mouse.current != null)
            {
                rightClickHeld = Mouse.current.rightButton.isPressed;
            }

            if (rightClickHeld)
            {
                // Manual override active - use manual controls for both X and Y
                Vector2 lookInput = inputActions.Player.Look.ReadValue<Vector2>();
                float sensitivity = Mathf.Approximately(lookInput.magnitude, 0f) ? 
                    controllerSensitivity : mouseSensitivity;

                freeLookCamera.m_XAxis.m_InputAxisValue = lookInput.x * sensitivity;
                freeLookCamera.m_YAxis.m_InputAxisValue = lookInput.y * sensitivity;
                
                wasManualLastFrame = true;
                timeSinceLastMovement = 0f;
            }
            else
            {
                // Just released right-click - stop any momentum
                if (wasManualLastFrame)
                {
                    // Reset velocity to prevent infinite spinning
                    currentRotationVelocity = 0f;
                    freeLookCamera.m_XAxis.m_InputAxisValue = 0f;
                    freeLookCamera.m_YAxis.m_InputAxisValue = 0f;
                    wasManualLastFrame = false;
                }
                
                // Auto-follow mode - follows character facing direction
                if (inputActions != null && playerTransform != null)
                {
                    // Read movement input
                    Vector2 rawMoveInput = inputActions.Player.Move.ReadValue<Vector2>();
                    
                    // Smooth input to filter noise from attacks/animations
                    smoothedMoveInput = Vector2.SmoothDamp(
                        smoothedMoveInput,
                        rawMoveInput,
                        ref moveInputVelocity,
                        0.2f // Moderate smoothing
                    );
                    
                    float inputMagnitude = smoothedMoveInput.magnitude;
                    
                    // Get character's facing direction
                    Vector3 playerForward = playerTransform.forward;
                    playerForward.y = 0;
                    
                    if (playerForward.magnitude > 0.1f)
                    {
                        playerForward.Normalize();
                        
                        // Calculate target angle to be behind character
                        float targetAngle = Mathf.Atan2(playerForward.x, playerForward.z) * Mathf.Rad2Deg;
                        
                        // Get current camera angle
                        float currentAngle = freeLookCamera.m_XAxis.Value;
                        
                        if (inputMagnitude > inputDeadzone) // Player is moving
                        {
                            // Active movement - follow closely
                            float smoothedAngle = Mathf.SmoothDampAngle(
                                currentAngle,
                                targetAngle,
                                ref currentRotationVelocity,
                                rotationDamping,
                                autoRotationSpeed * 100f,
                                Time.deltaTime
                            );
                            
                            freeLookCamera.m_XAxis.Value = smoothedAngle;
                            timeSinceLastMovement = 0f;
                        }
                        else if (alignWhenIdle && timeSinceLastMovement > idleAlignDelay)
                        {
                            // Player idle - GENTLY align behind character
                            float smoothedAngle = Mathf.SmoothDampAngle(
                                currentAngle,
                                targetAngle,
                                ref currentRotationVelocity,
                                idleAlignDamping, // Much slower damping for gentle alignment
                                autoRotationSpeed * 50f, // Half speed for gentle movement
                                Time.deltaTime
                            );
                            
                            freeLookCamera.m_XAxis.Value = smoothedAngle;
                        }
                        else
                        {
                            // Just stopped - hold position briefly
                            currentRotationVelocity *= 0.85f;
                            timeSinceLastMovement += Time.deltaTime;
                        }
                    }
                    
                    lastPlayerPosition = playerTransform.position;
                }

                // In auto mode, Y-axis is also locked (no free vertical movement)
                // Player must hold right-click to control vertical camera
                freeLookCamera.m_YAxis.m_InputAxisValue = 0f;
            }
        }

        /// <summary>
        /// Switch camera mode at runtime
        /// </summary>
        public void SetCameraMode(CameraMode mode)
        {
            cameraMode = mode;
            timeSinceLastMovement = 0f;
        }

        /// <summary>
        /// Get current camera mode
        /// </summary>
        public CameraMode GetCameraMode()
        {
            return cameraMode;
        }

        /// <summary>
        /// Toggle between Manual and Auto modes
        /// </summary>
        public void ToggleCameraMode()
        {
            cameraMode = (cameraMode == CameraMode.Manual) ? CameraMode.Auto : CameraMode.Manual;
            timeSinceLastMovement = 0f;
        }
    }
}
