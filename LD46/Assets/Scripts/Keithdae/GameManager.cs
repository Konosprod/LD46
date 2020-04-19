using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;

    public List<BallController> balls;
    public GameObject ballPrefab;

    public bool isGameActive = false;

    public Text speedText;

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
        GameObject ball = Instantiate(ballPrefab, Vector3.zero, Quaternion.identity);
        GameObject ball2 = Instantiate(ballPrefab, new Vector3(1f, 1f, 0f), Quaternion.identity);
        GameObject ball3 = Instantiate(ballPrefab, new Vector3(-1f, -1f, 0f), Quaternion.identity);
        balls.Add(ball.GetComponent<BallController>());
        balls.Add(ball2.GetComponent<BallController>());
        balls.Add(ball3.GetComponent<BallController>());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }

        speedText.text = "Speed : " + GetAverageSpeed().ToString("F2");
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

    public void StartGame()
    {
        foreach (BallController ball in balls)
        {
            ball.SetActive(true);
        }
    }

    public void GameOver()
    {
        Debug.Log("Game is over noob");

    }
}
