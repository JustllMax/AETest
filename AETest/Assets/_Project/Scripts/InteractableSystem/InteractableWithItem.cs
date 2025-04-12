using System;
using AE.Interfaces;
using AE.Managers;
using UnityEngine;

namespace AE
{
    public abstract class InteractableWithItem<TItem> : InteractableBase, IWithSetUp
        where TItem : InteractableItem
    {
        TItem _item;
        public TItem Item => _item;

        [SerializeField] private string attachedItemInteractionText;
        protected abstract bool RequiresSpecialItemType { get; }
        protected Type RequiredItemType;

        public bool HasItem => Item != null;
        public override bool CanBeInteractedWith { get; protected set; } = true;
        
        public virtual void SetUp()
        {
            RequiredItemType = typeof(TItem);
        }


        public void UseItem(TItem item)
        {
            if (item == null) return;
            
            if (RequiresSpecialItemType)
            {
                if (item.GetType() != RequiredItemType)
                {
                    IncorrectInteraction();
                    return;
                }
            } 
            
            _item = item;
            _item.OnPickedUp += DetachItem;
            OnItemUsed(_item);
        }

        protected virtual void OnItemUsed(TItem item)
        {
            Item.ResetLayer();
        }

        protected virtual void OnDetachItem() { }
        public void DetachItem()
        {
            OnDetachItem();
            if(_item != null)
                _item.OnPickedUp -= DetachItem;
            
            _item = null;
        }
        public override void OnInteraction()
        {
            if (HasItem) InteractionWithItemAttached();
            else DefaultInteraction();
        }

        public virtual void SetBaseItem(TItem item)
        {
            _item = item;
            _item.OnPickedUp += DetachItem;
        }

        protected virtual void InteractionWithItemAttached()
        {
            TextManager.Instance?.ShowText(attachedItemInteractionText);
        }

        public virtual void TearDown()
        {
            
        }
    }
}
