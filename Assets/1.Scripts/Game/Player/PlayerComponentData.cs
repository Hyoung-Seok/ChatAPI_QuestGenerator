using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerComponentData : MonoBehaviour
{
    [Header("Component")]
    public Rigidbody Rig;
    public Transform PlayerTransform;
    public Transform CameraDir;
    public Animator Animator;
    public AudioSource AudioSource;
    public PlayerInput PlayerInput;
}
