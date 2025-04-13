using AE.InteractableSystem;
using UnityEngine;

namespace AE
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
