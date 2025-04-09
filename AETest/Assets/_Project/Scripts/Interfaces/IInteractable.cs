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
        
        /// <summary>
        /// Called after interaction
        /// </summary>
        public void OnInteraction();

        /// <summary>
        /// Called when is ready to be interacted with by Player
        /// </summary>
        public void ShowVisualisation();


        /// <summary>
        /// Stop all visualisation
        /// </summary>
        public void StopVisualization();

    }
}