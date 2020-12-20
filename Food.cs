using UnityEngine;

public class Food : MonoBehaviour
{
    public float nutrition = 50f;

    private PlayfieldController pfc;
    private float start;

    void Start()
    {
        pfc = GameObject.FindWithTag("GameController").GetComponent<PlayfieldController>();
        start = Time.time;
    }

    void FixedUpdate()
    {
        transform.Rotate(0.0f, 0.25f, 0.0f, Space.Self);

        if (start + SimulationParameters.FOOD_SPOIL_TIME < Time.time)
        {
            pfc.remove(this);
        }
    }

    public float consume()
    {
        pfc.remove(this);

        return nutrition;
    }
}