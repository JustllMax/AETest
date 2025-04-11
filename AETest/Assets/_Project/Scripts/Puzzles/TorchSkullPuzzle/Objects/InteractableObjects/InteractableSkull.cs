using AE.Interfaces;
using AE.Player;
using UnityEngine;

namespace AE.Puzzles.TorchSkullPuzzle.Objects.InteractableItems
{
    public class InteractableSkull : InteractableItem, IAttachListeners
    {

        public enum SkullType
        {
            Red =0,
            Pink =1,
            Blue =2,
            Purple =3,
        }
        
        [SerializeField] private SkullType skullType;
        
        [Header("On skull wear")]
        [SerializeField] Color startPulseColor;
        [SerializeField] Color endPulseColor;
        public SkullType Type => skullType;
        
        public Color StartPulseColor => startPulseColor;
        public Color EndPulseColor => endPulseColor;
        

        public void AttachListeners()
        {
            TSPuzzleManager.Instance.OnPuzzleCompleted += OnPuzzleComplete;
        }
        
        private void OnPuzzleComplete()
        {
            CanBeInteractedWith = false;
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }
        
        public void DetachListeners()
        {
            if(TSPuzzleManager.Instance)
                TSPuzzleManager.Instance.OnPuzzleCompleted += OnPuzzleComplete;
        }
        
    }
}
