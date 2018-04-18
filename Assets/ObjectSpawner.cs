using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectSpawner : MonoBehaviour {

    public static ObjectSpawner Instance;
    public GameObject[] prefabs;
    public Transform SpawnPoint;
    public Text next_furniture;
    public GameObject global;

    public OVRInput.Button nextKey = OVRInput.Button.One;
    public OVRInput.Button spawnKey = OVRInput.Button.Two;


    //[HideInInspector]
    public List<GameObject> spawnedItems;
    private int currentSelection;
    private Vector3 spawnPosition;

    void Awake()
    {
        spawnedItems = new List<GameObject>();
        Instance = this;
        currentSelection = 0;
        spawnPosition = SpawnPoint.position;
    }


    void Update()
    {
        //Debug.Log(OVRInput.Get(nextKey));
        if (OVRInput.GetUp(nextKey, OVRInput.Controller.RTouch))
        {
            //Debug.Log("----------------1------------");
            SelectNextItem();
        }
        if (OVRInput.GetUp(spawnKey, OVRInput.Controller.RTouch))
        {
            SpawnItem();
        }
    }

    void SelectNextItem()
    {
        currentSelection = (currentSelection + 1) % (prefabs.Length);
        //Debug.Log("1");
        for(int i=0; i < prefabs.Length; ++i)
        {
            if( i == currentSelection)
            {
                //Debug.Log("2");
                next_furniture.text = "Next Furniture (Use right finger to change): " + prefabs[i].name;
            }

        }
    }

    void SpawnItem()
    {
        GameObject obj = Instantiate(prefabs[currentSelection]) as GameObject;
        obj.transform.position = new Vector3(spawnPosition.x, spawnPosition.y, spawnPosition.z);
        obj.transform.rotation = Quaternion.Euler(-90.0f, 180.0f, 0.0f);
        //obj.transform.position = new Vector3(spawnPosition.x, 0.40f, spawnPosition.z);
        BoxCollider bc = obj.AddComponent(typeof(BoxCollider)) as BoxCollider;
        obj.AddComponent(typeof(Rigidbody));
        obj.AddComponent(typeof(OVRGrabbable));
        obj.GetComponent<OVRGrabbable>().enabled = true;
        Collider[] nc = new Collider[] { obj.GetComponent<BoxCollider>() };
        obj.GetComponent<OVRGrabbable>().m_grabPoints = nc;
        switch (currentSelection)
        {
            case 0:
                bc.size = new Vector3(0.5f, 0.3f, 0.82f);
                //obj.transform.position = new Vector3(spawnPosition.x, 0.401f, spawnPosition.z);
                break;
            case 1:
                bc.size = new Vector3(4.4f, 12.1f, 11f);
                obj.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
                break;
            case 2:
                bc.size = new Vector3(8f, 8f, 11.7f);
                obj.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
                //obj.transform.position = new Vector3(spawnPosition.x, 0.16f, spawnPosition.z);
                break;
            case 3:
                bc.size = new Vector3(0.3f, 0.7f, 0.48f);
                //obj.transform.position = new Vector3(spawnPosition.x, 0.227f, spawnPosition.z);
                break;
            case 4:
                bc.size = new Vector3(0.1f, 0.1f, 0.7f);
                //obj.transform.position = new Vector3(spawnPosition.x, 0.352f, spawnPosition.z);
                break;
        }
        //obj.GetComponent<OVRGrabbable>().GetComponents<Collider> = obj.GetComponent<BoxCollider>;
        obj.transform.parent = global.transform;
    }

}

