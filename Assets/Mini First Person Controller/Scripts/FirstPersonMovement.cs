using System.Collections.Generic;
using UnityEngine;

public class FirstPersonMovement : MonoBehaviour
{
    public float speed = 5;

    [Header("Running")]
    public bool canRun = true;
    public bool IsRunning { get; private set; }
    public float runSpeed = 9;
    public KeyCode runningKey = KeyCode.LeftShift;
    [SerializeField]
    LayerMask wallLayer;
    [SerializeField]
    float distanceRay = 3f;
    public Vector3 gravity;

    Rigidbody rigidbody;
    /// <summary> Functions to override movement speed. Will use the last added override. </summary>
    public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();



    void Awake()
    {
        // Get the rigidbody on this.
        rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Update IsRunning from input.
        IsRunning = canRun && Input.GetKey(runningKey);

        // Get targetMovingSpeed.
        float targetMovingSpeed = IsRunning ? runSpeed : speed;
        if (speedOverrides.Count > 0)
        {
            targetMovingSpeed = speedOverrides[speedOverrides.Count - 1]();
        }

        // Get targetVelocity from input.
        Vector2 targetVelocity =new Vector2( Input.GetAxis("Horizontal") * targetMovingSpeed, Input.GetAxis("Vertical") * targetMovingSpeed);

        // Apply movement.
        rigidbody.velocity = transform.rotation * new Vector3(targetVelocity.x, rigidbody.velocity.y, targetVelocity.y);
        ChangeGravity();
        gravity = Physics.gravity;
    }

    void ChangeGravity () {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, distanceRay, wallLayer)) {
            if (hit.transform != null) {
                Physics.gravity = RoundComponentToZero(-hit.normal) * 9.81f;
                transform.position = hit.transform.GetChild(0).position;
                transform.rotation = hit.transform.GetChild(0).rotation;
            }
        }
    }

    Vector3 RoundComponentToZero (Vector3 value) {
        if (Mathf.Abs(value.x) <= 0.0001f) {
            value.x = 0;
        }
        if (Mathf.Abs(value.y) <= 0.0001f) {
            value.y = 0;
        }
        if (Mathf.Abs(value.z) <= 0.0001f) {
            value.z = 0;
        }
        return value;
    }

    void OnDrawGizmos () {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position + transform.up, transform.position + transform.up + transform.forward*distanceRay);
    }
}