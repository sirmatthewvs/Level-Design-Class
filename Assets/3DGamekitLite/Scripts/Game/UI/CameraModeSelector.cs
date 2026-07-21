namespace Gamekit3D
{
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;

    /// <summary>
    /// UI component for switching between camera modes
    /// Can be added to settings menu or as a quick toggle
    /// </summary>
    public class CameraModeSelector : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CinemachineInputProvider cameraInputProvider;
        
        [Header("UI Elements (Optional)")]
        [SerializeField] private Toggle modeToggle;
        [SerializeField] private Button toggleButton;
        [SerializeField] private TextMeshProUGUI statusText;
        
        [Header("Settings")]
        [SerializeField] private bool findCameraAutomatically = true;
        [SerializeField] private KeyCode quickToggleKey = KeyCode.C;
        [SerializeField] private bool enableQuickToggle = true;

        void Start()
        {
            // Find camera provider automatically if not assigned
            if (cameraInputProvider == null && findCameraAutomatically)
            {
                cameraInputProvider = FindFirstObjectByType<CinemachineInputProvider>();
                
                if (cameraInputProvider == null)
                {
                    Debug.LogError("CameraModeSelector: Could not find CinemachineInputProvider in scene!");
                    enabled = false;
                    return;
                }
            }

            // Setup UI elements
            if (modeToggle != null)
            {
                modeToggle.isOn = (cameraInputProvider.GetCameraMode() == CameraMode.Auto);
                modeToggle.onValueChanged.AddListener(OnToggleChanged);
            }

            if (toggleButton != null)
            {
                toggleButton.onClick.AddListener(OnButtonClicked);
            }

            UpdateStatusText();
        }

        void Update()
        {
            // Quick toggle with keyboard
            if (enableQuickToggle && Input.GetKeyDown(quickToggleKey))
            {
                ToggleMode();
            }
        }

        void OnToggleChanged(bool isAuto)
        {
            if (cameraInputProvider == null) return;
            
            cameraInputProvider.SetCameraMode(isAuto ? CameraMode.Auto : CameraMode.Manual);
            UpdateStatusText();
        }

        void OnButtonClicked()
        {
            ToggleMode();
        }

        public void ToggleMode()
        {
            if (cameraInputProvider == null) return;
            
            cameraInputProvider.ToggleCameraMode();
            
            // Update toggle if present
            if (modeToggle != null)
            {
                modeToggle.isOn = (cameraInputProvider.GetCameraMode() == CameraMode.Auto);
            }
            
            UpdateStatusText();
        }

        public void SetManualMode()
        {
            if (cameraInputProvider == null) return;
            cameraInputProvider.SetCameraMode(CameraMode.Manual);
            UpdateUI();
        }

        public void SetAutoMode()
        {
            if (cameraInputProvider == null) return;
            cameraInputProvider.SetCameraMode(CameraMode.Auto);
            UpdateUI();
        }

        void UpdateUI()
        {
            if (modeToggle != null)
            {
                modeToggle.isOn = (cameraInputProvider.GetCameraMode() == CameraMode.Auto);
            }
            UpdateStatusText();
        }

        void UpdateStatusText()
        {
            if (statusText == null || cameraInputProvider == null) return;

            CameraMode currentMode = cameraInputProvider.GetCameraMode();
            
            switch (currentMode)
            {
                case CameraMode.Manual:
                    statusText.text = "Camera: Manual (Full Control)";
                    break;
                case CameraMode.Auto:
                    statusText.text = "Camera: Auto (Hold Right-Click to Override)";
                    break;
            }
        }

        void OnDestroy()
        {
            if (modeToggle != null)
            {
                modeToggle.onValueChanged.RemoveListener(OnToggleChanged);
            }

            if (toggleButton != null)
            {
                toggleButton.onClick.RemoveListener(OnButtonClicked);
            }
        }
    }
}
