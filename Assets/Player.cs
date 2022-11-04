using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform Cam;
    public Transform CamOrigin;

    [Space(5)]
    public float WalkSpeed = 2.5f; // макс. скорость
    public float WalkAcceleration = 15; // ускорение

    private Rigidbody rb;
    private Animator anim;

    private Vector3 direction;
    private float h, v;
    private float speed = 1;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }
    void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        CamOrigin.position = transform.position;
    }
    private void FixedUpdate()
    {
        // вектор направления движения
        direction = new Vector3(h, 0, v);
        direction = Cam.TransformDirection(direction);
        direction = new Vector3(direction.x, 0, direction.z);

        if (Mathf.Abs(v) > 0 || Mathf.Abs(h) > 0) // разворот тела по вектору движения
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), 5 * Time.deltaTime);
        }

        Vector2 MoveDir = Vector2.zero;
        MoveDir.x = Mathf.Cos((transform.rotation.eulerAngles.y - 90) * Mathf.Deg2Rad);
        MoveDir.y = Mathf.Sin((transform.rotation.eulerAngles.y + 90) * Mathf.Deg2Rad);

        if (h > 0.1f || h < -0.1f || v > 0.1f || v < -0.1f) //персонаж двигается
        {
                rb.AddForce(new Vector3(MoveDir.x, 0, MoveDir.y).normalized * WalkAcceleration * rb.mass * WalkSpeed);
                //anim.SetBool("run", false);
                //anim.SetBool("walk", true);
                //anim.SetBool("idle", false);
        }
        else //персонаж стоит
        {
            //anim.SetBool("run", false);
            //anim.SetBool("walk", false);
            //anim.SetBool("idle", true);
        }
    }
}
