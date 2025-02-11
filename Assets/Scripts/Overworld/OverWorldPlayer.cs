using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverWorldPlayer : MonoBehaviour
{
    [SerializeField] private WorldSpacePanelController worldSpacePanelController;
    
    [SerializeField] private int currentHealth;
    [SerializeField] private int maxHealth;
    
    [SerializeField] private int catCash;
    public void ShowPanel() => worldSpacePanelController.UpdateEnemyName();
    public void HidePanel() => worldSpacePanelController.MinimizePanel();
}
