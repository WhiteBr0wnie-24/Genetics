using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayfieldController : MonoBehaviour
{
    public GameObject foodPrefab;
    public GameObject enemyPrefab;

    public float leftMarker, rightMarker, topMarker, bottomMarker;

    private List<Food> food = new List<Food>();
    private List<Enemy> enemies = new List<Enemy>();
    private List<Enemy> deadEnemies = new List<Enemy>();

    private int round = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Spawn Food
        for (int i = 0; i < SimulationParameters.FOOD_START_AMOUNT; i++)
        {
            Vector3 position = new Vector3(Random.Range(leftMarker, rightMarker), 0, Random.Range(bottomMarker, topMarker));
            GameObject g = Instantiate(foodPrefab, position, Quaternion.identity);
            Food f = g.GetComponent<Food>();

            f.nutrition = SimulationParameters.FOOD_NUTRITION;
            food.Add(f);
        }

        StartCoroutine("spawnFood");
        StartCoroutine("mateEnemies");
        StartCoroutine("advanceTime");
    }

    private IEnumerator advanceTime()
    {
        while(true)
        {
            try
            {
                foreach (Enemy e in enemies)
                {
                    e.increaseAge();
                }
            }
            catch
            { }

            yield return new WaitForSeconds(SimulationParameters.YEAR_LENGTH);
        }
    }

    private IEnumerator spawnFood()
    {
        while(true)
        {
            if(food.Count < SimulationParameters.MAX_FOOD)
            {
                Vector3 position = new Vector3(Random.Range(leftMarker, rightMarker), 0, Random.Range(bottomMarker, topMarker));
                GameObject g = Instantiate(foodPrefab, position, Quaternion.identity);
                Food f = g.GetComponent<Food>();

                f.nutrition = SimulationParameters.FOOD_NUTRITION;
                food.Add(f);
            }

            yield return new WaitForSeconds(SimulationParameters.GENERATE_FOOD_DELAY);
        }
    }

    private IEnumerator mateEnemies()
    {
        while (true)
        {
            if (enemies.Count == 0)
            {
                // Use previous generations to create new ones
                if (deadEnemies.Count > 0)
                {
                    List<Enemy> currentDeadOnes = new List<Enemy>();
                    int limit = Mathf.FloorToInt(deadEnemies.Count() / 2f);

                    foreach (Enemy e in deadEnemies)
                        currentDeadOnes.Add(e);

                    currentDeadOnes.Sort((x, y) => y.getFitness().CompareTo(x.getFitness()));
                    deadEnemies.Clear();

                    foreach (var e in currentDeadOnes)
                    {
                        Debug.Log(e.getFitness());
                    }

                    // Mate the top 50%
                    for (int i = 0; i < limit; i += 2)
                    {
                        if (i + 1 < currentDeadOnes.Count)
                        {
                            Vector3 position = new Vector3(Random.Range(leftMarker, rightMarker), 0, Random.Range(bottomMarker, topMarker));
                            GameObject child = Instantiate(enemyPrefab, position, Quaternion.identity);
                            Enemy e = child.GetComponent<Enemy>();

                            e.father = currentDeadOnes[i].getGenome();
                            e.mother = currentDeadOnes[i + 1].getGenome();
                            enemies.Add(e);
                        }
                    }
                }

                // Fill the remaining slots with new random enemies
                while (enemies.Count < SimulationParameters.POPULATION_START_SIZE)
                {
                    Vector3 position = new Vector3(Random.Range(leftMarker, rightMarker), 0, Random.Range(bottomMarker, topMarker));
                    GameObject g = Instantiate(enemyPrefab, position, Quaternion.identity);
                    Enemy e = g.GetComponent<Enemy>();

                    enemies.Add(e);
                }
            }

            yield return new WaitForSeconds(SimulationParameters.GENERATE_OFFSPRING_DELAY);
        }
    }

    public Vector3 findNearestFood(Vector3 pos, float perceptionDistance)
    {
        GameObject nearest = null;
        float smallestDistance = float.MaxValue;

        foreach (Food f in food)
        {
            float d = Vector3.Distance(f.transform.position, pos);

            if (d < smallestDistance && d <= perceptionDistance)
            {
                nearest = f.gameObject;
                smallestDistance = d;

                if (d < SimulationParameters.FOOD_DISTANCE_RETURN_THRESHOLD)
                    return f.transform.position;
            }
        }

        return pos;
    }

    public void remove(Enemy e)
    {
        enemies.Remove(e);
        deadEnemies.Add(e);

        Destroy(e.gameObject);
    }

    public void remove(Food f)
    {
        food.Remove(f);
    }
}