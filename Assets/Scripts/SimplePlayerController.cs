using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class SimplePlayerController : NetworkBehaviour
{
    public NetworkVariable<ulong> PlayerID;

    private InputSystem_Actions Accion;

    public GameObject ProjectilePrefab;
    public Transform FirePoint;

    private Animator animator;
    public float Speed;
    public LayerMask GroundLayer;
    public Rigidbody rb;
    public int JumpForce;


    private void OnEnable()
    {
        
    }
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
        if (Input.GetKeyDown(KeyCode.C))
        {
            ShootRpc();
        }
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        ShootRpc();
    }

    [Rpc(SendTo.Server)]
    public void UpdatePositionRpc(float x,float y)
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
        rb=GetComponent<Rigidbody>();
        rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
        animator.SetTrigger(animationName);
    }

    [Rpc(SendTo.Server)]
    public void ShootRpc()
    {
        GameObject proj = Instantiate(ProjectilePrefab,FirePoint.position,Quaternion.identity);
        proj.GetComponent<NetworkObject>().Spawn(true);

        proj.GetComponent<Rigidbody>().AddForce(Vector3.forward * 5, ForceMode.Impulse);
    }
}