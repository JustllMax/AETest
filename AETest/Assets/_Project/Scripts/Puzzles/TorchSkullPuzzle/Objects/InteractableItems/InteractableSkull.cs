using System.Collections.Generic;
using AE.InteractableSystem;
using AE.Interfaces;
using AE.Player;
using AE.Puzzles.SwordCoffinPuzzle;
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
        
        [Header("On Coffin Complete")]
        [SerializeField] Material redMaterial;
        [SerializeField] private List<MeshRenderer> EyesMeshes = new();
        public SkullType Type => skullType;
        
        public Color StartPulseColor => startPulseColor;
        public Color EndPulseColor => endPulseColor;
        

        public void AttachListeners()
        {
            TSPuzzleManager.Instance.OnPuzzleCompleted += OnPuzzleComplete;
            SCPuzzleManager.Instance.OnPuzzleCompleted += OnCoffinPuzzleComplete;
        }
        
        private void OnPuzzleComplete()
        {
            CanBeInteractedWith = false;
            gameObject.layer = Utils.IgnoreRaycastMask;
        }
        
        private void OnCoffinPuzzleComplete()
        {
            for (int i = 0; i < EyesMeshes.Count; i++)
            {
                EyesMeshes[i].material = redMaterial;
            }
        }
        
        public void DetachListeners()
        {
            if(TSPuzzleManager.Instance)
                TSPuzzleManager.Instance.OnPuzzleCompleted -= OnPuzzleComplete;
            if(SCPuzzleManager.Instance)
                SCPuzzleManager.Instance.OnPuzzleCompleted -= OnCoffinPuzzleComplete;

        }
        
    }
}
