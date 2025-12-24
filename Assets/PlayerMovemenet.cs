using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float randomAngleMin = 1f;
    public float randomAngleMax = 2f;

    [Header("Sprites")]
    public Sprite aliveSprite;
    public Sprite deathSprite;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector2 lastVelocity;

    public bool isDead = false;

    // Staticki brojac zivih igraca
    private static int alivePlayers = 0;

    void Awake()
    {
        alivePlayers++; // svaki player povecava broj zivih
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        sr.sprite = aliveSprite;
        rb.freezeRotation = true;

        Vector2 randomDir = Random.insideUnitCircle.normalized;
        rb.linearVelocity = randomDir * speed;
    }

    void FixedUpdate()
    {
        if (isDead)
            return;

        lastVelocity = rb.linearVelocity;

        if (rb.linearVelocity.sqrMagnitude < 0.01f)
            rb.linearVelocity = lastVelocity.normalized * speed;
        else
            rb.linearVelocity = rb.linearVelocity.normalized * speed;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (isDead) return;

        if (col.collider.CompareTag("Spike"))
        {
            Die();
            return;
        }

        Vector2 normal = col.contacts[0].normal;
        Vector2 newDir = Vector2.Reflect(lastVelocity.normalized, normal);

        float randomAngle = Random.Range(randomAngleMin, randomAngleMax);
        if (Random.value > 0.5f) randomAngle = -randomAngle;

        newDir = RotateVector(newDir, randomAngle);

        rb.linearVelocity = newDir.normalized * speed;
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;

        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = true;

        Collider2D[] cols = GetComponents<Collider2D>();
        foreach (var c in cols)
            c.enabled = false;

        sr.sprite = deathSprite;

        alivePlayers--;

        // Ako su svi igraci mrtvi → restart level
        if (alivePlayers <= 0)
        {
            StartCoroutine(RestartLevelAfterDelay(2f));
        }
    }

    private Vector2 RotateVector(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(rad);
        float cos = Mathf.Cos(rad);

        return new Vector2(
            v.x * cos - v.y * sin,
            v.x * sin + v.y * cos
        );
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("WinGate") && !isDead)
        {
            StartCoroutine(WinSequence());
        }
    }

    private IEnumerator WinSequence()
    {
        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = true;

        Collider2D[] cols = GetComponents<Collider2D>();
        foreach (var c in cols)
            c.enabled = false;

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator RestartLevelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
