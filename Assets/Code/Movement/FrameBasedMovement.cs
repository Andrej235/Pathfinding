using UnityEngine;

public class FrameBasedMovement : MonoBehaviour, IMovement
{
    [SerializeField] private float speed;
    public void MoveTo(Vector2 targetPos) => transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
    public void MoveBy(float x, float y) => MoveTo(transform.position + new Vector3(x, y));
    public void MoveBy(Vector2 moveBy) => MoveTo((Vector2)transform.position + moveBy);
}
