using AE._Project.Scripts.InteractableSystem;

namespace AE._Project.Scripts.Puzzles
{
    public class BasicInteractable : InteractableBase
    {
        public override bool CanBeInteractedWith { get; protected set; } = true;

        protected override void OnInteraction()
        {
            DisplayDefaultInteraction();
        }
    }
}