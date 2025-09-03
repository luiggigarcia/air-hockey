using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MalletController : MonoBehaviour
{
    public Collider2D movementArea;
    public float speed = 20f;
    public bool useMouse = true;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic; // Kinematic garante que não grude na parede
    }

    void FixedUpdate()
    {
        Vector2 target = rb.position;

        if (useMouse)
        {
            Vector3 ms = Input.mousePosition;
            ms.z = -Camera.main.transform.position.z;
            Vector3 world = Camera.main.ScreenToWorldPoint(ms);
            target = new Vector2(world.x, world.y);
        }
        else
        {
            float mx = Input.GetAxisRaw("Horizontal");
            float my = Input.GetAxisRaw("Vertical");
            Vector2 dir = new Vector2(mx, my).normalized;
            target = rb.position + dir * speed * Time.fixedDeltaTime;
        }

        // limitar dentro da área
        if (movementArea != null)
        {
            Bounds b = movementArea.bounds;
            float minX = b.min.x;
            float maxX = b.max.x;
            float minY = b.min.y;
            float maxY = b.max.y;

            target.x = Mathf.Clamp(target.x, minX, maxX);
            target.y = Mathf.Clamp(target.y, minY, maxY);
        }

        // move Kinematic
        rb.MovePosition(target);
    }
}
