using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bot : MonoBehaviour
{
    public float WalkSpeed = 2.5f; // макс. скорость
    public int JumpForce;

    private Rigidbody rb;
    private Animator anim;
    private PhotonView pv;

    private Vector3 direction;

    public float RayDis;
    private bool InAir;



    public float Dis;
    public bool IsStop = false;

    private Transform CurrentPoint;
    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        if (PhotonNetwork.IsMasterClient)
        {
            CurrentPoint = Manager.Instance.Ways[0];
            for (int i = 1; i < Manager.Instance.Ways.Length; i++)
            {
                if (Vector3.Distance(transform.position, CurrentPoint.position) > Vector3.Distance(transform.position, Manager.Instance.Ways[i].position))
                {
                    CurrentPoint = Manager.Instance.Ways[i];
                }
            }
        }
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            direction = transform.TransformDirection(direction);
            if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(CurrentPoint.position.x, CurrentPoint.position.z)) < Dis)
            {
                GetNextPoint();
            }
            if (!IsStop)
            {
                direction = new Vector3(CurrentPoint.position.x, 0, CurrentPoint.position.z) - new Vector3(transform.position.x, 0, transform.position.z);
                direction = direction.normalized;
            }
            else
            {
                direction = Vector3.zero;
            }
        }
    }
    public IEnumerator Jump()
    {
        anim.SetTrigger("jump");
        yield return new WaitForSeconds(.45f);
        rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
        yield return new WaitForSeconds(1f);
    }

    public void GetNextPoint()
    {
        Point point = CurrentPoint.gameObject.GetComponent<Point>();
        CurrentPoint = point.NextPoints[Random.Range(0, point.NextPoints.Length)];
        Point NewPoint = CurrentPoint.GetComponent<Point>();
        if (NewPoint.IsJump)
            pv.RPC("JumpOnline", RpcTarget.All);
        IsStop = NewPoint.IsEnd;
    }

    [PunRPC]
    public void JumpOnline()
    {
        StartCoroutine(Jump());
    }
    private void FixedUpdate()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Vector2 MoveDir = Vector2.zero;
            MoveDir.x = Mathf.Cos((transform.rotation.eulerAngles.y - 90) * Mathf.Deg2Rad);
            MoveDir.y = Mathf.Sin((transform.rotation.eulerAngles.y + 90) * Mathf.Deg2Rad);
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), 5 * Time.deltaTime);
                rb.AddForce(new Vector3(MoveDir.x, 0, MoveDir.y).normalized * rb.mass * WalkSpeed);
                anim.SetBool("Run", true);
                anim.SetBool("idle", false);
            }
            else
            {
                anim.SetBool("Run", false);
                anim.SetBool("idle", true);
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
    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(transform.position + direction * 5, Vector3.one);
        Gizmos.DrawCube(CurrentPoint.position, Vector3.one);
    }
}