using AE.Core.Generics;
using TMPro;
using UnityEngine;

namespace AE.Managers
{
    public class TextManager : MonoBehaviourSingleton<TextManager>
    {
        [SerializeField] private TMP_Text textDisplay;
        [SerializeField] private float textSpeed;
        [SerializeField] private float disappearDelay;

        public void ShowText(string text)
        {
            
        }
    }
}
