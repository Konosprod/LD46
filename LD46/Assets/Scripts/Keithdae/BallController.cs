using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public float AccelerationFactor = 1.1f;

    private Vector3 speed;

    // Start is called before the first frame update
    void Start()
    {
        speed = new Vector3(1f, -2.5f, 0);
    }



    // Update is called once per frame
    private void Update()
    {
        Vector3 screenPos = Camera.main.WorldToViewportPoint(transform.position);
        bool onScreen = screenPos.x > -0.05f && screenPos.x < 1.05f && screenPos.y > -0.05f && screenPos.y < 1.05f;

        if (!onScreen)
        {
            // GameOver
            Debug.Log("Out of camera bounds");
            GameManager._instance.GameOver();
        }
    }


    void FixedUpdate()
    {
        transform.position += speed * Time.fixedDeltaTime;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("OnCollisionEnter2D");
        Debug.Log("Contact count : " + col.contactCount);

        Vector2 normal = col.GetContact(0).normal;

        Debug.Log("Normal : " + normal.ToString("f6"));

        //float speedMagnitude = Vector3.Magnitude(speed);

        Vector3 perpendicularVector = Vector3.Dot(speed, normal) * normal;
        Vector3 parallelVector = speed - perpendicularVector;

        speed = parallelVector - perpendicularVector;

        speed *= AccelerationFactor;

        Destroy(col.gameObject);
    }
}
