using ParkourRunner.Scripts.Managers;
using AEngine;

namespace ParkourRunner.Scripts.Track.Pick_Ups.Bonuses
{
    public class BonusPickUp : PickUp
    {
        public BonusName BonusName;

        private AudioManager _audio;

        protected override void Pick()
        {
            GameManager.Instance.AddBonus(BonusName);
            PoolManager.Instance.Remove(gameObject);

            _audio = AudioManager.Instance;
            
            switch (this.BonusName)
            {
                case BonusName.DoubleCoins:
                    _audio.PlaySound(Sounds.BonusX2);
                    break;

                case BonusName.Shield:
                    _audio.PlaySound(Sounds.Bonus);
                    _audio.PlaySound(Sounds.BonusShield);
                    break;

                default:
                    _audio.PlaySound(Sounds.Bonus);
                    break;
            }
        }
    }
}
