using System;
using AE.Core.Generics;
using UnityEngine;

namespace AE.Puzzles.SwordCoffinPuzzle
{
    /// <summary>
    /// Class that manages Sword-Coffin puzzle
    /// </summary>
    public class SCPuzzleManager : MonoBehaviourSingleton<SCPuzzleManager>
    {

        public event Action OnPuzzleCompleted;
        
        public void NotifyPuzzleCompleted() => OnPuzzleCompleted?.Invoke();
    }
}
