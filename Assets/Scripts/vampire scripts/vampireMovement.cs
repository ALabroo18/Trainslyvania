using UnityEngine;

public class vampireMovement : MonoBehaviour
{
    //Mason Kuhn

    public Transform target;//transform of train
    public Collider targetCollider;    //train collider
    public float moveSpeed = 3f;

    void Update()
    {
        if (target == null || targetCollider == null) return;

        Vector3 closestPoint = targetCollider.ClosestPoint(transform.position);

        Vector3 direction = closestPoint - transform.position;
        direction.y = 0f;

        float distance = direction.magnitude;

        if (distance <= 0.05f)
            return;

        Vector3 moveDir = direction.normalized;
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        if (moveDir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveDir);
        }
    }
}
