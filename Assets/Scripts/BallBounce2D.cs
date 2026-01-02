using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BallBounce2D : MonoBehaviour
{
    public float bounceIntensity = 1f;   // 1 = savršeno elastično
    public float minSpeed = 3f;          // loptica nikad ne staje
    public float pushAwayForce = 0.2f;   // blagi odmak od površine da se ne zalepi

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.freezeRotation = true;
    }

    private void FixedUpdate()
    {
        // ako nekad brzina bude premala, poguraj lopticu da ne stoji
        if (rb.linearVelocity.magnitude < minSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * minSpeed;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 v = rb.linearVelocity;

        if (v.sqrMagnitude < 0.01f)
            v = collision.contacts[0].normal * -1f; // ako je baš stala

        Vector2 normal = collision.contacts[0].normal;

        // refleksija
        Vector2 reflect = Vector2.Reflect(v, normal) * bounceIntensity;

        // minimalna brzina
        if (reflect.magnitude < minSpeed)
            reflect = reflect.normalized * minSpeed;

        rb.linearVelocity = reflect;

        // dodatni “kick” da se loptica ne zalepljuje za površinu
        rb.position += normal * pushAwayForce;
    }
}
