using UnityEngine;
using UnityEngine.Events;

public class PhotoCell : MonoBehaviour
{
    public UnityEvent myEvent;
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            myEvent.Invoke();
        }
    }
}
