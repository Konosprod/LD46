using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickGenerator : MonoBehaviour
{
    public float brickGenerationTime = 5f;  // Time in seconds to generate a brick
    public int maxBricks = 12;              // Maximum number of bricks created by this generator

    public GameObject brickPrefab;          // The type of brick to create


    [Header("Spawn area bounds")]
    public float minX = 0f;
    public float maxX = 0f;
    public float minY = 0f;
    public float maxY = 0f;


    private float currentTime;

    private List<Brick> bricks = new List<Brick>();

    // Start is called before the first frame update
    void Start()
    {
        currentTime = 0.5f; 
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager._instance.isGameActive)
        {
            currentTime -= Time.deltaTime;

            if (currentTime <= 0f && bricks.Count < maxBricks)
            {
                Vector3 pos = transform.position + new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), 0f);
                GameObject newBrick = Instantiate(brickPrefab, pos, brickPrefab.transform.rotation, transform);
                Brick newBrickBrick = newBrick.GetComponent<Brick>();
                newBrickBrick.creator = this;
                bricks.Add(newBrickBrick);
                currentTime = brickGenerationTime;
            }
        }
    }


    public void RemoveBrick(Brick brick)
    {
        bricks.Remove(brick);
    }
}
