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

        private CancellationTokenSource _cts = new();
  
        private UniTask _currentTask = UniTask.CompletedTask;
        private bool isDuringDisplay = false;
        private string currentText;

        public void ShowText(string text) => AsyncShowText(text).Forget(ex => 
        {
            Debug.LogWarning(ex);
        });

        /// <summary>
        /// Shows new text after ensuring any currently displaying text is cancelled
        /// </summary>
        private async UniTask AsyncShowText(string text)
        {
            // Cancel previous typing if in progress

            CancelCurrentOperation();
            try
            {
                await _currentTask; // Wait for cancellation to complete
            }
            catch (Exception e)
            {
                Debug.LogWarning("Previous task cancelled.");
            }

            // Create new token
            _cts.Dispose();
            _cts = new CancellationTokenSource();
            // Call text display and cache it
            _currentTask = StartTextDisplay(text, _cts.Token);
        }

        /// <summary>
        /// Displays the text character by character.
        /// </summary>
        private async UniTask StartTextDisplay(string text, CancellationToken token)
        {
            ClearVisuals();
            try
            {
                isDuringDisplay = true;
                textContainer.SetActive(true);
                currentText = string.Empty;

                for (int i = 0; i < text.Length; i++)
                {
                    // Append next character.
                    currentText += text[i];
                    SetText(currentText);

                    // Wait between characters
                    await UniTask.WaitForSeconds(textSpeed, cancellationToken: token);
                    // Fail-safe
                    token.ThrowIfCancellationRequested(); 
                }

                // Wait before disappearing.
                await UniTask.WaitForSeconds(disappearDelay, cancellationToken: token);
                // Fail-safe
                token.ThrowIfCancellationRequested(); 
                
                ClearVisuals();
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning($"{gameObject.name} text display for \"{text}\" was cancelled.");
            }
            finally
            {
                isDuringDisplay = false;
            }
        }

        /// <summary>
        /// Cancels the current text display.
        /// </summary>
        private void CancelCurrentOperation()
        {
            if (_cts != null && !_cts.IsCancellationRequested)
            {
                _cts.Cancel();
            }
        }

        /// <summary>
        /// Clears text and deactivates the container.
        /// </summary>
        private void ClearVisuals()
        {
            SetText(string.Empty);
            textContainer.SetActive(false);
            currentText = string.Empty;
        }

        private void SetText(string text) => textDisplay.SetText(text);
        

        private void OnDestroy()
        {
            // Cleanup to avoid leftover tasks.
            _cts?.Cancel();
            _cts?.Dispose();
            
        }

    }
}
