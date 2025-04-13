using System;
using System.Threading;
using AE.InteractableSystem;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace AE.Puzzles.SwordCoffinPuzzle
{
    public class InteractableCoffinSkeleton : InteractableWithItem<InteractableSword>
    {
        [Foldout("References")]
        [SerializeField] Animator animator;
        
        [Header("Attach Animation Settings")]
        [SerializeField] private float attachDuration = 1f;
        [SerializeField] private Transform attachStartTransform;
        [SerializeField] private Transform attachEndTransform;
        protected override bool RequiresSpecialItemType { get; } = true;

        int animHash = Animator.StringToHash("Anim_OpenEyes");

        private CancellationTokenSource cts = new CancellationTokenSource();

        Tween tween;

        protected override void OnItemUsed(InteractableSword item)
        {
            base.OnItemUsed(item);
            
            item.gameObject.layer = LayerMask.NameToLayer("Environment");
            gameObject.layer = LayerMask.NameToLayer("Environment");
            
            // Start async attach skull animation
            try
            {
                AttachSkull(cts.Token).Forget();
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Skeleton coffin outside catch");
            }
        }
        
        
        private async UniTaskVoid AttachSkull(CancellationToken token)
        {
            // Set base positon
            Item.transform.SetParent(transform);
            Item.transform.eulerAngles = attachStartTransform.eulerAngles;
            Item.transform.position = attachStartTransform.position;
            Item.transform.localScale = attachStartTransform.localScale;
            
            // Start animation
            tween = Item.transform.DOMove(attachEndTransform.position, attachDuration);
            // Await until animation has been completed
            try
            {
                await tween.AsyncWaitForCompletion().AsUniTask().AttachExternalCancellation(token);
                
                DisplayCorrectInteraction();
                
                SCPuzzleManager.Instance?.NotifyPuzzleCompleted();
                
                animator.CrossFade(animHash, 0f);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Torch inside catch");

                tween?.Kill();
                return;
            }
        }
        
        protected override void CleanUp()
        {
            base.CleanUp();
            cts?.Cancel();
            cts?.Dispose();
            tween?.Kill();

        }
    }
}
