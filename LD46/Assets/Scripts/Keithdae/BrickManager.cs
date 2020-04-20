using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrickManager : MonoBehaviour
{

    [Header("Brick prefabs")]
    public GameObject brickNormalHorizontalPrefab;
    public GameObject brickNormalVerticalPrefab;

    public GameObject brickIceHorizontalPrefab;
    public GameObject brickIceVerticalPrefab;

    public GameObject brickWideHorizontalPrefab;
    public GameObject brickWideVerticalPrefab;

    public Transform brickHolder;

    public long brickPoints = 50;

    private LayerMask ballMask;

    [HideInInspector]
    public Brick.BrickType selectedBrickType = Brick.BrickType.Normal;
    private GameObject selectedBrickTypeHorizontalPrefab;
    private GameObject selectedBrickTypeVerticalPrefab;


    // Start is called before the first frame update
    void Start()
    {
        ballMask = LayerMask.GetMask("Ball");
        selectedBrickTypeHorizontalPrefab = brickNormalHorizontalPrefab;
        selectedBrickTypeVerticalPrefab = brickNormalVerticalPrefab;
    }

    // Update is called once per frame
    void Update()
    {
        // Left-click => horizontal brick
        if (Input.GetMouseButtonDown(0) && CheckSpace())
        {
            Vector3 brickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            brickPosition.z = 0;
            Instantiate(selectedBrickTypeHorizontalPrefab, brickPosition, brickNormalHorizontalPrefab.transform.rotation, brickHolder);
        }

        // Right-click => vertical brick
        if (Input.GetMouseButtonDown(1) && CheckSpace())
        {
            Vector3 brickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            brickPosition.z = 0;
            Instantiate(selectedBrickTypeVerticalPrefab, brickPosition, brickNormalVerticalPrefab.transform.rotation, brickHolder);
        }


        // 1 is normal brick type
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedBrickType = Brick.BrickType.Normal;
            selectedBrickTypeHorizontalPrefab = brickNormalHorizontalPrefab;
            selectedBrickTypeVerticalPrefab = brickNormalVerticalPrefab;
        }

        // 2 is ice brick type
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedBrickType = Brick.BrickType.Ice;
            selectedBrickTypeHorizontalPrefab = brickIceHorizontalPrefab;
            selectedBrickTypeVerticalPrefab = brickIceVerticalPrefab;
        }

        // 3 is wide brick type
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedBrickType = Brick.BrickType.Wide;
            selectedBrickTypeHorizontalPrefab = brickWideHorizontalPrefab;
            selectedBrickTypeVerticalPrefab = brickWideVerticalPrefab;
        }
    }


    // True if the space if ball-free, false otherwise
    private bool CheckSpace()
    {
        bool res = true;

        Vector2 checkPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Collider2D collider = Physics2D.OverlapCircle(checkPosition, 1f, ballMask);
        if (collider != null)
            res = false;

        return res;
    }
}
