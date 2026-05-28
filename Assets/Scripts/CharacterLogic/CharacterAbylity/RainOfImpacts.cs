using UnityEngine;

namespace CharacterLogic.CharacterAbility
{
    public class RainOfImpacts : MonoBehaviour
    {
        [SerializeField] private float _areaOfImpact;

        private void ActivateAbility()
        {
            var colldiers = Physics2D.OverlapCircle(transform.position, _areaOfImpact);
            
            
        }
    }
}
