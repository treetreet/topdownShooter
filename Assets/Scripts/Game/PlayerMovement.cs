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

    private NetworkVariable<bool> _netIsDead = new(false); // ✅ 사망 상태 공유
    private bool _isDead = false;
    private bool _isReloading = false;

    public override void OnNetworkSpawn()
    {
        _tr = transform;
    }

    private void Update()
    {
        if (!IsOwner || _netIsDead.Value) return;

        Fire();

        // 🔁 클라이언트에서 재장전 키 입력 → 서버에게 요청
        if (Input.GetKeyDown(KeyCode.R) && !_isReloading && _currentAmmo.Value < _maxAmmo)
        {
            ReloadRequestServerRpc();
        }
    }

    private void FixedUpdate()
    {
        if (!IsOwner || _netIsDead.Value) return;

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
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Vector3 newRotation = new Vector3(0, 0, angle - 90f);

        _tr.rotation = Quaternion.Euler(newRotation);
        FirePoint.rotation = Quaternion.Euler(newRotation);

        UpdateRotationServerRpc(newRotation);
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
        var netObj = bullet.GetComponent<NetworkObject>();
        if (netObj != null)
        {
            netObj.Spawn();
        }

        bullet.GetComponent<Rigidbody2D>().AddForce(bullet.transform.up * _bulletSpeed);
        Destroy(bullet, 3f);

        _currentAmmo.Value--;
    }

    [ServerRpc]
    void ReloadRequestServerRpc()
    {
        if (_isReloading || _currentAmmo.Value == _maxAmmo) return;
        StartCoroutine(ReloadCoroutine());
    }

    IEnumerator ReloadCoroutine()
    {
        _isReloading = true;
        Debug.Log("재장전 중");
        yield return new WaitForSeconds(_reloadTime);
        _currentAmmo.Value = _maxAmmo;
        _isReloading = false;
        Debug.Log("재장전 완료");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer) return;

        if (other.CompareTag("Bullet"))
        {
            _hp.Value -= 10;
            Destroy(other.gameObject);

            if (_hp.Value <= 0 && !_isDead)
            {
                _isDead = true;
                _netIsDead.Value = true;
                _isReloading = true;

                DieServerRpc(OwnerClientId); // ✅ 죽은 플레이어 ID 전송
                StartCoroutine(Respawn());
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    void DieServerRpc(ulong deadPlayerId)
    {
        foreach (var obj in FindObjectsOfType<PlayerMovement>())
        {
            if (obj.OwnerClientId == deadPlayerId)
            {
                obj._netIsDead.Value = true;
                obj.DieClientRpc(deadPlayerId);
                break;
            }
        }
    }
    [ClientRpc]
    void DieClientRpc(ulong deadPlayerId)
    {
        foreach (var obj in FindObjectsOfType<PlayerMovement>())
        {
            if (obj.OwnerClientId == deadPlayerId)
            {
                obj.GetComponent<SpriteRenderer>().enabled = false;
                obj.GetComponent<Collider2D>().enabled = false;
                break;
            }
        }
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(5f);

        if (RespawnPoint != null)
        {
            transform.position = RespawnPoint.position;
            transform.rotation = RespawnPoint.rotation;
        }

        _hp.Value = _maxHp;
        _currentAmmo.Value = _maxAmmo;
        _isReloading = false;
        _isDead = false;
        _netIsDead.Value = false;

        ReviveServerRpc(OwnerClientId);
    }
    [ServerRpc(RequireOwnership = false)]
    void ReviveServerRpc(ulong revivedPlayerId)
    {
        foreach (var obj in FindObjectsOfType<PlayerMovement>())
        {
            if (obj.OwnerClientId == revivedPlayerId)
            {
                obj._netIsDead.Value = false;
                obj.ReviveClientRpc(revivedPlayerId);
                break;
            }
        }
    }
    [ClientRpc]
    void ReviveClientRpc(ulong revivedPlayerId)
    {
        foreach (var obj in FindObjectsOfType<PlayerMovement>())
        {
            if (obj.OwnerClientId == revivedPlayerId)
            {
                obj.GetComponent<SpriteRenderer>().enabled = true;
                obj.GetComponent<Collider2D>().enabled = true;
                break;
            }
        }
    }

    [ServerRpc]
    void UpdateRotationServerRpc(Vector3 rot)
    {
        UpdateRotationClientRpc(rot);
    }

    [ClientRpc]
    void UpdateRotationClientRpc(Vector3 rot)
    {
        if (!IsOwner)
        {
            _tr.rotation = Quaternion.Euler(rot);
            FirePoint.rotation = Quaternion.Euler(rot);
        }
    }
}
