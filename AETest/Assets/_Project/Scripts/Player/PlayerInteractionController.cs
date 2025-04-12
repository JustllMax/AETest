using System;
using AE.Core.Generics;
using AE.InputManagement;
using AE.Interfaces;
using AE.Managers;
using AE.Puzzles.TorchSkullPuzzle.Objects.InteractableItems;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AE.Player
{
    [Serializable]
    public class PlayerInteractionController : InGameMonoBehaviour, IAttachListeners
    {

        [SerializeField] private string incorrectPickupInteractionText = "Cannot interact with this";
        [SerializeField] private Transform pickupTransform;
       
        [Foldout("Debug"), ReadOnly] [SerializeField] private InteractableItem currentItem = null;
        [Foldout("Debug"), ReadOnly] [SerializeField] private InteractableBase currentPlayerTarget = null;
        
        public InteractableItem Item => currentItem;
        private bool CanPickupItem => currentItem == null;
        
        public void AttachListeners()
        {
            PlayerRaycastController.OnPlayerTargetChanged += OnPlayerTargetChanged;
            InputManager.Instance.PlayerControls.Attack.performed += OnPlayerInteractionButton;
            InputManager.Instance.PlayerControls.Interact.performed += OnPlayerInteractionButton;
            InputManager.Instance.PlayerControls.Aim.performed += OnPlayerRMB;
        }

        private void OnPlayerTargetChanged(GameObject newTarget)
        {
            currentPlayerTarget?.HideOutline();
            
            currentPlayerTarget = null;
            if(newTarget == null) return;
            
            // Try get interactable
            if(!newTarget.TryGetComponent(out InteractableBase interactable)) return;

            currentPlayerTarget = interactable;    
            if(interactable.CanBeInteractedWith) interactable.ShowOutline();
        }

        /// <summary>
        /// Handles interaction with items
        /// </summary>
        private bool TryUseItem()
        {
            if(Item == null) return false;

            if (TryGet(out InteractableWithItem<InteractableSkull> usableItem))
            {
                Debug.Log($"found a usable item {usableItem.name}");
                usableItem?.UseItem(Item as InteractableSkull);
                return true;
            }
            Debug.Log($"Not found a usable item");

            return false;
        }
        
        /// <summary>
        /// Handles interaction with items
        /// </summary>
        private bool TryPickupItem()
        {

            if (TryGet(out InteractableItem newItem))
            {
                if (CanPickupItem)
                {
                    // If item can be picked up, pick it up
                    if (newItem.CanBePickedUp) Pickup(newItem);
                    // If item cannot be picked up, try interacting with it
                    else  newItem.Interact();
                    return true;
                }
                else
                {
                    // Show incorrect interaction
                    IncorrectInteraction();
                    return true;
                }

            }
            return false;
        }

        /// <summary>
        /// Handles interaction with objects
        /// </summary>
        private bool TryInteractWithObject()
        {
            if (TryGet(out InteractableBase interactable))
            {
                if(interactable.CanBeInteractedWith) interactable.Interact();
                else IncorrectInteraction();
            }
            return false;
        }
        /// <summary>
        /// Handles raycasts and checks for pickupable items
        /// </summary>
        private bool TryGet<TInteractable>(out TInteractable foundInteractable)
        where TInteractable : InteractableBase
        {
            foundInteractable = null;
         
            if(currentPlayerTarget == null) return false;
            
            if (currentPlayerTarget.TryGetComponent(out InteractableBase interactable))
            {
                if (interactable is not TInteractable searchedInteractable) return false;
                
                foundInteractable = searchedInteractable;
                return true;

            }
            return false;
        }
        
        

        /// <summary>
        /// Handles pickup logic
        /// </summary>
        private void Pickup(InteractableItem newItem)
        {
            // Cache item
            currentItem = newItem;
            
            // Call pickup
            currentItem?.Pickup(pickupTransform);
            
            // If item was attached to something, detach it
            if(currentItem is AttachableItem attachableItem)
                attachableItem.Detach();
        }

        /// <summary>
        /// Handles drop logic
        /// </summary>
        private void Drop()
        {
            (Vector3, Vector3) dropData = Utils.GetRandomGroundPositionAndRotation(transform, 1.5f, 2);
            currentItem?.Drop(dropData.Item1, dropData.Item2);
            ClearCurrentItem();
        }

        
        /// <summary>
        /// Clears out currently held item
        /// </summary>
        private void ClearCurrentItem() => currentItem = null;
        
        /// <summary>
        /// Displays information for player on incorrect interaction
        /// </summary>
        private void IncorrectInteraction()
        {
            TextManager.Instance?.ShowText(incorrectPickupInteractionText);
        }





        private void OnPlayerInteractionButton(InputAction.CallbackContext context)
        {
            if (TryUseItem())
            {
                ClearCurrentItem();
                return;
            }
            if(TryPickupItem()) return;
            TryInteractWithObject();
        }
        
        private void OnPlayerRMB(InputAction.CallbackContext context)
        {
            Drop();
        }
        
        public void DetachListeners()
        {            
            PlayerRaycastController.OnPlayerTargetChanged -= OnPlayerTargetChanged;
            if (InputManager.Instance)
            {
                InputManager.Instance.PlayerControls.Attack.performed -= OnPlayerInteractionButton;
                InputManager.Instance.PlayerControls.Interact.performed -= OnPlayerInteractionButton;
                InputManager.Instance.PlayerControls.Aim.performed -= OnPlayerRMB;
            }
        }
        
        

    }
}
