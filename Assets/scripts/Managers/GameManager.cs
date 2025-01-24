using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    
    [SerializeField] private GridSystem gridSystem;
    public TextMeshProUGUI CountDownText;
    private bool isRoundFinished = false;
    public CollectablePoolManager collectablePool;
    public TimeController timeController;
    public event Action OnReset;
  
    public void OnSetupComplete()
    {
        SoundManager.Instance.PlayLoopingSound(SoundManager.Instance.ambientSource, "AmbientBird");
        SoundManager.Instance.PlayLoopingSound(SoundManager.Instance.windSource, "Wind");
         
        CountDownText.text = "Setup complete. Starting countdown...";
        // Start pre-match countdown (3...2...1...Go!)
        timeController.Countdown(4, StartGame);
    }
    bool isPaused = false;
    public void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            Time.timeScale = 0; // Pause the game
            // Pause players and timer
            foreach (PlayerData player in gridSystem.playersList)
            {
                player.State = PlayerState.Locked;
                player.SetRoundStatus(false);
            }
            timeController.PauseTimer();
            SoundManager.Instance.StopLoopingSound(SoundManager.Instance.musicSource);
        }
        else
        {
            Time.timeScale = 1; // Resume the game
            // Resume players and timer
            foreach (PlayerData player in gridSystem.playersList)
            {
                player.State = PlayerState.Active;
                player.SetRoundStatus(true);
            }
            timeController.ResumeTimer();
            SoundManager.Instance.PlayLoopingSound(SoundManager.Instance.musicSource, "country song");
        }
    }

    private void StartGame()
    {
        CountDownText.text = "Go!";
        foreach (PlayerData player in gridSystem.playersList)
        {
            player.State = PlayerState.Active; // Enable players
            player.SetRoundStatus(true);
        }

        SoundManager.Instance.PlayLoopingSound(SoundManager.Instance.musicSource, "country song");

        // Start round timer (e.g., 90 seconds)
        timeController.StartTimer(90f, EndRound);
    }

    public GameObject EndMenu,GameHUD;

    private void EndRound()
    {
        SoundManager.Instance.StopLoopingSound(SoundManager.Instance.musicSource);
        SoundManager.Instance.PlaySoundOnce(SoundManager.Instance.musicSource, "GuitarEnding");
        foreach (PlayerData player in gridSystem.playersList)
        {
            player.SetRoundStatus(false);
            player.State = PlayerState.Locked;
        }
        isRoundFinished = true;

        // Perform round-end logic (e.g., calculate scores)
        CountDownText.text = "Round Over!";
        collectablePool.ReclaimAllCollectables();
        collectablePool.PauseSpawning();
        EndMenu.SetActive(true);
        GameHUD.SetActive(false);
    }


    public void RerunRound()
    {
           GameHUD.SetActive(true);
        // Reset round state
             isRoundFinished = false;
            CountDownText.text = "Rerunning round...";

            // Reset players
            foreach (PlayerData player in gridSystem.playersList)
            {
                player.ResetPlayer();
                player.State = PlayerState.Locked; // Ensure players are locked until the new round starts
                player.SetRoundStatus(false);
            }

            // Reset collectable pool
            collectablePool.ReclaimAllCollectables();
            collectablePool.ResumeSpawning();

            // Trigger reset event
            OnReset?.Invoke();

            // Start the pre-match countdown again
            timeController.Countdown(4, StartGame);
       
    }
}
