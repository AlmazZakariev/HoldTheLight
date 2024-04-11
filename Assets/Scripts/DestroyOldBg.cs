using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOld : MonoBehaviour
{
    public float topBound = 20;
    public float topBoundDelta = 40;
    private GameManager gameManagerScript;
    // Start is called before the first frame update
    void Start()
    {
        gameManagerScript = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTopBound();
        if (transform.position.y > topBound)
        {
            Destroy(gameObject);
        }
    }
    private void UpdateTopBound()
    {
        topBound = gameManagerScript.nextYPosition + topBoundDelta;
    }
}
