using UnityEngine;

public abstract class SimulationParameters
{
    // Generic Simulation parameters

    public static float HUNGER_DAMAGE_THRESHOLD = 70f;
	public static float FOOD_DISTANCE_RETURN_THRESHOLD = 10f;
    public static float FEAR_PERCENTAGE_WHEN_HUNGRY = 0.5f;
    public static float GENERATE_OFFSPRING_DELAY = 3;
    public static float GENERATE_FOOD_DELAY = 0.001f;
    public static float BULLET_SPEED = 30f;
    public static float BULLET_DAMAGE_MULTIPLIER = 100f;
    public static float BULLET_FIRE_DELAY = 0.1f;
    public static float FATHER_GENE_PROBABILITY = 0.90f;
    public static float MUTATION_PROBABILITY = 0.25f;
    public static float FOOD_SPOIL_TIME = 10f;

    public static float ABSOLUTE_MAX_HEALTH = 100f; // The max. health an enemy can have at any time (not just at the spawn)
    public static float ABSOLUTE_MAX_HUNGER = 100f; // The max. hunger an enemy can have at any time (not just at the spawn)

    public static int MAX_FOOD = 1000;
    public static int MAX_ENEMIES = 100;
    public static int MAX_ENTITIES = 800;
    public static int ENTITY_POPULATION_START_SIZE = 500;
    public static int FOOD_START_AMOUNT = 750;
    public static int FOOD_NUTRITION = 100;

    public static int MAX_LIFE_EXPECTANCY = 12;
    public static int YEAR_LENGTH = 1; // In seconds

    public static bool SHOW_ATTACKING_ENEMIES = true;

    // Minimum values for random populations and offsprings

    public static float MIN_HEALTH = 50f;
    public static float MIN_SPEED = 1f;
    public static float MIN_HUNGER = 0f;
    public static float MIN_HUNGER_INCREASE_OVER_TIME = 1.0f;
    public static float MIN_DAMAGE_WHEN_HUNGRY = 0.5f;
    public static float MIN_REACTION_TIME = 0.1f;
    public static float MIN_FEAR = 5f;
    public static float MIN_BRAVENESS = 65f;
    public static float MIN_DAMAGE = 10f;
    public static float MIN_ATTACK_DAMAGE = 25f;
    public static float MIN_PERCEPTION = 5f;
    public static int MIN_LIFE_EXPECTANCY = 3;

    // Maximum values for random populations and offsprings

    public static float MAX_HEALTH = 100f;
    public static float MAX_SPEED = 15.0f;
    public static float MAX_HUNGER = 100f;
    public static float MAX_HUNGER_INCREASE_OVER_TIME = 10.0f;
    public static float MAX_DAMAGE_WHEN_HUNGRY = 5f;
    public static float MAX_REACTION_TIME = 5f;
    public static float MAX_FEAR = 25f;
    public static float MAX_BRAVENESS = 100f;
    public static float MAX_DAMAGE = 100f;
    public static float MAX_ATTACK_DAMAGE = 50f;
    public static float MAX_PERCEPTION = 50f;

    public static float GENERATE_ENEMIES_DELAY = 5f;
    public static int ENEMY_POPULATION_START_SIZE = 15;

    /**
     * Returns a random value within a range and with a specified bias towards one
     * of the values (min or max). Lower biases (less than 1.0) will produce values
     * more towards the upper edge, higher biases (more than 1.0) will produce
     * values more towards the minimum.
     **/
    public static float random(float min, float max, float bias)
    {
        float r = Random.Range(0f,1f);
        r = Mathf.Pow(r, bias);

        return min + (max - min) * r;
    }
}