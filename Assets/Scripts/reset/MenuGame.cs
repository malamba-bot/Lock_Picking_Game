using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuManager : MonoBehaviour
{
    public static GameMenuManager Instance;

    // this variable survives scene loads! it carries your win/loss result to the end screen.
    public static bool didPlayerWin = false;

    [Header("Exact Scene Names")]
    public string mainMenuSceneName = "MainMenu";
    public string gameSceneName = "GameScene";
    public string endSceneName = "EndScene";

    [Header("Main Menu UI (Only assign in Main Menu)")]
    public GameObject instructionsPanel; 

    [Header("End Scene UI (Only assign in End Scene)")]
    public GameObject victoryText;

    public GameObject gameOverText;

    void Awake()
    {
        Instance = this; 
    }

    void Start()
    {
        //Logic for the End Scene
        if (SceneManager.GetActiveScene().name == endSceneName)
        {
            if (didPlayerWin == true) 
            {
                if (victoryText) victoryText.SetActive(true);
                if (gameOverText) gameOverText.SetActive(false);
            } 
            else 
            {
                if (victoryText) victoryText.SetActive(false);
                if (gameOverText) gameOverText.SetActive(true);
            }
        }

        //Logic for the Main Menu
        if (SceneManager.GetActiveScene().name == mainMenuSceneName)
        {
            if (instructionsPanel) instructionsPanel.SetActive(false);
        }
    }
    
    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void ShowInstructions()
    {
        if (instructionsPanel) instructionsPanel.SetActive(true);
    }

    public void HideInstructions()
    {
        if (instructionsPanel) instructionsPanel.SetActive(false);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void ShowVictory()
    {
        didPlayerWin = true; 
        SceneManager.LoadScene(endSceneName); 
    }

    public void ShowGameOver()
    {
        didPlayerWin = false; 
        SceneManager.LoadScene(endSceneName); 
    }
}