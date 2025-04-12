using System;
using AE.Interfaces;
using UnityEngine;

namespace AE.InteractableSystem
{
    
    /// <summary>
    /// Represents an item, that can be picked up and used on interactable objects
    /// </summary>
    public class InteractableItem : InteractableBase, IPickupAble
    {
        [Header("Pickup Settings")] 
        [SerializeField] private Vector3 heldPosition;
        [SerializeField] private Vector3 heldRotation;

        private int holdableLayerMask;
        private int objectDefaultLayer;
        public event Action OnPickedUp;
        public override bool CanBeInteractedWith { get; protected set; } = true;

        public bool CanBePickedUp { get; protected set; } = true;
        protected override void Awake()
        {
            base.Awake();
            objectDefaultLayer = gameObject.layer;
            holdableLayerMask = LayerMask.NameToLayer("Holding");
        }


        protected override void OnInteraction()
        {
            DisplayDefaultInteraction();
        }

        public void Pickup(Transform pickupTransform)
        {
            HideOutline();   
            OnPickedUp?.Invoke();
            SetLayerForChildrenObjects(holdableLayerMask);
            transform.SetParent(pickupTransform);
            Quaternion heldRotationQuaternion = Quaternion.Euler(heldRotation);
            transform.SetLocalPositionAndRotation(heldPosition, heldRotationQuaternion);

            OnPickupText();
        }
        
        public void Drop(Vector3 dropPosition, Vector3 dropRotation)
        {
            SetLayerForChildrenObjects(objectDefaultLayer);
            transform.SetParent(null);
            transform.localPosition = dropPosition;
            transform.eulerAngles = dropRotation;
            // PlaySound at source
        }
        public void ResetLayer() => SetLayerForChildrenObjects(objectDefaultLayer);

        private void OnPickupText() => DisplayDefaultInteraction();
        private void SetLayerForChildrenObjects(int layer)
        {
            foreach (Transform t in gameObject.GetComponentsInChildren<Transform>(true))
                t.gameObject.layer = layer;
        }
    }
}
