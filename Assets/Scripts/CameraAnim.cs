using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnim : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnPreRender()
    {
        transform.position = new Vector3(-0.4f + 0.8f * (Input.mousePosition.x / Screen.width), 0.72f, -2.46f);
    }
}
