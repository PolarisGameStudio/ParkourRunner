using System;
using ParkourRunner.Scripts.Managers;
using AEngine;

namespace ParkourRunner.Scripts.Player
{
    public static class RandomTricks
    {
        public static event Action OnJumpOverObstacle;

        public static string GetRoll()
        {
            var trick = ProgressManager.Instance.GetRandomRoll();
            return trick.AnimationName;
        }

        public static string GetTrick(string playAnimation)
        {
            HUDManager hm = HUDManager.Instance;
            GameManager gm = GameManager.Instance;

            Trick trick;
            switch (playAnimation)
            {
                //TODO зависимость от открытых трюков
                case ("Roll"):
                    trick = ProgressManager.Instance.GetRandomRoll();
                    gm.DoTrick(trick);
                    AudioManager.Instance.PlayUniqueSound(Gender.Kind == Gender.GenderKinds.Male ? Sounds.Rift2 : Sounds.RiftFem);
                    return trick.AnimationName;

                case ("Slide"):
                    trick = ProgressManager.Instance.GetRandomSlide();
                    gm.DoTrick(trick);
                    AudioManager.Instance.PlayUniqueSound(Gender.Kind == Gender.GenderKinds.Male ? Sounds.Rift2 : Sounds.RiftFem);
                    return trick.AnimationName;

                case ("JumpOverFar"):
                    trick = ProgressManager.Instance.GetRandomJumpOver();
                    gm.DoTrick(trick);
                    OnJumpOverObstacle.SafeInvoke();
                    AudioManager.Instance.PlayUniqueSound(Gender.Kind == Gender.GenderKinds.Male ? Sounds.JumpOver : Sounds.JumpOverFem);
                    return trick.AnimationName;

                case ("JumpOverClose"):
                    //AudioManager.Instance.PlaySound(Sounds.Caption);
                    return "JumpOver";

                case ("Stand"):
                    //TODO proc reward
                    HUDManager.Instance.Flash();
                    trick = ProgressManager.Instance.GetRandomStand();
                    gm.DoTrick(trick);
                    HUDManager.Instance.ShowTrickName(trick, gm.TrickMultipiler * gm.CoinMultipiler);
                    return trick.AnimationName;

                default:
                    return playAnimation; //Для actions, которые не могут быть рандомными
            }

            //TODO Зависисость от открытых трюков
            
        }
    }
}