using AE._Project.Scripts.Core.Generics;
using AE._Project.Scripts.Interfaces;
using AE._Project.Scripts.UI.TextManagement;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace AE._Project.Scripts.InteractableSystem
{
    /// <summary>
    /// Represents base class for object that can be interacted with
    /// </summary>
    [RequireComponent(typeof(Outline))]
    public abstract class InteractableBase : InGameMonoBehaviour, IInteractable
    {
        [FormerlySerializedAs("outline")] [Foldout("Outline Settings")] [SerializeField]
        private Outline _outline;

        [FormerlySerializedAs("outlineColor")] [Foldout("Outline Settings")] [SerializeField]
        private Color _outlineColor = Color.yellow;

        [FormerlySerializedAs("itemDescription")] [Foldout("General")] [SerializeField]
        protected string _itemDescription;

        private Color _outlineColorInvisible;

        private Color _outlineColorVisible;

        protected virtual void Awake()
        {
            if (_outline == null)
            {
                _outline = GetComponent<Outline>();
            }

            _outlineColorVisible = _outlineColor;
            _outlineColorInvisible = new Color(_outlineColor.r, _outlineColor.g, _outlineColor.b, 0f);
            _outline.OutlineMode = Outline.Mode.OutlineVisible;
            HideOutline();
        }

        /// <summary>
        ///     asda
        /// </summary>
        public abstract bool CanBeInteractedWith { get; protected set; }

        public void Interact()
        {
            if (!CanBeInteractedWith)
            {
                return;
            }

            OnInteraction();
        }

        /// <summary>
        ///     Logic called on interaction
        /// </summary>
        protected abstract void OnInteraction();

        protected void DisplayDefaultInteraction()
        {
            TextManager.Instance?.ShowText(_itemDescription);
        }

        public void ShowOutline()
        {
            _outline.OutlineColor = _outlineColorVisible;
        }

        public void HideOutline()
        {
            _outline.OutlineColor = _outlineColorInvisible;
        }
    }
}