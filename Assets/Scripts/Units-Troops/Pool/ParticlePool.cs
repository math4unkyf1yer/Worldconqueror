using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ParticlePool : MonoBehaviour
{
    [SerializeField] private List<GameObject> ParticleList = new List<GameObject>();
    [SerializeField] private GameObject particle;
    [SerializeField] private int amountOfParticle = 15;

    [SerializeField] Color critColor;
    [SerializeField] Color sturdyColor;
    public static ParticlePool Instance { get; private set; }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        for (int i = 0; i < amountOfParticle; i++)
        {
            GameObject particleClone = Instantiate(particle, this.transform);
            particleClone.SetActive(false);
            ParticleList.Add(particleClone);
        }

    }

    public void RemoveParticle(GameObject par)
    {
        ParticleList.Remove(par);
        par.SetActive(true);
    }
    public void ReceiveParticle(GameObject par)
    {
        par.SetActive(false);
        ParticleList.Add(par);
    }

    public ParticleSystem GetParticle(string particleType)//get troops from the lest
    {
        if (ParticleList.Count > 0)
        {
            GameObject parG = ParticleList[0];
            RemoveParticle(parG);
            ParticleSystem par = parG.GetComponent<ParticleSystem>();
            var main = par.main;

            if (particleType ==  "CritBurst")
            {
                //change color 
                main.startColor = critColor;
            }
            else
            {
                main.startColor = sturdyColor;
            }
            //change color 
            return par;
        }
        else
        {
            return null;
        }
    }
}
