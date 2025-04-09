using System.Collections;
using AE.Core.Generics;
using AE.InputManagement;
using AE.Interfaces;
using AE.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using InputSystem = AE.InputManagement.InputSystem;

namespace Terra.Player
{
    
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : InGameMonoBehaviour, IAttachListeners, IWithSetUp
    {
        [Header("Movement Settings")]
        [SerializeField] private float walkSpeed = 6f;
        [SerializeField] private float gravity = 10f;
        [SerializeField] private float _cameraSensitivity = 0.1f;
        [Header("Camera Settings")]
        [SerializeField] private Vector2 verticalAngleConstraints = new Vector2(-90f, 90f);
        [Header("References")] 
        [SerializeField] private Transform playerCameraRig;
        
        [Header("Debug")]
        [SerializeField] private bool _canPlayerRotate = true;        
        [SerializeField] private bool _canPlayerMove = true;        
        [SerializeField] private Vector2 movementInput;
        [SerializeField] private Vector3 moveDirection = Vector3.zero;
        [SerializeField] private float _cameraAngleX;
        
        private CharacterController characterController;
        private InputSystem.PlayerActions inputActions;
        public bool CanPlayerMove { 
            get => _canPlayerMove;
            set => _canPlayerMove = value;
        }
        public bool CanPlayerRotate { 
            get => _canPlayerRotate;
            set => _canPlayerRotate = value;
        }
        
        public void SetUp()
        {
            characterController = GetComponent<CharacterController>();
            
            if(InputManager.Instance) inputActions = InputManager.Instance.PlayerControls;
            else Debug.LogError("Input Manager doesn't exist.");
        }
        public void AttachListeners()
        {
            inputActions.Move.performed += OnMovementInput;
            inputActions.Move.canceled += OnMovementInput;
        }


        void Update()
        {
            if (!PlayerManager.Instance)
            {
                return;
            }
            
            if(CanPlayerRotate) RotateCharacter();
            if(CanPlayerMove) HandleMovement();
        }

        private void RotateCharacter()
        {
            var mouseInput = inputActions.Look.ReadValue<Vector2>();
            
            // Rotate player transform on horizontal axis
            transform.Rotate(new Vector3(0, mouseInput.x * _cameraSensitivity, 0));

            // Compute vertical angle
            _cameraAngleX += mouseInput.y * _cameraSensitivity;
            _cameraAngleX = Mathf.Clamp(_cameraAngleX, verticalAngleConstraints.x, verticalAngleConstraints.y);

            // Rotate camera on vertical axisd
            playerCameraRig.localRotation = Quaternion.Euler(new Vector3(-_cameraAngleX, 0, 0));
        }
        private void HandleMovement()
        {
            // Vertical movement
            Vector3 forward = transform.forward * movementInput.y;
            // Horizontal movement
            Vector3 right = transform.right * movementInput.x;
            
            // Compuet direction
            moveDirection = (forward + right) * walkSpeed;

            // Adds gravity
            if (!characterController.isGrounded) moveDirection.y -= gravity * Time.deltaTime;
            
            else moveDirection.y = 0;
            

            // Character Movement
            characterController.Move(moveDirection * Time.deltaTime);
        }

        private void OnMovementInput(InputAction.CallbackContext context)
        {
            if (!CanPlayerMove)
            {
                Debug.LogWarning("Player cannot move. Ignoring movement input.");
                return;
            }

            // Reads System input (Movement -> Vector2)
            movementInput = context.ReadValue<Vector2>();
        }
        


        public void DetachListeners()
        {
            if (inputActions.Move != null)
            {
                inputActions.Move.performed -= OnMovementInput;
                inputActions.Move.canceled -= OnMovementInput;
            }
        }
        
        public void TearDown()
        {
            
        }
        
    }
}