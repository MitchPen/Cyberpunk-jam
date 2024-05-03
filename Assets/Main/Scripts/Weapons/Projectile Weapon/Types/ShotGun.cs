using UnityEngine;

namespace Main.Scripts.Weapons.Projectile_Weapon.Types
{
    public class ShotGun : DefaultProjectileWeapon
    {
        protected override void Shoot()
        {
            for (int i = 0; i < _data.bulletPerShot; i++)
            {
                base.Shoot();
            }
        }

        protected override Vector3 CalcBulletDirection()
        {
            var randDispersion = new Vector3();
            randDispersion.x = Random.Range(-_data.dispersion, _data.dispersion);
            randDispersion.y = Random.Range(-_data.dispersion, _data.dispersion);
            return base.CalcBulletDirection() + randDispersion;
        }
    }
}
