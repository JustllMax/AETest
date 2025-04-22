using AE._Project.Scripts.Components;
using UnityEngine;

namespace AE._Project.Scripts.Puzzles.SwordCoffinPuzzle
{
    public class LightChanger : OnScPuzzleComplete
    {
        [SerializeField] private LightComponent _lightComponent;

        [Header("Coffin Puzzle Complete")] [SerializeField]
        private Color _startColor = Color.red;

        [SerializeField] private Color _endPulseColor = Color.red;

        protected override void OnComplete()
        {
            base.OnComplete();

            _lightComponent.SetCurrentColor(_startColor);
            _lightComponent.PulseData._pulseColor = _endPulseColor;
        }
    }
}