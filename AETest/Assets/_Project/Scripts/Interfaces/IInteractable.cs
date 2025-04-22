namespace AE._Project.Scripts.Interfaces
{
    public interface IInteractable
    {
        /// <summary>
        ///     Checks if available for interaction
        /// </summary>
        public bool CanBeInteractedWith { get; }

        /// <summary>
        ///     Called on interaction
        /// </summary>
        public void Interact();
    }
}