using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public int MaxHealth = 100;
    public int CurrentHealth = 100;
    public UnityEvent<int, Vector3> OnTakeDamage;
    public UnityEvent<int> OnHeal;
    public UnityEvent OnDeath;
    
    public void Heal(int healAmount)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + healAmount, MaxHealth);
        
        if (OnHeal != null)
        {
            OnHeal.Invoke(healAmount);
        }
    }

    public void TakeDamage(int damageAmount, Vector3 contactPoint)
    {
        if (damageAmount < 0)
        {
            print("Cannot deal negative damage.");
            return;
        }

        if (OnTakeDamage != null)
        {
            OnTakeDamage.Invoke(damageAmount, contactPoint);
        }

        if (CurrentHealth <= 0 && OnDeath != null)
        {
            OnDeath.Invoke();
        }
    }

    public void DestroyObject()
    {
        Destroy(this.gameObject);
    }
}
