using System.Collections.Generic;
using AE._Project.Scripts.InteractableSystem;
using AE._Project.Scripts.Interfaces;
using AE._Project.Scripts.Puzzles.SwordCoffinPuzzle;
using UnityEngine;
using UnityEngine.Serialization;

namespace AE._Project.Scripts.Puzzles.TorchSkullPuzzle.Objects.InteractableItems
{
    public class InteractableSkull : InteractableItem, IAttachListeners
    {
        public enum SkullType
        {
            Red = 0,
            Pink = 1,
            Blue = 2,
            Purple = 3
        }

        [FormerlySerializedAs("skullType")] [SerializeField]
        private SkullType _skullType;

        [FormerlySerializedAs("startPulseColor")] [Header("On skull wear")] [SerializeField]
        private Color _startPulseColor;

        [FormerlySerializedAs("endPulseColor")] [SerializeField]
        private Color _endPulseColor;

        [FormerlySerializedAs("redMaterial")] [Header("On Coffin Complete")] [SerializeField]
        private Material _redMaterial;

        [FormerlySerializedAs("EyesMeshes")] [SerializeField]
        private List<MeshRenderer> _eyesMeshes = new();

        public SkullType Type => _skullType;

        public Color StartPulseColor => _startPulseColor;
        public Color EndPulseColor => _endPulseColor;


        public void AttachListeners()
        {
            TsPuzzleManager.Instance.OnPuzzleCompleted += OnPuzzleComplete;
            ScPuzzleManager.Instance.OnPuzzleCompleted += OnCoffinPuzzleComplete;
        }

        public void DetachListeners()
        {
            if (TsPuzzleManager.Instance)
            {
                TsPuzzleManager.Instance.OnPuzzleCompleted -= OnPuzzleComplete;
            }

            if (ScPuzzleManager.Instance)
            {
                ScPuzzleManager.Instance.OnPuzzleCompleted -= OnCoffinPuzzleComplete;
            }
        }

        private void OnPuzzleComplete()
        {
            CanBeInteractedWith = false;
            gameObject.layer = Utils.Utils.IgnoreRaycastMask;
        }

        private void OnCoffinPuzzleComplete()
        {
            foreach (var t in _eyesMeshes) t.material = _redMaterial;
        }
    }
}