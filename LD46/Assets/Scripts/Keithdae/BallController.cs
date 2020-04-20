using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public float AccelerationFactor = 1.05f;
    public float previewLengthCap = 12f;

    [HideInInspector]
    public Vector3 speed;

    public LineRenderer linePreview;
    private LayerMask brickMask;

    [HideInInspector]
    public bool isActive = false;       // Allowed to move or not
    [HideInInspector]
    public bool isPreviewActive = true; // Trajectory preview

    private void Awake()
    {
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
        bool onScreen = screenPos.x > -0.05f && screenPos.x < 1.05f && screenPos.y > -0.05f && screenPos.y < 0.95f;

        if (!onScreen)
        {
            // GameOver
            Debug.Log("Out of camera bounds");
            GameManager._instance.GameOver();
            gameObject.SetActive(false);    // Disable the ball on game over
        }
    }


    void FixedUpdate()
    {
        if (isActive)
        {
            ComputeLinePreview();

            Vector2 pos = new Vector2(transform.position.x, transform.position.y);
            Vector2 dir = new Vector2(speed.x, speed.y).normalized;
            float remainingMagnitude = speed.magnitude * Time.fixedDeltaTime;
            float initialMagnitude = speed.magnitude;

            RaycastHit2D[] raycastHits = new RaycastHit2D[20];
            bool over = false;
            bool hasBrokenBrick = false;
            bool hasHitIceBrick = false;
            float iceBrickSlowFactor = 1f;

            List<Collider2D> collidersHit = new List<Collider2D>();

            while (!over)
            {
                raycastHits = Physics2D.CircleCastAll(pos, 0.5f, dir, remainingMagnitude, brickMask);

                RaycastHit2D raycastHit = new RaycastHit2D();
                foreach (RaycastHit2D hit in raycastHits)
                {
                    if (!collidersHit.Contains(hit.collider))
                    {
                        raycastHit = hit;
                        break;
                    }
                }

                //Debug.Log("Pos : " + pos.ToString("f6") + ", Dir : " + dir.ToString("f6") + ", Remaining magnitude : " + remainingMagnitude);

                if (raycastHit.collider != null && !collidersHit.Contains(raycastHit.collider) && remainingMagnitude > 0f)
                {
                    Vector2 normal = raycastHit.normal;

                    //Debug.Log("Collision Normal : " + normal.ToString("f6") + ", point : " + raycastHit.point.ToString("f6"));

                    Vector2 perpendicularVector = Vector2.Dot(dir, normal) * normal;
                    Vector2 parallelVector = dir - perpendicularVector;

                    dir = parallelVector - perpendicularVector;

                    float dist = Vector2.Distance(pos, raycastHit.point + normal * 0.5f);
                    remainingMagnitude -= dist;
                    pos = raycastHit.point + normal * 0.5f;

                    hasBrokenBrick = true;

                    Brick brick = raycastHit.collider.gameObject.GetComponent<Brick>();
                    if (brick.creator != null)
                    {
                        brick.creator.RemoveBrick(brick);
                    }

                    switch (brick.type)
                    {
                        case Brick.BrickType.Ice:
                            hasHitIceBrick = true;
                            iceBrickSlowFactor = brick.iceBrickSlowFactor;
                            break;
                        default:
                            break;
                    }

                    brick.hp--;
                    if(brick.hp <= 0)
                        brick.DestroyBrick();
                }
                else
                {
                    over = true;
                    transform.position = pos;
                    speed = dir * initialMagnitude;
                }
            }

            if (hasHitIceBrick)
                speed *= iceBrickSlowFactor;


            if (hasBrokenBrick)
                speed *= AccelerationFactor;


            transform.position += speed * Time.fixedDeltaTime;
        }
    }



    private void ComputeLinePreview()
    {
        linePreview.positionCount = 2;
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);
        Vector2 dir = new Vector2(speed.x, speed.y).normalized;
        float remainingMagnitude = speed.magnitude > previewLengthCap ? previewLengthCap : speed.magnitude;

        RaycastHit2D[] raycastHits = new RaycastHit2D[20];
        int linePositionIndex = 1;
        bool over = false;

        List<Collider2D> collidersHit = new List<Collider2D>();

        while (!over)
        {
            raycastHits = Physics2D.CircleCastAll(pos, 0.5f, dir, remainingMagnitude, brickMask);

            RaycastHit2D raycastHit = new RaycastHit2D();
            foreach (RaycastHit2D hit in raycastHits)
            {
                if (!collidersHit.Contains(hit.collider))
                {
                    raycastHit = hit;
                    break;
                }
            }

            //Debug.Log("Pos : " + pos.ToString("f6") + ", Dir : " + dir.ToString("f6") + ", Remaining magnitude : " + remainingMagnitude);

            if (raycastHit.collider != null && !collidersHit.Contains(raycastHit.collider) && remainingMagnitude > 0f)
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


    public void SetInitialSpeed(float initSpeed)
    {

        float x = Random.Range(-1f, 1f);
        float y = Random.Range(-1f, 1f);
        speed = new Vector3(x, y, 0f).normalized * initSpeed;
        linePreview.positionCount = 2;
        linePreview.SetPosition(1, speed);
        linePreview.enabled = true;
    }
}
