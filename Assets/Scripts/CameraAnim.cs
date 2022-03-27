using UnityEngine;

public class CameraAnim : MonoBehaviour
{
    Vector3 DefaultPosition;
    [SerializeField, Range(0, 1.0f)] float AnimationWide;

    void Start()
    {
        DefaultPosition = transform.position - AnimationWide * 0.5f * Vector3.right;
    }

    private void OnPreRender()
    {
        transform.position = DefaultPosition + AnimationWide * Vector3.right * (Input.mousePosition.x / Screen.width);
    }
}