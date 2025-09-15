using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class SimplePlayerController : NetworkBehaviour
{
    public NetworkVariable<ulong> PlayerID;

    public GameObject projectilePrefab;
    public Transform firePoint;

    private Animator animator;
    public float Speed;
    public LayerMask GroundLayer;
    public Rigidbody rb;
    public int JumpForce;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!IsOwner) return;

        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            float VelX = Input.GetAxisRaw("Horizontal") * Speed * Time.deltaTime;
            float VelY = Input.GetAxisRaw("Vertical") * Speed * Time.deltaTime;
            UpdatePositionRpc(VelX, VelY);
        }

        CheckGroundRpc();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            AnimatorSetTriggerRpc("Jump");
        }

        // Rotar localmente para que el jugador sienta respuesta inmediata
        RotateToMouse();

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 shootDirection = CalculateShootDirection();
            ShootServerRpc(shootDirection);
        }
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        Vector3 shootDirection = CalculateShootDirection();
        ShootServerRpc(shootDirection);
    }

    [Rpc(SendTo.Server)]
    public void UpdatePositionRpc(float x, float y)
    {
        transform.position += new Vector3(x, 0, y);
    }

    [Rpc(SendTo.Server)]
    public void AnimatorSetTriggerRpc(string animationName)
    {
        animator.SetTrigger(animationName);
    }

    [Rpc(SendTo.Server)]
    public void CheckGroundRpc()
    {
        if (Physics.Raycast(transform.position, Vector3.down, 1.1f, GroundLayer))
        {
            animator.SetBool("Grounded", true);
            animator.SetBool("FreeFall", false);
        }
        else
        {
            animator.SetBool("Grounded", false);
            animator.SetBool("FreeFall", true);
        }
    }

    [Rpc(SendTo.Server)]
    public void JumpTrigerRpc(string animationName)
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
        animator.SetTrigger(animationName);
    }

    // Nuevo ServerRpc para disparar con la dirección enviada desde el cliente
    [ServerRpc]
    public void ShootServerRpc(Vector3 shootDirection)
    {
        if (shootDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(shootDirection);
        }

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(shootDirection));
        proj.GetComponent<NetworkObject>().Spawn(true);

        Projectile projectileScript = proj.GetComponent<Projectile>();
        Playerlab4 playerLab = GetComponent<Playerlab4>();
        if (projectileScript != null && playerLab != null)
        {
            projectileScript.damage = playerLab.attack.Value; // Asigna daño del jugador al proyectil
        }

        Vector3 forceDirection = new Vector3(shootDirection.x, 0f, shootDirection.z).normalized;
        proj.GetComponent<Rigidbody>().AddForce(forceDirection * 20f, ForceMode.Impulse);
    }


    // Método que calcula la dirección hacia el mouse en el cliente
    private Vector3 CalculateShootDirection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        float rayDistance;

        Vector3 shootDirection = transform.forward;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 pointToLook = ray.GetPoint(rayDistance);
            Vector3 direction = pointToLook - firePoint.position;
            direction.y = 0;
            shootDirection = direction.normalized;
        }
        return shootDirection;
    }

    public void RotateToMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 pointToLook = ray.GetPoint(rayDistance);

            Vector3 lookDirection = pointToLook - transform.position;
            lookDirection.y = 0;

            if (lookDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
            }
        }
    }
}
