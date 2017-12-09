using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Game : MonoBehaviour {

    bool _newHighScore;

    public Text ScoreText;
    public Text HighScoreText;
    public Canvas GameOverCanvas;
    public Text NewHighScoreText;
    public Text GameOverText;

    public static Game Instance { get; private set; }

	void Awake ()
    {
        Instance = this;
        _newHighScore = false;
    }

    void Update ()
    {
        ShowScore();

        if (PlayerPrefs.GetInt("score") > PlayerPrefs.GetInt("highscore"))
            _newHighScore = true;

        ShowHighScore();
    }

    // method for Checking if branch is in player's position
    public bool CheckForBranchAndPlayer()
    {
        if (TreeManager.Instance.ActiveWoods[0].gameObject.name == "WoodWithLeftBranch(Clone)" && PlayerScript.Instance.OnLeft)
            return true;
            
        else if (TreeManager.Instance.ActiveWoods[0].gameObject.name == "WoodWithRightBranch(Clone)" && PlayerScript.Instance.OnRight)
            return true;
           
        return false;
    }

    void ShowScore()
    {
        ScoreText.text = "Score: " + PlayerPrefs.GetInt("score");
    }

    void ShowHighScore()
    {      
        HighScoreText.text = "Highscore: " + PlayerPrefs.GetInt("highscore");
    }

    public void ShowGameOverScreen()
    {
        TreeManager.Instance.enabled = false;
        UIControl.Instance.CurrentGameState = GameState.GameIsOver;

        if (_newHighScore)
        {
            PlayerPrefs.SetInt("highscore", PlayerPrefs.GetInt("score"));
            GameOverText.gameObject.SetActive(false);
            NewHighScoreText.text += " " + PlayerPrefs.GetInt("score");
        }
        else
            NewHighScoreText.gameObject.SetActive(false);

        // show the game over screen
        GameOverCanvas.gameObject.SetActive(true);
    }
}
