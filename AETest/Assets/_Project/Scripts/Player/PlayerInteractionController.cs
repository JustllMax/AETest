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
        [Foldout("Debug"), ReadOnly] [SerializeField] private GameObject currentPlayerTarget = null;
        
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
            currentPlayerTarget = newTarget;
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
            (Vector3, Vector3) dropData = GetRandomGroundPositionAndRotation(transform, 1.5f, 2);
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
            TextManager.Instance.ShowText(incorrectPickupInteractionText);
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
        
        
        /// <summary>
        /// Creates a donut shaped ring, then sends raycast 
        /// </summary>
        /// <param name="center">Center of the ring</param>
        /// <param name="minRadius">Inner radius</param>
        /// <param name="maxRadius">Outer radius</param>
        /// <returns>Hit position and rotation towards center</returns>
        (Vector3 position, Vector3 rotation) GetRandomGroundPositionAndRotation(Transform center, float minRadius, float maxRadius)
        {
            LayerMask groundLayer = LayerMask.GetMask("Environment");
            
            const int maxAttempts = 5;
            
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {

                // Pick a random point in a ring (between minRadius and maxRadius)
                float distance = UnityEngine.Random.Range(minRadius, maxRadius);

                Vector2 randomCircle = UnityEngine.Random.insideUnitCircle.normalized * distance;

                // Start position 2 units above
                Vector3 samplePos = center.position + new Vector3(randomCircle.x, 2f, randomCircle.y);

                // Send raycast downwards, trying to hit ground
                if (Physics.Raycast(samplePos, Vector3.down, out RaycastHit hit, 10f, groundLayer))
                {
                    Vector3 direction = center.position - hit.point;
                    direction.y = 0;

                    Quaternion lookRotation = Quaternion.LookRotation(direction.normalized);
                    Vector3 eulerAngles = lookRotation.eulerAngles;
                    return (hit.point, eulerAngles);
                }
            }
            
            // If none of the attempts hit, return fallback
            return (center.position, Vector3.zero);
        }
    }
}
