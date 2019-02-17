using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BBox : MonoBehaviour, IFocusable {

    Material BBMat;
    Material currMat;

    GameObject SRSBoundingBox;
    Bounds SRSBounds;
    public bool isActive;
    public bool BoundingBoxCreated;

    public bool isRootObject;

    // Use this for initialization
    void Start()
    {
        BBMat = Resources.Load("Materials/BBMat", typeof(Material)) as Material;
    }

    // Update is called once per frame
    void Update()
    {
        // Create BoundingBox is not yet created & if it is root
        if (!BoundingBoxCreated && isRootObject) {
            if (gameObject.name != "DefaultCursor" && gameObject.name != "TransformUITools") {
                CreateBoundingBox();
                BoundingBoxCreated = true;
                return;
            }
        }

        if (SRSBoundingBox != null) {
            if (isActive) {
                SRSBoundingBox.SetActive(true);
            }
            else {
                SRSBoundingBox.SetActive(false);
                return;
            }
        }
    }

    public void OnFocusEnter() {
        if (!BoundingBoxCreated || isActive) { return; }
        Debug.Log("BBox: On Focus");
        NRSRManager.SendFocusedObjectToManager(gameObject);
        isActive = true;
    }

    public void OnFocusExit() {
        if (!BoundingBoxCreated || !isActive) { return; }
        if (NRSRManager.holdSelectedObject_UsingTransformTool) { return; }
        if (NRSRManager.holdSelectedObject_LookingAtTransformTool) { return; }
        Debug.Log("BBox: Off Focus");
        NRSRManager.ClearFocusedObjectFromManager();
        isActive = false;
    }

    void CreateEndPoints(Vector3 position) {
        GameObject CornerHandle = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        CornerHandle.name = "Handle";
        CornerHandle.tag = "NRSRTools";

        CornerHandle.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
        CornerHandle.transform.position = position;
        CornerHandle.transform.parent = SRSBoundingBox.transform;
        CornerHandle.GetComponent<Renderer>().material = BBMat;

    }

    void CreateBoundingBox() {
        Debug.Log("Create Bounding Box: " + gameObject.name);

        SRSBoundingBox = Instantiate(gameObject);
        SRSBoundingBox.name = "BoundingBox";

        //New object should have BBox attached to it
        if (SRSBoundingBox.GetComponent<BBox>() == null)
        {
            Destroy(SRSBoundingBox);
            return;
        }
        else
        {
            Destroy(SRSBoundingBox.GetComponent<BBox>());
            SRSBoundingBox.tag = "NRSRTools";
            SRSBoundingBox.transform.localScale *= 1.1f;

            // Make sure it is parented to the copied obj.
            SRSBoundingBox.transform.parent = gameObject.transform;

            // For each children (transform), check meshrender
            List<Transform> children = new List<Transform>(
                SRSBoundingBox.GetComponentsInChildren<Transform>());

            foreach (Transform child in children) {
                child.tag = "NRSRTools";
                if (child.GetComponent<MeshRenderer>() != null) {
                    child.transform.parent = SRSBoundingBox.transform;
                }
            }

            // For each meshrender, create bounds
            List<MeshFilter> childrenBounds = new List<MeshFilter>(
                SRSBoundingBox.GetComponentsInChildren<MeshFilter>());

            foreach (MeshFilter meshRen in childrenBounds) {
                if (meshRen.GetComponent<MeshFilter>() != null) {
                    SRSBounds.Encapsulate(meshRen.GetComponent<MeshFilter>().mesh.bounds);
                }
            }

            Vector3 SRSPoint0 = SRSBounds.min * gameObject.transform.localScale.x * 1.1f;
            Vector3 SRSPoint1 = SRSBounds.max * gameObject.transform.localScale.z * 1.1f;
            Vector3 SRSPoint2 = new Vector3(SRSPoint0.x, SRSPoint0.y, SRSPoint1.z);
            Vector3 SRSPoint3 = new Vector3(SRSPoint0.x, SRSPoint1.y, SRSPoint0.z);
            Vector3 SRSPoint4 = new Vector3(SRSPoint1.x, SRSPoint0.y, SRSPoint0.z);
            Vector3 SRSPoint5 = new Vector3(SRSPoint0.x, SRSPoint1.y, SRSPoint1.z);
            Vector3 SRSPoint6 = new Vector3(SRSPoint1.x, SRSPoint0.y, SRSPoint1.z);
            Vector3 SRSPoint7 = new Vector3(SRSPoint1.x, SRSPoint1.y, SRSPoint0.z);

            CreateEndPoints(SRSPoint0 + transform.position);
            CreateEndPoints(SRSPoint1 + transform.position);
            CreateEndPoints(SRSPoint2 + transform.position);
            CreateEndPoints(SRSPoint3 + transform.position);
            CreateEndPoints(SRSPoint4 + transform.position);
            CreateEndPoints(SRSPoint5 + transform.position);
            CreateEndPoints(SRSPoint6 + transform.position);
            CreateEndPoints(SRSPoint7 + transform.position);

        }

    }
}
