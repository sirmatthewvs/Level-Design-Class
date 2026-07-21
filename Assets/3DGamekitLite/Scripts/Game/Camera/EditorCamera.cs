namespace Gamekit3D.Cameras
{
    using UnityEngine;
    using UnityEngine.InputSystem;

    /// <summary>
    /// A class that mimic's the Unity Editor camera using the new Input System
    /// </summary>
    public class EditorCamera : MonoBehaviour
    {
        public Vector3 velocity;
        public Vector3 angles;

        Vector3 m_MousePosition;
        Vector3 m_MouseDelta;
        Quaternion m_OriginRotation;

        void Start()
        {
            m_MousePosition = Mouse.current.position.ReadValue();
            m_OriginRotation = transform.localRotation;
        }

        void Update()
        {
            // Mouse delta tracking
            Vector3 currentMousePos = Mouse.current.position.ReadValue();
            m_MouseDelta = currentMousePos - m_MousePosition;
            m_MousePosition = currentMousePos;

            // Keyboard input for movement
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null) return;

            bool isShiftPressed = keyboard.leftShiftKey.isPressed;
            float speedMultiplier = isShiftPressed ? 10 : 3;

            if (keyboard.wKey.isPressed)
                velocity.z += Time.deltaTime * speedMultiplier;
            else if (keyboard.sKey.isPressed)
                velocity.z -= Time.deltaTime * speedMultiplier;
            else
                velocity.z *= Time.deltaTime;

            if (keyboard.aKey.isPressed)
                velocity.x -= Time.deltaTime * speedMultiplier;
            else if (keyboard.dKey.isPressed)
                velocity.x += Time.deltaTime * speedMultiplier;
            else
                velocity.x *= Time.deltaTime;

            // Mouse button for rotation
            if (Mouse.current.rightButton.isPressed)
            {
                angles.x += m_MouseDelta.y;
                angles.y += m_MouseDelta.x;
            }

            transform.Translate(velocity * Time.deltaTime, Space.Self);

            var yaw = Quaternion.AngleAxis(angles.y, Vector3.up);
            var pitch = Quaternion.AngleAxis(angles.x, Vector3.left);
            transform.localRotation = m_OriginRotation * yaw * pitch;
        }
    }
}
