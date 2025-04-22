using System;
using System.Collections.Generic;
using AE._Project.Scripts.InteractableSystem;
using AE._Project.Scripts.Managers;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace AE._Project.Scripts.Puzzles.SwordCoffinPuzzle
{
    public class InteractableCoffinTop : InteractableBase
    {
        [FormerlySerializedAs("source")] [SerializeField] private AudioSource _source;
        [FormerlySerializedAs("clip")] [SerializeField] private AudioClip _clip;
        [FormerlySerializedAs("coffinSkeleton")] [SerializeField] private GameObject _coffinSkeleton;
        [FormerlySerializedAs("waypoints")] [SerializeField] private List<Transform> _waypoints = new();
        [FormerlySerializedAs("moveSpeed")] [SerializeField] private float _moveSpeed = 0.5f;
        public override bool CanBeInteractedWith { get; protected set; } = true;

        protected override void OnInteraction()
        {
            StartMoveAnimation().Forget();
        }

        private async UniTaskVoid StartMoveAnimation()
        {
            gameObject.layer = LayerMask.NameToLayer("Environment");

            if (_clip)
            {
                AudioManager.Instance?.PlaySfxAtSource(_clip, _source);
            }

            var sequence = DOTween.Sequence();

            for (var i = 0; i < _waypoints.Count; i++)
            {
                sequence.Append(transform.DOMove(_waypoints[i].position, _moveSpeed));
                sequence.Join(transform.DORotate(_waypoints[i].rotation.eulerAngles, _moveSpeed)).WaitForCompletion();
            }

            try
            {
                await sequence.ToUniTask(cancellationToken: OnDestoryCancellationToken);
                _coffinSkeleton.layer = LayerMask.NameToLayer("Interactable");
            }
            catch (OperationCanceledException)
            {
                sequence.Kill();
            }
        }

        protected override void CleanUp()
        {
            base.CleanUp();

            DOTween.Kill(this);
        }
    }
}