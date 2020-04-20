using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public static UiManager _instance;


    [Header("Block Selection")]
    public Image brickNormalImage;
    public Image brickNormalSelected;
    public Image brickIceImage;
    public Image brickIceSelected;
    public Image brickWideImage;
    public Image brickWideSelected;

    [Header("Texts")]
    public Text levelText;
    public Text speedText;
    public Text timerText;

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
}
