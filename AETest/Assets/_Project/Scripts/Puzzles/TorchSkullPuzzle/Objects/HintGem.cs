using AE._Project.Scripts.Core.Generics;
using AE._Project.Scripts.Interfaces;
using AE._Project.Scripts.Puzzles.TorchSkullPuzzle.Objects.InteractableItems;
using AE._Project.Scripts.Puzzles.TorchSkullPuzzle.Objects.InteractableObjects;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace AE._Project.Scripts.Puzzles.TorchSkullPuzzle.Objects
{
    public class HintGems : InGameMonoBehaviour, IAttachListeners, IWithSetUp
    {
        [FormerlySerializedAs("hintSkullType")] [SerializeField] private InteractableSkull.SkullType _hintSkullType;
        [FormerlySerializedAs("notCorrectLight")] [SerializeField] private GameObject _notCorrectLight;
        [FormerlySerializedAs("correctLight")] [SerializeField] private GameObject _correctLight;

        [FormerlySerializedAs("isCorrect")] [Foldout("Debug")] [SerializeField] private bool _isCorrect;

        public void AttachListeners()
        {
            InteractableTorch.OnSkullTypeChanged += OnSkullAttached;
            TsPuzzleManager.Instance.OnPuzzleCompleted += SwapLights;
        }

        public void DetachListeners()
        {
            if (TsPuzzleManager.Instance)
            {
                TsPuzzleManager.Instance.OnPuzzleCompleted -= SwapLights;
            }

            InteractableTorch.OnSkullTypeChanged -= OnSkullAttached;
        }

        public void SetUp()
        {
            SwapLights();
        }

        public void TearDown()
        {
            //Noop
        }

        private void OnSkullAttached(InteractableSkull.SkullType skullType, bool correct)
        {
            if (skullType != _hintSkullType)
            {
                return;
            }

            // Copy current state
            var correctCopy = _isCorrect;

            _isCorrect = correct;


            // Check for state change
            if (_isCorrect != correctCopy)
            {
                SwapLights();
            }
        }

        private void SwapLights()
        {
            _notCorrectLight.SetActive(!_isCorrect);
            _correctLight.SetActive(_isCorrect);
        }
    }
}