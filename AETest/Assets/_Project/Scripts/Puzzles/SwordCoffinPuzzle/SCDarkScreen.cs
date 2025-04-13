using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace AE.Puzzles.SwordCoffinPuzzle
{
    public class SCDarkScreen : OnSCPuzzleComplete
    {
        [SerializeField] private Image image;
        [SerializeField] private float delayDuration = 1.0f;
        [SerializeField] private float darkDuration = 2f;
        private Sequence sequence;
        
        CancellationTokenSource cts = new CancellationTokenSource();
        
        public static event Action OnDarkScreenComplete; 
        protected override void OnComplete()
        {
            base.OnComplete();
            StartAnimation(cts.Token).Forget();
        }

        private async UniTaskVoid StartAnimation(CancellationToken token)
        {
            sequence = DOTween.Sequence();
            
            sequence.AppendInterval(delayDuration);
            sequence.Append(image.DOFade(1f, darkDuration));
            sequence.AppendInterval(1f);

            try
            {
                await sequence.AsyncWaitForCompletion().AsUniTask().AttachExternalCancellation(token);
                OnDarkScreenComplete?.Invoke();
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning("Dark Screen animation was cancelled");
            }
        }
        protected override void CleanUp()
        {
            base.CleanUp();
            
            cts?.Cancel();
            cts?.Dispose();
            
            if(sequence != null)
                DOTween.Kill(sequence);
        }
    }
}
