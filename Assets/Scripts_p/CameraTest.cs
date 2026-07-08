using Unity.Mathematics.Geometry;
using Unity.VisualScripting;
using UnityEngine;

public class CameraTest : MonoBehaviour
{
    [SerializeField] private Transform target;
    void LateUpdate()
    {
        transform.position = new Vector3(Mathf.Lerp(transform.position.x,target.position.x,5f),0,-10);
    }
}
