// Some of this code is from a tutorial by Brackeys
// https://www.youtube.com/watch?v=_QajrabyTJc

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 4f;
    public float gravity = -9.81f;
    public float jumpHeight = 0.5f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private bool isGrounded;

    public static bool explosionJustHappened = false;
    public static float timeOfLastExplosion = 0;

    Vector3 velocity;

    // Update is called once per frame
    void Update()
    {
        if(Time.time - timeOfLastExplosion > 1f)
        {
            explosionJustHappened = false;
        }
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        else if (!isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        if(PlayerMovement.explosionJustHappened && Input.GetButtonDown("Jump"))
        {
            PlayerMovement.explosionJustHappened = false;
            velocity.y = Mathf.Sqrt(15*jumpHeight * -2f * gravity);
        }

        else if (isGrounded && Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        if(transform.position.y < -10)
        {
            velocity.y = 0;
        }
        else
        {
            controller.Move(velocity * Time.deltaTime);
        }
    }
}



