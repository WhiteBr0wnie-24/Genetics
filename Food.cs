using UnityEngine;

public class Food : MonoBehaviour
{
    public float nutrition = 50;

    private PlayfieldController pfc;

    void Start()
    {
        pfc = GameObject.FindWithTag("GameController").GetComponent<PlayfieldController>();
    }

    void FixedUpdate()
    {
        transform.Rotate(0.0f, 0.25f, 0.0f, Space.Self);
    }

    public float consume()
    {
        pfc.remove(this);
        Destroy(gameObject);

        return nutrition;
    }
}