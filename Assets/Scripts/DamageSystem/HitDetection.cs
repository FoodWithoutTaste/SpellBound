// HitDetection.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HitDetection : MonoBehaviour
{
    public HPType hpType; // Reference to the HPType component

    public Material whiteMat;
    private List<List<Material>> matsList;
    public Renderer[] renderers;

    public GameObject hitText;
    public LayerMask faction;

    private void Awake()
    {
        matsList = new List<List<Material>>();

        for (int i = 0; i < renderers.Length; i++)
        {
            List<Material> materials = new List<Material>(renderers[i].materials);
            matsList.Add(materials);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Damage" && faction != other.GetComponent<DealDamage>().faction)
        {
            if(other.GetComponent<DealDamage>().faction != transform.gameObject.layer )
            {
              
                // Calculate damage based on damage type
                float modifiedDamage = other.GetComponent<DealDamage>().Damage;

                var HitTextVar = Instantiate(hitText, other.transform.position, Quaternion.identity, transform);
                HitTextVar.GetComponent<TextMeshPro>().text = modifiedDamage.ToString();
                HitTextVar.GetComponent<TextMeshPro>().fontSize = Random.Range(30f, 40f);
                hpType.TakeDamage(modifiedDamage, other.GetComponent<DealDamage>().dmgType); // Modify this based on your HPType implementation
                StartCoroutine("GetHit");
            }

        

        }
    }

    private IEnumerator GetHit()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            Material[] materials = renderers[i].materials;

            for (int j = 0; j < materials.Length; j++)
            {
                materials[j] = whiteMat;
            }

            renderers[i].materials = materials;
        }

        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < renderers.Length; i++)
        {
            Material[] materials = renderers[i].materials;

            for (int j = 0; j < materials.Length; j++)
            {
                materials[j] = matsList[i][j];
            }

            renderers[i].materials = materials;
        }
    }

   
}
