using UnityEngine;

public class VisualRecoil : MonoBehaviour
{
    [Header("References")]
    [SerializeField]private Weapon weapon;
    [SerializeField] private Transform recoilPosition;
    [SerializeField] private Transform rotationPoint;

    Vector3 rotationRecoil;
    Vector3 positionRecoil;
    Vector3 Rot;

    private void Start()
    {
        weapon = GetComponent<Weapon>();
    }
    void FixedUpdate()
    {

        rotationRecoil = Vector3.Lerp(rotationRecoil, Vector3.zero, weapon.weaponData.vRotationReturnSpeed * Time.fixedDeltaTime);
        positionRecoil = Vector3.Lerp(positionRecoil, Vector3.zero, weapon.weaponData.vPositionReturnSpeed * Time.fixedDeltaTime);

        recoilPosition.localPosition = Vector3.Slerp(recoilPosition.localPosition, positionRecoil, weapon.weaponData.vPositionRecoilSpeed * Time.fixedDeltaTime);
        Rot = Vector3.Slerp(Rot, rotationRecoil, weapon.weaponData.vRotationRecoilSpeed * Time.fixedDeltaTime);
        rotationPoint.localRotation = Quaternion.Euler(Rot);

    }

    public void VisualRecoilFire(Vector3 recoilRot,Vector3 recoilKickback)
    {
        rotationRecoil += new Vector3(-recoilRot.x, Random.Range(-recoilRot.y, recoilRot.y), Random.Range(-recoilRot.z, recoilRot.z));
        positionRecoil += new Vector3(Random.Range(-recoilKickback.x, recoilKickback.x), Random.Range(-recoilKickback.y, recoilKickback.y), recoilKickback.z);
    }
}
