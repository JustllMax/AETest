using System;
using System.Threading;
using AE.Interfaces;
using AE.Player;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace AE
{
    public class InteractableTorch : InteractableWithItem<InteractableSkull>
    {
        protected override bool RequiresSpecialItemType { get; } = true;
        public override bool CanBeInteractedWith { get; } = true;
        [SerializeField] LightComponent _lightComponent;
        [SerializeField] private InteractableSkull.SkullType requiredSkullType;

        [Header("Attach Animation Settings")]
        [SerializeField] private float attachDuration = 1f;
        [SerializeField] private Transform attachSkullStartTransform;
        [SerializeField] private Transform attachSkullEndTransform;

        public Vector3 AttachSkullStartPosition => attachSkullStartTransform.position;
        public Vector3 AttachSkullEndPosition => attachSkullEndTransform.position;

        public static event Action<InteractableSkull.SkullType, bool> OnSkullTypeChanged;

        private Tween _attachSkullTween;
        private CancellationTokenSource cts = new CancellationTokenSource();
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

        private bool IsCorrectSkullAttached()
        {
            return Item.Type == requiredSkullType;
        }
        
        private async UniTaskVoid AttachSkull(CancellationToken token)
        {
            Item.transform.SetParent(transform);
            _attachSkullTween = Item.transform.DOLocalMove(AttachSkullEndPosition, attachDuration);

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
            _lightComponent.SetUseColorTemperature(false);
            _lightComponent.SetCurrentColor(Item.StartPulseColor);
            _lightComponent.PulseData.pulseColor = Item.EndPulseColor;
        }
        
        
        protected override void OnDetachItem()
        {   
            base.OnDetachItem();
            OnSkullTypeChanged?.Invoke(requiredSkullType, false);
            _lightComponent.SetUseColorTemperature(true);
            _lightComponent.ResetColorToDefault();
        }
       
    }
}
