using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class PlayfieldController : MonoBehaviour
{
    public GameObject foodPrefab;
    public GameObject entityPrefab;
    public GameObject enemyPrefab;

    public float leftMarker, rightMarker, topMarker, bottomMarker;

    private List<Food> food = new List<Food>();
    private List<Entity> entities = new List<Entity>();
    private List<Enemy> enemies = new List<Enemy>();

    private float averageEntityFitness = 0f;
    private float averageEnemyFitness = 0f;
    private int entityGeneration = 0;
    private int enemyGeneration = 0;
    private int experiment = 1;

    private string filePath = @"C:\Users\Daniel\Desktop\";
    private StreamWriter entity_writer;
    private StreamWriter enemy_writer;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("spawnFood");
        StartCoroutine("mateEntities");
        StartCoroutine("mateEnemies");
        StartCoroutine("advanceTime");
        StartCoroutine("debugFitness");
    }

    private IEnumerator debugFitness()
    {
        while(true)
        {
            float entity_fitnessSum = 0f;
            float enemy_fitnessSum = 0f;

            foreach (Entity e in entities)
                entity_fitnessSum += e.getFitness();

            foreach (Enemy e in enemies)
                enemy_fitnessSum += e.getFitness();

            averageEntityFitness = entity_fitnessSum / entities.Count;
            averageEnemyFitness = enemy_fitnessSum / enemies.Count;

            Debug.Log(entityGeneration + ": (" + entities.Count + " entities) Total fitness: " + entity_fitnessSum + ", average fitness: " + averageEntityFitness);

            yield return new WaitForSeconds(SimulationParameters.GENERATE_OFFSPRING_DELAY + 0.15f);
        }
    }

    public GameObject getClosestEntity(GameObject enemy)
    {
        float minDistance = float.MaxValue;
        Entity ret = null;

        foreach (Entity e in entities)
        {
            float d = Vector3.Distance(e.gameObject.transform.position, enemy.gameObject.transform.position);

            if (d < minDistance)
            {
                ret = e;
                minDistance = d;
            }
        }

        return (ret != null) ? ret.gameObject : null;
    }

    public GameObject getClosestEnemy(GameObject entity)
    {
        float minDistance = float.MaxValue;
        Enemy ret = null;

        foreach (Enemy e in enemies)
        {
            float d = Vector3.Distance(e.gameObject.transform.position, entity.gameObject.transform.position);

            if(d < minDistance)
            {
                ret = e;
                minDistance = d;
            }
        }

        return (ret != null) ? ret.gameObject : null;
    }

    private IEnumerator advanceTime()
    {
        while(true)
        {
            try
            {
                foreach (Entity e in entities)
                {
                    e.increaseAge();
                }

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
                Vector3 position = new Vector3(UnityEngine.Random.Range(leftMarker, rightMarker), 0, UnityEngine.Random.Range(bottomMarker, topMarker));
                GameObject g = Instantiate(foodPrefab, position, Quaternion.identity);
                Food f = g.GetComponent<Food>();

                f.nutrition = SimulationParameters.FOOD_NUTRITION;
                food.Add(f);
            }

            yield return new WaitForSeconds(SimulationParameters.GENERATE_FOOD_DELAY);
        }
    }

    private IEnumerator mateEntities()
    {
        while (true)
        {
            List<Entity> pool = new List<Entity>();

            if (entities.Count > SimulationParameters.MAX_ENTITIES)
                continue;

            if(entityGeneration > 0)
                entityGeneration++;

            foreach (Entity e in entities)
            {
                if (e.getFitness() >= averageEntityFitness)
                    pool.Add(e);
            }

            pool.Sort((x, y) => y.getFitness().CompareTo(x.getFitness()));

            for (int i = 0; i < pool.Count; i += 2)
            {
                if (entities.Count >= SimulationParameters.MAX_ENTITIES)
                    break;

                if (i + 1 < pool.Count)
                {
                    Vector3 position = new Vector3(UnityEngine.Random.Range(leftMarker, rightMarker), 0, UnityEngine.Random.Range(bottomMarker, topMarker));
                    GameObject child = Instantiate(entityPrefab, position, Quaternion.identity);
                    Entity e = child.GetComponent<Entity>();

                    // e.father = pool[i].getGenome();
                    // e.mother = pool[i + 1].getGenome();
                    e.father = null;
                    e.mother = null;

                    entities.Add(e);
                }
            }

            saveGenerationValues();

            yield return new WaitForSeconds(SimulationParameters.GENERATE_OFFSPRING_DELAY);
        }
    }

    private IEnumerator mateEnemies()
    {
        while (true)
        {
            List<Enemy> pool = new List<Enemy>();

            if (enemies.Count > SimulationParameters.MAX_ENEMIES)
                continue;

            if(enemyGeneration > 0)
                enemyGeneration++;

            foreach (Enemy e in enemies)
            {
                if (e.getFitness() >= averageEnemyFitness)
                    pool.Add(e);
            }

            pool.Sort((x, y) => y.getFitness().CompareTo(x.getFitness()));

            for (int i = 0; i < pool.Count; i += 2)
            {
                if (enemies.Count >= SimulationParameters.MAX_ENEMIES)
                    break;

                if (i + 1 < pool.Count)
                {
                    Vector3 position = new Vector3(UnityEngine.Random.Range(leftMarker, rightMarker), 0, UnityEngine.Random.Range(bottomMarker, topMarker));
                    GameObject child = Instantiate(enemyPrefab, position, Quaternion.identity);
                    Enemy e = child.GetComponent<Enemy>();

                    // e.father = pool[i].getGenome();
                    // e.mother = pool[i + 1].getGenome();
                    e.father = null;
                    e.mother = null;

                    enemies.Add(e);
                }
            }

            saveGenerationValues();

            yield return new WaitForSeconds(SimulationParameters.GENERATE_ENEMIES_DELAY);
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

    public void remove(Entity e)
    {
        entities.Remove(e);

        Destroy(e.gameObject);
    }

    public void remove(Food f)
    {
        food.Remove(f);

        Destroy(f.gameObject);
    }

    public void remove(Enemy enemy)
    {
        enemies.Remove(enemy);

        Destroy(enemy.gameObject);
    }

    void saveGenerationValues()
    {
        if (entityGeneration > 0 && entityGeneration < 80)
        {
            float sumEntitySpeed = 0;
            float sumEntityHunger = 0;
            float sumEntityHungerIncreaseOverTime = 0;
            float sumEntityDamageWhenHungry = 0;
            float sumEntityReactionTime = 0;
            float sumEntityFear = 0;
            float sumEntityBraveness = 0;
            float sumEntityPerception = 0;
            float sumEntityLifeExpectancy = 0;

            float sumEnemySpeed = 0;
            float sumEnemyHungerIncreaseOverTime = 0;
            float sumEnemyDamageWhenHungry = 0;
            float sumEnemyReactionTime = 0;
            float sumEnemyPerception = 0;
            float sumEnemyLifeExpectancy = 0;

            foreach (Entity e in entities)
            {
                if (e.getGenome() == null)
                    continue;

                sumEntitySpeed += e.getGenome().speed;
                sumEntityHunger += e.getGenome().hunger;
                sumEntityHungerIncreaseOverTime += e.getGenome().hungerIncreaseOverTime;
                sumEntityDamageWhenHungry += e.getGenome().damageWhenHungry;
                sumEntityReactionTime += e.getGenome().reactionTime;
                sumEntityFear += e.getGenome().fear;
                sumEntityBraveness += e.getGenome().braveness;
                sumEntityPerception += e.getGenome().perception;
                sumEntityLifeExpectancy += e.getGenome().lifeExpectancy;
            }

            entity_writer.WriteLine(entityGeneration +
                    "," + entities.Count +
                    "," + averageEntityFitness +
                    "," + sumEntitySpeed / entities.Count +
                    "," + sumEntityHunger / entities.Count +
                    "," + sumEntityHungerIncreaseOverTime / entities.Count +
                    "," + sumEntityDamageWhenHungry / entities.Count +
                    "," + sumEntityReactionTime / entities.Count +
                    "," + sumEntityFear / entities.Count +
                    "," + sumEntityBraveness / entities.Count +
                    "," + sumEntityPerception / entities.Count +
                    "," + sumEntityLifeExpectancy / entities.Count);

            entity_writer.Flush();

            foreach (Enemy e in enemies)
            {
                if (e.getGenome() == null)
                    continue;

                sumEnemySpeed += e.getGenome().speed;
                sumEnemyHungerIncreaseOverTime += e.getGenome().hungerIncreaseOverTime;
                sumEnemyDamageWhenHungry += e.getGenome().damageWhenHungry;
                sumEnemyReactionTime += e.getGenome().reactionTime;
                sumEnemyPerception += e.getGenome().perception;
                sumEnemyLifeExpectancy += e.getGenome().lifeExpectancy;
            }

            enemy_writer.WriteLine(enemyGeneration +
                    "," + enemies.Count +
                    "," + averageEnemyFitness +
                    "," + sumEnemySpeed / entities.Count +
                    "," + sumEnemyHungerIncreaseOverTime / entities.Count +
                    "," + sumEnemyDamageWhenHungry / entities.Count +
                    "," + sumEnemyReactionTime / entities.Count +
                    "," + sumEnemyPerception / entities.Count +
                    "," + sumEnemyLifeExpectancy / entities.Count);

            enemy_writer.Flush();
        }
        else if(entityGeneration == 0)
        {
            // Spawn Food
            for (int i = 0; i < SimulationParameters.FOOD_START_AMOUNT; i++)
            {
                Vector3 position = new Vector3(UnityEngine.Random.Range(leftMarker, rightMarker), 0, UnityEngine.Random.Range(bottomMarker, topMarker));
                GameObject g = Instantiate(foodPrefab, position, Quaternion.identity);
                Food f = g.GetComponent<Food>();

                f.nutrition = SimulationParameters.FOOD_NUTRITION;
                food.Add(f);
            }

            // Spawn entities
            while (entities.Count < SimulationParameters.ENTITY_POPULATION_START_SIZE)
            {
                Vector3 position = new Vector3(UnityEngine.Random.Range(leftMarker, rightMarker), 0, UnityEngine.Random.Range(bottomMarker, topMarker));
                GameObject g = Instantiate(entityPrefab, position, Quaternion.identity);
                Entity e = g.GetComponent<Entity>();

                entities.Add(e);
            }

            entityGeneration = 1;

            // Spawn enemies
            while (enemies.Count < SimulationParameters.ENEMY_POPULATION_START_SIZE)
            {
                Vector3 position = new Vector3(UnityEngine.Random.Range(leftMarker, rightMarker), 0, UnityEngine.Random.Range(bottomMarker, topMarker));
                GameObject g = Instantiate(enemyPrefab, position, Quaternion.identity);
                Enemy e = g.GetComponent<Enemy>();

                enemies.Add(e);
            }

            enemyGeneration = 1;

            entity_writer = new StreamWriter(filePath + "experiment_entities_" + experiment + ".csv");
            entity_writer.WriteLine("Generation,NumCreatures,AvgFitness,AvgSpeed,AvgHunger,AvgHungerIncreaseOverTime,AvgDamageWhenHungry,AvgReactionTime,AvgFear,AvgBraveness,AvgPerception,AvgLifeExpectancy");

            enemy_writer = new StreamWriter(filePath + "experiment_enemies_" + experiment + ".csv");
            enemy_writer.WriteLine("Generation,NumCreatures,AvgFitness,AvgSpeed,AvgHungerIncreaseOverTime,AvgDamageWhenHungry,AvgReactionTime,AvgPerception,AvgLifeExpectancy");
        }
        else
        {
            if (entity_writer != null)
                entity_writer.Close();

            if (enemy_writer != null)
                enemy_writer.Close();
        }
    }

    private void OnDestroy()
    {
        if (entity_writer != null)
            entity_writer.Close();

        if (enemy_writer != null)
            enemy_writer.Close();
    }
}