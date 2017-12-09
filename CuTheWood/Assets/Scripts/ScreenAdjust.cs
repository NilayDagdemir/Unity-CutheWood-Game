using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScreenAdjust : MonoBehaviour
{
    List<GameObject> _grasses;
    List<GameObject> _groundElements;
    List<GameObject> _decorTrees;
    Vector3 _screenBoundaries;
    Vector3 _tempPos;
    int _randomizer;

    public GameObject Grass;
    public GameObject Ground;
    public GameObject DecorTree1;
    public GameObject DecorTree2;
    public GameObject DecorTree3;

    void Awake()
    {
        _grasses = new List<GameObject>();
        _groundElements = new List<GameObject>();
        _decorTrees = new List<GameObject>() { DecorTree1, DecorTree2, DecorTree3 };
        _screenBoundaries = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        InitializeTextures(Ground, "ground");
        InitializeTextures(Grass, "grass");
        InitializeDecorTrees(DecorTree1);
    }

    void InitializeTextures(GameObject tempGameObj, string objName)
    {
        _tempPos = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 1));
        GameObject tempObj = Instantiate(tempGameObj);

        // setting the x-axises & y-axises
        if (objName == "ground" || objName == "grass")
        {
            _tempPos.x += tempObj.GetComponent<SpriteRenderer>().bounds.extents.x;

            if (objName == "ground")
            {
                _tempPos.y += tempObj.GetComponent<SpriteRenderer>().bounds.extents.y;
                _groundElements.Add(tempObj);
            }
            else if (objName == "grass")
            {
                _tempPos.y += tempObj.GetComponent<SpriteRenderer>().bounds.extents.y * 2;
                _grasses.Add(tempObj);
            }
            tempObj.transform.position = _tempPos;
        }

        while (_tempPos.x < _screenBoundaries.x)
        {
            tempObj = Instantiate(tempGameObj);

            if (objName == "ground")
            {
                _groundElements.Add(tempObj);
                SetPositionForTextures(_groundElements[_groundElements.Count - 2], _groundElements[_groundElements.Count - 1]);
            }

            else if (objName == "grass")
            {
                _grasses.Add(tempObj);
                SetPositionForTextures(_grasses[_grasses.Count - 2], _grasses[_grasses.Count - 1]);
            }
        }
    }

    void InitializeDecorTrees(GameObject tempGameObj)
    {
        _tempPos = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 1));
        GameObject tempObj = Instantiate(tempGameObj);

        // setting decortree's x-axis and y-scale according to the height of the screen
        _tempPos.x += tempObj.GetComponent<SpriteRenderer>().bounds.extents.x * 3;
        _tempPos.y += tempObj.GetComponent<SpriteRenderer>().bounds.extents.y;
        tempObj.transform.position = _tempPos;

        while (_tempPos.x < _screenBoundaries.x)
        {
            _randomizer = Random.Range(0, 3);
            tempObj = Instantiate(_decorTrees[_randomizer]);
            SetPositionForTextures(tempObj);
        }
    }

    void SetPositionForTextures(GameObject decorTree)
    {
        _tempPos.x += decorTree.GetComponent<SpriteRenderer>().bounds.extents.x * 5;
        decorTree.transform.position = _tempPos;
    }

    void SetPositionForTextures(GameObject previousTexture, GameObject currentTexture)
    {
        _tempPos = previousTexture.transform.position;
        _tempPos.x += currentTexture.GetComponent<SpriteRenderer>().bounds.extents.x * 2;
        currentTexture.transform.position = _tempPos;
    }
}
