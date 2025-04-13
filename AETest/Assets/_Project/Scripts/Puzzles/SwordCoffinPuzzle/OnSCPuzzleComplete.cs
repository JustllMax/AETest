using AE.Core.Generics;
using AE.Interfaces;
using UnityEngine;

namespace AE.Puzzles.SwordCoffinPuzzle
{
    public class OnSCPuzzleComplete : InGameMonoBehaviour, IAttachListeners
    {
        public virtual void AttachListeners()
        {
            if (SCPuzzleManager.Instance)
                SCPuzzleManager.Instance.OnPuzzleCompleted += OnComplete;
        }

        protected virtual void OnComplete()
        {
            
        }
        
        public virtual void DetachListeners()
        {
            if (SCPuzzleManager.Instance)
                SCPuzzleManager.Instance.OnPuzzleCompleted -= OnComplete;
        }
    }
}
