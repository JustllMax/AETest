using System;
using System.Collections.Generic;
using System.Threading;
using AE.InteractableSystem;
using AE.Managers;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace AE.Puzzles.SwordCoffinPuzzle
{
    public class InteractableCoffinTop : InteractableBase
    {
        [SerializeField] AudioSource source;
        [SerializeField] AudioClip clip;
        [SerializeField] private GameObject coffinSkeleton;
        [SerializeField] List<Transform> waypoints = new List<Transform>();
        [SerializeField] float moveSpeed = 0.5f;
        public override bool CanBeInteractedWith { get; protected set; } = true;

        protected override void OnInteraction()
        {
            StartMoveAnimation().Forget();
        }
        
        private async UniTaskVoid StartMoveAnimation()
        {
            gameObject.layer = LayerMask.NameToLayer("Environment");
            
            if(clip)
                AudioManager.Instance?.PlaySFXAtSource(clip, source);
    
            Sequence sequence = DOTween.Sequence();
            
            for (int i = 0; i < waypoints.Count; i++)
            {
                sequence.Append(transform.DOMove(waypoints[i].position, moveSpeed));
                sequence.Join(transform.DORotate(waypoints[i].rotation.eulerAngles, moveSpeed)).WaitForCompletion();
            }

            try
            {
                await sequence.ToUniTask(cancellationToken: _OnDestoryCancellationToken);
                coffinSkeleton.layer = LayerMask.NameToLayer("Interactable");
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
