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
        balls.Add(ball.GetComponent<BallController>());
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

        foreach (BallController ball in balls)
        {
            res += ball.speed.magnitude;
        }

        return res / balls.Count;
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
