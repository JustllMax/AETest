using System;
using System.Collections.Generic;
using AE.Core.Generics;
using AE.CursorManagement;
using AE.InteractableSystem;
using AE.Interfaces;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace AE.Managers
{
    /// <summary>
    /// Handles management of the cursor
    /// </summary>
    public class CursorManager : PersistentMonoSingleton<CursorManager>, IAttachListeners
    { 
        
        [SerializeField] private Image cursorImage;
        [Header("Default Cursor Animation Config")]
        [SerializeField] private DefaultCursorData cursorDefaultData;
        [Header("Interaction Cursor Animation Config")]
        [SerializeField] private CursorInteractionData cursorInteractionData;

        private Dictionary<int, CursorAnimation> cursorAnimationsDictionary = new();
        
        
        [Foldout("Debug"), ReadOnly][SerializeField] CursorAnimation currentAnimation;
        [Foldout("Debug"), ReadOnly][SerializeField] GameObject currentTarget;
        protected override void Awake()
        {
            base.Awake();
            
            // Create cursor animation states
            CursorDefault cursorDefault = new(cursorImage, cursorDefaultData);
            CursorInteraction cursorInteraction = new(cursorImage, cursorInteractionData);
            
            // Add created animations to dictionary
            cursorAnimationsDictionary.Add((int)CursorType.DefaultCursor, cursorDefault);
            cursorAnimationsDictionary.Add((int)CursorType.InteractionCursor, cursorInteraction);
        }
        
        public void AttachListeners()
        {
            PlayerRaycastController.OnPlayerTargetChanged += OnCurrentlyLookedAtObjectChanged;
        }
        
        /// <summary>
        /// Called when the currently looked at object has changed
        /// </summary>
        private void OnCurrentlyLookedAtObjectChanged(GameObject newTarget)
        {
            currentTarget = newTarget;
            TryChangingCurrentCursor();
        }

        /// <summary>
        /// Manages changing of the current cursor animations
        /// </summary>
        private void TryChangingCurrentCursor()
        {
            if (currentTarget != null)
            {
                if (currentTarget.TryGetComponent<InteractableBase>(out var interactable) && interactable.CanBeInteractedWith)
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
        /// Changes current cursor animation
        /// </summary>
        private void ChangeCursorAnim(CursorType cursorType)
        {
            if (!cursorAnimationsDictionary.TryGetValue((int)cursorType, out CursorAnimation newAnimation))
            {
                Debug.LogError($"Cursor Animation of type {cursorType} not found");
                return;
            }
            
            currentAnimation?.OnExit();
            currentAnimation = newAnimation;
            currentAnimation?.OnEnter();
            
        }
        
        /// <summary>
        /// Sets whether the system cursor is visible
        /// </summary>
        public void SetCursorVisibility(bool isVisible)
        {
            Cursor.visible = isVisible;
        }
        

        /// <summary>
        /// Sets whether the cursor is locked in place of confined to game window
        /// </summary>
        public void SetCursorLockState(bool isLocked)
        {
            if (isLocked) Cursor.lockState = CursorLockMode.Locked;

            else Cursor.lockState = CursorLockMode.Confined;
        }
        
        public void DetachListeners()
        {
            PlayerRaycastController.OnPlayerTargetChanged -= OnCurrentlyLookedAtObjectChanged;
        }
    }
}