using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 10.0f;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        float dx = Input.GetAxisRaw("Horizontal");
        float dy = Input.GetAxisRaw("Vertical");

        spriteRenderer.flipX = dx < 0.0f;

        Vector3 offset = new Vector3(dx, dy, 0);
        offset *= speed;
        transform.Translate(offset);
    }
}
