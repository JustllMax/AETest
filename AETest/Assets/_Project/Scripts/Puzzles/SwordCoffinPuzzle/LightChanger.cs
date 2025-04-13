using AE.Core.Generics;
using AE.Interfaces;
using UnityEngine;

namespace AE.Puzzles.SwordCoffinPuzzle
{
    public class LightChanger : OnSCPuzzleComplete
    {
        [SerializeField] private LightComponent _lightComponent;
        
        [Header("Coffin Puzzle Complete")] 
        [SerializeField] Color _startColor = Color.red;
        [SerializeField] Color _endPulseColor = Color.red;

        protected override void OnComplete()
        {
            base.OnComplete();
            
            _lightComponent.SetCurrentColor(_startColor);
            _lightComponent.PulseData.pulseColor = _endPulseColor;
        }
    }
}
