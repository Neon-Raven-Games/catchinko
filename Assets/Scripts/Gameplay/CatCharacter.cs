using Minimalist.Quantity;
using UnityEngine;

namespace Gameplay
{
    public class CatCharacter : MonoBehaviour
    {
        [SerializeField] private int attackDamage = 10;
        [SerializeField] private int health = 100;
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private QuantityBhv healthBar;
        private Animator _animator;
        
        [SerializeField] private CatCharacter enemy;
        public bool IsAlive => health > 0;
        
        private int _combo = 1;
        private static readonly int _SHurt = Animator.StringToHash("Hurt");
        private static readonly int _SAttack = Animator.StringToHash("Attack");
        
        private void Start()
        {
            _animator = GetComponent<Animator>();
            healthBar.MaximumAmount = maxHealth;
            healthBar.Amount = health;
        }
        
        // recieves the damage and plays hurt
        private void ReceiveHit(int combo, int damage)
        {
            healthBar.Amount -= damage * combo;
            
            _animator.SetTrigger(_SHurt);
            health = (int) healthBar.Amount;
            
            if (health <= 0) CombatController.EndGame();
        }

        // called by the goal zones or combat controller for enemy
        public void Attack()
        {
            _animator.SetTrigger(_SAttack);
        }
        
        // called from animation event, this plays damage animation and health bar shit
        public void DealHit()
        {
           enemy.ReceiveHit(_combo, attackDamage);
           _combo = 1;
        }

        public void IncreaseCombo()
        {
            _combo++;
            _combo = Mathf.Clamp(_combo, 1, 4);
        }
        
    }
}