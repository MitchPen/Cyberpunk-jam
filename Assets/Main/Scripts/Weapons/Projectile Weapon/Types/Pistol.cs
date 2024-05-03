namespace Main.Scripts.Weapons.Projectile_Weapon.Types
{
    public class Pistol : DefaultProjectileWeapon
    {
        protected override void Shoot()
        {
            base.Shoot();
            //_shootParticles.Play();
            _audioPlayer.Play();
            _gunAnimation.PlayShotAnimation(true);
        }


        public override void Hide()
        {
            base.Hide();
        }

        public override void Reload()
        {
            base.Reload();
        }

        public override void Show()
        {
            base.Show();
        }
    }
}