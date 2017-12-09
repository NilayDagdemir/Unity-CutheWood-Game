using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour 
{
    bool _onRight;
    bool _onLeft;
    Animator _anim;

    public bool InItsPosition;
    public int score;
    public bool KeyPressed;

    public bool OnRight { get { return _onRight;  } }

    public bool OnLeft { get { return _onLeft; } }

    public static PlayerScript Instance { get; private set; }

	void Awake ()
    {
        Instance = this;
        
        // default pos of the player
        _onLeft = false;
        _onRight = true;
        score = 0;
        PlayerPrefs.SetInt("score", score);
        _anim = GetComponent<Animator>();
    }

	void Update () 
    {
        if (UIControl.Instance.CurrentGameState == GameState.IsContinuous)
        {
            if (Input.GetKeyDown(KeyCode.A))
                MoveLeft();

            else if (Input.GetKeyDown(KeyCode.D))
                MoveRight();
        }
    }

    void MoveLeft()
    {
        if (transform.position.x > 0)
        {
            InItsPosition = false;
            transform.position = new Vector2(-2.4f, 2);
            PlayerScript.Instance.GetComponent<SpriteRenderer>().flipX = true;
        }

        // the player is on left
        _onLeft = true;
        _onRight = false;
        InItsPosition = true;
    }

    void MoveRight()
    {
        if (transform.position.x < 0)
        {
            InItsPosition = false;
            transform.position = new Vector2(2.3f, 2);
            PlayerScript.Instance.GetComponent<SpriteRenderer>().flipX = false;
        }

        // the player is on right
        _onRight = true;
        _onLeft = false;
        InItsPosition = true;
    }

    public void PlayAnimation()
    {
        if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Cut"))
            _anim.SetTrigger("Idle");

        else
        {
            _anim.SetTrigger("Cut");
            _anim.SetTrigger("Idle");
        }

    }

    public void Die()
    {
        Game.Instance.ShowGameOverScreen();
        // erase player from the scene, since its dead..
        gameObject.SetActive(false);
    }
}
