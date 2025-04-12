using AE.Core.Generics;
using AE.Interfaces;
using UnityEngine;

namespace AE.InteractableSystem
{
    
    //NOTE: Could be used for anything, for example setting base state for LightComponent
    // As it is a small project, it is only used for setting base item for the objects
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
