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
    private List<EnemyGenome> deadEnemies = new List<EnemyGenome>();

    private float averageFitness = 0f;

    // Start is called before the first frame update
    void Start()
    {
        // Spawn a random population
        for (int i = 0; i < SimulationParameters.POPULATION_START_SIZE; i++)
        {
            Vector3 position = new Vector3(Random.Range(leftMarker, rightMarker), 0, Random.Range(bottomMarker, topMarker));
            GameObject g = Instantiate(enemyPrefab, position, Quaternion.identity);
            Enemy e = g.GetComponent<Enemy>();

            enemies.Add(e);
        }

        // Spawn Food
        for (int i = 0; i < SimulationParameters.FOOD_START_AMOUNT; i++)
        {
            Vector3 position = new Vector3(Random.Range(leftMarker, rightMarker), 0, Random.Range(bottomMarker, topMarker));
            GameObject g = Instantiate(foodPrefab, position, Quaternion.identity);
            Food f = g.GetComponent<Food>();

            f.nutrition = SimulationParameters.FOOD_NUTRITION;
            food.Add(f);
        }

        Invoke("startCoroutines", 1.0f);
    }

    private void startCoroutines()
    {
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
            int limit;
            float fitnessSum = 0f;
            float oldAverageFitness = averageFitness;
            List<Enemy> currentPopulation = new List<Enemy>();

            foreach (Enemy e in enemies)
            {
                currentPopulation.Add(e);
                fitnessSum += e.getFitness();
            }

            currentPopulation.Sort((x, y) => y.getFitness().CompareTo(x.getFitness()));
            limit = Mathf.FloorToInt(currentPopulation.Count / 2.0f);

            for (int i = 0; i < limit; i+=2)
            {
                if (i + 1 < limit)
                {
                    Vector3 position = new Vector3(Random.Range(leftMarker, rightMarker), 0, Random.Range(bottomMarker, topMarker));
                    GameObject child = Instantiate(enemyPrefab, position, Quaternion.identity);
                    Enemy e = child.GetComponent<Enemy>();

                    e.father = currentPopulation[i].getGenome();
                    e.mother = currentPopulation[i+1].getGenome();
                    enemies.Add(e);

                    fitnessSum += e.getFitness();
                }
            }

            averageFitness = fitnessSum / enemies.Count;

            Debug.Log("Spawned " + (enemies.Count - currentPopulation.Count) + " new enemies. Average fitness changed from " + oldAverageFitness + " to " + averageFitness);

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
        Debug.Log("Enemy died! Remaining: " + enemies.Count);

        enemies.Remove(e);
        deadEnemies.Add(e.getGenome());

        Destroy(e.gameObject);
    }

    public void remove(Food f)
    {
        food.Remove(f);
    }
}