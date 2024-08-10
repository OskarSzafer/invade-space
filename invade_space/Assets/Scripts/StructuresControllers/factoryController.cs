using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class factoryController : MonoBehaviour
{
    [SerializeField] private ParticleSystem cell;
    [SerializeField] private GameObject cellLauncher;
    [SerializeField] private ParticleSystem sub;

    // Start is called before the first frame update
    void Start()
    {
        var main = cell.main;
        main.startRotation = -Mathf.PI/4 - transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        cellLauncher.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        sub.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        InvokeRepeating("makeCell", 0.0f, 2.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void makeCell()
    {
        cell.Play();
    }
}
