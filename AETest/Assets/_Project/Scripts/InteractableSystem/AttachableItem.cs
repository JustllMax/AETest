using System;
using AE.Core.Generics;
using AE.Interfaces;

namespace AE
{
    public class AttachableItem : InteractableItem, IAttachable
    {
        
        
        public event Action OnItemAttached;
        
        public event Action OnItemDetached;
        
        public virtual void Attach()
        {
            OnItemAttached?.Invoke();
        }

        public virtual void Detach()
        {
            OnItemDetached?.Invoke();
        }
    }
}
