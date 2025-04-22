using AE._Project.Scripts.Core.Generics;
using AE.InputManagement;
using UnityEngine;

namespace AE._Project.Scripts.InputManagement
{
    public class InputManager : MonoBehaviourSingleton<InputManager>
    {
        private InputSystem InputSystem { get; set; }

        public InputSystem.PlayerActions PlayerControls => InputSystem.Player;
        public InputSystem.AllTimesActions AllTimeControls => InputSystem.AllTimes;


        protected override void Awake()
        {
            base.Awake();
            InputSystem = new InputSystem();
        }

        private void Start()
        {
            // Activate global controls
            EnableAllTimeControls();
            // Activate player controls
            EnablePlayerControls();
        }


        /// <summary>
        ///     Method handles switching enable state of <see cref="AllTimeControls" />
        /// </summary>
        public void SetAllTimeControlsState(bool value)
        {
            if (value)
            {
                EnableAllTimeControls();
            }

            else
            {
                DisableAllTimeControls();
            }
        }

        /// <summary>
        ///     Method handles switching enable state of <see cref="PlayerControls" />
        /// </summary>
        public void SetPlayerControlsState(bool value)
        {
            if (value)
            {
                EnablePlayerControls();
            }

            else
            {
                DisablePlayerControls();
            }
        }


        /// <summary>
        ///     Method enables AllTime controls
        /// </summary>
        private void EnableAllTimeControls()
        {
            if (InputSystem?.AllTimes == null)
            {
                Debug.LogError("InputActions or AllTime is null in " + this);
                return;
            }

            InputSystem?.AllTimes.Enable();
        }

        /// <summary>
        ///     Method enables AllTime controls
        /// </summary>
        private void DisableAllTimeControls()
        {
            if (InputSystem?.AllTimes == null)
            {
                Debug.LogError("InputActions or AllTime is null in " + this);
                return;
            }

            InputSystem?.AllTimes.Disable();
        }


        /// <summary>
        ///     Method enables Player controls
        /// </summary>
        private void EnablePlayerControls()
        {
            if (InputSystem?.Player == null)
            {
                Debug.LogError("InputActions or PlayerControls is null in " + this);
                return;
            }

            InputSystem?.Player.Enable();
        }

        /// <summary>
        ///     Method disables Player controls
        /// </summary>
        private void DisablePlayerControls()
        {
            if (InputSystem?.Player == null)
            {
                Debug.LogWarning("PlayerControls is null in " + this);
                return;
            }

            InputSystem?.Player.Disable();
        }
    }
}