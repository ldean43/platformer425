using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{

    private float mouseX;
    private float mouseY;
    private float xRotation;
    private float yRotation;
    private Vector3 moveInput;
    private Vector3 up;
    private bool jumpQueued;

    public Rigidbody rb;
    public Transform head;
    public Camera cam;
    public Collider capsule;

    [Header("Configurations")]
    public float MAX_ACCEL;
    public float MAX_SPEED;
    public float MAX_AIR_ACCEL;
    public float MAX_AIR_SPEED;
    public float MAX_JUMP;
    public float airControl;

    public float xSens;
    public float ySens;
    public bool autoBhop;
    public float KF;

    [Header("Data")]
    public float speed;
    public Vector3 friction;
    public Vector3 wishDir;

    void Start() {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

        rb = gameObject.GetComponent<Rigidbody>();
        head = gameObject.transform.GetChild(0);
        cam = GameObject.Find("Camera").GetComponent<Camera>();
        
        rb.velocity = new Vector3(0,0,0);
        wishDir = new Vector3(0,0,0);
        moveInput = new Vector3(0,0,0);
        up = new Vector3(0,1,0);
    }

    void Update() {

        // CAMERA ROTATION //

        mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * xSens;
        mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * ySens;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90, 90);

        moveInput.z = Input.GetAxisRaw("Vertical");
        moveInput.x = Input.GetAxisRaw("Horizontal");

        cam.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        cam.transform.position = head.transform.position;
        QueueJump();

    }

    private void FixedUpdate() {

    rb.rotation = Quaternion.Euler(0, yRotation, 0); // handle rb rotation at fixed update

        // two styles of movement
        if (isGrounded()) {
            GroundMove();
        } else {
            AirMove();
        }

        // zeroing out velocity if less than threshold
        if (rb.velocity.magnitude < .01) { 
            rb.velocity *= 0; 
        }

        // updating speed data
        speed = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;
    }
    private void AirMove() {
        wishDir = head.TransformDirection(moveInput); // Maps moveInput into heads coordinate system.
        wishDir = wishDir.normalized;

        Accelerate(MAX_AIR_ACCEL, MAX_AIR_SPEED);

        AirControl();
    }

    private void AirControl() {
        float tempY = rb.velocity.y;
        Vector3 vel = rb.velocity;
        vel.y = 0;
        float currSpeed = vel.magnitude;
        vel = vel.normalized;
        
        float proj = Vector3.Dot(vel, wishDir);
        Debug.Log("Proj: " + proj);
        float k = 10;
        k *= airControl * proj * proj * Time.fixedDeltaTime;
        Debug.Log("K: " + k);

        if (proj > 0) {
            vel.x *= currSpeed + k * wishDir.x;
            vel.z *= currSpeed + k * wishDir.z;
        }

        vel = vel.normalized;

        vel *= currSpeed;
        Debug.Log("Vel: " + vel);
        vel.y = tempY;
        rb.velocity = vel;
    }

    private void GroundMove() {
        wishDir = head.TransformDirection(moveInput); // Maps moveInput into heads coordinate system.
        wishDir = wishDir.normalized;

        rb.velocity += new Vector3(0, -rb.velocity.y, 0); // zeroing out y position

        if (!jumpQueued) { // 1 frame to avoid friction
            Friction();
        }

        Accelerate(MAX_ACCEL, MAX_SPEED);

        if (jumpQueued) {
            rb.velocity += MAX_JUMP * up;
            jumpQueued = false;
        }
    }
    private void Accelerate(float maxAccel, float maxSpeed) {
        float proj = Vector3.Dot(new Vector3(rb.velocity.x, 0, rb.velocity.z), wishDir);
        float addSpeed, accel;

        if (proj >= maxSpeed) {
            return;
        }

        addSpeed = maxSpeed - proj;
        accel = maxAccel * Time.fixedDeltaTime * maxSpeed;

        if (accel + proj > maxSpeed) {
            accel = addSpeed;
        }

        rb.velocity += new Vector3(accel * wishDir.x, 0, accel * wishDir.z);
    }

    private void Friction() {
        if (rb.velocity.magnitude > 0) {
            friction.x = -KF * rb.velocity.x * Time.fixedDeltaTime;
            friction.z = -KF * rb.velocity.z * Time.fixedDeltaTime;
            rb.velocity += friction;
        }
    }

    private void QueueJump() {
        if (autoBhop) {
            jumpQueued = Input.GetButton("Jump"); // returns true on hold
            return;
        }

        jumpQueued = Input.GetButtonDown("Jump"); // returns true during the frame when pressed, false next frame
    }

    // check if capsule cast collides with ground
    private bool isGrounded() {
        return Physics.CheckCapsule(capsule.bounds.center, new Vector3(capsule.bounds.center.x, capsule.bounds.min.y + .09f, capsule.bounds.center.z),
            0.1f);
    }
}
