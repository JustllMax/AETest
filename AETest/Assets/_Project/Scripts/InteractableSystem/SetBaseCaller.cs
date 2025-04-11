using AE.Core.Generics;
using AE.Interfaces;
using UnityEngine;

namespace AE.InteractableSystem
{
    
    /// <summary>
    /// Handles using item on interactable object at the start of the game
    /// </summary>
    public class SetBaseCaller<TItem> : MonoBehaviour
    where TItem : InteractableItem
    {
        [SerializeField] private InteractableWithItem<TItem> itemTarget;
        [SerializeField] private TItem itemToBeUsed;
        public void SetBase()
        {
            if (itemToBeUsed == null || itemTarget == null)
            {
                Debug.LogError($"{this} {gameObject.name} itemTarget or itemToBeUsed is null");
                return;
            }
            itemTarget.SetBaseItem(itemToBeUsed);

        }
    }
}
