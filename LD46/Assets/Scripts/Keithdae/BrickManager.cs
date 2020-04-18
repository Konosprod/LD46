using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickManager : MonoBehaviour
{
    public GameObject brickNormalHorizontalPrefab;
    public GameObject brickNormalVerticalPrefab;

    public Transform brickHolder;

    public long brickPoints = 50;

    private LayerMask ballMask;

    // Start is called before the first frame update
    void Start()
    {
        ballMask = LayerMask.GetMask("Ball");
    }

    // Update is called once per frame
    void Update()
    {
        // Left-click => horizontal brick
        if (Input.GetMouseButtonDown(0) && CheckSpace())
        {
            Vector3 brickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            brickPosition.z = 0;
            Instantiate(brickNormalHorizontalPrefab, brickPosition, brickNormalHorizontalPrefab.transform.rotation, brickHolder);
        }

        // Right-click => vertical brick
        if (Input.GetMouseButtonDown(1) && CheckSpace())
        {
            Vector3 brickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            brickPosition.z = 0;
            Instantiate(brickNormalVerticalPrefab, brickPosition, brickNormalVerticalPrefab.transform.rotation, brickHolder);
        }

    }


    // True if the space if ball-free, false otherwise
    private bool CheckSpace()
    {
        bool res = true;

        Vector2 checkPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Collider2D collider = Physics2D.OverlapCircle(checkPosition, 2f, ballMask);
        if (collider != null)
            res = false;

        return res;
    }
}
