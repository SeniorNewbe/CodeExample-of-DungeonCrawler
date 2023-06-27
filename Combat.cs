using SkeletonEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour
{
    List<int> enemies = new List<int>();

    [SerializeField] UIController uIController;
    GameObject target;
    int index;

    //Check for opponent in front of character
    public int OpponentRegister(int opp)
    {
        enemies.Add(opp);
        index = enemies.Count;
        return index;
    }

    public void StartCombat(GameObject opp)
    {
        target = opp;
        opp.GetComponent<OpponentController>();
        uIController.ToggleCombatScreen(true);
    }

    public void EndCombat()
    {
        uIController.ToggleCombatScreen(false);
        uIController.ReviveWindow(false, 1);
    }

    public void PlayerAttack(int damage)
    {
        var temp = target.GetComponent<OpponentController>();
        temp.TakeDamage(damage);
    }

    public void Revive()
    {
        target.GetComponent<OpponentController>().ReviveMinion();
    }

    public void DestroyTarget()
    {
        target.GetComponent<OpponentController>().DestroyMe();
    }
}
