using AE.Core.Generics;
using AE.Interfaces;
using AE.Managers;
using NaughtyAttributes;
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

        [Foldout("Outline Settings")][SerializeField] private Outline outline;
        [Foldout("Outline Settings")][SerializeField] private Color outlineColor = Color.yellow;
        
        [Foldout("General")][SerializeField] protected string itemDescription;

        private Color outlineColorVisible;
        private Color outlineColorInvisible;

        protected virtual void Awake()
        {
            if (outline == null) outline = GetComponent<Outline>();
            outlineColorVisible = outlineColor;
            outlineColorInvisible = new Color(outlineColor.r, outlineColor.g, outlineColor.b, 0f);
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
        
        public void ShowOutline() => outline.OutlineColor = outlineColorVisible;
        public void HideOutline() => outline.OutlineColor = outlineColorInvisible;

    }
}