using AE.Core.Generics;
using AE.Interfaces;
using Terra.Player;
using UnityEngine;

namespace AE.Player
{
    public class PlayerManager : MonoBehaviourSingleton<PlayerManager>, IWithSetUp, IAttachListeners
    {
        [SerializeField] private PlayerMovement playerMovement;
        
        public PlayerMovement PlayerMovement => playerMovement;
        public void SetUp()
        {
            playerMovement = GetComponent<PlayerMovement>();
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
