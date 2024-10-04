using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    public GameObject[] weapons;
    private int currentWeaponIndex = 0;
    
    public float fireRate = 0.1f;
    public float weaponRange = 50f;
    public float damageAmount = 10f;
    private float nextFireTime = 0f;

    public int maxAmmo = 30;
    private int currentAmmo;

    public GameObject bulletPrefab;
    public float bulletSpeed = 20f;
    public Transform firePoint;

    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float gravity = -9.81f;
    public float zoomFOV = 30f;
    private float normalFOV;
    private CharacterController controller;
    private Camera playerCamera;

    private Vector3 playerVelocity;
    private bool isGrounded;

    void Start()
    {
        currentAmmo = maxAmmo;
        controller = GetComponent<CharacterController>();
        playerCamera = Camera.main;
        normalFOV = playerCamera.fieldOfView;

        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(i == 0);
        }
    }

    void Update()
    {
        HandleWeaponControls();
        HandleMovement();
        HandleZoom();
    }

    void HandleWeaponControls()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SwitchWeapon();
        }

        for (int i = 0; i < weapons.Length; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                SwitchToWeapon(i);
            }
        }

        if (Input.GetMouseButton(0) && Time.time >= nextFireTime && currentAmmo > 0)
        {
            FireWeapon();
            nextFireTime = Time.time + fireRate;
            currentAmmo--;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }

    void HandleMovement()
    {
        isGrounded = controller.isGrounded;

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = transform.right * moveHorizontal + transform.forward * moveVertical;
        controller.Move(movement * moveSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }
    }

    void HandleZoom()
    {
        if (Input.GetMouseButton(1))
        {
            playerCamera.fieldOfView = zoomFOV;
        }
        else
        {
            playerCamera.fieldOfView = normalFOV;
        }
    }

    void SwitchWeapon()
    {
        weapons[currentWeaponIndex].SetActive(false);
        currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Length;
        weapons[currentWeaponIndex].SetActive(true);
        Reload();
    }

    void SwitchToWeapon(int index)
    {
        if (index < 0 || index >= weapons.Length) return;
        weapons[currentWeaponIndex].SetActive(false);
        currentWeaponIndex = index;
        weapons[currentWeaponIndex].SetActive(true);
        Reload();
    }

    void FireWeapon()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.speed = bulletSpeed;
        }

        RaycastHit hit;
        if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, weaponRange))
        {
            Debug.Log("Hit: " + hit.transform.name);
            HealthSystem targetHealth = hit.transform.GetComponent<HealthSystem>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(damageAmount);
            }
        }
    }

    void Reload()
    {
        currentAmmo = maxAmmo;
    }

    public int GetCurrentAmmo()
    {
        return currentAmmo;
    }

    public int GetMaxAmmo()
    {
        return maxAmmo;
    }

    public string GetCurrentWeaponName()
    {
        return weapons[currentWeaponIndex].name;
    }
}