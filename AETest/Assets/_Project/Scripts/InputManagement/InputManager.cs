using System;
using AE.Core.Generics;
using UnityEngine;

namespace AE.InputManagement
{
    public class InputManager : MonoBehaviourSingleton<InputManager>
    {
        private InputSystem _inputSystem;

        public InputSystem InputSystem => _inputSystem;
        public InputSystem.PlayerActions PlayerControls => _inputSystem.Player;
        public InputSystem.AllTimesActions AllTimeControls => _inputSystem.AllTimes;
        

        protected override void Awake()
        {
            base.Awake();
            _inputSystem = new InputSystem();
        }

        void Start()
        {
            // Activate global controls
            EnableAllTimeControls();
            // Activate player controls
            EnablePlayerControls();
        }


        /// <summary>
        /// Method handles switching enable state of <see cref="AllTimeControls"/>
        /// </summary>
        public void SetAllTimeControlsState(bool value)
        {
            if (value)EnableAllTimeControls();
            
            else DisableAllTimeControls();
        }

        /// <summary>
        /// Method handles switching enable state of <see cref="PlayerControls"/>
        /// </summary>
        public void SetPlayerControlsState(bool value)
        {
            if (value) EnablePlayerControls();
            
            else DisablePlayerControls();
        }



        /// <summary>
        /// Method enables AllTime controls
        /// </summary>
        private void EnableAllTimeControls()
        {
            if (_inputSystem?.AllTimes == null)
            {
                Debug.LogError("InputActions or AllTime is null in " + this);
                return;
            }

            _inputSystem?.AllTimes.Enable();
        }

        /// <summary>
        /// Method enables AllTime controls
        /// </summary>
        private void DisableAllTimeControls()
        {
            if (_inputSystem?.AllTimes == null)
            {
                Debug.LogError("InputActions or AllTime is null in " + this);
                return;
            }

            _inputSystem?.AllTimes.Disable();
        }


        /// <summary>
        /// Method enables Player controls
        /// </summary>
        private void EnablePlayerControls()
        {
            if (_inputSystem?.Player == null)
            {
                Debug.LogError("InputActions or PlayerControls is null in " + this);
                return;
            }

            _inputSystem?.Player.Enable();
        }

        /// <summary>
        /// Method disables Player controls
        /// </summary>
        private void DisablePlayerControls()
        {
            if (_inputSystem?.Player == null)
            {
                Debug.LogWarning("PlayerControls is null in " + this);
                return;
            }

            _inputSystem?.Player.Disable();
        }

    }
}