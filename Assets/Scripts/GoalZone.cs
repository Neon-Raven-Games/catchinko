using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GoalZone : MonoBehaviour
{
    public int scoreValue = 1;
    [SerializeField] private UnityEvent onScore;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            // Destroy(other.gameObject);
            // GameResultsManager.GameOver(true);
            onScore.Invoke();
        }
    }
}
