using System;
using AE.Core.Generics;
using NaughtyAttributes;
using UnityEngine;

namespace AE
{
    public class PlayerRaycastController : InGameMonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private float interactionRange = 10;
        [SerializeField] private LayerMask targetLayer;
        [SerializeField] private Transform raycastFirePoint;
        
        [Foldout("Debug"), ReadOnly] [SerializeField] private GameObject currentTarget;

        public static Action<GameObject> OnPlayerTargetChanged;
        void Update()
        {
            GetObjectInFront();
        }

        /// <summary>
        /// Handles raycasts and getting an object in front of the player
        /// </summary>
        private void GetObjectInFront()
        {
            RaycastHit[] hits = new RaycastHit[1];
            Physics.RaycastNonAlloc(raycastFirePoint.position, raycastFirePoint.forward, hits, interactionRange, targetLayer);

            // Get looked at gameobejct, can be null
            GameObject newTarget = hits[0].transform?.gameObject;

            // If the same target, return
            if(newTarget == currentTarget) return;
            
            // Set new target
            currentTarget = newTarget;
            
            //Invoke event
            OnPlayerTargetChanged?.Invoke(currentTarget);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(raycastFirePoint.position, raycastFirePoint.position + raycastFirePoint.forward * interactionRange);
        }
    }
}
