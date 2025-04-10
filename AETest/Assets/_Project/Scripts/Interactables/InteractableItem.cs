using System;
using AE.Core.Generics;
using AE.Interfaces;
using UnityEngine;

namespace AE
{
    public class InteractableItem : InteractableBase, IPickupAble
    {
        [Header("Pickup Settings")] 
        [SerializeField] private Vector3 heldPosition;
        [SerializeField] private Vector3 heldRotation;

        private int holdableLayerMask;
        private int objectDefaultLayer;
        public event Action OnPickedUp; 
        public override bool CanBeInteractedWith { get; }

        public bool CanBePickedUp { get; }
        protected virtual void Awake()
        {
            objectDefaultLayer = gameObject.layer;
            holdableLayerMask = LayerMask.NameToLayer("Holding");
        }

        public override void OnInteraction()
        {
            // Used when cannot be picked up
            DefaultInteraction();
        }

        public void Pickup(Transform pickupTransform)
        {
            
            OnPickedUp?.Invoke();
            gameObject.layer = holdableLayerMask;
            transform.SetParent(pickupTransform);
            transform.localPosition = heldPosition;
            transform.eulerAngles = heldRotation;
        }

        public void Drop(Vector3 dropPosition, Vector3 dropRotation)
        {
            gameObject.layer = objectDefaultLayer;
            transform.SetParent(null);
            transform.localPosition = dropPosition;
            transform.eulerAngles = dropRotation;
        }
    }
}
