using TMPro;
using UnityEngine;

namespace _Scripts.Game
{
    // Displays a fade-in end message (e.g., "You failed...")
    public class GameEndFadeUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private float fadeDuration = 1.5f;

        private void Awake()
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
        }

        // Starts the fade-in animation with a custom message
        public void Show(string message)
        {
            messageText.text = message;
            StartCoroutine(FadeIn());
        }

        private System.Collections.IEnumerator FadeIn()
        {
            float timer = 0f;
            canvasGroup.blocksRaycasts = true;

            while (timer < fadeDuration)
            {
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
                timer += Time.deltaTime;
                yield return null;
            }

            canvasGroup.alpha = 1f;
        }
    }
}