using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TargettingMobs : MonoBehaviour {

    public List<Transform> targets;

    public Transform selectedTarget;

    private Transform myTransfrom;


	// Use this for initialization
	void Start ()
    {
	    targets = new List<Transform>();
        selectedTarget = null;
        myTransfrom = transform;

        AddAllEnemies();
	}

    public void AddAllEnemies()
    {
        GameObject[] go = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in go)
            AddTarget(enemy.transform);
    }

    public void AddTarget(Transform enemy)
    {
        targets.Add(enemy);
    }


    private void SortTargetsByDistance()
    {
        targets.Sort(delegate(Transform t1, Transform t2) {
            return Vector3.Distance(t1.position, myTransfrom.position).CompareTo(Vector3.Distance(t2.position, myTransfrom.position)); 
        });
    }

    private void TargetEnemy()
    {
        if (selectedTarget == null)
        {
            SortTargetsByDistance();
            selectedTarget = targets[0];
        }
        else
        {
            int index = targets.IndexOf(selectedTarget);

            if (index < targets.Count - 1)
            {
                index++;
            }
            else
            {
                index = 0;
            }
            DeselectTarget();
            selectedTarget = targets[index];
          }
        SelectTarget();
    }

    private void SelectTarget()
    {
        Transform name = selectedTarget.FindChild("Name");

        if (name == null)
        {
            Debug.LogError("Could not find the Name on" + selectedTarget.name);
            return;
        }
        name.GetComponent<TextMesh>().text = "Zombie";
        name.GetComponent<MeshRenderer>().enabled = true;
        
        
        
    }

    private void DeselectTarget()
    {
        selectedTarget.FindChild("Name").GetComponent<MeshRenderer>().enabled = false;
        selectedTarget = null;
    }

    
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            TargetEnemy();
        }
	}
}
