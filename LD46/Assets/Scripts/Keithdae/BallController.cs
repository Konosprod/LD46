using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public float AccelerationFactor = 1.05f;
    public float initialSpeedMagnitude = 3.5f;
    public float previewLengthCap = 12f;

    private Vector3 speed;

    public LineRenderer linePreview;
    private LayerMask brickMask;

    [HideInInspector]
    public bool isActive = false;       // Allowed to move or not
    [HideInInspector]
    public bool isPreviewActive = true; // Trajectory preview

    private void Awake()
    {
        float x = Random.Range(-1f, 1f);
        float y = Random.Range(-1f, 1f);
        speed = new Vector3(x, y, 0f).normalized * initialSpeedMagnitude;
        linePreview.SetPosition(1, speed);
        linePreview.enabled = true;

        brickMask = LayerMask.GetMask("Brick");
    }

    // Start is called before the first frame update
    void Start()
    {

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

        if (isActive)
        {
            ComputeLinePreview();
        }
    }


    void FixedUpdate()
    {
        if (isActive)
        {
            Vector2 pos = new Vector2(transform.position.x, transform.position.y);
            Vector2 dir = new Vector2(speed.x, speed.y).normalized;
            float remainingMagnitude = speed.magnitude;

            RaycastHit2D raycastHit;
            raycastHit = Physics2D.CircleCast(pos, 0.5f, dir, remainingMagnitude * Time.fixedDeltaTime, brickMask);
            if (raycastHit.collider != null)
            {
                //Debug.Log("Pos : " + pos.ToString("f6") + ", Dir : " + dir.ToString("f6") + ", Remaining magnitude : " + remainingMagnitude);

                //Debug.Log("Collider hit : " + raycastHit.collider);
                Vector2 normal = raycastHit.normal;

                //Debug.Log("Collision Normal : " + normal.ToString("f6") + ", point : " + raycastHit.point.ToString("f6"));

                Vector3 perpendicularVector = Vector3.Dot(speed, normal) * normal;
                Vector3 parallelVector = speed - perpendicularVector;

                speed = parallelVector - perpendicularVector;

                speed *= AccelerationFactor;

                Destroy(raycastHit.collider.gameObject);
            }

            transform.position += speed * Time.fixedDeltaTime;
        }
    }

    /*void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("OnCollisionEnter2D");
        Debug.Log("Contact count : " + col.contactCount);

        Vector2 normal = col.GetContact(0).normal;

        Debug.Log("Collision Normal : " + normal.ToString("f6") + "Collision point : " + col.GetContact(0).point.ToString("f6"));

        //float speedMagnitude = Vector3.Magnitude(speed);

        Vector3 perpendicularVector = Vector3.Dot(speed, normal) * normal;
        Vector3 parallelVector = speed - perpendicularVector;

        speed = parallelVector - perpendicularVector;

        speed *= AccelerationFactor;

        linePreview.SetPosition(1, speed);

        Destroy(col.gameObject);
    }*/


    private void ComputeLinePreview()
    {
        linePreview.positionCount = 2;
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);
        Vector2 dir = new Vector2(speed.x, speed.y).normalized;
        float remainingMagnitude = speed.magnitude > previewLengthCap ? previewLengthCap : speed.magnitude;

        RaycastHit2D raycastHit;
        int linePositionIndex = 1;
        bool over = false;

        List<Collider2D> collidersHit = new List<Collider2D>();

        while (!over)
        {
            raycastHit = Physics2D.CircleCast(pos, 0.5f, dir, remainingMagnitude, brickMask);

            //Debug.Log("Pos : " + pos.ToString("f6") + ", Dir : " + dir.ToString("f6") + ", Remaining magnitude : " + remainingMagnitude);

            if (raycastHit.collider != null && !collidersHit.Contains(raycastHit.collider))
            {
                //Debug.Log("Collider hit : " + raycastHit.collider);
                Vector2 normal = raycastHit.normal;

                // Debug.Log("Predicted Normal : " + normal.ToString("f6") + ", point : " + raycastHit.point.ToString("f6"));

                Vector2 perpendicularVector = Vector3.Dot(dir, normal) * normal;
                Vector2 parallelVector = dir - perpendicularVector;

                dir = parallelVector - perpendicularVector;

                float dist = Vector2.Distance(pos, raycastHit.point + normal * 0.5f);
                remainingMagnitude -= dist;
                pos = raycastHit.point + normal * 0.5f;

                //Debug.DrawLine(pos + dir * 0.5f, pos + dir * 1.5f, Color.red, 1f);

                linePreview.SetPosition(linePositionIndex, new Vector3(pos.x, pos.y, 0f) - transform.position);
                linePositionIndex++;
                linePreview.positionCount++;

                collidersHit.Add(raycastHit.collider);

                //Debug.Break();
            }
            else
                over = true;
        }
        linePreview.SetPosition(linePositionIndex, new Vector3(pos.x, pos.y, 0f) + new Vector3(dir.x, dir.y, 0f) * remainingMagnitude - transform.position);

    }


    public void SetActive(bool active)
    {
        isActive = active;
        /*isPreviewActive = !active;
        linePreview.enabled = !active;*/
    }
}
