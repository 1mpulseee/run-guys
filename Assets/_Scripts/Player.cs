using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Player : MonoBehaviour
{
    public Transform Cam;
    public Transform CamOrigin;

    [Space(5)]
    public float WalkSpeed = 2.5f; // макс. скорость
    public int JumpForce;
    public int SlideForwardForce;
    public int SlideUpForce;

    private Rigidbody rb;
    private Animator anim;
    private PhotonView pv;

    private Vector3 direction;
    private float h, v;

    public float RayDis;
    private bool InAir;

    private IEnumerator _Jump;
    private IEnumerator _Slide;

    [HideInInspector] public bool IsLocalPlayer = true;
    public GameObject LocalObjects;
    private void Awake()
    {
        pv = GetComponentInParent<PhotonView>();
        if (pv.IsMine)
        {
            IsLocalPlayer = true;
        }
        else
        {
            IsLocalPlayer = false;
            LocalObjects.SetActive(false);
        }
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }
    void Update()
    {
        if (IsLocalPlayer)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");
            CamOrigin.position = transform.position;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (_Slide == null)
                {
                    if (_Jump != null || InAir)
                    {
                        _Slide = Slide();
                        StartCoroutine(_Slide);
                        pv.RPC("SlideOnline", RpcTarget.Others);
                    }
                }
                if (!InAir && _Jump == null)
                {
                    _Jump = Jump();
                    StartCoroutine(_Jump);
                    pv.RPC("JumpOnline", RpcTarget.Others);
                }
            }
        }
    }
    //[PunRPC]
    public IEnumerator Jump()
    {
        anim.SetTrigger("jump");
        yield return new WaitForSeconds(.45f);
        rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
        yield return new WaitForSeconds(1f);
        _Jump = null;
    }
    //[PunRPC]
    public IEnumerator Slide()
    {
        anim.SetBool("slide", true);
        anim.SetBool("Run", false);
        anim.SetBool("idle", false);
        rb.AddForce(Vector3.up * SlideUpForce, ForceMode.Impulse);
        rb.AddForce(transform.forward * SlideForwardForce, ForceMode.Impulse);
        yield return new WaitForSeconds(.2f);
        rb.drag = .1f;
        while (true)
        {
            if (rb.velocity.magnitude < 3)
            {
                anim.SetBool("slide", false);
                rb.drag = 2;
                _Slide = null;
                break;
            }
            yield return new WaitForSeconds(.05f);
        }
    }

    [PunRPC]
    private void SlideOnline()
    {
        StartCoroutine(Slide());
    }

    [PunRPC]
    private void JumpOnline()
    {
        StartCoroutine(Jump());
    }
    private void FixedUpdate()
    {
        if (IsLocalPlayer)
        {
            direction = new Vector3(h, 0, v);
            direction = Cam.TransformDirection(direction);
            direction = new Vector3(direction.x, 0, direction.z);
            if (Mathf.Abs(v) > 0 || Mathf.Abs(h) > 0)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), 5 * Time.deltaTime);
            }
            Vector2 MoveDir = Vector2.zero;
            MoveDir.x = Mathf.Cos((transform.rotation.eulerAngles.y - 90) * Mathf.Deg2Rad);
            MoveDir.y = Mathf.Sin((transform.rotation.eulerAngles.y + 90) * Mathf.Deg2Rad);
            if (_Slide == null)
            {
                if (h > 0.1f || h < -0.1f || v > 0.1f || v < -0.1f)
                {
                    rb.AddForce(new Vector3(MoveDir.x, 0, MoveDir.y).normalized * rb.mass * WalkSpeed);
                    anim.SetBool("Run", true);
                    anim.SetBool("idle", false);
                }
                else
                {
                    anim.SetBool("Run", false);
                    anim.SetBool("idle", true);
                }
            }
            if (Physics.Raycast(transform.position + new Vector3(0, .1f, 0), Vector3.down, RayDis) || Physics.Raycast(transform.position + new Vector3(0, .6f, 0), Vector3.down, RayDis))
            {
                InAir = false;
                anim.SetBool("InJump", false);
            }
            else
            {
                InAir = true;
                anim.SetBool("InJump", true);
            }
        }
    }   
}