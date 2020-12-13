using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject bulletPrefab;

    public float speed = 2.5f;
    public float health = 100.0f;
    public float bulletSpawnDistance = 2.0f;

    private float lastFireTime = 0;

    private Ray cameraRay;
    private RaycastHit hit;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        // If the ray strikes an object...
        if (Physics.Raycast(cameraRay, out hit))
        {
            // ...and if that object is the ground...
            // if (hit.transform.CompareTag("GameController"))
            {
                // ...make the cube rotate (only on the Y axis) to face the ray hit's position 
                Vector3 targetPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);

                transform.LookAt(targetPosition);
            }
        }

        // Walk if the player presses the appropriate buttons
        transform.position = transform.position + transform.forward * v * speed * Time.deltaTime;
        transform.position = transform.position + transform.right * h * speed * Time.deltaTime;

        if (Input.GetMouseButtonDown(0) && lastFireTime + SimulationParameters.BULLET_FIRE_DELAY <= Time.time)
        {
            Instantiate(bulletPrefab, transform.position + transform.forward * bulletSpawnDistance, transform.rotation);
            lastFireTime = Time.time;
        }

        if (health <= 0f)
            die();
    }

    public void die()
    {
        Destroy(gameObject);
    }

    public void damage(float d)
    {
        health -= d;
    }
}
