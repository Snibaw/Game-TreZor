using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;

public class Projectiles : MonoBehaviour
{
    public Transform target;
    [SerializeField] float speed = 10;
    // Start is called before the first frame update
   

    // Update is called once per frame
    void Update()
    {
      if (target != null)
        {
            transform.LookAt(GetAimLocation());
            transform.Translate(Vector3.forward * speed *Time.deltaTime);
        }
    }
   

    public Vector3 GetAimLocation()
    {
        CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
        if ( targetCapsule == null)
        { return target.transform.position; }
        return target.transform.position + Vector3.up * targetCapsule.height / 2;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            Destroy(gameObject);
        }
    }

}
