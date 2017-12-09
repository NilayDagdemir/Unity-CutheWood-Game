using UnityEngine;
using UnityEngine.UI;

public class UIControl : MonoBehaviour {

    float _decreaseAmount;

    public GameObject Player;
    public Image TimeBar;
    public GameState CurrentGameState;

    public static UIControl Instance { get; private set; }

    void Awake()
    {
       Instance = this;
       _decreaseAmount = 0.2f;
       CurrentGameState = GameState.IsContinuous;
    }

    void Update()
    {
        if (TimeBar != null && CurrentGameState == GameState.IsContinuous)
        {
            TimeBar.fillAmount -= _decreaseAmount * Time.deltaTime;

            // time is out
            if (TimeBar.fillAmount < 0.01f)
                Game.Instance.ShowGameOverScreen();
        }
    }

    public void IncreaseTimeBar()
    {
        if (TimeBar != null)
             TimeBar.fillAmount += 0.2f;
    }

    public void ChangeScene(string sceneName)
    {   
        // after hitting the "retry" button, the user can play the game again
        Application.LoadLevel(sceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
