using System.Collections;
using UnityEngine;

public class UISectionController : MonoBehaviour
{
    [Header("Settings")]
    public float fadeDuration = 0.5f; // Default duration for fade transitions

    [SerializeField]private CanvasGroup currentSection; // Tracks the currently active section

    // Method to switch between sections
    public void SwitchSection(CanvasGroup newSection)
    {
        if (currentSection == newSection)
            return; // Avoid switching to the same section

        // Fade out the current section
        if (currentSection != null)
        {
            StartCoroutine(FadeCanvasGroup(currentSection, currentSection.alpha, 0, fadeDuration, false));
        }

        // Fade in the new section
        if (newSection != null)
        {
            StartCoroutine(FadeCanvasGroup(newSection, newSection.alpha, 1, fadeDuration, true));
        }

        // Update the current section
        currentSection = newSection;
    }

    // Coroutine to handle the fade animation
    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration, bool enable)
    {
        float elapsed = 0f;

        if (enable)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = endAlpha;

        if (!enable)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
}
