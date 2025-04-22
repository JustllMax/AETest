using AE._Project.Scripts.Core.Generics;
using AE._Project.Scripts.InputManagement;
using AE._Project.Scripts.InteractableSystem;
using AE._Project.Scripts.Interfaces;
using AE._Project.Scripts.UI.TextManagement;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace AE._Project.Scripts.Player
{
    /// <summary>
    ///     Class that manages logic with interactable objects
    /// </summary>
    public class PlayerInteractionController : InGameMonoBehaviour, IAttachListeners
    {
        [FormerlySerializedAs("incorrectInteractionText")] [SerializeField] private string _incorrectInteractionText = "Cannot interact with this";
        [FormerlySerializedAs("incorrectPickupText")] [SerializeField] private string _incorrectPickupText = "My hands are full";
        [FormerlySerializedAs("incorrectUseItemText")] [SerializeField] private string _incorrectUseItemText = "Cannot use it like this";
        [FormerlySerializedAs("pickupTransform")] [SerializeField] private Transform _pickupTransform;

        [FormerlySerializedAs("currentItem")] [Foldout("Debug")] [ReadOnly] [SerializeField]
        private InteractableItem _currentItem;

        [FormerlySerializedAs("currentPlayerTarget")] [Foldout("Debug")] [ReadOnly] [SerializeField]
        private InteractableBase _currentPlayerTarget;

        public InteractableItem Item => _currentItem;
        private bool HasItem => _currentItem != null;

        public void AttachListeners()
        {
            PlayerRaycastController.OnPlayerTargetChanged += OnPlayerTargetChanged;
            InputManager.Instance.PlayerControls.Attack.performed += OnPlayerInteractionButton;
            InputManager.Instance.PlayerControls.Interact.performed += OnPlayerInteractionButton;
            InputManager.Instance.PlayerControls.Aim.performed += OnPlayerRMB;
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

        private void OnPlayerTargetChanged(GameObject newTarget)
        {
            _currentPlayerTarget?.HideOutline();

            _currentPlayerTarget = null;
            if (newTarget == null)
            {
                return;
            }

            // Try get interactable
            if (!newTarget.TryGetComponent(out InteractableBase interactable))
            {
                return;
            }

            _currentPlayerTarget = interactable;
            if (interactable.CanBeInteractedWith)
            {
                interactable.ShowOutline();
            }
        }

        /// <summary>
        ///     Handles interaction with items
        /// </summary>
        private bool TryUseItem()
        {
            if (TryGet(out InteractableWithItemBase usableItem))
            {
                Debug.Log($"found a usable item {usableItem.name}");

                // If item cannot be used, return
                if (!usableItem.CanUseItem(Item))
                {
                    // Cannot use currently held item on the object
                    IncorrectUsage();
                    return false;
                }

                // Use item on the interactable object
                if (usableItem.UseItem(Item))
                    // Item has been correctly used
                    // Remove current item
                {
                    ClearCurrentItem();
                }


                //Item has not been correctly used
                return true;
            }

            Debug.Log("Not found a usable item");
            return false;
        }

        /// <summary>
        ///     Handles interaction with items
        /// </summary>
        private bool TryPickupItem()
        {
            if (HasItem)
            {
                IncorrectPickup();
                return false;
            }

            // Cannot get item, return
            if (!TryGet(out InteractableItem newItem))
            {
                return false;
            }

            // If item can be picked up, pick it up
            if (newItem.CanBePickedUp)
            {
                Pickup(newItem);
            }
            // If item cannot be picked up, try interacting with it
            else
            {
                InteractWithObject(newItem);
            }

            //NOTE: Return true, as there is no need for further interaction tries
            return true;
        }

        /// <summary>
        ///     Handles interaction with objects
        /// </summary>
        private void TryInteractWithObject()
        {
            if (TryGet(out InteractableBase interactable) is false)
            {
                return;
            }

            InteractWithObject(interactable);
        }

        private void InteractWithObject(InteractableBase newInteractable)
        {
            if (newInteractable.CanBeInteractedWith)
            {
                newInteractable.Interact();
            }
            else
            {
                IncorrectInteraction();
            }
        }

        /// <summary>
        ///     Handles raycasts and checks for pickupable items
        /// </summary>
        private bool TryGet<TInteractable>(out TInteractable foundInteractable)
            where TInteractable : InteractableBase
        {
            foundInteractable = null;

            if (_currentPlayerTarget is null)
            {
                return false;
            }

            if (!_currentPlayerTarget.TryGetComponent(out InteractableBase interactable)) return false;
            if (interactable is not TInteractable searchedInteractable)
            {
                return false;
            }

            foundInteractable = searchedInteractable;
            return true;

        }
        
        /// <summary>
        ///     Handles pickup logic
        /// </summary>
        private void Pickup(InteractableItem newItem)
        {
            // Cache item
            _currentItem = newItem;

            // Call pickup
            _currentItem?.Pickup(_pickupTransform);
        }

        /// <summary>
        ///     Handles drop logic
        /// </summary>
        private void Drop()
        {
            (Vector3, Vector3) dropData = Utils.Utils.GetRandomGroundPositionAndRotation(transform, 1.5f, 2);
            _currentItem?.Drop(dropData.Item1, dropData.Item2);
            ClearCurrentItem();
        }


        /// <summary>
        ///     Clears out currently held item
        /// </summary>
        private void ClearCurrentItem()
        {
            _currentItem = null;
        }
        
        private void OnPlayerInteractionButton(InputAction.CallbackContext context)
        {
            if (TryUseItem())
            {
                return;
            }

            // No interactable object to use item on
            if (HasItem)
            {
                return;
            }

            // Try picking up object
            if (TryPickupItem())
            {
                return;
            }

            // Basic interaction with the object
            TryInteractWithObject();
        }

        private void OnPlayerRMB(InputAction.CallbackContext context)
        {
            Drop();
        }

        /// <summary>
        ///     Displays information for player on incorrect interaction
        /// </summary>
        private void IncorrectInteraction()
        {
            TextManager.Instance?.ShowText(_incorrectInteractionText);
        }

        /// <summary>
        ///     Displays information for player on incorrect pickup try
        /// </summary>
        private void IncorrectPickup() => TextManager.Instance?.ShowText(_incorrectPickupText);
        

        /// <summary>
        ///     Displays information for player on incorrect item usage
        /// </summary>
        private void IncorrectUsage()
        {
            TextManager.Instance?.ShowText(_incorrectUseItemText);
        }
    }
}