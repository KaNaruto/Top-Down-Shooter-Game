using UnityEngine;

namespace Combat.Firearm
{
    public class GunController : MonoBehaviour
    {
        [SerializeField] Transform weaponHold;
        [SerializeField] Gun[] guns;

        public Gun EquippedGun { get; private set; }

       
        
        public void OnTriggerHold()
        {
            if (EquippedGun != null)
                EquippedGun.OnTriggerHold();
        }

        public void OnTriggerReleased()
        {
            if(EquippedGun !=null)
                EquippedGun.OnTriggerReleased();
        }
        void EquipGun(Gun gunToEquip)
        {
            if (EquippedGun != null)
                Destroy(EquippedGun.gameObject);
            EquippedGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation);
            EquippedGun.transform.parent = weaponHold;
        }

        public void EquipGun(int weaponIndex)
        {
            EquipGun(guns[weaponIndex]);
        }

        public float GetGunHeight => weaponHold.position.y;

        public void Aim(Vector3 aimPoint)
        {
            if(EquippedGun!=null)
                EquippedGun.Aim(aimPoint);
        }

        public void Reload()
        {
            if(EquippedGun!=null)
                EquippedGun.Reload();
        }
    }
}