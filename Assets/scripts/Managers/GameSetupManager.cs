using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSetupManager : MonoBehaviour
{
    [SerializeField] private GameData gameData;
    [SerializeField] private GameObject PlayerUIPrefab;
    [SerializeField] private GameObject PlayerPanel;
    [SerializeField] private GridSystem gridSystem;
    [SerializeField] private AbilityPoolManager AbilityPoolManager;
    [SerializeField] private CollectablePoolManager CollectablePool;
    [SerializeField] private GameManager gameManager;

    private List<PlayerData> allPlayers = new List<PlayerData>();
    private const int numberOfAICharacters = 3;

    private async void Awake()
    {
        bool gridInitialized = await gridSystem.InitializeGridAsync();
        if (gridInitialized)
        {
 
            SetupPlayers();
        }
        else
        {
            Debug.LogError("GridSystem initialization failed. Aborting setup.");
        }
    }

    private void SetupPlayers()
    {
        try
        {
            AddUserPlayer();
            AddAIPlayers();
            AssignOpponents();
            gridSystem.HandlePlayerManager();

           
            StartCoroutine(StartGame());
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"SetupPlayers failed: {ex.Message}");
            throw;
        }
    }

    private void AddUserPlayer()
    {
        // Instantiate and position the user player
        Vector3 userWorldPosition = gridSystem.GetWorldPositionFromGrid(gameData.userStartPosition);
        PlayerData userPlayer = Instantiate(gameData.userCharacterPrefabs[gameData.selectedCharacterIndex], userWorldPosition, Quaternion.identity).GetComponent<PlayerData>();

        // Instantiate the PlayerUI and assign it to the PlayerPanel
        GameObject playerUI = Instantiate(PlayerUIPrefab, PlayerPanel.transform);
        PlayerUIInfo playerUIInfo = playerUI.GetComponent<PlayerUIInfo>();

        if (playerUIInfo != null)
        {
            userPlayer.playerUIInfo = playerUIInfo;
            playerUIInfo.playerFace.sprite = gameData.characterSprites[gameData.selectedCharacterIndex];
        }
        else
        {
            Debug.LogWarning("PlayerUI does not have an Image component.");
        }

        userPlayer.ImageIndex = 0;
        allPlayers.Add(userPlayer);
    }

    private void AddAIPlayers()
    {
        int aiSlot = 0; // Tracks the position in the grid for AI players

        for (int index = 0; index < gameData.aiCharacterPrefabs.Length; index++)
        {
            // Skip the user-selected character and ensure no duplicate colors
            if (index == gameData.selectedCharacterIndex || aiSlot >= numberOfAICharacters)
            {
                continue;
            }

            // Instantiate and position the AI player
            Vector3 aiWorldPosition = gridSystem.GetWorldPositionFromGrid(gameData.aiStartPositions[aiSlot]);
            PlayerData aiPlayer = Instantiate(gameData.aiCharacterPrefabs[index], aiWorldPosition, Quaternion.identity).GetComponent<PlayerData>();

            // Instantiate the PlayerUI and assign it to the PlayerPanel
            GameObject playerUI = Instantiate(PlayerUIPrefab, PlayerPanel.transform);
            PlayerUIInfo playerUIInfo = playerUI.GetComponent<PlayerUIInfo>();

            if (playerUIInfo != null)
            {
                aiPlayer.playerUIInfo = playerUIInfo;
                playerUIInfo.playerFace.sprite = gameData.characterSprites[index]; // Assign the correct sprite
            }
            else
            {
                Debug.LogWarning("PlayerUI does not have the required PlayerUIInfo component.");
            }

            aiPlayer.ImageIndex = aiSlot + 1; // Assign a unique UI index for the AI
            allPlayers.Add(aiPlayer); // Add the AI player to the player list
            aiSlot++; // Move to the next AI slot
        }
    }
    private void AssignOpponents()
    {
        foreach (var player in allPlayers)
        {
            player.OpponentsWithUI = allPlayers
                .Where(opponent => opponent != player)
                .Select(opponent => (opponent, opponent.ImageIndex))
                .ToList();
        }
    }

    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(1);
        gameManager.OnSetupComplete();
        AbilityPoolManager.Initialize();
        CollectablePool.Initialize();
    }
}
