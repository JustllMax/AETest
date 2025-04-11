using AE.Puzzles.TorchSkullPuzzle.Objects.InteractableItems;
using UnityEngine;

namespace AE.Interfaces
{
    public interface IHasSetBaseState
    {
        /// <summary>
        /// Method called after setup, looks for caller to set base state
        /// </summary>
        public void UseCaller();
    }
}
