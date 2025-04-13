using System;
using System.Collections;
using AE.Core.Generics;
using AE.InputManagement;
using AE.Interfaces;
using AE.Managers;
using AE.Player;
using AE.Puzzles.SwordCoffinPuzzle;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;
using InputSystem = AE.InputManagement.InputSystem;

namespace Terra.Player
{
    
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : InGameMonoBehaviour, IAttachListeners
    {
        [Header("Movement Settings")]
        [SerializeField] private float walkSpeed = 6f;
        [SerializeField] private float gravity = 10f;
        [SerializeField] private float _cameraSensitivity = 0.1f;
        [Header("Camera Settings")]
        [SerializeField] private Vector2 verticalAngleConstraints = new Vector2(-90f, 90f);
        
        [Foldout("References")][SerializeField] private Transform playerCameraRig;
        [Foldout("References")][SerializeField] private AudioSource walkSource;
        
        [Foldout("Debug"), ReadOnly]
        [SerializeField] private bool _canPlayerRotate = true;        
        [Foldout("Debug"), ReadOnly]
        [SerializeField] private bool _canPlayerMove = true;        
        [Foldout("Debug"), ReadOnly]
        [SerializeField] private Vector2 movementInput;
        [Foldout("Debug"), ReadOnly]
        [SerializeField] private Vector3 moveDirection = Vector3.zero;
        [Foldout("Debug"), ReadOnly]
        [SerializeField] private float _cameraAngleX;
        [Foldout("Debug"), ReadOnly]
        [SerializeField] private Vector2 mouseInput;
        
        private CharacterController characterController;
        public bool CanPlayerMove { 
            get => _canPlayerMove;
            set => _canPlayerMove = value;
        }
        public bool CanPlayerRotate { 
            get => _canPlayerRotate;
            set => _canPlayerRotate = value;
        }

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
        }

        public void AttachListeners()
        {
            if (InputManager.Instance)
            {
                InputManager.Instance.PlayerControls.Move.performed += OnMovementInput;
                InputManager.Instance.PlayerControls.Move.canceled += OnMovementInput;
            }
            else Debug.LogError("Input Manager doesn't exist.");

            SCPuzzleManager.Instance.OnPuzzleCompleted += OnCoffinPuzzleCompleted;
        }


        void Update()
        {
            if (!PlayerManager.Instance)
            {
                return;
            } 
            if(CanPlayerRotate) RotateCharacter();
        }

        private void OnCoffinPuzzleCompleted() => _canPlayerMove = false;
        private void FixedUpdate()
        {
            HandleMovement();
        }

        /// <summary>
        /// Handles rotating player
        /// </summary>
        private void RotateCharacter()
        {
            if(TimeManager.Instance.IsTimePaused) return;
            
            if(InputManager.Instance)
                mouseInput = InputManager.Instance.PlayerControls.Look.ReadValue<Vector2>();

            // Rotate player transform on horizontal axis
            transform.Rotate(new Vector3(0, mouseInput.x * _cameraSensitivity, 0));

            // Compute vertical angle
            _cameraAngleX += mouseInput.y * _cameraSensitivity;
            _cameraAngleX = Mathf.Clamp(_cameraAngleX, verticalAngleConstraints.x, verticalAngleConstraints.y);

            // Rotate camera on vertical axisd
            playerCameraRig.localRotation = Quaternion.Euler(new Vector3(-_cameraAngleX, 0, 0));
        }
        
        /// <summary>
        /// Reads current player movement inputs
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
            movementInput = context.ReadValue<Vector2>();
        }
        
        /// <summary>
        /// Handles adding velocity to player
        /// </summary>
        private void HandleMovement()
        {
            // If player cannot move, change current move direction to 0, but still allow gravity
            if(!CanPlayerMove)
            {
                moveDirection = new Vector3(0, moveDirection.y, 0);
            }
            else
            {
                // Vertical movement
                Vector3 forward = transform.forward * movementInput.y;
                // Horizontal movement
                Vector3 right = transform.right * movementInput.x;
            
                // Compute direction
                moveDirection = (forward + right) * walkSpeed;  
            }
            
            // Adds gravity
            if (!characterController.isGrounded) moveDirection.y -= gravity;
            else moveDirection.y = 0;
            
            // Character Movement
            characterController.Move(moveDirection * Time.deltaTime);
            // Play walk sounds
            //NOTE: Player controller is broken, while standing still it changes the 'isGrounded'
            if(moveDirection.x != 0|| moveDirection.z != 0 && (moveDirection.y >= -15) )
                AudioManager.Instance.PlaySFXAtSourceOnce(walkSource.clip, walkSource);
            else walkSource.Stop();
        }

        public void DetachListeners()
        {
            if (InputManager.Instance)
            {
                InputManager.Instance.PlayerControls.Move.performed -= OnMovementInput;
                InputManager.Instance.PlayerControls.Move.canceled -= OnMovementInput;
            }
            if(SCPuzzleManager.Instance)
                SCPuzzleManager.Instance.OnPuzzleCompleted -= OnCoffinPuzzleCompleted;
        }
    }
}