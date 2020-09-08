using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public enum Direction
    {
        North,
        East,
        West,
        South
    };
    public Direction playerDirection = Direction.South;
    private Animator playerAnimator;
    public float maxSpeed = 7;
    private Vector2 targetVelocity;
    private Collider2D playerCollider;
    private ContactFilter2D movementContactFilter;

    private float minMoveDistance = 0.001f;
    private float shellRadius = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        playerCollider = GetComponent<Collider2D>();
        movementContactFilter = BuildContactFilter2DForLayer(LayerMask.LayerToName(gameObject.layer));
    }

    // Update is called once per frame
    void Update()
    {
        ComputeVelocity();
        
        Vector2 velocityX = new Vector2();
        velocityX.x = targetVelocity.x * Time.deltaTime;

        Vector2 velocityY = new Vector2();
        velocityY.y = targetVelocity.y * Time.deltaTime;

        MovePlayer(velocityX);
        MovePlayer(velocityY);
    }

    private ContactFilter2D BuildContactFilter2DForLayer(string layerName)
    {
        ContactFilter2D contactFilter2D = new ContactFilter2D();
        contactFilter2D.useTriggers = false;
        contactFilter2D.SetLayerMask(Physics2D.GetLayerCollisionMask(LayerMask.NameToLayer(layerName)));
        contactFilter2D.useLayerMask = true;

        return contactFilter2D;
    }

    private void ComputeVelocity()
    {
        Vector2 move = Vector2.zero;
        move.x = Input.GetAxis("Horizontal");
        move.y = Input.GetAxis("Vertical");
        targetVelocity = move.normalized * maxSpeed;
    }

    private void MovePlayer(Vector2 move)
    {
        float distance = move.magnitude;
        RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
        if (distance > minMoveDistance)
        {
            int movementCollisionHitCount = playerCollider.Cast(move, movementContactFilter, hitBuffer, distance + shellRadius);
            List<RaycastHit2D> hitBufferList = BufferArrayHitToList(hitBuffer, movementCollisionHitCount);

            for (int i = 0; i < hitBufferList.Count; i++)
            {
                float currentDistance = hitBufferList[i].distance - shellRadius;

                if (currentDistance < distance)
                {
                    distance = currentDistance;
                }
            }
        }

        gameObject.transform.Translate(move.normalized * distance);
    }

    private List<RaycastHit2D> BufferArrayHitToList(RaycastHit2D[] hitBuffer, int count) 
    {
        List<RaycastHit2D> bufferHitList = new List<RaycastHit2D>();
        bufferHitList.Clear();
        for (int i = 0; i < count; i++)
        {
            bufferHitList.Add(hitBuffer[i]);
        }


        return bufferHitList;
    }
}
