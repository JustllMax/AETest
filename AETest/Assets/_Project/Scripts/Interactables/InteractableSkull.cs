using AE.Player;
using UnityEngine;

namespace AE
{
    public class InteractableSkull : InteractableItem
    {

        public enum SkullType
        {
            Red,
            Pink,
            Blue,
            Purple
        }
        public override bool CanBeInteractedWith { get; } = true;
        
        [SerializeField] private SkullType skullType;
        
        [Header("On skull wear")]
        [SerializeField] Color startPulseColor;
        [SerializeField] Color endPulseColor;
        public SkullType Type => skullType;
        
        public Color StartPulseColor => startPulseColor;
        public Color EndPulseColor => endPulseColor;
        
        public override void OnInteraction()
        {
            //Pickup skull
            
        }
        
    }
}
