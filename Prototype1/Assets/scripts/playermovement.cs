using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playermovement : MonoBehaviour
{
    public float speed = 5f;
    public int FacingDirection = 1;

    public Rigidbody2D rb;
    public Animator anim;

       void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(horizontal, vertical);

        anim.SetFloat("horizontal", Mathf.Abs(horizontal));
        anim.SetFloat("vertical", Mathf.Abs(vertical));

        // 1. This is to Move the player
        rb.linearVelocity = movement * speed;

        // 2. Rotate to face direction of travel
       if (movement != Vector2.zero)
{
    // Calculate angle
    float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
    
    // i subtracted 90 degrees to align the sprite ;-;
    rb.rotation = angle - 90f; 
}

    }

}

