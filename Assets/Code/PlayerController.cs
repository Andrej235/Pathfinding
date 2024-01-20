using UnityEngine;

public class PlayerController : MonoBehaviour, IHostileTarget
{
    private IMovement movement;
    void Start()
    {
        if (!gameObject.TryGetComponent(out movement))
            Debug.LogError($"{nameof(PlayerController)} requires a component of type {nameof(IMovement)} to function");
    }

    void Update()
    {
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");
        Move(horizontal, vertical);
    }

    private void Move(float xDir, float yDir)
    {
        if (xDir == 0 && yDir == 0)
            return;

        movement.MoveBy(new Vector2(xDir, yDir).normalized / 3);
    }
}
