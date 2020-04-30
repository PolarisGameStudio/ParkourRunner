using UnityEngine;

public class Gender : MonoBehaviour
{
    public enum GenderKinds
    {
        Male,
        Female
    }
        
    public static GenderKinds Kind { get; set; }

    [SerializeField] private GenderKinds _kind;

    private void OnEnable()
    {
        Kind = _kind;
    }
}