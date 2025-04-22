using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace AE._Project.Scripts.Puzzles.SwordCoffinPuzzle
{
    public class ScDarkScreen : OnScPuzzleComplete
    {
        [FormerlySerializedAs("image")] [SerializeField]
        private Image _image;

        [FormerlySerializedAs("delayDuration")] [SerializeField]
        private float _delayDuration = 1.0f;

        [FormerlySerializedAs("darkDuration")] [SerializeField]
        private float _darkDuration = 2f;

        private Sequence _sequence;


        public static event Action OnDarkScreenComplete;

        protected override void OnComplete()
        {
            base.OnComplete();
            StartAnimation(OnDestoryCancellationToken).Forget();
        }

        private async UniTaskVoid StartAnimation(CancellationToken token)
        {
            _sequence = DOTween.Sequence();

            _sequence.AppendInterval(_delayDuration);
            _sequence.Append(_image.DOFade(1f, _darkDuration));
            _sequence.AppendInterval(1f);

            try
            {
                await _sequence.ToUniTask(cancellationToken: token);
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
            if (_sequence != null)
            {
                DOTween.Kill(_sequence);
            }
        }
    }
}