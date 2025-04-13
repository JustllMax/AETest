using System;
using System.Threading;
using AE.Core.Generics;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace AE.Managers
{
    public class TextManager : MonoBehaviourSingleton<TextManager>
    {
        [SerializeField] private GameObject textContainer;
        [SerializeField] private TMP_Text textDisplay;
        [SerializeField] private float textSpeed;
        [SerializeField] private float disappearDelay;

        private CancellationTokenSource _cts;
        

        /// <summary>
        /// Shows text by canceling any ongoing display and starting a new one.
        /// </summary>
        public void ShowText(string text)
        {
            if(string.IsNullOrEmpty(text)) return;
            
            // Cancel any ongoing operation first
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
            }
            
            // Create new token source for this operation
            _cts = new CancellationTokenSource();
            
            // Start new text display operation
            DisplayTextAsync(text, _cts.Token).Forget();
        }

        /// <summary>
        /// Displays text character by character.
        /// </summary>
        private async UniTaskVoid DisplayTextAsync(string text, CancellationToken token)
        {
            try
            {
                // Setup display
                textContainer.SetActive(true);
                string currentText = string.Empty;
                SetText(currentText);
                
                // Display text character by character
                for (int i = 0; i < text.Length; i++)
                {
                    token.ThrowIfCancellationRequested();
                    
                    // Add next character
                    currentText += text[i];
                    SetText(currentText);

                    // Wait between characters
                    await UniTask.WaitForSeconds(textSpeed, cancellationToken: token);
                }

                // Wait before hiding
                await UniTask.WaitForSeconds(disappearDelay, cancellationToken: token);
                
                // Hide the text
                ClearVisuals();
            }
            catch (OperationCanceledException)
            {
                // Expected during cancellation, no need to log
                // Don't clear visuals here - let the new task handle it
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error in text display: {ex.Message}");
                ClearVisuals();
            }
        }

        /// <summary>
        /// Clears text and hides the container.
        /// </summary>
        private void ClearVisuals()
        {
            SetText(string.Empty);
            textContainer.SetActive(false);
        }

        private void SetText(string text) => textDisplay.SetText(text);

        protected override void CleanUp()
        {
            base.CleanUp();

            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
            }
        }
    }
}
