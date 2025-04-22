using System;
using System.Threading;
using AE._Project.Scripts.InteractableSystem;
using AE._Project.Scripts.Managers;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace AE._Project.Scripts.Puzzles.SwordCoffinPuzzle
{
    public class InteractableCoffinSkeleton : InteractableWithItem<InteractableSword>
    {
        [FormerlySerializedAs("animator")] [Foldout("References")] [SerializeField]
        private Animator _animator;

        [FormerlySerializedAs("choirClip")] [SerializeField]
        private AudioClip _choirClip;

        [FormerlySerializedAs("attachDuration")] [Header("Attach Animation Settings")] [SerializeField]
        private float _attachDuration = 1f;

        [FormerlySerializedAs("attachStartTransform")] [SerializeField]
        private Transform _attachStartTransform;

        [FormerlySerializedAs("attachEndTransform")] [SerializeField]
        private Transform _attachEndTransform;

        private readonly int _animHash = Animator.StringToHash("Anim_OpenEyes");


        private Tween _tween;
        protected override bool RequiresSpecialItemType { get; } = true;


        protected override void OnItemUsed(InteractableSword item)
        {
            base.OnItemUsed(item);

            gameObject.layer = Utils.Utils.IgnoreRaycastMask;

            // Start async attach skull animation
            try
            {
                AttachSword(OnDestoryCancellationToken).Forget();
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
            Item.transform.eulerAngles = _attachStartTransform.eulerAngles;
            Item.transform.position = _attachStartTransform.position;
            Item.transform.localScale = _attachStartTransform.localScale;

            AudioManager.Instance?.PlayMusic(_choirClip);

            //Invoke puzzle end event
            ScPuzzleManager.Instance?.NotifyPuzzleCompleted();

            // Start animation
            _tween = Item.transform.DOMove(_attachEndTransform.position, _attachDuration);
            // Await until animation has been completed
            try
            {
                await _tween.ToUniTask(cancellationToken: token);

                DisplayCorrectInteraction();

                _animator.CrossFade(_animHash, 0f);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Torch inside catch");

                _tween?.Kill();
            }
        }

        protected override void CleanUp()
        {
            base.CleanUp();
            _tween?.Kill();
        }
    }
}