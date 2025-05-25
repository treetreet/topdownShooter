using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Transform RespawnPoint;

    private float _moveSpeed = 5f;
    public GameObject BulletPrefab;
    public Transform FirePoint;
    private float _bulletSpeed = 100f;
    private Transform _tr;
    private float _initHp;
    private float _maxHp = 100f;
    private float _fireRate = 0.2f;
    private float _fireTimer = 0f;

    private int _maxAmmo = 30;
    private int _currentAmmo;
    private bool _isReloading = false;
    private float _reloadTime = 2f;

    public GameObject HitEffectPrefab;

    void Start()
    {
        _initHp = _maxHp;
        _tr = transform;
        _currentAmmo = _maxAmmo;
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Bullet")
        {
            _initHp -= 10;
            Destroy(coll.gameObject);

            GameObject hitEffect = Instantiate(HitEffectPrefab, transform.position, Quaternion.identity);
            Destroy(hitEffect, 3f);
        }
    }

    void Update()
    {
        Aim();

        if (_isReloading)
            return;

        _fireTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetMouseButton(0) && _fireTimer <= 0f && _currentAmmo > 0)
        {
            Shoot();
            _fireTimer = _fireRate;
            Debug.Log("남은 총알 : " + _currentAmmo);
        }

        if (_currentAmmo <= 0)
        {
            StartCoroutine(Reload());
        }

        if (_initHp <= 0)
        {

            SetChildrenRenderersEnabled(false);
            SetChildrenCollidersEnabled(false);

            _isReloading = true;
            StartCoroutine(Respawn());
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
        _tr.Translate(dir.normalized * Time.deltaTime * _moveSpeed, Space.World);
    }

    void Aim()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;

        Vector2 direction = (mouseWorld - _tr.position).normalized;

        FirePoint.position = _tr.position + (Vector3)(direction);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        FirePoint.rotation = Quaternion.Euler(0, 0, angle - 90f);
        _tr.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(BulletPrefab, FirePoint.position, FirePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(bullet.transform.up * _bulletSpeed);
        Destroy(bullet, 3f);

        _currentAmmo--;
    }

    IEnumerator Reload()
    {
        _isReloading = true;
        Debug.Log("재장전 중");
        yield return new WaitForSeconds(_reloadTime);
        _currentAmmo = _maxAmmo;
        _isReloading = false;
        Debug.Log("재장전 완료");
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(5f);

        if (RespawnPoint != null)
        {
            _tr.position = RespawnPoint.position;
            _tr.rotation = RespawnPoint.rotation;
        }

        SetChildrenRenderersEnabled(true);
        SetChildrenCollidersEnabled(true);

        _initHp = _maxHp;
        _currentAmmo = _maxAmmo;
        _isReloading = false;

        Debug.Log("리스폰 완료");
    }

    void SetChildrenRenderersEnabled(bool enabled)
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in renderers)
        {
            rend.enabled = enabled;
        }
    }

    void SetChildrenCollidersEnabled(bool enabled)
    {
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            col.enabled = enabled;
        }
    }
}
