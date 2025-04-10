using System;
using AE.Core.Generics;
using AE.Interfaces;
using AE.Managers;
using UnityEngine;

namespace AE
{
    [Serializable]
    public class PlayerInteractionController : InGameMonoBehaviour
    {
        [SerializeField] private Transform raycastFirePoint;
        [SerializeField] private string incorrectPickupInteractionText = "Cannot interact with this";
        [SerializeField] private Transform pickupTransform;
        private InteractableItem currentItem;
        public InteractableItem Item => currentItem;


        [SerializeField] private float interactionRange = 10;
        [SerializeField] private LayerMask targetLayer;
        private bool CanPickupItem => currentItem != null;
        
        //TODO: Add input callbacks
        //TODO: Add HighlightManager and move raycast logic there, then get item from that manager and check for type
        
        private void Update()
        {
            if(TryPickupItem()) return;

            TryInteractWithObject();
        }

        /// <summary>
        /// Handles interaction with items
        /// </summary>
        private bool TryPickupItem()
        {

            if (CheckForPickup(out InteractableItem newItem))
            {
                // If item can be picked up, pick it up
                if (CanPickupItem && newItem.CanBePickedUp) Pickup(newItem);
                // If item cannot be picked up, try interacting with it
                else if (newItem.CanBeInteractedWith) newItem.Interact();
                // Show incorrect interaction
                else IncorrectInteraction();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Handles interaction with objects
        /// </summary>
        private bool TryInteractWithObject()
        {
            if (CheckForInteractable(out InteractableBase interactable))
            {
                if(interactable.CanBeInteractedWith) interactable.Interact();
                else IncorrectInteraction();
            }
            return false;
        }
        /// <summary>
        /// Handles raycasts and checks for pickupable items
        /// </summary>
        private bool CheckForPickup(out InteractableItem interactableItem)
        {
            interactableItem = null;
            RaycastHit[] hits = new RaycastHit[1];
            Physics.RaycastNonAlloc(raycastFirePoint.position, raycastFirePoint.forward, hits, interactionRange, targetLayer);
            
            // No hits
            if(!hits[0].collider) return false;
            
            hits[0].collider.TryGetComponent(out interactableItem);
            
            return interactableItem;
        }
        

        /// <summary>
        /// Handles raycasts and checks for interactable objects
        /// </summary>
        private bool CheckForInteractable(out InteractableBase interactable)
        {
            interactable = null;
            RaycastHit[] hits = new RaycastHit[1];
            Physics.RaycastNonAlloc(raycastFirePoint.position, raycastFirePoint.forward, hits, interactionRange, targetLayer);
            
            // No hits
            if(!hits[0].collider) return false;
            
            hits[0].collider.TryGetComponent(out interactable);
            
            return interactable;
        }

        /// <summary>
        /// Handles pickup logic
        /// </summary>
        private void Pickup(InteractableItem newItem)
        {
            //TODO: Possibly add swaping items
            //Drop();
            currentItem = newItem;
            currentItem?.Pickup(pickupTransform);
        }

        /// <summary>
        /// Handles drop logic
        /// </summary>
        private void Drop()
        {
            //TODO: Get position by player and rotation facing player
            currentItem?.Drop(Vector3.zero, Vector3.zero);
        }

        /// <summary>
        /// Displays information for player on incorrect interaction
        /// </summary>
        private void IncorrectInteraction()
        {
            TextManager.Instance.ShowText(incorrectPickupInteractionText);
        }
        
    }
}
