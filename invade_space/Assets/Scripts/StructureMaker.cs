using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureMaker : MonoBehaviour
{
    private string path = "Structures/";
    [SerializeField] private List<string> structuresTypes = new List<string> {"factory", "mine"};
    // existing structures
    [HideInInspector] public List<GameObject> Structures;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void makeStructure(string type, Vector2 side)
    {
        if (!structuresTypes.Contains(type))
        {
            Debug.Log("Structure type not found");
            return;
        }

        // check if structure have enough space
        side -= (Vector2)transform.position;
        float positionAngle = Mathf.Atan2(side.y, side.x) * Mathf.Rad2Deg - 135.0f;

        foreach (GameObject other in Structures)
        {
            float angleDiff = Mathf.Abs(Mathf.DeltaAngle(positionAngle, other.transform.rotation.eulerAngles.z));
            if (angleDiff < 6.0f)
            {
                Debug.Log("Structure too close to another structure");
                return;
            }
        }

        GameObject structure = Instantiate(Resources.Load(path + type, typeof(GameObject)), transform.position, Quaternion.identity) as GameObject;
        Structures.Add(structure);

        structure.transform.parent = transform;
        structure.transform.localScale = new Vector3(1, 1, 1);
        structure.transform.rotation = Quaternion.Euler(0, 0, positionAngle);
    }
}
