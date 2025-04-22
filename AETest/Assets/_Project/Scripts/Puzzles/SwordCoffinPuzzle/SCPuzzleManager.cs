using System;
using AE._Project.Scripts.Core.Generics;
using AE._Project.Scripts.Managers;

namespace AE._Project.Scripts.Puzzles.SwordCoffinPuzzle
{
    /// <summary>
    ///     Class that manages Sword-Coffin puzzle
    /// </summary>
    public class ScPuzzleManager : MonoBehaviourSingleton<ScPuzzleManager>
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