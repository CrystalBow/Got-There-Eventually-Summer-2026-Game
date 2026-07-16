using UnityEngine;

public class EnemyCombatant // : MonoBehaviour
{
    public string enemyName { get; init; }
    public int maxHP { get; init; }
    public int currentHP { get; set; }
    public int attack { get; init; }
    public int defense { get; init; }
    public int speed { get; init; }

    public EnemyCombatant(string name, int hp, int attack, int defense, int speed)
    {
        this.enemyName = name;
        this.maxHP = hp;
        this.currentHP = hp;
        this.attack = attack;
        this.defense = defense;
        this.speed = speed;
    }

    public void damage(int damageNumber)
    {
        if (defense >= damageNumber)
        {
            return;
        }

        currentHP -= (damageNumber - defense);
    }

    public bool isDead()
    {
        if (currentHP <= 0)
        {
            return true;
        }
        return false;
    }
}
