using System;
using System.Threading;
using AE.Core.Generics;
using AE.Interfaces;
using AE.Managers;
using AE.Puzzles.TorchSkullPuzzle;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace AE.Puzzles.SwordCoffinPuzzle
{
    public class MovablePillar : InGameMonoBehaviour,IAttachListeners
    {
        [SerializeField] AudioSource audioSource;
        [SerializeField] InteractableSword sword;
        [SerializeField] Vector3 endPosition;
        [SerializeField] float duration;
        
        CancellationTokenSource cts = new CancellationTokenSource();

        public void AttachListeners()
        {
            if (TSPuzzleManager.Instance)
            {
                TSPuzzleManager.Instance.OnPuzzleCompleted += MovePillar;
            }
        }


        private void MovePillar()
        {
            AsyncStartMove().Forget();
        }

        private async UniTaskVoid AsyncStartMove()
        {
            
            Sequence sequence = DOTween.Sequence();
            
            AudioManager.Instance.PlaySFXAtSourceOnce(audioSource.clip, audioSource);
            sequence.Append(transform.DOMove(endPosition, duration));

            try
            {
                await sequence.AsyncWaitForCompletion().AsUniTask().AttachExternalCancellation(cts.Token);
                sword.gameObject.layer = LayerMask.NameToLayer("Pickupable");

            }
            catch (OperationCanceledException)
            {
                sequence.Kill();
            }
            

        }
        public void DetachListeners()
        {
            if (TSPuzzleManager.Instance)
            {
                TSPuzzleManager.Instance.OnPuzzleCompleted -= MovePillar;
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
