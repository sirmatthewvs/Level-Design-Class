namespace Gamekit3D
{
    using UnityEngine;
    using UnityEngine.InputSystem;
    using UnityEngine.Events;

    /// <summary>
    /// Triggers an interaction when the player presses the interact button
    /// </summary>
    public class InteractOnButton : InteractOnTrigger
    {
        public UnityEvent OnButtonPress;

        bool m_CanExecuteButtons = false;
        InputSystem_Actions m_InputActions;

        void Awake()
        {
            m_InputActions = new InputSystem_Actions();
            if (m_InputActions.Player.Interact != null)
                m_InputActions.Player.Interact.performed += OnInteract;
        }

        void OnEnable()
        {
            if (m_InputActions != null)
                m_InputActions.Enable();
        }

        void OnDisable()
        {
            if (m_InputActions != null)
                m_InputActions.Disable();
        }

        void OnDestroy()
        {
            if (m_InputActions != null)
            {
                if (m_InputActions.Player.Interact != null)
                    m_InputActions.Player.Interact.performed -= OnInteract;
                m_InputActions.Dispose();
            }
        }

        protected override void ExecuteOnEnter(Collider other)
        {
            m_CanExecuteButtons = true;
        }

        protected override void ExecuteOnExit(Collider other)
        {
            m_CanExecuteButtons = false;
        }

        void OnInteract(InputAction.CallbackContext context)
        {
            if (m_CanExecuteButtons)
            {
                OnButtonPress.Invoke();
            }
        }
    } 
}
