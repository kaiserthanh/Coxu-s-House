using UnityEngine;
using UnityEngine.InputSystem;

namespace CoxusHouse.Player
{
    // Local first-person walk, sprint and mouse look. From T4 on, the network player
    // enables this only on the machine that owns the player.
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Transform _cameraPivot;
        [SerializeField] private float _walkSpeed = 4f;
        [SerializeField] private float _sprintSpeed = 6.5f;
        [SerializeField] private float _acceleration = 25f;
        [SerializeField, Tooltip("Degrees of turn per pixel of mouse movement")] private float _lookSensitivity = 0.1f;
        [SerializeField] private float _maxPitch = 85f;
        [SerializeField] private float _gravity = -20f;

        private CharacterController _controller;
        private Vector3 _horizontalVelocity;
        private float _verticalVelocity;
        private float _pitch;

        public GameInput Actions { get; private set; }
        public Transform CameraPivot => _cameraPivot;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            Actions = new GameInput();
        }

        private void OnEnable()
        {
            Actions.Enable();
            SetCursorLocked(true);
        }

        private void OnDisable()
        {
            Actions.Disable();
            SetCursorLocked(false);
        }

        private void OnDestroy()
        {
            Actions.Dispose();
        }

        private void Update()
        {
            UpdateCursorLock();
            bool hasControl = Cursor.lockState == CursorLockMode.Locked;
            if (hasControl)
            {
                Look();
            }
            Move(hasControl);
        }

        // Esc frees the mouse (for the editor or other windows); clicking in the game takes it back.
        private static void UpdateCursorLock()
        {
            Keyboard keyboard = Keyboard.current;
            Mouse mouse = Mouse.current;
            if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
            {
                SetCursorLocked(false);
            }
            else if (mouse != null && mouse.leftButton.wasPressedThisFrame && Cursor.lockState != CursorLockMode.Locked)
            {
                SetCursorLocked(true);
            }
        }

        private void Look()
        {
            Vector2 delta = Actions.Look.ReadValue<Vector2>() * _lookSensitivity;
            transform.Rotate(0f, delta.x, 0f);
            _pitch = Mathf.Clamp(_pitch - delta.y, -_maxPitch, _maxPitch);
            _cameraPivot.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        }

        private void Move(bool hasControl)
        {
            Vector2 input = hasControl ? Vector2.ClampMagnitude(Actions.Move.ReadValue<Vector2>(), 1f) : Vector2.zero;
            float speed = Actions.Sprint.IsPressed() ? _sprintSpeed : _walkSpeed;
            Vector3 targetVelocity = (transform.right * input.x + transform.forward * input.y) * speed;
            _horizontalVelocity = Vector3.MoveTowards(_horizontalVelocity, targetVelocity, _acceleration * Time.deltaTime);

            // A small constant push keeps isGrounded reliable on steps and slopes.
            if (_controller.isGrounded && _verticalVelocity < 0f)
            {
                _verticalVelocity = -2f;
            }
            _verticalVelocity += _gravity * Time.deltaTime;

            _controller.Move((_horizontalVelocity + Vector3.up * _verticalVelocity) * Time.deltaTime);
        }

        private static void SetCursorLocked(bool locked)
        {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }
    }
}
