using UnityEngine;

public class SingleStillTargetBasedMovement : MonoBehaviour, IMovement
{
    [SerializeField] private float speed;
    private bool isMoving;
    private Vector2 targetPos;

    public void MoveBy(float x, float y) => MoveTo(transform.position + new Vector3(x, y));
    public void MoveBy(Vector2 moveBy) => MoveTo((Vector2)transform.position + moveBy);

    public void MoveTo(Vector2 targetPos)
    {
        isMoving = true;
        this.targetPos = targetPos;
    }

    void Update()
    {
        if (isMoving)
        {
            if ((Vector2)transform.position != targetPos)
                transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            else
                isMoving = false;
        }
    }
}
