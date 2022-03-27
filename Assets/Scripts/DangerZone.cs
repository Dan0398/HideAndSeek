using UnityEngine;

public class DangerZone : MonoBehaviour
{
    [SerializeField] Enemy Parent;

    private void OnTrigger(Collider Target)
    {
        if (Target.CompareTag("Player"))
        { 
            Parent.UpdatePlayerPosition(Target.transform.position); 
        }
    }
}