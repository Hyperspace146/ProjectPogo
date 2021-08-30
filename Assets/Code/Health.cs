using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public int MaxHealth = 100;
    public int CurrentHealth = 100;
    public UnityEvent<int> OnHealthChange;
    public UnityEvent OnDeath;
    
    public void ChangeHealth(int change)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + change, MaxHealth);

        if (CurrentHealth <= 0)
        {
            print("dead");
            OnDeath.Invoke();
        }

        OnHealthChange.Invoke(change);
    }

    public void DestroyObject()
    {
        Destroy(this.gameObject);
    }
}
