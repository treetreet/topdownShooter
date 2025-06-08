using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class PlayerMovement : NetworkBehaviour
{
    public Transform RespawnPoint;
    public GameObject BulletPrefab;
    public Transform FirePoint;
    public GameObject HitEffectPrefab;

    private Transform _tr;
    private float _moveSpeed = 5f;
    private float _bulletSpeed = 100f;
    private float _fireRate = 0.2f;
    private float _fireTimer = 0f;
    private float _reloadTime = 2f;

    private NetworkVariable<float> _hp = new(100f);
    private float _maxHp = 100f;

    private NetworkVariable<int> _currentAmmo = new(30);
    private int _maxAmmo = 30;

    private bool _isReloading = false;
    private bool _isDead = false;
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            _tr = transform;
        }
    }

    void Update()
    {
        if (!IsOwner) return;
        if (_isDead) return;
        
        Fire(); 

        if (_currentAmmo.Value <= 0 && !_isReloading)
        {
            StartCoroutine(Reload());
        }

        if (_hp.Value <= 0 && !_isReloading)
        {
            _isDead = true;
            _isReloading = true;
            RespawnServerRpc();
        }
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        if (_isDead) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(h, v, 0f);
        MoveServerRpc(dir);
    }

    void Fire()
    {
        Aim();

        if (_isReloading) return;

        _fireTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetMouseButton(0) && _fireTimer <= 0f && _currentAmmo.Value > 0)
        {
            ShootServerRpc(FirePoint.position, FirePoint.rotation);
            _fireTimer = _fireRate;
        }
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

    [ServerRpc]
    void MoveServerRpc(Vector3 dir)
    {
        transform.Translate(dir.normalized * Time.fixedDeltaTime * _moveSpeed, Space.World);
    }

    [ServerRpc]
    void ShootServerRpc(Vector3 firePos, Quaternion fireRot)
    {
        GameObject bullet = Instantiate(BulletPrefab, firePos, fireRot);

        // 총알에 NetworkObject 컴포넌트가 있어야 함
        NetworkObject netObj = bullet.GetComponent<NetworkObject>();
        if (netObj != null)
        {
            netObj.Spawn(); // 네트워크 전체에 동기화
        }
        else
        {
            Debug.LogError("Bullet prefab에 NetworkObject가 없습니다!");
        }

        bullet.GetComponent<Rigidbody2D>().AddForce(bullet.transform.up * _bulletSpeed);
        Destroy(bullet, 3f);

        _currentAmmo.Value--;
    }

    IEnumerator Reload()
    {
        _isReloading = true;
        Debug.Log("재장전 중");
        yield return new WaitForSeconds(_reloadTime);
        ReloadServerRpc();
    }

    [ServerRpc]
    void ReloadServerRpc()
    {
        _currentAmmo.Value = _maxAmmo;
        _isReloading = false;
        Debug.Log("재장전 완료");
    }

    [ServerRpc]
    void RespawnServerRpc()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(5f);

        if (RespawnPoint != null)
        {
            transform.position = RespawnPoint.position;
            transform.rotation = RespawnPoint.rotation;
        }
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<Collider2D>().enabled = true;
        _hp.Value = _maxHp;
        _currentAmmo.Value = _maxAmmo;
        _isReloading = false;
        _isDead = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer) return;

        if (other.gameObject.CompareTag("Bullet"))
        {
            _hp.Value -= 10;
            Destroy(other.gameObject);

            //SpawnHitEffectClientRpc(transform.position);
        }
    }

    /*[ClientRpc]
    void SpawnHitEffectClientRpc(Vector3 position)
    {
        GameObject hitEffect = Instantiate(HitEffectPrefab, position, Quaternion.identity);
        Destroy(hitEffect, 3f);
    }*/
}
