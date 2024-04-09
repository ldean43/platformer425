using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    private float mouseX;
    private float mouseY;
    private Vector3 moveInput;
    private Vector3 up;

    [Header("References")]
    public GameObject player;
    public Rigidbody rb;
    public Transform head;
    public Camera cam;
    public Collider capsule;

    public float xRotation;
    public float yRotation;

    [Header("Movement")]
    public float MAX_ACCEL;
    public float MAX_SPEED;
    public float MAX_AIR_ACCEL;
    public float MAX_AIR_SPEED;
    public float airControl;
    public float jump;
    public float speed;
    public Vector3 friction;
    public Vector3 wishDir;
    public bool jumpQueued;

    public float xSens;
    public float ySens;

    public bool autoBhop;
    public float kF; // kinetic friction

    void Start() {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        
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
    // Rigid Body Rotation //
    rb.rotation = Quaternion.Euler(0, yRotation, 0);

        if (isGrounded()) {
            GroundMove();
        } else {
            AirMove();
        }

        if (rb.velocity.magnitude < .01) { 
            rb.velocity *= 0; 
        }

        speed = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;
    }
    private void AirMove() {
        wishDir = head.TransformDirection(moveInput);
        wishDir = wishDir.normalized;

        float proj = Vector3.Dot(new Vector3(rb.velocity.x, 0, rb.velocity.z), wishDir);
        float addSpeed, accel;

        if (proj >= MAX_AIR_SPEED) {
            return;
        }

        addSpeed = MAX_AIR_SPEED - proj;
        accel = MAX_AIR_ACCEL * Time.fixedDeltaTime * MAX_AIR_SPEED;

        if (accel + proj > MAX_AIR_SPEED) {
            accel = addSpeed;
        }

        rb.velocity += new Vector3(accel * wishDir.x, 0, accel * wishDir.z);

        //AirControl();
    }

    /*private void AirControl() {
        float tempY = rb.velocity.y;
        float speed = rb.velocity.magnitude;
        Vector3 vel = rb.velocity.normalized;
        vel.y = 0;
        
        float proj = Vector3.Dot(vel, wishDir);
        float k = 32;
        k *= airControl * proj * proj * Time.fixedDeltaTime;

        vel.x *= speed + k * wishDir.x;
        vel.y *= speed + k * wishDir.y;
        vel.z *= speed + k * wishDir.z;

        vel = vel.normalized;

        vel *= speed;
        vel.y = tempY;
        rb.velocity = vel;
    } */
    private void GroundMove() {
        wishDir = head.TransformDirection(moveInput);
        wishDir = wishDir.normalized;
        rb.velocity += new Vector3(0, -rb.velocity.y, 0);

        if (!jumpQueued) {
            Friction();
        }

        Accelerate();

        if (jumpQueued) {
            Debug.Log("Frame " + Time.fixedTime + ", Velocity y: " + rb.velocity.y);
            rb.velocity += jump * up;
            jumpQueued = false;
        }
    }
    private void Accelerate() {
        float proj = Vector3.Dot(new Vector3(rb.velocity.x, 0, rb.velocity.z), wishDir);
        float addSpeed, accel;

        if (proj >= MAX_SPEED) {
            return;
        }

        addSpeed = MAX_SPEED - proj;
        accel = MAX_ACCEL * Time.fixedDeltaTime * MAX_SPEED;

        if (accel + proj > MAX_SPEED) {
            accel = addSpeed;
        }

        rb.velocity += new Vector3(accel * wishDir.x, 0, accel * wishDir.z);
    }

    private void Friction() {
        if (rb.velocity.magnitude > 0) {
            friction.x = -kF * rb.velocity.x * Time.fixedDeltaTime;
            friction.z = -kF * rb.velocity.z * Time.fixedDeltaTime;
            rb.velocity += friction;
        }
    }

    private void QueueJump() {
        if (autoBhop) {
            jumpQueued = Input.GetButton("Jump");
            return;
        }

        jumpQueued = Input.GetButtonDown("Jump");
    }
    private bool isGrounded() {
        return Physics.CheckCapsule(capsule.bounds.center, new Vector3(capsule.bounds.center.x, capsule.bounds.min.y + .09f, capsule.bounds.center.z),
            0.1f);
    }
}
