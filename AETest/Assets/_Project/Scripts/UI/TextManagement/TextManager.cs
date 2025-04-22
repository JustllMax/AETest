using System;
using System.Threading;
using AE._Project.Scripts.Core.Generics;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace AE._Project.Scripts.UI.TextManagement
{
    public class TextManager : MonoBehaviourSingleton<TextManager>
    {
        [FormerlySerializedAs("textContainer")] [SerializeField] private GameObject _textContainer;
        [FormerlySerializedAs("textDisplay")] [SerializeField] private TMP_Text _textDisplay;
        [FormerlySerializedAs("textSpeed")] [SerializeField] private float _textSpeed;
        [FormerlySerializedAs("disappearDelay")] [SerializeField] private float _disappearDelay;

        private CancellationTokenSource _cts;


        /// <summary>
        ///     Shows text by canceling any ongoing display and starting a new one.
        /// </summary>
        public void ShowText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

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
        ///     Displays text character by character.
        /// </summary>
        private async UniTaskVoid DisplayTextAsync(string text, CancellationToken token)
        {
            try
            {
                // Setup display
                _textContainer.SetActive(true);
                var currentText = string.Empty;
                SetText(currentText);

                // Display text character by character
                for (var i = 0; i < text.Length; i++)
                {
                    token.ThrowIfCancellationRequested();

                    // Add next character
                    currentText += text[i];
                    SetText(currentText);

                    // Wait between characters
                    await UniTask.WaitForSeconds(_textSpeed, cancellationToken: token);
                }

                // Wait before hiding
                await UniTask.WaitForSeconds(_disappearDelay, cancellationToken: token);

                // Hide the text
                ClearVisuals();
            }
            catch (OperationCanceledException)
            {
            }
        }

        /// <summary>
        ///     Clears text and hides the container.
        /// </summary>
        private void ClearVisuals()
        {
            SetText(string.Empty);
            _textContainer.SetActive(false);
        }

        private void SetText(string text)
        {
            _textDisplay.SetText(text);
        }

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