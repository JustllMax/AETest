using AE._Project.Scripts.Core.Generics;
using AE._Project.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.Serialization;

namespace AE._Project.Scripts.Player
{
    public class PlayerManager : MonoBehaviourSingleton<PlayerManager>, IWithSetUp, IAttachListeners
    {
        [FormerlySerializedAs("playerMovement")] [SerializeField] private PlayerMovement _playerMovement;
        [FormerlySerializedAs("interactionController")] [SerializeField] private PlayerInteractionController _interactionController;

        public PlayerMovement PlayerMovement => _playerMovement;
        public PlayerInteractionController InteractionController => _interactionController;

        public void AttachListeners()
        {
        }

        public void DetachListeners()
        {
        }

        public void SetUp()
        {
            _playerMovement = GetComponent<PlayerMovement>();
            _interactionController = GetComponent<PlayerInteractionController>();
        }

        public void TearDown()
        {
        }
    }
}