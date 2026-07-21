namespace Gamekit3D
{
    using UnityEngine;
    using UnityEngine.InputSystem;
    using System.Collections;

    /// <summary>
    /// Handles player input using the new Input System
    /// </summary>
    public class PlayerInput : MonoBehaviour
    {
        public static PlayerInput Instance
        {
            get { return s_Instance; }
        }

        protected static PlayerInput s_Instance;

        [HideInInspector]
        public bool playerControllerInputBlocked;

        protected Vector2 m_Movement;
        protected Vector2 m_Camera;
        protected bool m_Jump;
        protected bool m_Attack;
        protected bool m_Pause;
        protected bool m_ExternalInputBlocked;

        protected InputSystem_Actions m_InputActions;

        public Vector2 MoveInput
        {
            get
            {
                if(playerControllerInputBlocked || m_ExternalInputBlocked)
                    return Vector2.zero;
                return m_Movement;
            }
        }

        public Vector2 CameraInput
        {
            get
            {
                if(playerControllerInputBlocked || m_ExternalInputBlocked)
                    return Vector2.zero;
                return m_Camera;
            }
        }

        public bool JumpInput
        {
            get { return m_Jump && !playerControllerInputBlocked && !m_ExternalInputBlocked; }
        }

        public bool Attack
        {
            get { return m_Attack && !playerControllerInputBlocked && !m_ExternalInputBlocked; }
        }

        public bool Pause
        {
            get { return m_Pause; }
        }

        WaitForSeconds m_AttackInputWait;
        Coroutine m_AttackWaitCoroutine;

        const float k_AttackInputDuration = 0.03f;

        void Awake()
        {
            m_AttackInputWait = new WaitForSeconds(k_AttackInputDuration);

            if (s_Instance == null)
                s_Instance = this;
            else if (s_Instance != this)
                throw new UnityException("There cannot be more than one PlayerInput script.  The instances are " + s_Instance.name + " and " + name + ".");

            // Initialize Input Actions
            m_InputActions = new InputSystem_Actions();
            
            // Subscribe to action events
            if (m_InputActions.Player.Attack != null)
                m_InputActions.Player.Attack.performed += OnAttack;
            if (m_InputActions.Player.Pause != null)
                m_InputActions.Player.Pause.performed += OnPause;
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
            // Unsubscribe from events
            if (m_InputActions != null)
            {
                if (m_InputActions.Player.Attack != null)
                    m_InputActions.Player.Attack.performed -= OnAttack;
                if (m_InputActions.Player.Pause != null)
                    m_InputActions.Player.Pause.performed -= OnPause;
                m_InputActions.Dispose();
            }
        }

        void Update()
        {
            if (m_InputActions == null) return;
            
            // Read continuous input values
            if (m_InputActions.Player.Move != null)
                m_Movement = m_InputActions.Player.Move.ReadValue<Vector2>();
            if (m_InputActions.Player.Look != null)
                m_Camera = m_InputActions.Player.Look.ReadValue<Vector2>();
            if (m_InputActions.Player.Jump != null)
                m_Jump = m_InputActions.Player.Jump.IsPressed();
        }

        void OnAttack(InputAction.CallbackContext context)
        {
            if (m_AttackWaitCoroutine != null)
                StopCoroutine(m_AttackWaitCoroutine);

            m_AttackWaitCoroutine = StartCoroutine(AttackWait());
        }

        void OnPause(InputAction.CallbackContext context)
        {
            m_Pause = true;
            StartCoroutine(ResetPause());
        }

        IEnumerator AttackWait()
        {
            m_Attack = true;

            yield return m_AttackInputWait;

            m_Attack = false;
        }

        IEnumerator ResetPause()
        {
            yield return null;
            m_Pause = false;
        }

        public bool HaveControl()
        {
            return !m_ExternalInputBlocked;
        }

        public void ReleaseControl()
        {
            m_ExternalInputBlocked = true;
        }

        public void GainControl()
        {
            m_ExternalInputBlocked = false;
        }
    }
}
