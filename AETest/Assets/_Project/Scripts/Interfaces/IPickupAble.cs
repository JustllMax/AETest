using UnityEngine;

namespace AE.Interfaces
{
    public interface IPickupAble
    {
        public bool CanBePickedUp{get;}
        public void Pickup(Transform pickupTransform);
        
        public void Drop(Vector3 dropPosition, Vector3 dropRotation);
    }
}
