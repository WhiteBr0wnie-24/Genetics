using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public Material normalMaterial, attackMaterial, huntingMaterial;
    public EnemyGenome father, mother;

    private PlayfieldController pfc;
    private Renderer r;

    private bool alive = true;
    private EnemyState state = EnemyState.Waiting;
    private UnityEngine.AI.NavMeshAgent nav;

    private int age;
    private EnemyGenome genome;
    private float consumedCalories = 0f;

    // Start is called before the first frame update
    void Start()
    {
        if (mother == null || father == null)
            genome = new EnemyGenome();
        else
            genome = new EnemyGenome(father, mother);

        pfc = GameObject.FindWithTag("GameController").GetComponent<PlayfieldController>();
        r = GetComponent<Renderer>();
        nav = GetComponent<UnityEngine.AI.NavMeshAgent>();

        nav.speed = genome.speed;

        StartCoroutine("makeUpMind");
    }

    private IEnumerator makeUpMind()
    {
        while (alive)
        {
            GameObject closestEnemy = pfc.getClosestEnemy(gameObject);

            float currentFear = genome.fear;
            float distanceToNextEnemy = (closestEnemy == null) ? float.MaxValue : Vector3.Distance(gameObject.transform.position, closestEnemy.transform.position);

            // Increase the hunger over time
            genome.hunger = genome.hunger + genome.hungerIncreaseOverTime;

            // If the hunger is large enough, the enemy takes damage
            if (genome.hunger > SimulationParameters.HUNGER_DAMAGE_THRESHOLD)
            {
                genome.health = genome.health - genome.damageWhenHungry;
                currentFear = genome.fear * SimulationParameters.FEAR_PERCENTAGE_WHEN_HUNGRY;
            }

            // The enemies will do the following actions with descending importance:
            // 1. Flee when the fear is too large
            // 2. Search for food
            // 3. Attack the player
            // 4. Wait

            // TODO: Calculate chance for successful attack and then decide whether to flee or attack

            if (genome.health <= genome.braveness && distanceToNextEnemy < currentFear && distanceToNextEnemy <= genome.perception)
                state = EnemyState.Fleeing;
            else if (genome.hunger > genome.fear)
                state = EnemyState.Hunting;
            else if (genome.health > genome.braveness && distanceToNextEnemy <= genome.perception)
                state = EnemyState.Attacking;
            else
            {
                /*
                if (state == EnemyState.Fleeing)
                    timesEscaped += 1;
                */

                state = EnemyState.Waiting;
            }

            // Visual indicator that the enemy is trying to attack the player
            r.material = (SimulationParameters.SHOW_ATTACKING_ENEMIES && state == EnemyState.Attacking) ? attackMaterial : (state == EnemyState.Hunting ? huntingMaterial : normalMaterial);

            yield return new WaitForSeconds(genome.reactionTime);
        }
    }

    void Update()
    {
        if (genome.health <= 0f)
            die();

        if (!alive)
            return;

        if (state == EnemyState.Fleeing)
        {
            GameObject closestEnemy = pfc.getClosestEnemy(gameObject);

            Vector3 directionToPlayer = closestEnemy.gameObject.transform.position - transform.position;
            Vector3 fleePosition = transform.position - directionToPlayer;

            nav.isStopped = false;
            nav.SetDestination(fleePosition);
        }
        else if(state == EnemyState.Hunting)
        {
            Vector3 nearestFood = pfc.findNearestFood(transform.position, genome.perception);

            nav.isStopped = (nearestFood == null);
            nav.SetDestination(nearestFood);
        }
        else
        {
            nav.isStopped = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (alive && genome != null)
        {
            if (other.gameObject.CompareTag("Food"))
            {
                Food f = other.gameObject.GetComponent<Food>();
                float calories = f.consume();

                genome.hunger = Mathf.Clamp(genome.hunger - calories, 0f, SimulationParameters.ABSOLUTE_MAX_HUNGER);
                genome.health = Mathf.Clamp(genome.health + calories, genome.health, SimulationParameters.ABSOLUTE_MAX_HEALTH);
                consumedCalories += calories;
            }

            if (other.gameObject.CompareTag("Enemy"))
            {
                die();
            }
        }
    }

    private void die()
    {
        if (alive)
        {
            nav.isStopped = true;

            alive = false;
            pfc.remove(this);
        }
    }

    public float getFitness()
    {
        /*
        if (genome != null)
        {
            return genome.health - genome.hunger;
        }

        return float.NegativeInfinity;
        */
        return consumedCalories;
    }

    public void increaseAge()
    {
        if (alive)
        {
            age += 1;

            if (age > genome.lifeExpectancy)
            {
                float r = Random.Range(0f, 1f);

                // The individual will die with increasing probability every time he ages
                if (r >= 0.25f * (age - genome.lifeExpectancy))
                    die();
            }
        }
    }

    public EnemyGenome getGenome()
    {
        return genome;
    }
}