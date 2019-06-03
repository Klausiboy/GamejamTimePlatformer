using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(BoxCollider2D))]
public class scr_Controller2D : MonoBehaviour
{
    #region fields
    const float skinWidth = 0.015f;

    public int horizontalRayCount = 4, verticalRayCount = 4;
    public LayerMask colMask;
    public CollisionInfo colInfo;

    float horizontalRaySpacing, verticalRaySpacing;
    float maxClimbAngle = 61f, maxDescendAngle = 61f;
    BoxCollider2D collider;
    RaycastOrigins raycastOrgs;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //Get players collider
        collider = GetComponent<BoxCollider2D>();

        CalculateRaySpacing();
    }

    public void Move(Vector3 velocity)
    {
        UpdateRaycastOrigins();
        colInfo.Reset();

        colInfo.velocityPrevious = velocity;
        //Handle descending
        if (velocity.y < 0)
            DescendSlope(ref velocity);
        //Look for collisions
        if (velocity.x != 0)
            HorizontalCollision(ref velocity);
        if (velocity.y != 0)
            VerticalCollision(ref velocity);

        //Actually move players position
        transform.Translate(velocity);
    }

    void HorizontalCollision(ref Vector3 velocity)
    {
        //setup fields
        float dirX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        for (int i = 0; i < horizontalRayCount; i++)
        {
            //set raycast originin depending on velocitys direction
            Vector2 rayOrigin;
            if (dirX == -1)
                rayOrigin = raycastOrgs.bottomLeft;
            else
                rayOrigin = raycastOrgs.bottomRight;
            
            //???
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            
            //handle collision
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * dirX, rayLength, colMask);
            if (hit)
            {
                //Climb slope if the hit collider has an acceptable angle
                float angle = Vector2.Angle(hit.normal, Vector2.up);

                if (i == 0 && angle <= maxClimbAngle)
                {
                    //fixes a speed glitch that happens when going from descending to ascending
                    if (colInfo.isDescending)
                    {
                        colInfo.isDescending = false;
                        velocity = colInfo.velocityPrevious;
                    }

                    //steps to the start of a slope
                    float distToSlopeStart = 0;
                    if (angle != colInfo.slopeAnglePrevious)
                    {
                        distToSlopeStart = hit.distance - skinWidth;
                        velocity.x -= distToSlopeStart * dirX;
                    }

                    ClimbSlope(ref velocity, angle);
                    velocity.x += distToSlopeStart * dirX;
                }
                
                //step to the collided obstacle
                if (!colInfo.isClimbing || angle > maxClimbAngle)
                {
                    //sets horizontal velocity to the distance until the collided object
                    velocity.x = (hit.distance - skinWidth) * dirX;
                    //avoids overriding the velocity with other raycastorigins if they hit something further away
                    rayLength = hit.distance;
                    //handle vertical velocity if climbing
                    if (colInfo.isClimbing)
                        velocity.y = Mathf.Tan(colInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                    //Set collision info based on moved direction
                    colInfo.left = dirX == -1;
                    colInfo.right = dirX == 1;
                }
            }

            //Debug.DrawRay(rayOrigin, Vector2.right * dirX * rayLength, Color.red);
        }
    }
    void VerticalCollision(ref Vector3 velocity)
    {
        //setup fields
        float dirY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            //set raycast originin depending on velocitys direction
            Vector2 rayOrigin;
            if (dirY == -1)
                rayOrigin = raycastOrgs.bottomLeft;
            else
                rayOrigin = raycastOrgs.topLeft;
            //???
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * dirY, rayLength, colMask);
            //if the raycast hits anything, move to that collider
            if (hit)
            {
                velocity.y = (hit.distance - skinWidth) * dirY;
                //avoids overriding the velocity with by other raycastorigins if they hit something further away
                rayLength = hit.distance;
                //handle horizontal velocity if climbing (fixes glitch when there is a roof while climbing)
                if (colInfo.isClimbing)
                    velocity.x = velocity.y / Mathf.Tan(colInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                //Set collision info based on moved direction
                colInfo.above = dirY == 1;
                colInfo.below = dirY == -1;
            }

            //Debug.DrawRay(rayOrigin, Vector2.up * dirY * rayLength, Color.red);
        }
        if (colInfo.isClimbing)
        {
            float dirX = Mathf.Sign(velocity.x);
            rayLength = Mathf.Abs(velocity.x) + skinWidth;
            Vector2 rayOrg = ((dirX == -1) ? raycastOrgs.bottomLeft : raycastOrgs.bottomRight) + Vector2.up * velocity.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrg, Vector2.right * dirX, rayLength, colMask);
            if (hit)
            {
                float angle = Vector2.Angle(hit.normal, Vector2.up);
                if (angle != colInfo.slopeAngle)
                {
                    velocity.x = (hit.distance - skinWidth) * dirX;
                    colInfo.slopeAngle = angle;
                }
            }
        }
    }


    void ClimbSlope(ref Vector3 velocity, float angle)
    {
        float moveDistance = Mathf.Abs(velocity.x);
        float climbVelY = Mathf.Sin(angle * Mathf.Deg2Rad) * moveDistance;
        if (velocity.y <= climbVelY)
        {
            velocity.y = climbVelY;
            velocity.x = Mathf.Cos(angle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
            colInfo.below = true;
            colInfo.isClimbing = true;
            colInfo.slopeAngle = angle;
        }        
    }
    void DescendSlope(ref Vector3 velocity)
    {
        float dirX = Mathf.Sign(velocity.x);
        Vector2 raycastOrg = (dirX == -1) ? raycastOrgs.bottomRight : raycastOrgs.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast(raycastOrg, -Vector2.up, Mathf.Infinity, colMask);

        if (hit)
        {
            float angle = Vector2.Angle(hit.normal, Vector2.up);
            //(I don't like having the ifs nested, might make it just one && check)
            if (angle != 0 && angle <= maxDescendAngle)
            {
                if (Mathf.Sign(hit.normal.x) == dirX)
                {
                    if (hit.distance - skinWidth <= Mathf.Tan(angle*Mathf.Deg2Rad)*Mathf.Abs(velocity.x))
                    {
                        float moveDist = Mathf.Abs(velocity.x);
                        float DescVelY = Mathf.Sin(angle * Mathf.Deg2Rad) * moveDist;
                        velocity.x = Mathf.Cos(angle * Mathf.Deg2Rad) * moveDist * Mathf.Sign(velocity.x);
                        velocity.y -= DescVelY;

                        colInfo.slopeAngle = angle;
                        colInfo.isDescending = true;
                        colInfo.below = true;
                    }
                }
            }
        }
    }

    void UpdateRaycastOrigins()
    {
        //setup bounds
        Bounds bounds = collider.bounds;
        bounds.Expand(-skinWidth * 2);
        //setup raycast origins based on bounds
        raycastOrgs.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrgs.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrgs.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrgs.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }
    void CalculateRaySpacing()
    {
        //setup bounds
        Bounds bounds = collider.bounds;
        bounds.Expand(-skinWidth * 2);
        //???
        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }


    struct RaycastOrigins
    {
        public Vector2 topLeft, topRight, bottomLeft, bottomRight;
    }

    public struct CollisionInfo
    {
        public bool above, right, below, left, isClimbing, isDescending;
        public float slopeAngle, slopeAnglePrevious;
        public Vector3 velocityPrevious;

        public void Reset()
        {
            above = below = false;
            right = left = false;
            isClimbing = false;
            isDescending = false;

            slopeAnglePrevious = slopeAngle;
            slopeAngle = 0;
        }
    }
}
