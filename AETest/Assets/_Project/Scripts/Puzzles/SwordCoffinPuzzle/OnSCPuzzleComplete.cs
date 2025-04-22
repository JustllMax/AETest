using AE._Project.Scripts.Core.Generics;
using AE._Project.Scripts.Interfaces;

namespace AE._Project.Scripts.Puzzles.SwordCoffinPuzzle
{
    public class OnScPuzzleComplete : InGameMonoBehaviour, IAttachListeners
    {
        public virtual void AttachListeners()
        {
            if (ScPuzzleManager.Instance)
            {
                ScPuzzleManager.Instance.OnPuzzleCompleted += OnComplete;
            }
        }

        public virtual void DetachListeners()
        {
            if (ScPuzzleManager.Instance)
            {
                ScPuzzleManager.Instance.OnPuzzleCompleted -= OnComplete;
            }
        }

        protected virtual void OnComplete()
        {
        }
    }
}