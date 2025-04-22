using UnityEngine;

namespace AE._Project.Scripts.Interfaces
{
    /// <summary>
    ///     Marks object for player to be able to pick up
    /// </summary>
    public interface IPickupAble
    {
        public bool CanBePickedUp { get; }

        /// <summary>
        ///     Called when picked up
        /// </summary>
        public void Pickup(Transform pickupTransform);

        /// <summary>
        ///     Called when dropped
        /// </summary>
        public void Drop(Vector3 dropPosition, Vector3 dropRotation);
    }
}