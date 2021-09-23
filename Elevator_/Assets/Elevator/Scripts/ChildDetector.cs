using UnityEngine;

public class ChildDetector : MonoBehaviour
{
    [SerializeField]
    private Transform parent;
    private void OnCollisionEnter(Collision collision)
    {
        collision.transform.SetParent(parent);
    }
    private void OnCollisionExit(Collision collision)
    {
        collision.transform.SetParent(null);
    }
}
