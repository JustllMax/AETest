namespace AE.Interfaces
{


    public interface IInteractable
    {

        /// <summary>
        /// Checks if available for interaction
        /// </summary>
        public bool CanBeInteractedWith { get; }

        /// <summary>
        /// Checks for displaying visualisation
        /// </summary>
        public bool CanShowVisualisation { get; set; }

        /// <summary>
        /// Called on interaction
        /// </summary>
        public void Interact();
        
        
    }
}