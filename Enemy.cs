using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Material normalMaterial, attackMaterial, huntingMaterial;
    public EnemyGenome father, mother;

    private Player player;
    private PlayfieldController pfc;
    private Renderer r;

    private bool alive = true;
    private EnemyState state = EnemyState.Waiting;
    private UnityEngine.AI.NavMeshAgent nav;

    private float damageTakenFromPlayer = 0f;
    private float damageDealtToPlayer = 0f;
    private float timesEscaped = 0f;

    private int age;
    private EnemyGenome genome;

    // Start is called before the first frame update
    void Start()
    {
        if (mother == null || father == null)
            genome = new EnemyGenome();
        else
            genome = new EnemyGenome(father, mother);

        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        pfc = GameObject.FindWithTag("GameController").GetComponent<PlayfieldController>();
        r = GetComponent<Renderer>();
        nav = GetComponent<UnityEngine.AI.NavMeshAgent>();

        nav.speed = genome.speed;

        StartCoroutine("makeUpMind");
    }

    private IEnumerator makeUpMind()
    {
        while (alive && player)
        {
            float distanceToPlayer = Vector3.Distance(player.gameObject.transform.position, transform.position);
            float currentFear = genome.fear;

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

            if (genome.health <= genome.braveness && distanceToPlayer < currentFear && distanceToPlayer <= genome.perception)
                state = EnemyState.Fleeing;
            else if (genome.hunger > genome.fear)
                state = EnemyState.Hunting;
            else if (genome.health > genome.braveness && distanceToPlayer <= genome.perception)
                state = EnemyState.Attacking;
            else
            {
                if (state == EnemyState.Fleeing)
                    timesEscaped += 1;

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

        if (!alive || !player)
            return;

        if (state == EnemyState.Fleeing)
        {
            Vector3 directionToPlayer = player.gameObject.transform.position - transform.position;
            Vector3 fleePosition = transform.position - directionToPlayer;

            nav.isStopped = false;
            nav.SetDestination(fleePosition);
        }
        else if(state == EnemyState.Attacking)
        {
            nav.isStopped = false;
            nav.SetDestination(player.gameObject.transform.position);
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
            if (other.gameObject.CompareTag("Player"))
            {
                if (state != EnemyState.Attacking)
                {
                    genome.health -= genome.damage;
                    damageTakenFromPlayer += genome.damage;
                }
                else
                {
                    player.damage(genome.attackDamage);
                    damageDealtToPlayer += genome.attackDamage;
                }
            }

            if (other.gameObject.CompareTag("Food"))
            {
                Food f = other.gameObject.GetComponent<Food>();

                genome.hunger = Mathf.Clamp(genome.hunger - f.consume(), 0f, SimulationParameters.ABSOLUTE_MAX_HUNGER);
                genome.health += Mathf.Clamp(f.consume(), genome.health, SimulationParameters.ABSOLUTE_MAX_HEALTH);
            }

            if (other.gameObject.CompareTag("Bullet"))
            {
                genome.health -= genome.damage * SimulationParameters.BULLET_DAMAGE_MULTIPLIER;
                damageTakenFromPlayer += genome.damage * SimulationParameters.BULLET_DAMAGE_MULTIPLIER;
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
        if (genome != null)
        {
            float b = 1; // genome.health / ((genome.hunger < 1) ? 1f : genome.hunger);
            float s = damageDealtToPlayer - damageTakenFromPlayer;
            float r = b * (s > 0 ? s : 1) + timesEscaped;

            return r * age;
        }

        return 0;
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