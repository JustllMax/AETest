using System;
using System.Collections.Generic;
using AE._Project.Scripts.Core.Generics;
using AE._Project.Scripts.Interfaces;
using AE._Project.Scripts.Puzzles.TorchSkullPuzzle.Objects.InteractableItems;
using AE._Project.Scripts.Puzzles.TorchSkullPuzzle.Objects.InteractableObjects;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace AE._Project.Scripts.Puzzles.TorchSkullPuzzle
{
    /// <summary>
    ///     Class that manages Torch-Skull puzzle
    /// </summary>
    public class TsPuzzleManager : MonoBehaviourSingleton<TsPuzzleManager>, IAttachListeners
    {
        [FormerlySerializedAs("correctSkullTypes")] [Foldout("Debug")] [ReadOnly] [SerializeField]
        private List<bool> _correctSkullTypes = new();

        protected override void Awake()
        {
            base.Awake();
            var skullTypesAmount = Enum.GetValues(typeof(InteractableSkull.SkullType)).Length;

            for (var i = 0; i < skullTypesAmount; i++) _correctSkullTypes.Add(false);
        }

        public void AttachListeners()
        {
            InteractableTorch.OnSkullTypeChanged += ChangeSkullTypeValues;
        }

        public void DetachListeners()
        {
            InteractableTorch.OnSkullTypeChanged -= ChangeSkullTypeValues;
        }

        public event Action OnPuzzleCompleted;

        private void ChangeSkullTypeValues(InteractableSkull.SkullType newValue, bool isCorrect)
        {
            _correctSkullTypes[(int)newValue] = isCorrect;
            for (var i = 0; i < _correctSkullTypes.Count; i++)
                if (!_correctSkullTypes[i])
                {
                    return;
                }

            OnPuzzleCompleted?.Invoke();
        }
    }
}