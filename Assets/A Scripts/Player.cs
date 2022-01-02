using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static event System.Action OnReachedEndOfLevel;

    [SerializeField] float moveSpeed=7f;
    [SerializeField] float smoothMoveTime = .1f;
    [SerializeField] float turnSpeed=8;
    [SerializeField] float joystickHorizontalSensibility, joystickVerticalSensibility;
    [SerializeField] FixedJoystick fixedJoystick;

    private float angle;
    private float smoothInputMagnitude;
    private float smoothMoveVelocity;
    private Vector3 velocity;
    private Rigidbody myRigidbody;
    private bool disabled;

    public static Player instance;

    //for testing
    public bool isImmortal;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
            
    }
    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();

        Guard.OnGuardHasSpottedPlayer += Disable;
    }
    private void Update()
    {
        Vector3 inputDirection = Vector3.zero;
        if (!disabled)
        {
            inputDirection = new Vector3(fixedJoystick.Horizontal, 0, fixedJoystick.Vertical).normalized;
        }
            
        float inputMagnitude = inputDirection.magnitude;
        smoothInputMagnitude = Mathf.SmoothDamp(smoothInputMagnitude, inputMagnitude, ref smoothMoveVelocity, smoothMoveTime);

        float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
        angle = Mathf.LerpAngle(angle, targetAngle, Time.deltaTime * turnSpeed * inputMagnitude);

        velocity = transform.forward * moveSpeed * smoothInputMagnitude;

        //transform.eulerAngles = Vector3.up * angle;        
        //transform.Translate(transform.forward * smoothInputMagnitude * moveSpeed * Time.deltaTime, Space.World);

    }

    //Rigidbody should be updated in FixedUpdate method 
    private void FixedUpdate()
    {
        myRigidbody.MoveRotation(Quaternion.Euler(Vector3.up * angle));
        if (fixedJoystick.Horizontal >= joystickHorizontalSensibility || fixedJoystick.Horizontal <= -joystickHorizontalSensibility ||
            fixedJoystick.Vertical >= joystickVerticalSensibility || fixedJoystick.Vertical <= -joystickVerticalSensibility)
        {
            myRigidbody.MovePosition(myRigidbody.position + velocity * Time.deltaTime);
        }
        
        
    }
    void OnTriggerEnter(Collider hitCollider)
    {
        if (hitCollider.tag == "Finish")
        {
            Disable();
            if (OnReachedEndOfLevel != null)
            {
                OnReachedEndOfLevel();
            }
        }
    }
    private void Disable()
    {
        disabled = true;
    }

    private void OnDestroy()
    {
        Guard.OnGuardHasSpottedPlayer -= Disable;
    }
}
