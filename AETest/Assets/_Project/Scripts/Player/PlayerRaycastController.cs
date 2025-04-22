using System;
using AE._Project.Scripts.Core.Generics;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace AE._Project.Scripts.Player
{
    public class PlayerRaycastController : InGameMonoBehaviour
    {
        public static Action<GameObject> OnPlayerTargetChanged;

        [FormerlySerializedAs("interactionRange")] [Header("Config")] [SerializeField] private float _interactionRange = 10;

        [FormerlySerializedAs("targetLayer")] [SerializeField] private LayerMask _targetLayer;
        [FormerlySerializedAs("raycastFirePoint")] [SerializeField] private Transform _raycastFirePoint;

        [FormerlySerializedAs("currentTarget")] [Foldout("Debug")] [ReadOnly] [SerializeField]
        private GameObject _currentTarget;

        private void Update()
        {
            GetObjectInFront();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_raycastFirePoint.position,
                _raycastFirePoint.position + _raycastFirePoint.forward * _interactionRange);
        }

        /// <summary>
        ///     Handles raycasts and getting an object in front of the player
        /// </summary>
        private void GetObjectInFront()
        {
            var hits = new RaycastHit[1];
            Physics.RaycastNonAlloc(_raycastFirePoint.position, _raycastFirePoint.forward, hits, _interactionRange,
                _targetLayer);

            // Get looked at gameobejct, can be null
            var newTarget = hits[0].transform?.gameObject;

            // If the same target, return
            if (newTarget == _currentTarget)
            {
                return;
            }

            // Set new target
            _currentTarget = newTarget;

            //Invoke event
            OnPlayerTargetChanged?.Invoke(_currentTarget);
        }
    }
}