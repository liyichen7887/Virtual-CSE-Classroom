using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RayCast : MonoBehaviour
{

    private float teleport_thres_time;
    public GameObject ovr_camera;
    public GameObject avatar;
    public Text dist_display;
    public Vector3 old_pos;
    private bool scale;


    public OVRInput.Button selectItem = OVRInput.Button.One;
    public OVRInput.Button groupItem = OVRInput.Button.Two;
    public GameObject selected_group;
    public Transform copy_pos;
    private bool release;
    private int count;
    public Material line_mt;
    private LineRenderer lineRenderer;
    public GameObject left_hand;
    public GameObject right_hand;
    public GameObject global;
    private float old_hand_dist;

    // Use this for initialization
    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        teleport_thres_time = 0.0f;
        release = false;
        count = 0;
        scale = false;
        old_hand_dist = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(this.transform.position);
        //target.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, Camera.main.nearClipPlane));

        Ray myRay = new Ray(this.transform.position, this.transform.forward);
        Debug.DrawRay(myRay.origin, myRay.direction * 200, Color.blue);

        RaycastHit hitObject;

        if (OVRInput.Get(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.LTouch))
        {
            if (scale == false)
            {
                old_hand_dist = Vector3.Distance(left_hand.transform.position, right_hand.transform.position);
                scale = true;
            }
            else
            {
                float new_hand_dist = Vector3.Distance(left_hand.transform.position, right_hand.transform.position);
                global.transform.localScale = global.transform.localScale * (1 + (new_hand_dist - old_hand_dist)/10);

            }
        }
        else
        {
            scale = false;
        }


        if (Physics.Raycast(myRay, out hitObject, 100))
        {

            //Debug.Log(hitObject.collider.gameObject.name);

            if (OVRInput.GetUp(OVRInput.Button.Start, OVRInput.Controller.LTouch))
            {
                Debug.Log("--------------------");
                if (count == 0)
                {
                    Debug.Log("aaaaaaaaaaaaaa");
                    old_pos = hitObject.point;
                    Debug.Log(old_pos);
                    count++;
                    lineRenderer.positionCount = 0;
                }
                else if (count == 1)
                {
                    
                    Debug.Log("bbbbbbbbbbbbbbb");
                    float dist = Vector3.Distance(hitObject.point, old_pos);
                    Debug.Log(hitObject.point);
                    dist_display.text = "Distance: " + dist * 4 + "m";
                    Debug.Log(dist);
                    count = 0;
                    
                    lineRenderer.material = new Material(line_mt);
                    lineRenderer.widthMultiplier = 0.02f;
                    lineRenderer.positionCount = 2;
                    lineRenderer.SetPosition(0, old_pos);
                    lineRenderer.SetPosition(1, hitObject.point);

                }
            }



            if ("Floor Plane" == hitObject.collider.gameObject.name)
            {
                teleport_thres_time += Time.deltaTime;
                if (teleport_thres_time > 3.0f)
                {

                    ovr_camera.transform.position = new Vector3(hitObject.point.x, ovr_camera.transform.position.y, hitObject.point.z);
                    avatar.transform.position = new Vector3(hitObject.point.x, avatar.transform.position.y, hitObject.point.z);
                    teleport_thres_time = 0.0f;
                }

            }

            else if ("Mesh1" != hitObject.collider.gameObject.name)
            {
                if (OVRInput.Get(selectItem, OVRInput.Controller.LTouch))
                {
                    //Debug.Log("aaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
                    Vector2 xy_rotation = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);
                    Vector2 xy_translation = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
                    if (hitObject.transform.parent == selected_group.transform)
                    {
                        selected_group.transform.position = new Vector3(selected_group.transform.position.x + xy_translation.x / 50, selected_group.transform.position.y, selected_group.transform.position.z + xy_translation.y / 50);
                        selected_group.transform.eulerAngles = new Vector3(selected_group.transform.rotation.eulerAngles.x + xy_rotation.x, selected_group.transform.rotation.eulerAngles.y, selected_group.transform.rotation.eulerAngles.z + xy_rotation.y);

                    }
                    else
                    {
                        hitObject.transform.position = new Vector3(hitObject.transform.position.x + xy_translation.x / 50, hitObject.transform.position.y, hitObject.transform.position.z + xy_translation.y / 50);
                        hitObject.transform.eulerAngles = new Vector3(hitObject.transform.rotation.eulerAngles.x + xy_rotation.x, hitObject.transform.rotation.eulerAngles.y, hitObject.transform.rotation.eulerAngles.z + xy_rotation.y);
                    }
                }
                if (OVRInput.GetUp(groupItem, OVRInput.Controller.LTouch))
                {
                    //Debug.Log("aaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
                    GameObject hit_obj = hitObject.transform.gameObject;
                    MeshRenderer[] list = hit_obj.GetComponentsInChildren<MeshRenderer>();
                    foreach (MeshRenderer mr in list)
                    {
                        Material[] mt_list = mr.materials;
                        for (int i = 0; i < mt_list.Length; i++)
                        {
                            Material highlight = mt_list[i];
                            mt_list[i].SetColor("_Color", new Color(1.0f, 0.0f, 0.0f));
                            //mt_list[i] = highlight;

                        }

                    }


                    List<Collider> nc = new List<Collider>();

                    for (int i = 0; i < selected_group.GetComponent<OVRGrabbable>().m_grabPoints.Length + 1; i++)
                    {
                        if (i != selected_group.GetComponent<OVRGrabbable>().m_grabPoints.Length)
                        {
                            nc.Add(selected_group.GetComponent<OVRGrabbable>().m_grabPoints[i]);
                        }
                        else
                        {
                            //Debug.Log(i);
                            nc.Add(hit_obj.GetComponent<BoxCollider>());
                        }


                    }

                    selected_group.GetComponent<OVRGrabbable>().m_grabPoints = nc.ToArray();

                    if (count == 0)
                    {
                        selected_group.transform.position = hitObject.transform.position;
                    }

                    hit_obj.transform.parent = selected_group.transform;

                    Destroy(hit_obj.GetComponent<Rigidbody>());
                    Destroy(hit_obj.GetComponent<OVRGrabbable>());


                    count++;

                }
                if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch) == 0.0)
                {
                    if (release == true)
                    {
                        GameObject copy;
                        if (hitObject.transform.parent == selected_group.transform)
                        {
                            copy = Instantiate(selected_group.transform.gameObject);
                            copy.transform.position = copy_pos.position;
                        }
                        else
                        {
                            copy = Instantiate(hitObject.transform.gameObject);
                            copy.transform.position = copy_pos.position;

                        }
                        copy.transform.parent = global.transform;
                        release = false;
                    }

                }
                else
                {
                    release = true;
                }
                /*
                if (OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger, OVRInput.Controller.RTouch) == 0.0)
                {
                    if (release == true)
                    {

                        if (count == 0)
                        {
                            old_pos = hitObject.transform.position;
                            count++;
                        }
                        if (count == 1) {
                            float dist = Vector3.Distance(hitObject.transform.position, old_pos);
                            dist_display.text = "Distance: " + dist;
                            count = 0;
                        }
                       
                        release = false;
                    }

                }
                else
                {
                    release = true;
                }
                */
                
                    

                }
                else
                {
                    teleport_thres_time = 0.0f;
                }
            }
        }

    }


