using AE.Core.Generics;
using AE.Interfaces;
using AE.Puzzles.TorchSkullPuzzle.Objects.InteractableItems;
using AE.Puzzles.TorchSkullPuzzle.Objects.InteractableObjects;
using NaughtyAttributes;
using UnityEngine;

namespace AE.Puzzles.TorchSkullPuzzle.Objects
{
    public class HintGems : InGameMonoBehaviour, IAttachListeners, IWithSetUp
    {
        [SerializeField] private InteractableSkull.SkullType hintSkullType;
        [SerializeField] private GameObject notCorrectLight;
        [SerializeField] private GameObject correctLight;

        [Foldout("Debug")] [SerializeField] private bool isCorrect;

        public void SetUp()
        {
            SwapLights();
        }

        public void AttachListeners()
        {
            InteractableTorch.OnSkullTypeChanged += OnSkullAttached;
        }

        private void OnSkullAttached(InteractableSkull.SkullType skullType, bool correct)
        {
            if(skullType != hintSkullType) return;
            
            // Copy current state
            bool correctCopy = isCorrect;
            
            isCorrect = correct;
            
            
            // Check for state change
            if(isCorrect != correctCopy)
                SwapLights();
        }

        private void SwapLights()
        {
            notCorrectLight.SetActive(!isCorrect);
            correctLight.SetActive(isCorrect);
        }

        public void DetachListeners()
        {
            InteractableTorch.OnSkullTypeChanged -= OnSkullAttached;

        }
        
        public void TearDown()
        {
            //Noop
        }
    }
}
