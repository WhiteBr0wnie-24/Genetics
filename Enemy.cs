using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Material normalMaterial, attackMaterial, huntingMaterial;
    public EnemyGenome father, mother;

    private PlayfieldController pfc;

    private bool alive = true;
    private EnemyState state = EnemyState.Waiting;
    private UnityEngine.AI.NavMeshAgent nav;

    private int killedEntities = 0;
    private int age;

    private EnemyGenome genome;
    private GameObject target = null;

    // Start is called before the first frame update
    void Start()
    {
        if (mother == null || father == null)
            genome = new EnemyGenome();
        else
            genome = new EnemyGenome(father, mother);

        pfc = GameObject.FindWithTag("GameController").GetComponent<PlayfieldController>();
        nav = GetComponent<UnityEngine.AI.NavMeshAgent>();

        nav.speed = genome.speed;

        StartCoroutine("makeUpMind");
    }

    private IEnumerator makeUpMind()
    {
        while (alive)
        {
            GameObject closestEntity = pfc.getClosestEntity(gameObject);

            float distanceToNextEntity = (closestEntity != null) ? Vector3.Distance(gameObject.transform.position, closestEntity.transform.position) : float.MaxValue;

            // Increase the hunger over time
            genome.hunger = genome.hunger + genome.hungerIncreaseOverTime;

            // If the hunger is large enough, the enemy takes damage
            if (genome.hunger > SimulationParameters.HUNGER_DAMAGE_THRESHOLD)
                genome.health = genome.health - genome.damageWhenHungry;

            else if (distanceToNextEntity <= genome.perception)
            {
                if (target == null)
                    target = closestEntity;

                state = EnemyState.Attacking;
            }
            else
                state = EnemyState.Waiting;

            yield return new WaitForSeconds(genome.reactionTime);
        }
    }

    void Update()
    {
        if (genome.health <= 0f)
            die();

        if (!alive)
            return;

        if (state == EnemyState.Attacking && target != null)
        {
            nav.isStopped = false;
            nav.SetDestination(target.transform.position);
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
            if (other.gameObject.CompareTag("Entity"))
            {
                if (state == EnemyState.Attacking)
                {
                    killedEntities++;
                    genome.health = SimulationParameters.ABSOLUTE_MAX_HEALTH;
                    genome.hunger = 0;
                    target = null;
                }
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

    public void entityDied(GameObject e)
    {
        if (target.Equals(e))
            target = null;
    }

    public float getFitness()
    {
        return killedEntities;
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