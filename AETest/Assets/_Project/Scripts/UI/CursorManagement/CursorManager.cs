using System.Collections.Generic;
using AE._Project.Scripts.Core.Generics;
using AE._Project.Scripts.InteractableSystem;
using AE._Project.Scripts.Interfaces;
using AE._Project.Scripts.Player;
using AE._Project.Scripts.Puzzles.SwordCoffinPuzzle;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace AE._Project.Scripts.UI.CursorManagement
{
    /// <summary>
    ///     Handles management of the cursor
    /// </summary>
    public class CursorManager : PersistentMonoSingleton<CursorManager>, IAttachListeners
    {
        [FormerlySerializedAs("cursorImage")] [SerializeField]
        private Image _cursorImage;

        [FormerlySerializedAs("cursorDefaultData")] [Header("Default Cursor Animation Config")] [SerializeField]
        private DefaultCursorData _cursorDefaultData;

        [FormerlySerializedAs("cursorInteractionData")] [Header("Interaction Cursor Animation Config")] [SerializeField]
        private CursorInteractionData _cursorInteractionData;


        [FormerlySerializedAs("currentAnimation")] [Foldout("Debug")] [ReadOnly] [SerializeField]
        private CursorAnimation _currentAnimation;

        [FormerlySerializedAs("currentTarget")] [Foldout("Debug")] [ReadOnly] [SerializeField]
        private GameObject _currentTarget;

        private readonly Dictionary<int, CursorAnimation> _cursorAnimationsDictionary = new();

        protected override void Awake()
        {
            base.Awake();

            // Create cursor animation states
            CursorDefault cursorDefault = new(_cursorImage, _cursorDefaultData);
            CursorInteraction cursorInteraction = new(_cursorImage, _cursorInteractionData);

            // Add created animations to dictionary
            _cursorAnimationsDictionary.Add((int)CursorType.DefaultCursor, cursorDefault);
            _cursorAnimationsDictionary.Add((int)CursorType.InteractionCursor, cursorInteraction);
        }

        public void AttachListeners()
        {
            PlayerRaycastController.OnPlayerTargetChanged += OnCurrentlyLookedAtObjectChanged;
            ScPuzzleManager.Instance.OnPuzzleCompleted += OnSCPuzzleCompleted;
        }

        public void DetachListeners()
        {
            PlayerRaycastController.OnPlayerTargetChanged -= OnCurrentlyLookedAtObjectChanged;
        }

        private void OnSCPuzzleCompleted()
        {
            _cursorImage.color = new Color(1, 1, 1, 0);
        }

        /// <summary>
        ///     Called when the currently looked at object has changed
        /// </summary>
        private void OnCurrentlyLookedAtObjectChanged(GameObject newTarget)
        {
            _currentTarget = newTarget;
            TryChangingCurrentCursor();
        }

        /// <summary>
        ///     Manages changing of the current cursor animations
        /// </summary>
        private void TryChangingCurrentCursor()
        {
            if (_currentTarget != null)
            {
                if (_currentTarget.TryGetComponent<InteractableBase>(out var interactable) &&
                    interactable.CanBeInteractedWith)
                {
                    ChangeCursorAnim(CursorType.InteractionCursor);
                    return;
                }
            }

            ChangeCursorAnim(CursorType.DefaultCursor);
        }


        //NOTE: Used in tests
        /*
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                ChangeCursorAnim(CursorType.DefaultCursor);
            }
            if (Input.GetKeyDown(KeyCode.H))
            {
                ChangeCursorAnim(CursorType.InteractionCursor);
            }
        }
        */

        /// <summary>
        ///     Changes current cursor animation
        /// </summary>
        private void ChangeCursorAnim(CursorType cursorType)
        {
            if (!_cursorAnimationsDictionary.TryGetValue((int)cursorType, out var newAnimation))
            {
                Debug.LogError($"Cursor Animation of type {cursorType} not found");
                return;
            }

            _currentAnimation?.OnExit();
            _currentAnimation = newAnimation;
            _currentAnimation?.OnEnter();
        }

        /// <summary>
        ///     Sets whether the system cursor is visible
        /// </summary>
        public void SetCursorVisibility(bool isVisible)
        {
            Cursor.visible = isVisible;
        }


        /// <summary>
        ///     Sets whether the cursor is locked in place of confined to game window
        /// </summary>
        public void SetCursorLockState(bool isLocked)
        {
            if (isLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

            else
            {
                Cursor.lockState = CursorLockMode.Confined;
            }
        }
    }
}