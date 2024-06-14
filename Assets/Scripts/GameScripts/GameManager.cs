using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // Singleton instance
    public InventoryMenuUI inventoryMenuUI;
    public enum GameState
    {
        Playing,
        Paused,
        Inventory,
        GameOver
    }
    public GameState gameState;

    // UI elements for the pause menu
    public GameObject pauseMenu;
    public GameObject playerUI;
    public GameObject Invenotory;
    public Button resumeButton;
    public Button arrowButton;
    public Button quitButton;
    public CinemachineBrain cameraBrain;
    private void Awake()
    {
        // Ensure only one instance of the GameManager exists
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Initialize game state and UI

        gameState = GameState.Playing;
        Time.timeScale = 1;
        // Set up button click listeners for the pause menu
        resumeButton.onClick.AddListener(ResumeGame);
        arrowButton.onClick.AddListener(ResumeGame);
        quitButton.onClick.AddListener(QuitGame);

        // Hide pause menu initially
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
    }

    private void Update()
    {
        // Check for pause condition (for example, when the player presses the 'P' key)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }

        if (gameState == GameState.Paused || gameState == GameState.Inventory)
        {
            Time.timeScale = 0f; // Pause the game
        }

    }

    public void TogglePause()
    {
        if (gameState == GameState.Playing)
        {
            gameState = GameState.Paused;
            Time.timeScale = 0f; // Pause the game
            if(cameraBrain != null)
            cameraBrain.enabled = false;
            Cursor.lockState = CursorLockMode.None; // Unlock the cursor
            Cursor.visible = true; // Make the cursor visible
            // Show pause menu
            if (pauseMenu != null)
            {
                pauseMenu.SetActive(true);
                playerUI.SetActive(false);
            }
        }
        else if (gameState == GameState.Paused)
        {
            ResumeGame();
        }
    }
    public void ToggleInventory()
    {
        if (gameState == GameState.Playing)
        {
            inventoryMenuUI.RefreshMenu();
            gameState = GameState.Inventory;
            Time.timeScale = 0f; // Pause the game
            cameraBrain.enabled = false;

            Cursor.lockState = CursorLockMode.None; // Unlock the cursor
            Cursor.visible = true; // Make the cursor visible
            // Show pause menu
            if (Invenotory != null)
            {
                pauseMenu.SetActive(false);
                Invenotory.SetActive(true);
                playerUI.SetActive(false);
            }
        }
       
    }

   public void ResumeGame()
    {
      
        Time.timeScale = 1f; // Unpause the game
        cameraBrain.enabled = true;
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor
        Cursor.visible = false; // Make the cursor invisible
        // Hide pause menu
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
            playerUI.SetActive(true);
            Invenotory.SetActive(false);
        }
        gameState = GameState.Playing;
    }

    public void EndGame()
    {
        // Perform actions when the game is over, for example, show a game over screen
        gameState = GameState.GameOver;
        Time.timeScale = 1f; // Reset time scale to normal in case the game was paused
        Debug.Log("Game Over!");
    }

    void QuitGame()
    {
        // Add any additional actions before quitting the game (saving progress, etc.)
        Application.Quit();
    }
}
