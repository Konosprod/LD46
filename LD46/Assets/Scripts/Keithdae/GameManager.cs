using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public struct LevelInfo
    {
        public int nbBalls;
        public float initialSpeed;
        public float length;

        public LevelInfo(int nb, float spe, float len)
        {
            nbBalls = nb;
            initialSpeed = spe;
            length = len;
        }
    }

    private LevelInfo[] levels = new LevelInfo[20] { new LevelInfo(1, 3.5f, 20f), new LevelInfo(1, 5f, 30f), new LevelInfo(2, 3f, 25f), new LevelInfo(1, 10f, 15f), new LevelInfo(2, 4f, 45f),      //1-5
                                                     new LevelInfo(1, 12f, 30f), new LevelInfo(3, 3f, 30f), new LevelInfo(2, 7f, 15f), new LevelInfo(3, 4.5f, 20f), new LevelInfo(4, 3f, 45f),      //6-10
                                                     new LevelInfo(1, 25f, 25f), new LevelInfo(6, 2f, 30f), new LevelInfo(4, 4f, 25f), new LevelInfo(2, 13.5f, 20f), new LevelInfo(5, 3.2f, 45f),   //11-15
                                                     new LevelInfo(3, 20f, 10f), new LevelInfo(4, 5f, 30f), new LevelInfo(10, 2f, 20f), new LevelInfo(5, 7f, 15f), new LevelInfo(7, 3f, 45f)        //16-20
    };


    public static GameManager _instance;

    public List<BallController> balls;
    public GameObject ballPrefab;

    public bool isGameActive = false;


    private int level = 1;
    private float timer = 1337f;

    [Header("Upgrades")]
    public int initialBPLevel = 1;  // Upgrades the amount of BP you get at round start
    public int bpGenLevel = 1;      // Upgrades the BP generation amount


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
        SetupLevel();
        UiManager._instance.SelectNormalBrick();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartLevel();
        }

        if (isGameActive)
        {
            timer -= Time.deltaTime;
            UiManager._instance.UpdateSpeedText("Speed : " + GetAverageSpeed().ToString("F2"));
            UiManager._instance.UpdateTimerText("Time : " + (timer > 0f ? timer.ToString("F2") : "0"));

            if (timer <= 0f)
            {
                NextLevel();
            }
        }
    }

    private float GetAverageSpeed()
    {
        float res = 0f;

        int activeBallCount = 0;

        foreach (BallController ball in balls)
        {
            if (ball.gameObject.activeSelf)
            {
                res += ball.speed.magnitude;
                activeBallCount++;
            }
        }

        return res / (activeBallCount == 0 ? 1 : activeBallCount);
    }

    private void SetupLevel()
    {
        if (level > levels.Length)
        {
            WinGame();
        }
        else
        {
            LevelInfo lvl = levels[level - 1];
            int activeBallCount = 0;

            // Create som extra balls if needed
            if (lvl.nbBalls > balls.Count)
            {
                for (int i = balls.Count; i < lvl.nbBalls; i++)
                {
                    GameObject ball = Instantiate(ballPrefab, RandomCoordinatesForBall(), Quaternion.identity);
                    balls.Add(ball.GetComponent<BallController>());
                }
            }


            // Set active the right amount of balls and set them up
            foreach (BallController ball in balls)
            {
                if (activeBallCount < lvl.nbBalls)
                {
                    ball.gameObject.SetActive(true);
                    ball.SetActive(false);
                    ball.transform.position = RandomCoordinatesForBall();
                    ball.SetInitialSpeed(lvl.initialSpeed);
                    activeBallCount++;
                }
                else
                {
                    ball.gameObject.SetActive(false);
                }
            }


            timer = lvl.length;

            UiManager._instance.UpdateSpeedText("Speed : " + GetAverageSpeed().ToString("F2"));
            UiManager._instance.UpdateTimerText("Time : " + (timer > 0f ? timer.ToString("F2") : "0"));
            UiManager._instance.UpdateLevelText("Level " + level.ToString());


            BrickManager._instance.SetupLevel();
        }
    }

    private Vector3 RandomCoordinatesForBall()
    {
        return new Vector3(Random.Range(-1f, 1f), Random.Range(-1.5f, 1.5f), 0f);
    }



    private void StartLevel()
    {
        foreach (BallController ball in balls)
        {
            ball.SetActive(true);
        }
        isGameActive = true;
    }

    private void NextLevel()
    {
        // Upgrades every 5 levels YEAH
        if (level % 5 == 0)
        {
            initialBPLevel++;
            bpGenLevel++;
        }

        isGameActive = false;
        level++;
        SetupLevel();
    }

    public void GameOver()
    {
        isGameActive = false;
        BrickManager._instance.brickPoints = 0;
        UiManager._instance.ShowEndGame(false);

        foreach (BallController ball in balls)
        {
            ball.gameObject.SetActive(false);
        }

    }

    private void WinGame()
    {
        isGameActive = false;
        BrickManager._instance.brickPoints = 0;
        UiManager._instance.ShowEndGame(true);

        foreach (BallController ball in balls)
        {
            ball.gameObject.SetActive(false);
        }
    }

    public void ResetGame()
    {
        level = 1;
        initialBPLevel = 1;
        bpGenLevel = 1;
        SetupLevel();
    }


    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
