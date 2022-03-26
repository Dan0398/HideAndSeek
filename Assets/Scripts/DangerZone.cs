using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerZone : MonoBehaviour
{
    [SerializeField] Enemy Parent;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        { Parent.EyeView(other.transform.position); }
    }
}
