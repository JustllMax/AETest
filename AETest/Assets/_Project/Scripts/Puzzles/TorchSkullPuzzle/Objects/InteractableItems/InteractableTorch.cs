using System;
using System.Threading;
using AE.InteractableSystem;
using AE.Interfaces;
using AE.Puzzles.TorchSkullPuzzle.Objects.InteractableItems;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace AE.Puzzles.TorchSkullPuzzle.Objects.InteractableObjects
{
    public class InteractableTorch : InteractableWithItem<InteractableSkull>, IAttachListeners, IHasSetBaseState
    {
        protected override bool RequiresSpecialItemType { get; } = true;
        [SerializeField] LightComponent _lightComponent;
        [SerializeField] private InteractableSkull.SkullType requiredSkullType;

        [Header("Attach Animation Settings")]
        [SerializeField] private float attachDuration = 1f;
        [SerializeField] private Transform attachSkullStartTransform;
        [SerializeField] private Transform attachSkullEndTransform;

        public Vector3 AttachSkullRotation => attachSkullStartTransform.eulerAngles;
        public Vector3 AttachSkullStartPosition => attachSkullStartTransform.position;
        public Vector3 AttachSkullEndPosition => attachSkullEndTransform.position;
        
        private bool IsCorrectSkullAttached() => Item.Type == requiredSkullType;

        public static event Action<InteractableSkull.SkullType, bool> OnSkullTypeChanged;

        private Tween _attachSkullTween;
        private CancellationTokenSource cts = new CancellationTokenSource();
        
        public virtual void AttachListeners()
        {
            TSPuzzleManager.Instance.OnPuzzleCompleted += OnPuzzleComplete;
        }
        
        
        protected override void OnItemUsed(InteractableSkull item)
        {
            base.OnItemUsed(item);
            
            if(IsCorrectSkullAttached()) OnSkullTypeChanged?.Invoke(requiredSkullType, true);
            try
            {
                AttachSkull(cts.Token).Forget();
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Torch outside catch");
            }
        }
        
        
        private async UniTaskVoid AttachSkull(CancellationToken token)
        {
            Item.transform.SetParent(transform);
            Item.transform.eulerAngles = AttachSkullRotation;
            Item.transform.position = AttachSkullStartPosition;
            _attachSkullTween = Item.transform.DOMove(AttachSkullEndPosition, attachDuration);
            
            try
            {
                await _attachSkullTween.AsyncWaitForCompletion().AsUniTask().AttachExternalCancellation(token);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Torch inside catch");

                _attachSkullTween?.Kill();
                _attachSkullTween = null;
                return;
            }
            Item.ResetLayer();
            SetColorPulse();
        }
        
        
        protected override void OnDetachItem()
        {   
            base.OnDetachItem();
            OnSkullTypeChanged?.Invoke(requiredSkullType, false);
            _lightComponent.SetUseColorTemperature(true);
            _lightComponent.ResetColorToDefault();
        }
        

        private void OnPuzzleComplete()
        {
            CanBeInteractedWith = false;
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        }
        public virtual void DetachListeners()
        {
            if(TSPuzzleManager.Instance)
                TSPuzzleManager.Instance.OnPuzzleCompleted += OnPuzzleComplete;
        }

        private void SetColorPulse()
        {
            _lightComponent.SetUseColorTemperature(false);
            _lightComponent.SetCurrentColor(Item.StartPulseColor);
            _lightComponent.PulseData.pulseColor = Item.EndPulseColor;
            _lightComponent.ChangeLightMode(LightMode.Pulsating, true);
        }

        protected override void OnSetBaseItem()
        {
            base.OnSetBaseItem();
            Item.ResetLayer();
            if(IsCorrectSkullAttached()) OnSkullTypeChanged?.Invoke(requiredSkullType, true);
            SetColorPulse();
        }

        public void UseCaller()
        {
            if (gameObject.TryGetComponent(out TorchCaller torchCaller))
            {
                torchCaller.SetBase();
            }
        }
    }
}
