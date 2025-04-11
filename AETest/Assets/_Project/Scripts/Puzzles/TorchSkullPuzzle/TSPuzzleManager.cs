using System;
using System.Collections.Generic;
using AE.Core.Generics;
using AE.Interfaces;
using AE.Puzzles.TorchSkullPuzzle.Objects.InteractableItems;
using AE.Puzzles.TorchSkullPuzzle.Objects.InteractableObjects;
using NaughtyAttributes;
using UnityEngine;

namespace AE.Puzzles.TorchSkullPuzzle
{
    public class TSPuzzleManager : MonoBehaviourSingleton<TSPuzzleManager>, IWithSetUp, IAttachListeners
    {
        
        [Foldout("Debug"), ReadOnly] [SerializeField]
        private List<bool> correctSkullTypes = new();

        public event Action OnPuzzleCompleted;
        public void SetUp()
        {
            int skullTypesAmount = Enum.GetValues(typeof(InteractableSkull.SkullType)).Length;
            
            for (int i = 0; i <skullTypesAmount;  i++)
            {
                correctSkullTypes.Add(false);
            }
        }
        
        public void AttachListeners()
        {
            InteractableTorch.OnSkullTypeChanged += ChangeSkullTypeValues;

        }
        
        private void ChangeSkullTypeValues(InteractableSkull.SkullType newValue, bool isCorrect)
        {
            correctSkullTypes[(int)newValue] = isCorrect;
            for (int i = 0; i < correctSkullTypes.Count; i++)
            {
                if (!correctSkullTypes[i]) return;
            }
            
            OnPuzzleCompleted?.Invoke();
        }
        
        public void TearDown()
        {
            //Noop
        }
        
        public void DetachListeners()
        {
            InteractableTorch.OnSkullTypeChanged -= ChangeSkullTypeValues;

        }
    }
}
