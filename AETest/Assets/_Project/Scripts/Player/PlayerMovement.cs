using AE._Project.Scripts.Core.Generics;
using AE._Project.Scripts.InputManagement;
using AE._Project.Scripts.Interfaces;
using AE._Project.Scripts.Managers;
using AE._Project.Scripts.Puzzles.SwordCoffinPuzzle;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace AE._Project.Scripts.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : InGameMonoBehaviour, IAttachListeners
    {
        [FormerlySerializedAs("walkSpeed")] [Header("Movement Settings")] [SerializeField]
        private float _walkSpeed = 6f;

        [FormerlySerializedAs("gravity")] [SerializeField]
        private float _gravity = 10f;

        [SerializeField] private float _cameraSensitivity = 0.1f;

        [FormerlySerializedAs("verticalAngleConstraints")] [Header("Camera Settings")] [SerializeField]
        private Vector2 _verticalAngleConstraints = new(-90f, 90f);

        [FormerlySerializedAs("playerCameraRig")] [Foldout("References")] [SerializeField]
        private Transform _playerCameraRig;

        [FormerlySerializedAs("walkSource")] [Foldout("References")] [SerializeField]
        private AudioSource _walkSource;

        [Foldout("Debug")] [ReadOnly] [SerializeField]
        private bool _canPlayerRotate = true;

        [Foldout("Debug")] [ReadOnly] [SerializeField]
        private bool _canPlayerMove = true;

        [FormerlySerializedAs("movementInput")] [Foldout("Debug")] [ReadOnly] [SerializeField]
        private Vector2 _movementInput;

        [FormerlySerializedAs("moveDirection")] [Foldout("Debug")] [ReadOnly] [SerializeField]
        private Vector3 _moveDirection = Vector3.zero;

        [Foldout("Debug")] [ReadOnly] [SerializeField]
        private float _cameraAngleX;

        [FormerlySerializedAs("mouseInput")] [Foldout("Debug")] [ReadOnly] [SerializeField]
        private Vector2 _mouseInput;

        private CharacterController _characterController;

        public bool CanPlayerMove
        {
            get => _canPlayerMove;
            set => _canPlayerMove = value;
        }

        public bool CanPlayerRotate
        {
            get => _canPlayerRotate;
            set => _canPlayerRotate = value;
        }

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
        }


        private void Update()
        {
            if (!PlayerManager.Instance)
            {
                return;
            }

            if (CanPlayerRotate)
            {
                RotateCharacter();
            }
        }

        private void FixedUpdate()
        {
            HandleMovement();
        }

        public void AttachListeners()
        {
            if (InputManager.Instance)
            {
                InputManager.Instance.PlayerControls.Move.performed += OnMovementInput;
                InputManager.Instance.PlayerControls.Move.canceled += OnMovementInput;
            }
            else
            {
                Debug.LogError("Input Manager doesn't exist.");
            }

            ScPuzzleManager.Instance.OnPuzzleCompleted += OnCoffinPuzzleCompleted;
        }

        public void DetachListeners()
        {
            if (InputManager.Instance)
            {
                InputManager.Instance.PlayerControls.Move.performed -= OnMovementInput;
                InputManager.Instance.PlayerControls.Move.canceled -= OnMovementInput;
            }

            if (ScPuzzleManager.Instance)
            {
                ScPuzzleManager.Instance.OnPuzzleCompleted -= OnCoffinPuzzleCompleted;
            }
        }

        private void OnCoffinPuzzleCompleted()
        {
            _canPlayerMove = false;
        }

        /// <summary>
        ///     Handles rotating player
        /// </summary>
        private void RotateCharacter()
        {
            if (TimeManager.Instance.IsTimePaused)
            {
                return;
            }

            if (InputManager.Instance)
            {
                _mouseInput = InputManager.Instance.PlayerControls.Look.ReadValue<Vector2>();
            }

            // Rotate player transform on horizontal axis
            transform.Rotate(new Vector3(0, _mouseInput.x * _cameraSensitivity, 0));

            // Compute vertical angle
            _cameraAngleX += _mouseInput.y * _cameraSensitivity;
            _cameraAngleX = Mathf.Clamp(_cameraAngleX, _verticalAngleConstraints.x, _verticalAngleConstraints.y);

            // Rotate camera on vertical axisd
            _playerCameraRig.localRotation = Quaternion.Euler(new Vector3(-_cameraAngleX, 0, 0));
        }

        /// <summary>
        ///     Reads current player movement inputs
        /// </summary>
        /// <param name="context"></param>
        private void OnMovementInput(InputAction.CallbackContext context)
        {
            if (!CanPlayerMove || TimeManager.Instance.IsTimePaused)
            {
                Debug.LogWarning("Player cannot move. Ignoring movement input.");
                return;
            }

            // Reads System input (Movement -> Vector2)
            _movementInput = context.ReadValue<Vector2>();
        }

        /// <summary>
        ///     Handles adding velocity to player
        /// </summary>
        private void HandleMovement()
        {
            // If player cannot move, change current move direction to 0, but still allow gravity
            if (!CanPlayerMove)
            {
                _moveDirection = new Vector3(0, _moveDirection.y, 0);
            }
            else
            {
                // Vertical movement
                var forward = transform.forward * _movementInput.y;
                // Horizontal movement
                var right = transform.right * _movementInput.x;

                // Compute direction
                _moveDirection = (forward + right) * _walkSpeed;
            }

            // Adds gravity
            if (!_characterController.isGrounded)
            {
                _moveDirection.y -= _gravity;
            }
            else
            {
                _moveDirection.y = 0;
            }

            // Character Movement
            _characterController.Move(_moveDirection * Time.deltaTime);
            // Play walk sounds
            //NOTE: Player controller is broken, while standing still it changes the 'isGrounded'
            if (_moveDirection.x != 0 || (_moveDirection.z != 0 && _moveDirection.y >= -15))
            {
                AudioManager.Instance.PlaySFXAtSourceOnce(_walkSource.clip, _walkSource);
            }
            else
            {
                _walkSource.Stop();
            }
        }
    }
}