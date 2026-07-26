using JetBrains.Annotations;
using System;
using UnityEngine;
using static EffectsManager;

public class EffectsManager : MonoBehaviour
{
    public EffectNode[,] effectStateTable { get; set; }
    /*
     * EffectNode is a simple class for organizing any given effect
     * We have isActive to determine if the effect is occurring, and we have turnsTilEnd to determine when it ends.
     * The constructor is empty because it gets initialized at the beginning of combat.
     * This means that no effects are currently occurring, meaning that it's not valuable to create a separate constructor.
     */
    public class EffectNode
    {
        public bool isActive { get; set; }
        public int turnsTilEnd { get; set; }

        public EffectNode()
        {
            isActive = false;
            turnsTilEnd = 0;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
     * This is the constructor for the EffectsManager. 
     * This should be done at the very, very beginning of combat.
     * It the effectStateTable is an x by y array of EffectNode objects that manage effect times.
     * The column number is the effect number we reference in datacenter.
     * The row number is the position in initi
     */
    public EffectsManager(int numberOfCombatants, int numberOfEffects)
    {
        effectStateTable = new EffectNode[numberOfCombatants, numberOfEffects];

        for (int i = 0; i < numberOfCombatants; i++)
        {
            for (int j = 0; j < numberOfEffects; j++)
            {
                effectStateTable[i, j] = new EffectNode();
            }
        }
    }

    public void SetEffectState(int initiativeIndex, int effectCode, int turnsTilEnd)
    {
        this.effectStateTable[initiativeIndex, effectCode].isActive = true;
        this.effectStateTable[initiativeIndex, effectCode].turnsTilEnd = turnsTilEnd;
    }

    /*
     * This function is designed to, upon the start of a turn, decrement all effect times for the turn haver by 1.
     * This ensures that we can manage effect times with one little function!
     * It's important to recall that this is a per turn event, not a per round event.
     * 
     * We take only initiativeIndex from initiativeOrder in CombatCenter because it tells us whose turn it is.
     * We're decreasing all effect times by 1, so no need to specify which one with a variable.
     */
    public void DecrementEffectTimers(int initiativeIndex)
    {
        for (int i = 0; i < effectStateTable.GetLength(0); i++)
        {
            if (effectStateTable[initiativeIndex, i].isActive == true)
            {
                effectStateTable[initiativeIndex, i].turnsTilEnd -= 1;
                
                if (effectStateTable[initiativeIndex, i].turnsTilEnd < 0)
                {
                    effectStateTable[initiativeIndex, i].isActive = false;
                } 
            }
        }
    }
}
