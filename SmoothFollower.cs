using UnityEngine;

public class SmoothFollower : MonoBehaviour
{
    public GameObject target;

    void Update()
    {
        if (target)
        {
            Vector3 followPosition = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
            transform.position = Vector3.Lerp(transform.position, followPosition, 0.1f);
        }
    }
}