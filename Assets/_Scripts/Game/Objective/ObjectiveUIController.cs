using System.Collections;
using TMPro;
using UnityEngine;

namespace _Scripts.Game.Objective
{
    public class ObjectiveUIController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float fadeDuration = 0.5f;
        
        private Coroutine _fadeRoutine;

        public void ShowObjective(string title, string description)
        {
            if (_fadeRoutine != null)
                StopCoroutine(_fadeRoutine);
            _fadeRoutine = StartCoroutine(FadeOutThenIn(title, description));
        }

        public void HideObjective()
        {
            if(_fadeRoutine != null)
                StopCoroutine(_fadeRoutine);
            
            _fadeRoutine = StartCoroutine(FadeTo(0f));
        }
        
        private IEnumerator FadeOutThenIn(string title, string description)
        {
            // Fade out
            yield return FadeTo(0f);
            titleText.text = title;
            descriptionText.text = description;
            // Fade in
            yield return FadeTo(1f);
        }
        
        private IEnumerator FadeTo(float targetAlpha)
        {
            float startAlpha = canvasGroup.alpha;
            float time = 0f;
            while (time < fadeDuration)
            {
                time += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
                yield return null;
            }
            canvasGroup.alpha = targetAlpha;
        }
    }
}
