using System.Collections;
using System.Collections.Generic;
using Gameplay;
using Minimalist.Quantity;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    private static CombatController _instance;

    [Header("Basic Enemy Attack")] [SerializeField]
    private Vector2 enemyAttackSpeedRange = new Vector2(1.5f, 3);

    [SerializeField] private CatCharacter enemyCharacter;
    private bool _playing;

    private IEnumerator EnemyAttackRoutine()
    {
        while (_playing)
        {
            var time = Random.Range(enemyAttackSpeedRange.x, enemyAttackSpeedRange.y);
            yield return new WaitForSeconds(time);
            enemyCharacter.Attack();
        }
    }

    private void Start()
    {
        if (!_instance) _instance = this;
        else Destroy(gameObject);

        _playing = true;
        StartCoroutine(EnemyAttackRoutine());
    }

    public static void EndGame()
    {
        _instance._playing = false;
        _instance.StopAllCoroutines();
        GameResultsManager.GameOver(_instance.enemyCharacter.IsAlive);
    }
}