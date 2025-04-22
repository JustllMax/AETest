using System;
using AE._Project.Scripts.Interfaces;
using AE._Project.Scripts.UI.TextManagement;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace AE._Project.Scripts.InteractableSystem
{
    /// <summary>
    ///     Represents base class for interactable objects, that items can also be used on.
    /// </summary>
    public abstract class InteractableWithItemBase : InteractableBase
    {
        [FormerlySerializedAs("incorrectInteractionText")] [Foldout("General")] [SerializeField] protected string _incorrectInteractionText;
        [FormerlySerializedAs("correctInteractionText")] [Foldout("General")] [SerializeField] protected string _correctInteractionText;

        /// <summary>
        ///     Checks, whether the item can be used on the object
        /// </summary>
        public abstract bool CanUseItem<TInteractableItem>(TInteractableItem item)
            where TInteractableItem : InteractableItem;

        /// <summary>
        ///     Use item on the object
        /// </summary>
        public abstract bool UseItem<TInteractableItem>(TInteractableItem item)
            where TInteractableItem : InteractableItem;

        protected void DisplayCorrectInteraction()
        {
            TextManager.Instance?.ShowText(_correctInteractionText);
        }

        protected void DisplayIncorrectInteraction()
        {
            TextManager.Instance?.ShowText(_incorrectInteractionText);
        }
    }

    /// <summary>
    ///     Class for interactable objects to inherit from.
    /// </summary>
    /// <typeparam name="TItem">Item that can be used on the object</typeparam>
    public abstract class InteractableWithItem<TItem> : InteractableWithItemBase, IWithSetUp
        where TItem : InteractableItem
    {
        [FormerlySerializedAs("attachedItemInteractionText")] [SerializeField] private string _attachedItemInteractionText;
        protected Type RequiredItemType;
        protected TItem Item { get; private set; }

        protected abstract bool RequiresSpecialItemType { get; }

        public bool HasItem => Item != null;
        public override bool CanBeInteractedWith { get; protected set; } = true;

        public virtual void SetUp()
        {
            RequiredItemType = typeof(TItem);
        }

        public virtual void TearDown()
        {
        }

        public override bool CanUseItem<TInteractableItem>(TInteractableItem item)
        {
            if (item == null)
            {
                return false;
            }

            if (item as TItem == null)
            {
                return false;
            }

            return InternalCanUseItem(item as TItem);
        }

        /// <summary>
        ///     see <see cref="CanUseItem{TInteractableItem}" />, only it checks for the actual required type
        /// </summary>
        private bool InternalCanUseItem(TItem item)
        {
            if (RequiresSpecialItemType)
            {
                return item.GetType() == RequiredItemType;
            }

            return true;
        }

        /// <summary>
        ///     see <see cref="UseItem{TInteractableItem}" />, only it uses item of the correct type
        /// </summary>
        public override bool UseItem<TInteractableItem>(TInteractableItem item)
        {
            return InternalUseItem(item as TItem);
        }


        /// <summary>
        ///     Handles logic of using item on the object
        /// </summary>
        private bool InternalUseItem(TItem item)
        {
            if (item == null)
            {
                return false;
            }

            // Check is there already an item
            if (HasItem)
            {
                InteractionWithItemAttached();
                return false;
            }

            if (RequiresSpecialItemType)
            {
                if (item.GetType() != RequiredItemType)
                {
                    DisplayIncorrectInteraction();
                    return false;
                }
            }

            Item = item;
            Item.OnPickedUp += DetachItem;
            OnItemUsed(Item);

            return true;
        }

        /// <summary>
        ///     Additional logic, after the item has already been used on the object and it's cached
        /// </summary>
        protected virtual void OnItemUsed(TItem item)
        {
            item.SetIgnoreRaycastLayer();
            item.PlayUseSfx();
        }

        /// <summary>
        ///     Called when item is being detached from the object
        /// </summary>
        private void DetachItem()
        {
            OnDetachItem();
            if (Item != null)
            {
                Item.OnPickedUp -= DetachItem;
            }

            Item = null;
        }

        /// <summary>
        ///     Additional logic before item has been cleared out
        /// </summary>
        protected virtual void OnDetachItem()
        {
        }

        protected override void OnInteraction()
        {
            if (HasItem)
            {
                InteractionWithItemAttached();
            }
            else
            {
                DisplayDefaultInteraction();
            }
        }

        /// <summary>
        ///     Method used by the set state base caller.
        ///     Attaches item to the object, skipping the UseItem call.
        /// </summary>
        public void SetBaseItem(TItem item)
        {
            Item = item;
            Item.OnPickedUp += DetachItem;
            OnSetBaseItem();
        }

        /// <summary>
        ///     Additional logic when setting base state, called when item has been cached
        /// </summary>
        protected virtual void OnSetBaseItem()
        {
        }

        protected virtual void InteractionWithItemAttached()
        {
            TextManager.Instance?.ShowText(_attachedItemInteractionText);
        }
    }
}