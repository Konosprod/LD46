using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public static UiManager _instance;


    [Header("Block Selection")]
    public Image brickNormal;
    public Image brickIce;
    public Image brickWide;
    public Sprite brickNormalImage;
    public Sprite brickNormalSelected;
    public Sprite brickIceImage;
    public Sprite brickIceSelected;
    public Sprite brickWideImage;
    public Sprite brickWideSelected;

    [Header("Panel")]
    public GameObject panelEndOfGame;

    [Header("Texts")]
    public Text levelText;
    public Text speedText;
    public Text timerText;
    public Text endofgameText;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateLevelText(string text)
    {
        levelText.text = text;
    }

    public void UpdateSpeedText(string text)
    {
        speedText.text = text;
    }

    public void UpdateTimerText(string text)
    {
        timerText.text = text;
    }

    public void SelectIceBrick()
    {
        brickIce.sprite = brickIceSelected;
        brickNormal.sprite = brickNormalImage;
        brickWide.sprite = brickWideImage;
    }

    public void SelectWideBrick()
    {
        brickWide.sprite = brickWideSelected;
        brickIce.sprite = brickIceImage;
        brickNormal.sprite = brickNormalImage;
    }

    public void SelectNormalBrick()
    {
        brickNormal.sprite = brickNormalSelected;
        brickIce.sprite = brickIceImage;
        brickWide.sprite = brickWideImage;
    }

    public void ShowEndGame(bool win)
    {
        panelEndOfGame.SetActive(true);
        endofgameText.text = (win == true) ? "YOU DID IT !" : "MISSION FAILED";
    }
}
