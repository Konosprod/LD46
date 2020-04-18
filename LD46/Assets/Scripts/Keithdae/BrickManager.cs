using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickManager : MonoBehaviour
{
    public GameObject brickPrefab;

    public Transform brickHolder;

    public long brickPoints = 50;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 brickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            brickPosition.z = 0;
            Instantiate(brickPrefab, brickPosition, Quaternion.identity, brickHolder);
        }
    }
}
