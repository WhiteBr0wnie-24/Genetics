using UnityEngine;

public class EnemyGenome
{
    public float health = 100.0f;
    public float speed = 2.0f;
    public float hunger = 1.0f;
    public float hungerIncreaseOverTime = 1.0f;
    public float damageWhenHungry = 0.5f;

    public float reactionTime = 1.0f;
    public float fear = 10.0f; // How close the player has to come for the enemy to flee
    public float braveness = 75f; // If the health is above this value the enemy will try to attack the player
    public float damage = 2.0f; // How much damage the enemy takes when the player attacks him
    public float attackDamage = 2.0f; // How much damage the enemy deals when he attacks the player

    public float perception = 10.0f;
    public int lifeExpectancy = 5;

    public EnemyGenome()
    {
        health = SimulationParameters.random(SimulationParameters.MIN_HEALTH, SimulationParameters.MAX_HEALTH, 1f);
        speed = SimulationParameters.random(SimulationParameters.MIN_SPEED, SimulationParameters.MAX_SPEED, 3.0f);
        hunger = SimulationParameters.random(SimulationParameters.MIN_HUNGER, SimulationParameters.MAX_HUNGER, 0.5f);
        hungerIncreaseOverTime = SimulationParameters.random(SimulationParameters.MIN_HUNGER_INCREASE_OVER_TIME, SimulationParameters.MAX_HUNGER_INCREASE_OVER_TIME, 1f);
        damageWhenHungry = SimulationParameters.random(SimulationParameters.MIN_DAMAGE_WHEN_HUNGRY, SimulationParameters.MAX_DAMAGE_WHEN_HUNGRY, 1f);
        reactionTime = SimulationParameters.random(SimulationParameters.MIN_REACTION_TIME, SimulationParameters.MAX_REACTION_TIME, 1f);
        fear = SimulationParameters.random(SimulationParameters.MIN_FEAR, SimulationParameters.MAX_FEAR, 2.0f);
        braveness = SimulationParameters.random(SimulationParameters.MIN_BRAVENESS, SimulationParameters.MAX_BRAVENESS, 2.0f);
        damage = SimulationParameters.random(SimulationParameters.MIN_DAMAGE, SimulationParameters.MAX_DAMAGE, 0.5f);
        attackDamage = SimulationParameters.random(SimulationParameters.MIN_ATTACK_DAMAGE, SimulationParameters.MAX_ATTACK_DAMAGE, 3.0f);
        perception = SimulationParameters.random(SimulationParameters.MIN_PERCEPTION, SimulationParameters.MAX_PERCEPTION, 1f);
        lifeExpectancy = Random.Range(SimulationParameters.MIN_LIFE_EXPECTANCY, SimulationParameters.MAX_LIFE_EXPECTANCY);
    }

    public EnemyGenome(EnemyGenome father, EnemyGenome mother)
    {
        health = (Random.Range(0f, 1f) <= SimulationParameters.FATHER_GENE_PROBABILITY) ? father.health : mother.health;
        speed = (Random.Range(0f, 1f) <= SimulationParameters.FATHER_GENE_PROBABILITY) ? father.speed : mother.speed;
        hunger = (Random.Range(0f, 1f) <= SimulationParameters.FATHER_GENE_PROBABILITY) ? father.hunger : mother.hunger;
        hungerIncreaseOverTime = (Random.Range(0f, 1f) <= SimulationParameters.FATHER_GENE_PROBABILITY) ? father.hungerIncreaseOverTime : mother.hungerIncreaseOverTime;
        damageWhenHungry = (Random.Range(0f, 1f) <= SimulationParameters.FATHER_GENE_PROBABILITY) ? father.damageWhenHungry : mother.damageWhenHungry;
        reactionTime = (Random.Range(0f, 1f) <= SimulationParameters.FATHER_GENE_PROBABILITY) ? father.reactionTime : mother.reactionTime;
        fear = (Random.Range(0f, 1f) <= SimulationParameters.FATHER_GENE_PROBABILITY) ? father.fear : mother.fear;
        braveness = (Random.Range(0f, 1f) <= SimulationParameters.FATHER_GENE_PROBABILITY) ? father.braveness : mother.braveness;
        damage = (Random.Range(0f, 1f) <= SimulationParameters.FATHER_GENE_PROBABILITY) ? father.damage : mother.damage;
        attackDamage = (Random.Range(0f, 1f) <= SimulationParameters.FATHER_GENE_PROBABILITY) ? father.attackDamage : mother.attackDamage;
        perception = (Random.Range(0f, 1f) <= SimulationParameters.FATHER_GENE_PROBABILITY) ? father.perception : mother.perception;
        lifeExpectancy = (Random.Range(0f, 1f) <= SimulationParameters.FATHER_GENE_PROBABILITY) ? father.lifeExpectancy : mother.lifeExpectancy;

        #region Random Mutation of a random genome

        if (Random.Range(0f, 1f) <= SimulationParameters.MUTATION_PROBABILITY)
        {
            int i = Random.Range(0, 11);

            switch (i)
            {
                case 0:
                    health = SimulationParameters.random(SimulationParameters.MIN_HEALTH, SimulationParameters.MAX_HEALTH, 1f);
                    break;

                case 1:
                    speed = SimulationParameters.random(SimulationParameters.MIN_SPEED, SimulationParameters.MAX_SPEED, 3.0f);
                    break;

                case 2:
                    hunger = SimulationParameters.random(SimulationParameters.MIN_HUNGER, SimulationParameters.MAX_HUNGER, 0.5f);
                    break;

                case 3:
                    hungerIncreaseOverTime = SimulationParameters.random(SimulationParameters.MIN_HUNGER_INCREASE_OVER_TIME, SimulationParameters.MAX_HUNGER_INCREASE_OVER_TIME, 1f);
                    break;

                case 4:
                    damageWhenHungry = SimulationParameters.random(SimulationParameters.MIN_DAMAGE_WHEN_HUNGRY, SimulationParameters.MAX_DAMAGE_WHEN_HUNGRY, 1f);
                    break;

                case 5:
                    reactionTime = SimulationParameters.random(SimulationParameters.MIN_REACTION_TIME, SimulationParameters.MAX_REACTION_TIME, 1f);
                    break;

                case 6:
                    fear = SimulationParameters.random(SimulationParameters.MIN_FEAR, SimulationParameters.MAX_FEAR, 2.0f);
                    break;

                case 7:
                    braveness = SimulationParameters.random(SimulationParameters.MIN_BRAVENESS, SimulationParameters.MAX_BRAVENESS, 2.0f);
                    break;

                case 8:
                    damage = SimulationParameters.random(SimulationParameters.MIN_DAMAGE, SimulationParameters.MAX_DAMAGE, 0.5f);
                    break;

                case 9:
                    attackDamage = SimulationParameters.random(SimulationParameters.MIN_ATTACK_DAMAGE, SimulationParameters.MAX_ATTACK_DAMAGE, 3.0f);
                    break;

                case 10:
                    perception = SimulationParameters.random(SimulationParameters.MIN_PERCEPTION, SimulationParameters.MAX_PERCEPTION, 1f);
                    break;

                case 11:
                    lifeExpectancy = Random.Range(SimulationParameters.MIN_LIFE_EXPECTANCY, SimulationParameters.MAX_LIFE_EXPECTANCY);
                    break;

                default:
                    break;
            }
        }

        #endregion
    }
}