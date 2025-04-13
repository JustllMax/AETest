using System;
using AE.Core.Generics;
using AE.Managers;
using UnityEngine;

namespace AE.Puzzles.SwordCoffinPuzzle
{
    /// <summary>
    /// Class that manages Sword-Coffin puzzle
    /// </summary>
    public class SCPuzzleManager : MonoBehaviourSingleton<SCPuzzleManager>
    {

        public event Action OnPuzzleCompleted;
        
        public void NotifyPuzzleCompleted()
        { 
            OnPuzzleCompleted?.Invoke();
            
            // NOTE: Should be in other methods, but it will suffice here
            TimeManager.Instance?.AddPauseBlocker(gameObject);
        }
        
    }
}
