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


    private GameObject previewBrick;
    private GameObject previewBrickNormalH;
    private GameObject previewBrickNormalV;
    private GameObject previewBrickIceH;
    private GameObject previewBrickIceV;
    private GameObject previewBrickWideH;
    private GameObject previewBrickWideV;
    public GameObject previewRedCross;
    private float alphaFactorPreview = 0.25f;
    private bool isHorizontal = true;

    [HideInInspector]
    public long initialBrickPoints = 10;
    public long brickPoints = 10;

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


        previewBrickNormalH = Instantiate(brickNormalHorizontalPrefab, Vector3.zero, brickNormalHorizontalPrefab.transform.rotation, transform);
        previewBrickNormalH.GetComponent<Collider2D>().enabled = false;
        SpriteRenderer srpbnh = previewBrickNormalH.GetComponent<SpriteRenderer>();
        srpbnh.color = new Color(srpbnh.color.r, srpbnh.color.g, srpbnh.color.b, alphaFactorPreview);
        previewBrickNormalH.SetActive(false);

        previewBrickNormalV = Instantiate(brickNormalVerticalPrefab, Vector3.zero, brickNormalVerticalPrefab.transform.rotation, transform);
        previewBrickNormalV.GetComponent<Collider2D>().enabled = false;
        SpriteRenderer srpbnv = previewBrickNormalV.GetComponent<SpriteRenderer>();
        srpbnv.color = new Color(srpbnv.color.r, srpbnv.color.g, srpbnv.color.b, alphaFactorPreview);
        previewBrickNormalV.SetActive(false);

        previewBrickIceH = Instantiate(brickIceHorizontalPrefab, Vector3.zero, brickIceHorizontalPrefab.transform.rotation, transform);
        previewBrickIceH.GetComponent<Collider2D>().enabled = false;
        SpriteRenderer srpbih = previewBrickIceH.GetComponent<SpriteRenderer>();
        srpbih.color = new Color(srpbih.color.r, srpbih.color.g, srpbih.color.b, alphaFactorPreview);
        previewBrickIceH.SetActive(false);

        previewBrickIceV = Instantiate(brickIceVerticalPrefab, Vector3.zero, brickIceVerticalPrefab.transform.rotation, transform);
        previewBrickIceV.GetComponent<Collider2D>().enabled = false;
        SpriteRenderer srpbiv = previewBrickIceV.GetComponent<SpriteRenderer>();
        srpbiv.color = new Color(srpbiv.color.r, srpbiv.color.g, srpbiv.color.b, alphaFactorPreview);
        previewBrickIceV.SetActive(false);

        previewBrickWideH = Instantiate(brickWideHorizontalPrefab, Vector3.zero, brickWideHorizontalPrefab.transform.rotation, transform);
        previewBrickWideH.GetComponent<Collider2D>().enabled = false;
        SpriteRenderer srpbwh = previewBrickWideH.GetComponent<SpriteRenderer>();
        srpbwh.color = new Color(srpbwh.color.r, srpbwh.color.g, srpbwh.color.b, alphaFactorPreview);
        previewBrickWideH.SetActive(false);

        previewBrickWideV = Instantiate(brickWideVerticalPrefab, Vector3.zero, brickWideVerticalPrefab.transform.rotation, transform);
        previewBrickWideV.GetComponent<Collider2D>().enabled = false;
        SpriteRenderer srpbwv = previewBrickWideV.GetComponent<SpriteRenderer>();
        srpbwv.color = new Color(srpbwv.color.r, srpbwv.color.g, srpbwv.color.b, alphaFactorPreview);
        previewBrickWideV.SetActive(false);


        previewBrick = previewBrickNormalH;

        ChangePreviewBrick();
    }

    // Update is called once per frame
    void Update()
    {
        /// Brick preview
        Vector3 brickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        brickPosition.y = brickPosition.y > 10f ? 10f : brickPosition.y;
        brickPosition.z = 0;
        previewBrick.transform.position = brickPosition;

        bool costCheck = CheckCost();
        bool spaceCheck = CheckSpace();
        if (!costCheck || !spaceCheck)
        {
            previewRedCross.SetActive(true);
            previewRedCross.transform.position = brickPosition;
        }
        else
        {
            previewRedCross.SetActive(false);
        }


        /// Inputs
        // Left-click => horizontal brick
        if (Input.GetMouseButtonDown(0) && costCheck && spaceCheck)
        {
            if (isHorizontal)
                Instantiate(selectedBrickTypeHorizontalPrefab, brickPosition, selectedBrickTypeHorizontalPrefab.transform.rotation, brickHolder);
            else
                Instantiate(selectedBrickTypeVerticalPrefab, brickPosition, selectedBrickTypeVerticalPrefab.transform.rotation, brickHolder);

            BuyBrick();
        }

        // Right-click => swap between horizontal and vertical
        if (Input.GetMouseButtonDown(1))
        {
            isHorizontal = !isHorizontal;
            ChangePreviewBrick();
        }


        // 1 is normal brick type
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedBrickType = Brick.BrickType.Normal;
            selectedBrickTypeHorizontalPrefab = brickNormalHorizontalPrefab;
            selectedBrickTypeVerticalPrefab = brickNormalVerticalPrefab;
            UiManager._instance.SelectNormalBrick();
            ChangePreviewBrick();
        }

        // 2 is ice brick type
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedBrickType = Brick.BrickType.Ice;
            selectedBrickTypeHorizontalPrefab = brickIceHorizontalPrefab;
            selectedBrickTypeVerticalPrefab = brickIceVerticalPrefab;
            UiManager._instance.SelectIceBrick();
            ChangePreviewBrick();
        }

        // 3 is wide brick type
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedBrickType = Brick.BrickType.Wide;
            selectedBrickTypeHorizontalPrefab = brickWideHorizontalPrefab;
            selectedBrickTypeVerticalPrefab = brickWideVerticalPrefab;
            UiManager._instance.SelectWideBrick();
            ChangePreviewBrick();
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

    private void ChangePreviewBrick()
    {
        Vector3 pos = previewBrick.transform.position;
        previewBrick.SetActive(false);
        if (isHorizontal)
        {
            switch (selectedBrickType)
            {
                case Brick.BrickType.Normal:
                    previewBrick = previewBrickNormalH;
                    break;
                case Brick.BrickType.Ice:
                    previewBrick = previewBrickIceH;
                    break;
                case Brick.BrickType.Wide:
                    previewBrick = previewBrickWideH;
                    break;
                default:
                    Debug.LogError("UNKNOWN BRICK TYPE ANGRY FACE");
                    break;
            }
        }
        else
        {
            switch (selectedBrickType)
            {
                case Brick.BrickType.Normal:
                    previewBrick = previewBrickNormalV;
                    break;
                case Brick.BrickType.Ice:
                    previewBrick = previewBrickIceV;
                    break;
                case Brick.BrickType.Wide:
                    previewBrick = previewBrickWideV;
                    break;
                default:
                    Debug.LogError("UNKNOWN BRICK TYPE ANGRY FACE");
                    break;
            }
        }

        previewBrick.transform.position = pos;
        previewBrick.SetActive(true);
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
