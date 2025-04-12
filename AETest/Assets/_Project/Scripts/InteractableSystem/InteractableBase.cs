using AE.Core.Generics;
using AE.Interfaces;
using AE.Managers;
using UnityEngine;

namespace AE.InteractableSystem
{

    /// <summary>
    /// Represents base class for object that can be interacted with
    /// </summary>
    [RequireComponent(typeof(Outline))]
    public abstract class InteractableBase : InGameMonoBehaviour, IInteractable
    {
        public abstract bool CanBeInteractedWith { get; protected set; }

        [SerializeField] protected string itemDescription;
        [SerializeField] protected string incorrectInteractionText;
        [SerializeField] protected string correctInteractionText;
        [SerializeField] protected Outline outline;
        [SerializeField] protected Color outlineColor = Color.yellow;

        protected virtual void Awake()
        {
            if (outline == null) outline = GetComponent<Outline>();
            outline.OutlineColor = outlineColor;
            outline.OutlineMode = Outline.Mode.OutlineVisible;
            HideOutline();
        }

        /// <summary>
        /// Logic called on interaction
        /// </summary>
        protected abstract void OnInteraction();

        public void Interact()
        {
            if (!CanBeInteractedWith) return;
            OnInteraction();
        }

        protected void DisplayDefaultInteraction()
        {
            TextManager.Instance?.ShowText(itemDescription);
        }

        protected void DisplayCorrectInteraction()
        {
            TextManager.Instance?.ShowText(correctInteractionText);
        }

        protected void DisplayIncorrectInteraction()
        {
            TextManager.Instance?.ShowText(incorrectInteractionText);
        }

        public void ShowOutline() => outline.enabled = true;
        public void HideOutline() => outline.enabled = false;

    }
}