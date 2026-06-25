using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject cameraHolder;
    [SerializeField] float mouseSensitivity, sprintSpeed, walkSpeed, jumpForce, smoothTime;

    float verticalLookRotation;
    bool grounded;
    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;

    Rigidbody rb;

    PhotonView PV;
    [SerializeField] PlayerSettings playerSettings;

    [Header("Spraying")]
    [SerializeField] Grabbing grabbing;            // the player's grab script
    [SerializeField] ParticleSystem playerWater;   // built-in spray used when NOT holding a hose

    public bool lookAround = true;





void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
    }

    

    void Start()
    {
        if (!PV.IsMine)
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null)
            {
                Destroy(cam.gameObject);
            }
            
            if (rb != null)
            {
                rb.isKinematic = true;
            }
            return;
        }

        if (playerSettings == null)
        {
            playerSettings = FindObjectOfType<PlayerSettings>();
        }
        if (grabbing == null)
        {
            grabbing = GetComponentInChildren<Grabbing>();
        }
    }

    void Update()
    {
        if (!PV.IsMine)
            return;
            
        if (lookAround == true)
        {
            Look();
        }
        
        Move();
        Jump();
        CursorLockState();
        FallOutOfBoundsCheck();
        Menu();
        Spray();
    }

    bool waterOn;

    void Spray()
    {
        bool holding = Input.GetButton("Fire2"); // right mouse held
        FireHose hose = GetHeldHose();

        if (hose != null)
        {
            // holding the hose: the hose sprays, the built-in water stays off
            SetPlayerWater(false);
            hose.SetSpraying(holding); // hose water sync = step 4 (networked hose)
        }
        else
        {
            // no hose: the player's own water sprays on right-click
            SetPlayerWater(holding);
        }
    }

    // turn the built-in water on/off and tell other clients so they see it too
    void SetPlayerWater(bool on)
    {
        if (on == waterOn) return;
        waterOn = on;
        ApplyPlayerWater(on);
        if (PhotonNetwork.IsConnected) PV.RPC(nameof(SetWaterRPC), RpcTarget.Others, on);
    }

    void ApplyPlayerWater(bool on)
    {
        if (playerWater == null) return;
        if (on && !playerWater.isPlaying) playerWater.Play();
        else if (!on && playerWater.isPlaying) playerWater.Stop();
    }

    [PunRPC]
    void SetWaterRPC(bool on)
    {
        ApplyPlayerWater(on);
    }

    // returns the held hose only if the player is holding a FireHose-tagged object
    FireHose GetHeldHose()
    {
        if (grabbing == null) return null;
        GameObject held = grabbing.HeldObject;
        if (held == null || !held.CompareTag("FireHose")) return null;
        return held.GetComponentInChildren<FireHose>();
    }

    void Menu()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (playerSettings != null)
            {
                lookAround = false;
                playerSettings.OpenMenuAndUnlockCursor();
            }
        }
    }
    void FallOutOfBoundsCheck()
    {
        if (transform.position.y < -20f)
        {
            if (SpawnManager.Instance != null)
            {
                Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint();
                if (spawnpoint != null)
                {
                    transform.position = spawnpoint.position;
                    rb.velocity = Vector3.zero;
                }
            }
            else
            {
                Debug.LogWarning("SpawnManager.Instance is null! Resetting to origin.");
                transform.position = new Vector3(0f, 5f, 0f);
                rb.velocity = Vector3.zero;
            }
        }
    }
    
    void Look()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }
    void Move()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);
    }

    void Jump()
    {
        if (Input.GetKey(KeyCode.Space) && grounded)
        {
            rb.AddForce(transform.up * jumpForce);
        }
    }
    public void SetGroundedState(bool _grounded)
    {
        grounded = _grounded;
    }

    private void FixedUpdate()
    {
        if (!PV.IsMine)
            return;

        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);//movement speed isn't from fps but form fixed delta time
    }
   void CursorLockState()
    {
        if (playerSettings != null && playerSettings.isSettingsMenuOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            lookAround = false;

        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            lookAround = true;
        }
    }


}
