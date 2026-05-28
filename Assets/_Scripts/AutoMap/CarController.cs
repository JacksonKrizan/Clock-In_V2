using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ZeroAccelCar : MonoBehaviour
{
    [Header("Wheels")]
    public WheelCollider frontLeft;
    public WheelCollider frontRight;
    public WheelCollider rearLeft;
    public WheelCollider rearRight;

    [Header("Engine")]
    [Tooltip("Adjusted to 2500 for stable physics. 14000 was causing permanent burnouts.")]
    public float motorPower = 2500f;
    public float brakePower = 6000f;
    public float maxSpeed = 220f;
    public float rollingResistance = 2000f;

    [Header("Steering")]
    public float lowSpeedSteerAngle = 45f;
    public float highSpeedSteerAngle = 12f;

    [Header("Drift Settings")]
    public KeyCode driftKey = KeyCode.LeftShift; // Changed from TAB to avoid Unity UI focus bugs
    public float normalSidewaysStiffness = 1.5f;
    public float driftSidewaysStiffness = 0.35f;
    public float normalForwardStiffness = 1.5f;
    public float driftForwardStiffness = 0.8f;

    [Header("Physics Real Estate")]
    public Transform centerOfMassTransform;

    Rigidbody rb;
    float throttle;
    float steer;
    bool isBraking;
    private bool isDriftModeActive = false;
    float currentSteer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = 1600f;

        // UNITY 2022 CHANGE: Changed from linearDamping/angularDamping back to drag/angularDrag
        rb.drag = 0.05f;
        rb.angularDrag = 0.4f;

        if (centerOfMassTransform != null)
        {
            rb.centerOfMass = transform.InverseTransformPoint(centerOfMassTransform.position);
        }
        else
        {
            rb.centerOfMass = new Vector3(0, -0.1f, 0);
        }

        // Set initial grip
        UpdateRearWheelTraction();
    }

    void Update()
    {
        throttle = Input.GetAxis("Vertical");
        steer = Input.GetAxis("Horizontal");
        isBraking = Input.GetKey(KeyCode.Space);

        // Toggle Drift Mode
        if (Input.GetKeyDown(driftKey))
        {
            isDriftModeActive = !isDriftModeActive;
            UpdateRearWheelTraction();
            Debug.Log("Drift Mode: " + (isDriftModeActive ? "ENABLED 🏎️🔥" : "DISABLED 🛑"));
        }
    }

    void FixedUpdate()
    {
        // UNITY 2022 CHANGE: Changed from rb.linearVelocity to rb.velocity
        float speed = rb.velocity.magnitude * 3.6f;

        ApplyDrive(speed);
        ApplySteering(speed);
        ApplyBrakes();
    }

    // ---------------- DRIVE WITH ANTI-BURNOUT ----------------
    void ApplyDrive(float speed)
    {
        if (Mathf.Abs(throttle) < 0.1f)
        {
            rearLeft.motorTorque = 0f;
            rearRight.motorTorque = 0f;
            return;
        }

        // TRACTION CONTROL: If drift mode is OFF but the wheels are spinning insanely fast,
        // temporarily cut torque so the tires can catch the ground.
        if (!isDriftModeActive)
        {
            if (rearLeft.rpm > 800f || rearRight.rpm > 800f)
            {
                rearLeft.motorTorque = 100f; // Give it just a tiny nudge to regain grip
                rearRight.motorTorque = 100f;
                return;
            }
        }

        float torque = throttle * motorPower;
        float speedFactor = Mathf.Clamp01(speed / maxSpeed);
        torque *= (1f - speedFactor * 0.4f);

        rearLeft.motorTorque = torque;
        rearRight.motorTorque = torque;
    }

    // ---------------- STEERING ----------------
    void ApplySteering(float speed)
    {
        float speedFactor = Mathf.Clamp01(speed / maxSpeed);

        // Give slightly more steering freedom during a drift to allow counter-steering
        float maxSteer = isDriftModeActive ? lowSpeedSteerAngle * 0.6f : highSpeedSteerAngle;
        float currentMaxSteerAngle = Mathf.Lerp(lowSpeedSteerAngle, maxSteer, speedFactor);

        float target = steer * currentMaxSteerAngle;
        currentSteer = Mathf.Lerp(currentSteer, target, Time.fixedDeltaTime * 10f);

        frontLeft.steerAngle = currentSteer;
        frontRight.steerAngle = currentSteer;
    }

    // ---------------- BRAKES ----------------
    void ApplyBrakes()
    {
        if (isBraking)
        {
            SetBrakeTorque(brakePower);
        }
        else if (Mathf.Abs(throttle) < 0.1f)
        {
            // Less resistance during drifts so the car slides smoothly instead of stopping
            float dynamicResistance = isDriftModeActive ? rollingResistance * 0.2f : rollingResistance;
            SetBrakeTorque(dynamicResistance);
        }
        else
        {
            SetBrakeTorque(0f);
        }
    }

    void SetBrakeTorque(float amount)
    {
        frontLeft.brakeTorque = amount;
        frontRight.brakeTorque = amount;
        rearLeft.brakeTorque = amount;
        rearRight.brakeTorque = amount;
    }

    // ---------------- REAR TRACTION CONFIGURATOR ----------------
    void UpdateRearWheelTraction()
    {
        WheelFrictionCurve forwardFriction = rearLeft.forwardFriction;
        WheelFrictionCurve sidewaysFriction = rearLeft.sidewaysFriction;

        if (isDriftModeActive)
        {
            sidewaysFriction.stiffness = driftSidewaysStiffness;
            forwardFriction.stiffness = driftForwardStiffness;
        }
        else
        {
            sidewaysFriction.stiffness = normalSidewaysStiffness;
            forwardFriction.stiffness = normalForwardStiffness;

            // Kill wheel spin velocity instantly when exiting drift mode
            rearLeft.motorTorque = 0f;
            rearRight.motorTorque = 0f;
        }

        rearLeft.forwardFriction = forwardFriction;
        rearLeft.sidewaysFriction = sidewaysFriction;

        rearRight.forwardFriction = forwardFriction;
        rearRight.sidewaysFriction = sidewaysFriction;
    }
}