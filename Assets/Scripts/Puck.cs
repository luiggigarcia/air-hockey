using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Puck : MonoBehaviour
{
    [Header("Velocidade")]
    public float minSpeed = 9f;       // mais rápido para jogo arcade
    public float maxSpeed = 20f;      // limite p/ não atravessar
    public float serveSpeed = 13f;    // força do saque

    [Header("Anti-travamento")]
    public float nudgeFromWalls = 0.25f;   // empurrãozinho para dentro quando esmagado
    public float minHorizComponent = 0.15f; // evita movimento 100% vertical

    private Rigidbody2D rb;
    private Vector2 startPos;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;
    }

    void Start()
    {
        ResetAndServe();
    }

    void FixedUpdate()
    {
    Vector2 v = rb.linearVelocity;
    float sp = v.magnitude;

    // limitar velocidade
    if (sp > maxSpeed) v = v.normalized * maxSpeed;
    else if (sp < minSpeed && sp > 0.01f) v = v.normalized * minSpeed;

    // evita ping-pong vertical
    if (Mathf.Abs(v.x) < minHorizComponent)
    {
        float sign = (Random.value < 0.5f) ? -1f : 1f;
        v.x = sign * minHorizComponent; // só ajusta X, não normaliza todo o vetor
    }

    rb.linearVelocity = v;
    }

    public void ResetAndServe(Vector2? preferredDir = null)
    {
        rb.position = startPos;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        Vector2 dir = preferredDir.HasValue ? preferredDir.Value.normalized
                                            : Random.insideUnitCircle.normalized;
        rb.linearVelocity = dir * serveSpeed;
    }

    void OnCollisionStay2D(Collision2D col)
    {
        // Marque suas paredes com a tag "Wall"
        if (col.collider.CompareTag("Wall"))
        {
            // Se estiver empurrando contra a parede, reflita e dê um empurrão para dentro
            var cp = col.GetContact(0);
            Vector2 n = cp.normal;             // normal apontando da parede para o puck
            Vector2 v = rb.linearVelocity;

            if (Vector2.Dot(v, n) < 0f)        // estamos indo "para dentro" da parede
            {
                Vector2 reflected = Vector2.Reflect(v, n);
                reflected += n * nudgeFromWalls;  // empurrãozinho para separar
                rb.linearVelocity = Vector2.ClampMagnitude(reflected, maxSpeed);
            }
        }
    }
}
