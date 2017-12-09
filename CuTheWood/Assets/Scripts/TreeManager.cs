using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TreeManager : MonoBehaviour
{
    List<WoodScript> _deactiveWoods;
    List<GameObject> _nodes;
    float _speedOfSlicedMovement;
    Quaternion _rotationLeft;
    Quaternion _rotationRight;
    Vector2 _nodeForMovement;

    int _randomizer;

    public int AmountOfPooledObjects;
    public List<WoodScript> ActiveWoods;
    public GameObject BottomWood;
    public GameObject WoodWithLeftBranch;
    public GameObject WoodWithRightBranch;
    public GameObject WoodWithNoBranch;

    public static TreeManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
        ActiveWoods = new List<WoodScript>();
        _nodes = new List<GameObject>();
        _deactiveWoods = new List<WoodScript>();
        _speedOfSlicedMovement = 20f;
        _rotationLeft = new Quaternion(0, 0, -45, 6);
        _rotationRight = new Quaternion(0, 0, 45, 6);
        _nodeForMovement = new Vector2(5, 2);
    }

    void Start()
    {
        ObjectPooling();
        SetInitialPositions();
    }

    void ObjectPooling()
    {
        // instantiating the tree's bottom wood
        Instantiate(BottomWood);

        // instantiating the first wood (I need a wood with no branch only for the first initialization)
        GameObject obj = Instantiate(WoodWithNoBranch);
        _deactiveWoods.Add(obj.GetComponent<WoodScript>());

        // instantiating the pool
        for (int i = 1; i < AmountOfPooledObjects; i++)
        {
            if(obj.gameObject.name == "WoodWithLeftBranch(Clone)" || obj.gameObject.name == "WoodWithRightBranch(Clone)")
            {
                // after every wood with a branch, I need a wood with no branch to instantiate
                    obj = Instantiate(WoodWithNoBranch);
                    _deactiveWoods.Add(obj.GetComponent<WoodScript>());
                    obj.SetActive(false);
            }
            else
            {
                obj = InstantiateRandomPrefab();
                _deactiveWoods.Add(obj.GetComponent<WoodScript>());
                obj.SetActive(false);
            }
        }

        // instantiating nodes (tree's backbone)
        for (int i = 0; i < AmountOfPooledObjects; i++)
        {
            GameObject node = new GameObject();
            _nodes.Add(node);
        }
    }

    GameObject InstantiateRandomPrefab()
    {
        _randomizer = Random.Range(0, 4);

        // there are more chance to get wood with a brach than wood with no branch because I already instantiate wood with no branch between every branched wood
        if (_randomizer == 0 || _randomizer == 1)
        {
            GameObject obj = Instantiate(WoodWithRightBranch);
            return obj;
        }

        else if (_randomizer == 2 ||_randomizer == 3)
        {
            GameObject obj = Instantiate(WoodWithLeftBranch);
            return obj;
        }

        else
        {
            GameObject obj = Instantiate(WoodWithNoBranch);
            return obj;
        }
    }

    void SetInitialPositions()
    {
        for (int i = 0; i < _deactiveWoods.Count; i++)
        {
            // for the first wood (bottom wood)
            if (i == 0)         
                SetFirstPositions(BottomWood.GetComponent<WoodScript>(), _deactiveWoods[i]);

            // setting the initial positions for objects
            else
                SetFirstPositions(_deactiveWoods[i - 1], _deactiveWoods[i]);

            _nodes[i].transform.position = _deactiveWoods[i].gameObject.transform.position;
            _deactiveWoods[i].gameObject.SetActive(true);
            ActiveWoods.Add(_deactiveWoods[i]);
        }
        _deactiveWoods.Clear();
    }

    void SetFirstPositions(WoodScript previousWood, WoodScript currentWood)
    {
        // this piece of code will set the current wood's position according to previous wood's position
        currentWood.transform.position = new Vector2(previousWood.transform.position.x,
                                                        previousWood.GetComponent<SpriteRenderer>().bounds.max.y +
                                                        currentWood.GetComponent<SpriteRenderer>().bounds.extents.y);
    }

    void SetExactPositions(GameObject targetNode, WoodScript currentWood)
    {
        currentWood.gameObject.transform.position = targetNode.transform.position;
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            SliceMovement();
            StartCoroutine(DestroyFirstObj());
        }
    }

    bool CheckReadyForDestroy()
    {
        for (int i = 0; i < ActiveWoods.Count; i++)
        {
            // if any wood is still sliding
            if (ActiveWoods[i].IsSliding)       
                return false;
        }
        // if all of the woods finished sliding
        return true;
    }

    void StopAllSlidingMoves()
    {
        // if A || D key pressed again
        for (int i = 0; i < ActiveWoods.Count; i++)
        {
            ActiveWoods[i].StopAllCoroutines();
            ActiveWoods[i].IsSliding = false;

            // for the first wood
            if (i == 0)         
                SetExactPositions(_nodes[i], ActiveWoods[i]);

            else
                SetExactPositions(_nodes[i - 1], ActiveWoods[i]);
        }
    }

    void CheckStatusOfPlayer()
    {
        // if the first wood's branch and the player is on the same side
        if (Game.Instance.CheckForBranchAndPlayer())
            PlayerScript.Instance.Die();
            // :(
    }

    IEnumerator DestroyFirstObj()
    {
        if(PlayerScript.Instance.InItsPosition)
        {
            CheckStatusOfPlayer();

            ActiveWoods[0].gameObject.SetActive(false);

            yield return new WaitForFixedUpdate();

            if (!CheckReadyForDestroy())
                StopAllSlidingMoves();

            _deactiveWoods.Add(ActiveWoods[0]);

            ActiveWoods.RemoveAt(0);

            ActiveWoods.Add(GetObjectFromNonActiveWoods());

            SetFirstPositions(ActiveWoods[ActiveWoods.Count - 2], ActiveWoods[ActiveWoods.Count - 1]);

            ActiveWoods[ActiveWoods.Count - 1].gameObject.SetActive(true);

            UpdatePositionsAfterDeactivating();

            CheckStatusOfPlayer();

            IncreaseScoreAndTimeBar();
        }
    }

     void IncreaseScoreAndTimeBar()
    {
        if (UIControl.Instance.CurrentGameState != GameState.GameIsOver)
        {
            UIControl.Instance.IncreaseTimeBar();

            // everytime the user cuts one wood, gains 1 score
            PlayerScript.Instance.score++;
            PlayerPrefs.SetInt("score", PlayerScript.Instance.score);
        }
    }

   
    void SliceMovement()
    {
        if(PlayerScript.Instance.InItsPosition)
        {
            PlayerScript.Instance.PlayAnimation();
            GameObject slicedWood = Instantiate(WoodWithNoBranch);
            slicedWood.transform.position = _nodes[0].transform.position;
            slicedWood.SetActive(true);

            if (PlayerScript.Instance.OnLeft)
            {
                slicedWood.transform.rotation = _rotationLeft;
                StartCoroutine(MakeSlicedMovement(slicedWood, true));
            }

            else if (PlayerScript.Instance.OnRight)
            {
                slicedWood.transform.rotation = _rotationRight;
                StartCoroutine(MakeSlicedMovement(slicedWood, false));
            }
        }
    }

    // makes the wood's appereance like its sliced
    IEnumerator MakeSlicedMovement(GameObject tempSlicedWood, bool isRightMovement)
    {
        float movementAmount = isRightMovement ? _speedOfSlicedMovement * Time.deltaTime : -_speedOfSlicedMovement * Time.deltaTime;
        Vector2 tempPosition = tempSlicedWood.transform.position;
        Vector2 targetNode = new Vector2(_nodeForMovement.x * movementAmount * 2, _nodeForMovement.y);

        while (Vector2.Distance(tempSlicedWood.transform.position, targetNode) >= 0.1f)
        {
            tempPosition.x += movementAmount;
            tempSlicedWood.transform.position = tempPosition;
            yield return new WaitForFixedUpdate();
        }

        Destroy(tempSlicedWood);
    }

    WoodScript GetObjectFromNonActiveWoods()
    {
        _randomizer = Random.Range(0, _deactiveWoods.Count);

        WoodScript tempWood = _deactiveWoods[_randomizer];

        _deactiveWoods.Remove(_deactiveWoods[_randomizer]);

        return tempWood;
    }

    // updates other woods' positions, after deactivating the first wood from the scene
    void UpdatePositionsAfterDeactivating()
    {
        // since the bottom wood should remains still, it won't update the bottom wood's position
        for (int i = 0; i < ActiveWoods.Count; i++)
        {                                                
            ActiveWoods[i].StartCoroutine(ActiveWoods[i].SlidingMove(_nodes[i]));
        }
    }
}
