using System;
using AE._Project.Scripts.Core.Generics;
using AE._Project.Scripts.Interfaces;
using AE._Project.Scripts.Managers;
using AE._Project.Scripts.Puzzles.TorchSkullPuzzle;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace AE._Project.Scripts.Puzzles.SwordCoffinPuzzle
{
    public class MovablePillar : InGameMonoBehaviour, IAttachListeners
    {
        [FormerlySerializedAs("audioSource")] [SerializeField] private AudioSource _audioSource;
        [FormerlySerializedAs("sword")] [SerializeField] private InteractableSword _sword;
        [FormerlySerializedAs("endPosition")] [SerializeField] private Vector3 _endPosition;
        [FormerlySerializedAs("duration")] [SerializeField] private float _duration;


        public void AttachListeners()
        {
            if (TsPuzzleManager.Instance)
            {
                TsPuzzleManager.Instance.OnPuzzleCompleted += MovePillar;
            }
        }

        public void DetachListeners()
        {
            if (TsPuzzleManager.Instance)
            {
                TsPuzzleManager.Instance.OnPuzzleCompleted -= MovePillar;
            }
        }


        private void MovePillar()
        {
            AsyncStartMove().Forget();
        }

        private async UniTaskVoid AsyncStartMove()
        {
            var sequence = DOTween.Sequence();

            AudioManager.Instance.PlaySFXAtSourceOnce(_audioSource.clip, _audioSource);
            sequence.Append(transform.DOMove(_endPosition, _duration));

            try
            {
                await sequence.ToUniTask(cancellationToken: OnDestoryCancellationToken);
                _sword.gameObject.layer = LayerMask.NameToLayer("Pickupable");
            }
            catch (OperationCanceledException)
            {
                sequence.Kill();
            }
        }

        protected override void CleanUp()
        {
            base.CleanUp();
            DOTween.Kill(this);
        }
    }
}