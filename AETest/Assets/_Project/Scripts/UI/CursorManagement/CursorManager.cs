using System;
using System.Collections.Generic;
using AE.Core.Generics;
using AE.CursorManagement;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace AE.Managers
{
    /// <summary>
    /// Handles management of the cursor
    /// </summary>
    public class CursorManager : PersistentMonoSingleton<CursorManager>
    { 
        
        [SerializeField] private Image cursorImage;
        [Header("Default Cursor Animation Config")]
        [SerializeField] private DefaultCursorData cursorDefaultData;
        [Header("Interaction Cursor Animation Config")]
        [SerializeField] private CursorInteractionData cursorInteractionData;

        private Dictionary<int, CursorAnimation> cursorAnimationsDictionary = new();
        
        [Header("Debug")]
        [SerializeField] CursorAnimation currentAnimation;
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

        public void ChangeCursorAnim(CursorType cursorType)
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
        
        
        public void SetCursorVisibility(bool isVisible)
        {
            Cursor.visible = isVisible;
        }

        public void SetCursorLockState(bool isLocked)
        {
            if (isLocked) Cursor.lockState = CursorLockMode.Locked;

            else Cursor.lockState = CursorLockMode.Confined;
        }
        
    }
}