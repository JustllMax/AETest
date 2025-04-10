

using AE.Core.Generics;
using AE.Interfaces;
using AE.Managers;
using UnityEngine;

/// <summary>
/// Represents base class for object that can be interacted with
/// </summary>
public abstract class InteractableBase : InGameMonoBehaviour, IInteractable
{
    public abstract bool CanBeInteractedWith { get; }
    public virtual bool CanShowVisualisation { get; set; }
    
    [SerializeField] protected string itemDescription;
    [SerializeField] protected string incorrectInteractionText;
    [SerializeField] protected string correctInteractionText;
    public abstract void OnInteraction();

    public void Interact()
    {
        if(!CanBeInteractedWith) return;
        OnInteraction();
    }

    protected void DefaultInteraction()
    {
        TextManager.Instance.ShowText(itemDescription);
    }
    
    protected void CorrectInteraction()
    {
        TextManager.Instance.ShowText(correctInteractionText);
    }

    protected void IncorrectInteraction()
    {
        TextManager.Instance.ShowText(incorrectInteractionText);
    }
    
}