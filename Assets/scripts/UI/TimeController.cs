using TMPro;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    public TextMeshProUGUI CountDownText;

    private float remainingTime;
    private bool isTimerRunning = false;
    private bool isTimerPaused = false; // Tracks if the timer is paused
    private System.Action onTimerComplete;

    public bool IsTimerRunning { get { return isTimerRunning && !isTimerPaused; } }

    /// <summary>
    /// Starts a countdown (e.g., 3...2...1...Go!)
    /// </summary>
    public void Countdown(int startNumber, System.Action onComplete)
    {
        remainingTime = startNumber;
        onTimerComplete = onComplete;
        isTimerRunning = true;
        isTimerPaused = false;
    }

    /// <summary>
    /// Starts a regular timer with the specified duration.
    /// </summary>
    public void StartTimer(float duration, System.Action onComplete)
    {
        remainingTime = duration;
        onTimerComplete = onComplete;
        isTimerRunning = true;
        isTimerPaused = false;
    }

    /// <summary>
    /// Pauses the timer.
    /// </summary>
    public void PauseTimer()
    {
        if (isTimerRunning)
        {
            isTimerPaused = true;
        }
    }

    /// <summary>
    /// Resumes the timer if it was paused.
    /// </summary>
    public void ResumeTimer()
    {
        if (isTimerRunning && isTimerPaused)
        {
            isTimerPaused = false;
        }
    }

    /// <summary>
    /// Resets the timer.
    /// </summary>
    public void ResetTime()
    {
        remainingTime = 0;
        isTimerRunning = false;
        isTimerPaused = false;
        CountDownText.text = "";
    }

    private void Update()
    {
        if (!isTimerRunning || isTimerPaused) return;

        remainingTime -= Time.deltaTime;

        // Update countdown text based on time remaining
        if (remainingTime > 0)
        {
            if (remainingTime < 4) // For pre-match countdown
            {
                CountDownText.text = Mathf.CeilToInt(remainingTime).ToString();
            }
            else // For regular timer
            {
                int minutes = Mathf.FloorToInt(remainingTime / 60);
                int seconds = Mathf.FloorToInt(remainingTime % 60);
                CountDownText.text = $"{minutes:00}:{seconds:00}";
            }
        }
        else
        {
            // Timer complete
            isTimerRunning = false;
            remainingTime = 0;
            CountDownText.text = "";
            onTimerComplete?.Invoke();
        }
    }
}
