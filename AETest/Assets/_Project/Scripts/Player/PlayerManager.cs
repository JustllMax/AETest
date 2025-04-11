using AE.Core.Generics;
using AE.Interfaces;
using Terra.Player;
using UnityEngine;

namespace AE.Player
{
    public class PlayerManager : MonoBehaviourSingleton<PlayerManager>, IWithSetUp, IAttachListeners
    {
        [SerializeField] private PlayerMovement playerMovement;
        [SerializeField] private PlayerInteractionController interactionController;
        
        public PlayerMovement PlayerMovement => playerMovement;
        public PlayerInteractionController InteractionController => interactionController;
        public void SetUp()
        {
            playerMovement = GetComponent<PlayerMovement>();
            interactionController = GetComponent<PlayerInteractionController>();
        }

        public void TearDown()
        {
            
        }

        public void AttachListeners()
        {
           
        }

        public void DetachListeners()
        {
            
        }
    }
}
