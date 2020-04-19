using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    public enum BrickType
    {
        Normal = 0,
        Ice = 1,
        Wide = 2
    }

    [Header("Brick type parameters")]
    public BrickType type;
    public float iceBrickSlowFactor = 0.5f;

    public BrickGenerator creator = null;

    public ParticleSystem particles;

    public void DestroyBrick()
    {
        particles.time = 0;
        particles.Play();
        this.GetComponent<SpriteRenderer>().enabled = false;
        this.GetComponent<Collider2D>().enabled = false;
        Object.Destroy(this.gameObject, .5f);
    }
}
