using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    // The events sent when the state of the game changes (on main menu, in game and on game over)
    public delegate void GameStateEvent();

    public static GameStateEvent OnTitleScreen;
    public static GameStateEvent OnGameStart;
    public static GameStateEvent OnGameOver;

    // Using singleton pattern for the game manager as many elements in the game need the player's reference, the state of the game and the time elapsed since the start of the game
    public static GameManager instance = null;

    public enum GameState
    {
        TitleScreen,
        InGame,
        GameOver
    }

    //The canvas of the different screen (main menu, in game, on game over)
    public GameObject m_inGameScreen;
    public GameObject m_mainMenuScreen;
    public GameObject m_gameOverScreen;

    public GameObject m_player;
    public float m_gameTime;
    public GameState m_gameState = GameState.TitleScreen;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    private void OnEnable()
    {
        Player.OnPlayerDeath += GameOver;
    }

    private void OnDisable()
    {
        Player.OnPlayerDeath -= GameOver;
    }

    private void Start()
    {
        ToTitleScreen();
    }

    private void Update()
    {
        if (m_gameState == GameState.InGame)
            m_gameTime += Time.deltaTime;
    }


    /// <summary>
    /// Called by button on main menu to start game
    /// </summary>
    public void StartGame()
    {
        m_gameState = GameState.InGame;
        m_mainMenuScreen.SetActive(false);
        m_inGameScreen.SetActive(true);
        m_gameOverScreen.SetActive(false);
        BroadcastGameStartEvent();
    }

    /// <summary>
    /// Called when the player dies
    /// </summary>
    public void GameOver()
    {
        m_gameState = GameState.GameOver;
        m_mainMenuScreen.SetActive(false);
        m_inGameScreen.SetActive(false);
        m_gameOverScreen.SetActive(true);
        BroadcastGameOverEvent();
    }

    /// <summary>
    /// Called when the player returns to the main menu with the button on the game over screen
    /// </summary>
    public void ToTitleScreen()
    {
        m_gameState = GameState.TitleScreen;
        m_mainMenuScreen.SetActive(true);
        m_inGameScreen.SetActive(false);
        m_gameOverScreen.SetActive(false);
        BroadcastTitleScreenEvent();
    }


    #region BROADCASTERS
    private void BroadcastTitleScreenEvent()
    {
        if (OnTitleScreen != null)
            OnTitleScreen();
    }

    private void BroadcastGameStartEvent()
    {
        if (OnGameStart != null)
            OnGameStart();
    }

    private void BroadcastGameOverEvent()
    {
        if (OnGameOver != null)
            OnGameOver();
    }
    #endregion
}
