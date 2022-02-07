using UnityEngine;

public class Recoil : MonoBehaviour
{
    //arma actual
    private WeaponHolder weaponChanger;
    private Weapon currentWeapon;

    //rotaciones
    private Vector3 currentRot;
    private Vector3 targetRot;

    private void Start()
    {
        weaponChanger = FindObjectOfType<WeaponHolder>();
    }
    void Update()
    {
        // Comprobamos cual es el arma actual
        if (weaponChanger.isEmpty() || weaponChanger.GetCurrentItem().typeOfItem != GameUtils.TypeOfItem.GUN)
        return;
        currentWeapon = weaponChanger.GetCurrentItem().GetComponent<Weapon>();

        //Calcula la rotación para volver a reposo
        targetRot = Vector3.Lerp(targetRot, Vector3.zero, currentWeapon.ws.returnSpeed * Time.deltaTime);
        currentRot = Vector3.Slerp(currentRot, targetRot, currentWeapon.ws.recoilSpeed * Time.fixedDeltaTime);
        //aplica la rotación
        transform.localRotation = Quaternion.Euler(currentRot);
       
    }
    //método llamado en el script weapon
    public void RecoilFire(Vector3 recoil)
    {
        //recoil en los ejes x,z
        targetRot += new Vector3(recoil.x, Random.Range(-recoil.y, recoil.y), Random.Range(-recoil.z, recoil.z));
        
    }

}
