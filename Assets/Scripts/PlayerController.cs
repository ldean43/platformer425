using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
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
    private bool sloMo;

    [Header("References")]
    public Rigidbody rb;
    public Transform head;
    public Camera cam;
    public Collider capsule;

    [Header("Configurations")]
    public float MAX_ACCEL;
    public float MAX_DECEL;
    public float MAX_SPEED;
    public float MAX_AIR_ACCEL;
    public float MAX_AIR_DECEL;
    public float MAX_AIR_SPEED;
    public float MAX_JUMP;
    public float sloMoFactor;
    public float sloMoLength;
    public float airControl;

    public float xSens;
    public float ySens;
    public bool autoBhop;
    public float KF;

    [Header("Data")]
    public float speed;
    public Vector3 friction;
    public Vector3 wishDir;
    public float timer;

    void Start() {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

        rb = gameObject.GetComponent<Rigidbody>();
        head = gameObject.transform.GetChild(0);
        cam = GameObject.Find("Camera").GetComponent<Camera>();
        
        rb.velocity = new Vector3(0,0,0);
        cam.fieldOfView = 85;
        wishDir = new Vector3(0,0,0);
        moveInput = new Vector3(0,0,0);
        up = new Vector3(0,1,0);
        sloMo = false;
        timer = GameSettings.sloMoLength;

		Physics.queriesHitTriggers = false;

        if (GameSettings.autoBhopControl)
        {
            GameSettings.autoBhopControl = autoBhop;
        }
        autoBhop = GameSettings.autoBhopControl;

        if (GameSettings.fov == 80)
        {
            GameSettings.fov = cam.fieldOfView;
        }
        cam.fieldOfView = GameSettings.fov;

        if (GameSettings.sloMoLength == 2)
        {
            GameSettings.sloMoLength = sloMoLength;
        }
        sloMoLength = GameSettings.sloMoLength;      
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

        // SloMo
        if (Input.GetKey("left shift")){
            if (!sloMo && timer != 0) {
                sloMo = true;
                rb.velocity *= 1/sloMoFactor;
                Physics.gravity = Physics.gravity/10;
            } else {
                if (sloMo && timer <= 0) {
                    timer = 0;
                    sloMo = false;
                    rb.velocity *= sloMoFactor;
                    Physics.gravity = Physics.gravity * sloMoFactor;
                } else if (sloMo && timer > 0) {
                    timer += -Time.deltaTime;
                }
            }
        } else {
            if (sloMo) {
                sloMo = false;
                rb.velocity *= sloMoFactor;
                Physics.gravity = Physics.gravity * sloMoFactor;
            }
            if (timer < GameSettings.sloMoLength) {
                timer += Time.deltaTime;
            } else {
                timer = GameSettings.sloMoLength;
            }
        }
            
        // updating speed data
        speed = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;
        autoBhop = GameSettings.autoBhopControl;
        cam.fieldOfView = GameSettings.fov;
        sloMoLength = GameSettings.sloMoLength;
    }
    private void AirMove() {
        wishDir = head.TransformDirection(moveInput); // Maps moveInput into heads coordinate system.
        wishDir = wishDir.normalized;

        if (!sloMo) {
            Accelerate(MAX_AIR_ACCEL, MAX_AIR_DECEL, MAX_AIR_SPEED);
        } else {
            Accelerate(MAX_AIR_ACCEL/sloMoFactor, MAX_AIR_DECEL/sloMoFactor, MAX_AIR_SPEED/sloMoFactor);
        }

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
        float k = 32;
        k *= airControl * proj * proj * Time.deltaTime;
        Debug.Log("K: " + k);

        vel.x *= currSpeed + k * wishDir.x;
        vel.z *= currSpeed + k * wishDir.z;

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

        if (!sloMo) {
            Accelerate(MAX_ACCEL, MAX_DECEL, MAX_SPEED);
        } else {
            Accelerate(MAX_ACCEL/sloMoFactor, MAX_DECEL/sloMoFactor, MAX_SPEED/sloMoFactor);
        }

        if (jumpQueued) {
            if (sloMo) {
                rb.velocity += MAX_JUMP/sloMoFactor * up;
            } else {
                rb.velocity += MAX_JUMP * up;
            }
            jumpQueued = false;
        }
    }
    private void Accelerate(float maxAccel, float maxDecel, float maxSpeed) {
        float proj = Vector3.Dot(new Vector3(rb.velocity.x, 0, rb.velocity.z), wishDir);
        float addSpeed, accel;

        if (proj >= maxSpeed) {
            return;
        }

        addSpeed = maxSpeed - proj;
        if (proj >= 0){
            accel = maxAccel * Time.deltaTime * maxSpeed;
        } else {
            accel = maxDecel * Time.deltaTime * maxSpeed;
        }

        if (accel + proj > maxSpeed) {
            accel = addSpeed;
        }

        rb.velocity += new Vector3(accel * wishDir.x, 0, accel * wishDir.z);
    }

    private void Friction() {
        if (rb.velocity.magnitude > 0) {
            friction.x = -KF * rb.velocity.x * Time.deltaTime;
            friction.z = -KF * rb.velocity.z * Time.deltaTime;
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
            0.1f );
    }
}
