using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrickManager : MonoBehaviour
{
    public static BrickManager _instance;


    [Header("Brick prefabs")]
    public GameObject brickNormalHorizontalPrefab;
    public GameObject brickNormalVerticalPrefab;

    public GameObject brickIceHorizontalPrefab;
    public GameObject brickIceVerticalPrefab;

    public GameObject brickWideHorizontalPrefab;
    public GameObject brickWideVerticalPrefab;

    public Transform brickHolder;


    [HideInInspector]
    public long initialBrickPoints = 10;
    private long brickPoints = 10;
    [HideInInspector]
    public int brickGen = 1;       // Bricks earned per second

    private float bpGenTimer = 1f;

    [HideInInspector]
    public int normalBrickCost = 1;
    [HideInInspector]
    public int iceBrickCost = 10;
    [HideInInspector]
    public int wideBrickCost = 4;


    [Header("UI")]
    public Text BrickPointText;


    private LayerMask ballMask;

    [HideInInspector]
    public Brick.BrickType selectedBrickType = Brick.BrickType.Normal;
    private GameObject selectedBrickTypeHorizontalPrefab;
    private GameObject selectedBrickTypeVerticalPrefab;


    private void Awake()
    {
        if (_instance != null)
            Destroy(gameObject);
        else
            _instance = this;
    }

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

        /// Inputs
        // Left-click => horizontal brick
        if (Input.GetMouseButtonDown(0) && CheckSpace() && CheckCost())
        {
            Vector3 brickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            brickPosition.y = brickPosition.y > 10f ? 10f : brickPosition.y;
            brickPosition.z = 0;
            Instantiate(selectedBrickTypeHorizontalPrefab, brickPosition, brickNormalHorizontalPrefab.transform.rotation, brickHolder);

            BuyBrick();
        }

        // Right-click => vertical brick
        if (Input.GetMouseButtonDown(1) && CheckSpace() && CheckCost())
        {
            Vector3 brickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            brickPosition.y = brickPosition.y > 10f ? 10f : brickPosition.y;
            brickPosition.z = 0;
            Instantiate(selectedBrickTypeVerticalPrefab, brickPosition, brickNormalVerticalPrefab.transform.rotation, brickHolder);

            BuyBrick();
        }


        // 1 is normal brick type
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedBrickType = Brick.BrickType.Normal;
            selectedBrickTypeHorizontalPrefab = brickNormalHorizontalPrefab;
            selectedBrickTypeVerticalPrefab = brickNormalVerticalPrefab;
            UiManager._instance.SelectNormalBrick();
        }

        // 2 is ice brick type
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedBrickType = Brick.BrickType.Ice;
            selectedBrickTypeHorizontalPrefab = brickIceHorizontalPrefab;
            selectedBrickTypeVerticalPrefab = brickIceVerticalPrefab;
            UiManager._instance.SelectIceBrick();
        }

        // 3 is wide brick type
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedBrickType = Brick.BrickType.Wide;
            selectedBrickTypeHorizontalPrefab = brickWideHorizontalPrefab;
            selectedBrickTypeVerticalPrefab = brickWideVerticalPrefab;
            UiManager._instance.SelectWideBrick();
        }


        /// UI
        UiManager._instance.UpdateBpText("BP : " + brickPoints.ToString() + " (" + brickGen.ToString() + "/s)");


        /// BP Gen
        if (GameManager._instance.isGameActive)
        {
            bpGenTimer -= Time.deltaTime;
            if (bpGenTimer <= 0f)
            {
                brickPoints += brickGen;
                bpGenTimer = 1f;
            }
        }
    }


    public void SetupLevel()
    {
        brickPoints = initialBrickPoints;
        bpGenTimer = 1f;
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


    // True if brick can be afforded
    private bool CheckCost()
    {
        bool res = false;
        switch (selectedBrickType)
        {
            case Brick.BrickType.Normal:
                res = brickPoints >= normalBrickCost;
                break;
            case Brick.BrickType.Ice:
                res = brickPoints >= iceBrickCost;
                break;
            case Brick.BrickType.Wide:
                res = brickPoints >= wideBrickCost;
                break;
            default:
                Debug.LogError("Wrong brick type : " + selectedBrickType);
                break;
        }

        return res;
    }


    // Spends bp for the brick
    private void BuyBrick()
    {
        switch (selectedBrickType)
        {
            case Brick.BrickType.Normal:
                brickPoints -= normalBrickCost;
                break;
            case Brick.BrickType.Ice:
                brickPoints -= iceBrickCost;
                break;
            case Brick.BrickType.Wide:
                brickPoints -= wideBrickCost;
                break;
            default:
                Debug.LogError("Wrong brick type : " + selectedBrickType);
                break;
        }
    }
}
