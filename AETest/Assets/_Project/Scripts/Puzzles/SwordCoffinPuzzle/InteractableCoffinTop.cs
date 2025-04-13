using System;
using System.Collections.Generic;
using System.Threading;
using AE.InteractableSystem;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace AE.Puzzles.SwordCoffinPuzzle
{
    public class InteractableCoffinTop : InteractableBase
    {
        [SerializeField] private GameObject coffinSkeleton;
        [SerializeField] List<Transform> waypoints = new List<Transform>();
        [SerializeField] float moveSpeed = 0.5f;
        public override bool CanBeInteractedWith { get; protected set; } = true;
        CancellationTokenSource cts = new CancellationTokenSource();
        protected override void OnInteraction()
        {
            StartMoveAnimation().Forget();
        }
        
        private async UniTaskVoid StartMoveAnimation()
        {
            gameObject.layer = LayerMask.NameToLayer("Environment");
            
            Sequence sequence = DOTween.Sequence();
            
            for (int i = 0; i < waypoints.Count; i++)
            {
                sequence.Append(transform.DOMove(waypoints[i].position, moveSpeed));
                sequence.Join(transform.DORotate(waypoints[i].rotation.eulerAngles, moveSpeed)).WaitForCompletion();
            }

            try
            {
                await sequence.AsyncWaitForCompletion().AsUniTask().AttachExternalCancellation(cts.Token);
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

            cts?.Cancel();
            cts?.Dispose();
            
            DOTween.Kill(this);
        }
    }
}
