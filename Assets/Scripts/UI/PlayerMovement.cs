using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
        
    private float _moveSpeed = 5f;
    public GameObject BulletPrefab;
    public Transform FirePoint; 
    private float _bulletSpeed = 100f;
    private Transform _tr;
    private float _initHp;
    private float _maxHp = 100f;
    private float _fireRate = 0.2f; 
    private float _fireTimer = 0f;
    void Start()
    {
        _initHp = _maxHp;
        _tr = transform;
    }
    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Bullet")
        {
            _initHp -= 10;
            Destroy(coll.gameObject);
        }
    }
    void Update()
    {
        Aim();

        _fireTimer -= Time.deltaTime;

        if (Input.GetMouseButton(0) && _fireTimer <= 0f)
        {
            Shoot();
            _fireTimer = _fireRate;
        }

        if (_initHp <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 dir = new Vector3(h, v, 0);
        _tr.Translate(dir.normalized * Time.deltaTime * _moveSpeed);
    }

    void Aim()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;

        Vector2 direction = (mouseWorld - _tr.position).normalized;

        FirePoint.position = _tr.position + (Vector3)(direction);
            
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        FirePoint.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(BulletPrefab, FirePoint.position, FirePoint.rotation);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(bullet.transform.up * _bulletSpeed);
            
        Destroy(bullet, 3f);

    }
}