using UnityEngine;
using UnityEngine.Serialization;

namespace AE._Project.Scripts.InteractableSystem
{
    //NOTE: Could be used for anything, for example setting base state for LightComponent
    // As it is a small project, it is only used for setting base item for the objects
    /// <summary>
    ///     Handles using item on interactable object at the start of the game
    /// </summary>
    public class SetBaseCaller<TItem> : MonoBehaviour
        where TItem : InteractableItem
    {
        [FormerlySerializedAs("itemTarget")] [SerializeField] private InteractableWithItem<TItem> _itemTarget;
        [FormerlySerializedAs("itemToBeUsed")] [SerializeField] private TItem _itemToBeUsed;

        public void SetBase()
        {
            if (_itemToBeUsed == null || _itemTarget == null)
            {
                Debug.LogError($"{this} {gameObject.name} itemTarget or itemToBeUsed is null");
                return;
            }

            _itemTarget.SetBaseItem(_itemToBeUsed);
        }
    }
}