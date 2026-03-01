using UnityEngine;

public class vampireMovement : MonoBehaviour
{
    //Mason Kuhn

    private trainHealth targetCart;
    private Collider targetCollider;
    public float moveSpeed = 3f;

    void Update()
    {
        //find or refresh target
        if (targetCart == null || targetCart.isBreached)
        {
            AcquireTarget();
            if (targetCart == null) return;
        }

        MoveTowardTarget();
    }

    void AcquireTarget()
    {
        trainHealth[] carts = FindObjectsOfType<trainHealth>();

        float closestDist = Mathf.Infinity;
        trainHealth closest = null;

        foreach (trainHealth cart in carts)
        {
            if (cart.isBreached)
                continue;

            float dist = Vector3.Distance(transform.position, cart.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = cart;
            }
        }

        if (closest != null)
        {
            targetCart = closest;
            targetCollider = closest.GetComponent<Collider>();
        }
    }

    void MoveTowardTarget()
    {
        if (targetCollider == null) return;

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
