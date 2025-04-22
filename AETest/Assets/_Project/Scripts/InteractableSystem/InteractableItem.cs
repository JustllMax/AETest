using System;
using AE._Project.Scripts.Interfaces;
using AE._Project.Scripts.Managers;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace AE._Project.Scripts.InteractableSystem
{
    /// <summary>
    ///     Represents an item, that can be picked up and used on interactable objects
    /// </summary>
    public class InteractableItem : InteractableBase, IPickupAble
    {
        [FormerlySerializedAs("audioSource")] [Foldout("References")] [SerializeField]
        private AudioSource _audioSource;

        [FormerlySerializedAs("pickupSound")] [Foldout("References")] [SerializeField]
        private AudioClip _pickupSound;

        [FormerlySerializedAs("dropSound")] [Foldout("References")] [SerializeField]
        private AudioClip _dropSound;

        [FormerlySerializedAs("useSound")] [Foldout("References")] [SerializeField]
        private AudioClip _useSound;

        [FormerlySerializedAs("heldPosition")] [Header("Pickup Settings")] [SerializeField]
        private Vector3 _heldPosition;

        [FormerlySerializedAs("heldRotation")] [SerializeField]
        private Vector3 _heldRotation;

        [FormerlySerializedAs("heldScale")] [SerializeField]
        private Vector3 _heldScale = Vector3.one;

        public override bool CanBeInteractedWith { get; protected set; } = true;
        public bool CanBePickedUp { get; protected set; } = true;

        public void Pickup(Transform pickupTransform)
        {
            HideOutline();
            OnPickedUp?.Invoke();
            SetLayerForChildrenObjects(Utils.Utils.HoldableLayerMask);

            transform.SetParent(pickupTransform);
            transform.localScale = _heldScale;
            var heldRotationQuaternion = Quaternion.Euler(_heldRotation);
            transform.SetLocalPositionAndRotation(_heldPosition, heldRotationQuaternion);

            if (_pickupSound)
            {
                AudioManager.Instance?.PlaySfxAtSource(_pickupSound, _audioSource);
            }

            OnPickupText();
        }

        public void Drop(Vector3 dropPosition, Vector3 dropRotation)
        {
            SetLayerForChildrenObjects(Utils.Utils.PickupableLayerMask);
            transform.SetParent(null);
            transform.localPosition = dropPosition;
            transform.eulerAngles = dropRotation;

            if (_dropSound)
            {
                AudioManager.Instance?.PlaySfxAtSource(_dropSound, _audioSource);
            }
        }

        public event Action OnPickedUp;

        protected override void OnInteraction()
        {
            DisplayDefaultInteraction();
        }

        public void PlayUseSfx()
        {
            if (_useSound)
            {
                AudioManager.Instance?.PlaySfxAtSource(_useSound, _audioSource);
            }
        }

        public void ResetLayer()
        {
            SetLayerForChildrenObjects(Utils.Utils.PickupableLayerMask);
        }

        public void SetIgnoreRaycastLayer()
        {
            SetLayerForChildrenObjects(Utils.Utils.IgnoreRaycastMask);
        }

        private void OnPickupText()
        {
            DisplayDefaultInteraction();
        }

        private void SetLayerForChildrenObjects(int layer)
        {
            foreach (var t in gameObject.GetComponentsInChildren<Transform>(true))
                t.gameObject.layer = layer;
        }
    }
}