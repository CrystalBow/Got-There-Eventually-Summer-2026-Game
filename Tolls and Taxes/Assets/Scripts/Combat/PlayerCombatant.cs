using UnityEngine;

public class PlayerCombatant // : MonoBehaviour
{
    public string allyName { get; init; }
    public int maxHP { get; init; }
    public int currentHP { get; set; }
    public int maxMP { get; init; }
    public int currentMP { get; set; }
    public int attack { get; init; }
    public int defense { get; init; }
    public int speed { get; init; }
    public Deck thisDeck { get; init; }

    public PlayerCombatant(string name, int maxHP, int currentHP, int maxMP, int currentMP, int attack, int defense, int speed, Deck thisDeck)
    {
        this.allyName = name;
        this.maxHP = maxHP;
        this.currentHP = currentHP;
        this.maxMP = maxMP;
        this.currentMP = currentMP;
        this.attack = attack;
        this.defense = defense;
        this.speed = speed;
        this.thisDeck = thisDeck;
    }

    // if defense nullifies damage, nothing happens.
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
