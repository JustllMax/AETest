using System;
using System.Threading;
using AE._Project.Scripts.Components;
using AE._Project.Scripts.InteractableSystem;
using AE._Project.Scripts.Interfaces;
using AE._Project.Scripts.Puzzles.TorchSkullPuzzle.Objects.InteractableItems;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace AE._Project.Scripts.Puzzles.TorchSkullPuzzle.Objects.InteractableObjects
{
    public class InteractableTorch : InteractableWithItem<InteractableSkull>, IAttachListeners, IHasSetBaseState
    {
        [Foldout("References")] [SerializeField]
        private LightComponent _lightComponent;


        [FormerlySerializedAs("requiredSkullType")] [Header("Puzzle settings")] [SerializeField]
        private InteractableSkull.SkullType _requiredSkullType;

        [FormerlySerializedAs("attachDuration")] [Header("Attach Animation Settings")] [SerializeField]
        private float _attachDuration = 1f;

        [FormerlySerializedAs("attachSkullStartTransform")] [SerializeField]
        private Transform _attachSkullStartTransform;

        [FormerlySerializedAs("attachSkullEndTransform")] [SerializeField]
        private Transform _attachSkullEndTransform;

        private Tween _attachSkullTween;

        protected override bool RequiresSpecialItemType { get; } = true;
        private Vector3 AttachSkullRotation => _attachSkullStartTransform.eulerAngles;
        private Vector3 AttachSkullStartPosition => _attachSkullStartTransform.position;
        private Vector3 AttachSkullEndPosition => _attachSkullEndTransform.position;

        private bool IsCorrectSkullAttached => Item.Type == _requiredSkullType;

        public void AttachListeners()
        {
            TsPuzzleManager.Instance.OnPuzzleCompleted += OnPuzzleComplete;
        }


        public void DetachListeners()
        {
            if (TsPuzzleManager.Instance)
            {
                TsPuzzleManager.Instance.OnPuzzleCompleted -= OnPuzzleComplete;
            }
        }

        public void UseCaller()
        {
            if (gameObject.TryGetComponent(out TorchCaller torchCaller))
            {
                torchCaller.SetBase();
            }
        }

        public static event Action<InteractableSkull.SkullType, bool> OnSkullTypeChanged;


        protected override void OnItemUsed(InteractableSkull item)
        {
            base.OnItemUsed(item);

            // Check for skull type
            if (IsCorrectSkullAttached)
            {
                OnSkullTypeChanged?.Invoke(_requiredSkullType, true);
            }

            // Start async attach skull animation
            try
            {
                AttachSkull(OnDestoryCancellationToken).Forget();
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Torch outside catch");
            }
        }


        private async UniTaskVoid AttachSkull(CancellationToken token)
        {
            // Set base positon
            Item.transform.SetParent(transform);
            Item.transform.eulerAngles = AttachSkullRotation;
            Item.transform.position = AttachSkullStartPosition;

            // Start animation
            _attachSkullTween = Item.transform.DOMove(AttachSkullEndPosition, _attachDuration);

            // Await until animation has been completed
            try
            {
                await _attachSkullTween.ToUniTask(cancellationToken: token);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Torch inside catch");

                _attachSkullTween?.Kill();
                return;
            }

            // Reset item layer back to its base layer
            Item.ResetLayer();

            // Start light pulse
            SetColorPulse();
        }


        protected override void OnDetachItem()
        {
            base.OnDetachItem();
            OnSkullTypeChanged?.Invoke(_requiredSkullType, false);
            _lightComponent.SetUseColorTemperature(true);
            _lightComponent.ResetColorToDefault();
        }


        private void OnPuzzleComplete()
        {
            CanBeInteractedWith = false;
            gameObject.layer = Utils.Utils.IgnoreRaycastMask;
        }

        private void SetColorPulse()
        {
            _lightComponent.SetUseColorTemperature(false);
            _lightComponent.SetCurrentColor(Item.StartPulseColor);
            _lightComponent.PulseData._pulseColor = Item.EndPulseColor;
            _lightComponent.ChangeLightMode(LightMode.Pulsating, true);
        }

        protected override void OnSetBaseItem()
        {
            base.OnSetBaseItem();
            Item.ResetLayer();
            if (IsCorrectSkullAttached)
            {
                OnSkullTypeChanged?.Invoke(_requiredSkullType, true);
            }

            SetColorPulse();
        }

        protected override void CleanUp()
        {
            base.CleanUp();
            _attachSkullTween?.Kill();
        }
    }
}