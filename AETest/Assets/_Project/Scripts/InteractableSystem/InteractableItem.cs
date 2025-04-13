using System;
using AE.Interfaces;
using AE.Managers;
using NaughtyAttributes;
using UnityEngine;

namespace AE.InteractableSystem
{
    
    /// <summary>
    /// Represents an item, that can be picked up and used on interactable objects
    /// </summary>
    public class InteractableItem : InteractableBase, IPickupAble
    {
        [Foldout("References")] [SerializeField] private AudioSource audioSource;
        [Foldout("References")] [SerializeField] private AudioClip pickupSound;
        [Foldout("References")] [SerializeField] private AudioClip dropSound;
        [Foldout("References")] [SerializeField] private AudioClip useSound;
        [Header("Pickup Settings")] 
        [SerializeField] private Vector3 heldPosition;
        [SerializeField] private Vector3 heldRotation;
        [SerializeField] private Vector3 heldScale = Vector3.one;
        
        public event Action OnPickedUp;
        public override bool CanBeInteractedWith { get; protected set; } = true;

        public bool CanBePickedUp { get; protected set; } = true;
        protected override void Awake()
        {
            base.Awake();
        }


        protected override void OnInteraction()
        {
            DisplayDefaultInteraction();
        }

        public void Pickup(Transform pickupTransform)
        {
            HideOutline();   
            OnPickedUp?.Invoke();
            SetLayerForChildrenObjects(Utils.HoldableLayerMask);
            
            transform.SetParent(pickupTransform);
            transform.localScale = heldScale;
            Quaternion heldRotationQuaternion = Quaternion.Euler(heldRotation);
            transform.SetLocalPositionAndRotation(heldPosition, heldRotationQuaternion);
            
            if(pickupSound)
                AudioManager.Instance?.PlaySFXAtSource(pickupSound, audioSource);

            OnPickupText();
        }
        
        public void Drop(Vector3 dropPosition, Vector3 dropRotation)
        {
            SetLayerForChildrenObjects(Utils.PickupableLayerMask);
            transform.SetParent(null);
            transform.localPosition = dropPosition;
            transform.eulerAngles = dropRotation;
            
            if(dropSound)
                AudioManager.Instance?.PlaySFXAtSource(dropSound, audioSource);
        }

        public void PlayUseSFX()
        {
            if(useSound)
                AudioManager.Instance?.PlaySFXAtSource(useSound, audioSource);
        }
        public void ResetLayer() => SetLayerForChildrenObjects(Utils.PickupableLayerMask);
        public void SetIgnoreRaycastLayer() => SetLayerForChildrenObjects(Utils.IgnoreRaycastMask);

        private void OnPickupText() => DisplayDefaultInteraction();
        private void SetLayerForChildrenObjects(int layer)
        {
            foreach (Transform t in gameObject.GetComponentsInChildren<Transform>(true))
                t.gameObject.layer = layer;
        }
    }
}
