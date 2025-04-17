using System;
using System.Threading;
using AE.InteractableSystem;
using AE.Managers;
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

        [SerializeField] private AudioClip choirClip;
        
        [Header("Attach Animation Settings")]
        [SerializeField] private float attachDuration = 1f;
        [SerializeField] private Transform attachStartTransform;
        [SerializeField] private Transform attachEndTransform;
        protected override bool RequiresSpecialItemType { get; } = true;

        int animHash = Animator.StringToHash("Anim_OpenEyes");
        

        Tween tween;
        

        protected override void OnItemUsed(InteractableSword item)
        {
            base.OnItemUsed(item);

            gameObject.layer = Utils.IgnoreRaycastMask;
            
            // Start async attach skull animation
            try
            {
                AttachSword(_OnDestoryCancellationToken).Forget();
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Skeleton coffin outside catch");
            }
        }
        
        
        private async UniTaskVoid AttachSword(CancellationToken token)
        {
            // Set base positon
            Item.transform.SetParent(transform);
            Item.transform.eulerAngles = attachStartTransform.eulerAngles;
            Item.transform.position = attachStartTransform.position;
            Item.transform.localScale = attachStartTransform.localScale;
            
            AudioManager.Instance?.PlayMusic(choirClip);
            
            //Invoke puzzle end event
            SCPuzzleManager.Instance?.NotifyPuzzleCompleted();
            
            // Start animation
            tween = Item.transform.DOMove(attachEndTransform.position, attachDuration);
            // Await until animation has been completed
            try
            {
                await tween.ToUniTask(cancellationToken:token);
                
                DisplayCorrectInteraction();
                
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
            tween?.Kill();
        }
    }
}
