using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : SingletonBehaviour<UIManager>
{
    [SerializeField] private Image playerHealthBar;
    [SerializeField] private Image aiHealthBar;

    public ForceShield enemy, player;

    void Update()
    {
        LowerPlayerBaseHealth();
        LowerAIHealthBaseHealth();
    }
    private void LowerPlayerBaseHealth()
    {
        aiHealthBar.fillAmount = player.Health / 300;
    }
    private void LowerAIHealthBaseHealth()
    {
        playerHealthBar.fillAmount = enemy.Health / 300;
    }
}
