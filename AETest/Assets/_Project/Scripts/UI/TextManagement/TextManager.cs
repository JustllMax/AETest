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

        CancellationTokenSource cts = new CancellationTokenSource();

        private string currentText;
        public void ShowText(string text)
        {
              ClearText();
              StartTextDisplay(text, cts.Token).Forget();
        }

        private async UniTaskVoid StartTextDisplay(string text, CancellationToken token)
        {
            textContainer.SetActive(true);
            currentText = string.Empty;

            try
            {



                for (int i = 0; i < text.Length; i++)
                {
                    currentText += text[i];
                    
                    // Play audio maybe
                    
                    SetText(currentText);
                    await UniTask.WaitForSeconds(textSpeed, cancellationToken: token);
                }

                await UniTask.WaitForSeconds(disappearDelay, cancellationToken: token);
            }
            catch (OperationCanceledException)
            {
                Debug.LogWarning($"{gameObject.name} Text display of text \"{text}\" cancelled");
            }
        }
            

        public void ClearText()
        {
            cts.Cancel();
            SetText(string.Empty);
            textContainer.SetActive(false);
            currentText = string.Empty;
        }
        
        private void SetText(string text)
        {
            textDisplay.SetText(text);
        }

    }
}
