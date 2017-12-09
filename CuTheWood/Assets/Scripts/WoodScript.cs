using UnityEngine;
using System.Collections;

public class WoodScript : MonoBehaviour
{
    float _speedOfMovement = 15f;

    public bool IsSliding;

    public static WoodScript Instance { get; private set; }

    void Awake()
    {
        Instance = this;
        IsSliding = false;
    }

    public IEnumerator SlidingMove(GameObject targetNode)
    {
        Vector2 positionToGlide;

        IsSliding = true;

        positionToGlide = targetNode.transform.position;

        yield return new WaitForFixedUpdate();

        while (transform.position.y >= positionToGlide.y)
        {
             Vector2 tempPosition = transform.position;
             tempPosition.y -= _speedOfMovement * Time.deltaTime;
             transform.position = tempPosition;
             yield return new WaitForFixedUpdate();
        }
        IsSliding = false;
    } 
}
