using ParkourRunner.Scripts.Managers;
using AEngine;

namespace ParkourRunner.Scripts.Track.Pick_Ups
{
    public class HealLimb : PickUp {

        protected override void Pick()
        {
            AudioManager.Instance.PlaySound(Gender.Kind == Gender.GenderKinds.Male ? Sounds.BonusMed : Sounds.BonusMedFem);

            if (GameManager.Instance.HealLimb())
            {
                Destroy(gameObject);
            }
        }
    }
}