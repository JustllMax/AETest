using System;
using System.Threading;
using AE.InteractableSystem;
using AE.Interfaces;
using AE.Puzzles.SwordCoffinPuzzle;
using AE.Puzzles.TorchSkullPuzzle.Objects.InteractableItems;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace AE.Puzzles.TorchSkullPuzzle.Objects.InteractableObjects
{
    public class InteractableTorch : InteractableWithItem<InteractableSkull>, IAttachListeners, IHasSetBaseState
    {
     
        [Foldout("References")][SerializeField] LightComponent _lightComponent;
        

        [Header("Puzzle settings")]
        [SerializeField] private InteractableSkull.SkullType requiredSkullType;

        [Header("Attach Animation Settings")]
        [SerializeField] private float attachDuration = 1f;
        [SerializeField] private Transform attachSkullStartTransform;
        [SerializeField] private Transform attachSkullEndTransform;

        private Tween _attachSkullTween;
        
        protected override bool RequiresSpecialItemType { get; } = true;
        private Vector3 AttachSkullRotation => attachSkullStartTransform.eulerAngles;
        private Vector3 AttachSkullStartPosition => attachSkullStartTransform.position;
        private Vector3 AttachSkullEndPosition => attachSkullEndTransform.position;
        
        private bool IsCorrectSkullAttached => Item.Type == requiredSkullType;
        public static event Action<InteractableSkull.SkullType, bool> OnSkullTypeChanged;
        
        public virtual void AttachListeners()
        {
            TSPuzzleManager.Instance.OnPuzzleCompleted += OnPuzzleComplete;
        }
        
        
        protected override void OnItemUsed(InteractableSkull item)
        {
            base.OnItemUsed(item);
            
            // Check for skull type
            if(IsCorrectSkullAttached) OnSkullTypeChanged?.Invoke(requiredSkullType, true);
            
            // Start async attach skull animation
            try
            {
                AttachSkull(_OnDestoryCancellationToken).Forget();
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
            _attachSkullTween = Item.transform.DOMove(AttachSkullEndPosition, attachDuration);
            
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
            OnSkullTypeChanged?.Invoke(requiredSkullType, false);
            _lightComponent.SetUseColorTemperature(true);
            _lightComponent.ResetColorToDefault();
        }
        

        private void OnPuzzleComplete()
        {
            CanBeInteractedWith = false;
            gameObject.layer = Utils.IgnoreRaycastMask;
        }


        public virtual void DetachListeners()
        {
            if(TSPuzzleManager.Instance)
                TSPuzzleManager.Instance.OnPuzzleCompleted -= OnPuzzleComplete;
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
            if(IsCorrectSkullAttached) OnSkullTypeChanged?.Invoke(requiredSkullType, true);
            SetColorPulse();
        }

        public void UseCaller()
        {
            if (gameObject.TryGetComponent(out TorchCaller torchCaller))
            {
                torchCaller.SetBase();
            }
        }

        protected override void CleanUp()
        {
            base.CleanUp();
            _attachSkullTween?.Kill();

        }
    }
}
