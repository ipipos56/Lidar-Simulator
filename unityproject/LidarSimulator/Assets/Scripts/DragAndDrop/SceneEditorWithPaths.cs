/*
* @author: Philip Tibom
*/

using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Author: Philip Tibom
/// Creates drag and drop functionality.
/// </summary>
public class SceneEditorWithPaths : MonoBehaviour {
	public GameObject wayPoint;
	GameObject instantiatedWayPoint;

    private GameObject go;
	private GameObject placedNavObj = null;
	private GameObject previousWayPoint = null;

    private bool moveGameObject = false;
    private bool rotateGameObject = false;
	private bool placingPath = false;
    private float previousMousePos;
    private float lastClick;

	// Use this for initialization
	void Start ()
    {
		
	}

	// Update is called once per frame
	void Update ()
    {
        // Check if mouse is not over UI. And if we supposed to move object.
		if(placingPath && !EventSystem.current.IsPointerOverGameObject())
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out hit))
			{
				go.transform.position = hit.point;

				if (Input.GetMouseButtonDown(0))
				{
					if (hit.collider.gameObject.GetComponent<WayPoint>() != null)
					//Om man klickar på en annan waypoint i vägen.
					{
						if (previousWayPoint != null) 
						{
							go.GetComponent<WayPoint> ().ClosePath (hit.collider.gameObject.GetComponent<WayPoint> ()
								, previousWayPoint);
							placingPath = false;
						}
					}
					else if(previousWayPoint = null)
					{
						go.GetComponent<WayPoint>().SetStart(true);
						placedNavObj.GetComponent<AgentTest> ().target = go.transform;

						instantiatedWayPoint = GameObject.Instantiate (wayPoint) as GameObject;
						previousWayPoint = go;
						go = instantiatedWayPoint;
					}
					else
					{
						go.GetComponent<WayPoint>().AddToPath(previousWayPoint);

						instantiatedWayPoint = GameObject.Instantiate (wayPoint) as GameObject;
						previousWayPoint = go;
						go = instantiatedWayPoint;
					}
					lastClick = Time.time;
					//moveGameObject = false;
					//rotateGameObject = false;
				}
				if (Input.GetMouseButtonDown(1))
				{
					//om vägen finns, förstör vägen och börja om. Om den inte finns, sätt inte ut någon väg.
					Destroy(go);
					placingPath = false;
					placedNavObj = null;
					previousWayPoint = null;
				}
			}
		}
		else if (moveGameObject && !EventSystem.current.IsPointerOverGameObject())
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                go.transform.position = hit.point;

                if (Input.GetMouseButtonDown(0))
                {
                    lastClick = Time.time;
                    moveGameObject = false;
                    rotateGameObject = true;
                }
                if (Input.GetMouseButtonDown(1))
                {
                    Destroy(go);
                    moveGameObject = false;
                }
            }
        }
        else if (rotateGameObject && !EventSystem.current.IsPointerOverGameObject())
        {
            if (previousMousePos != 0)
            {
                go.transform.Rotate(Vector3.up, previousMousePos - Input.mousePosition.x);
            }
            previousMousePos = Input.mousePosition.x;

            if (Input.GetMouseButtonDown(0))
            {
                lastClick = Time.time;
                SetAlpha(1f);
                ActivateColliders(true);
                previousMousePos = 0;
                rotateGameObject = false;
				if (go.GetComponent<AgentTest> () != null) 
				{
					placedNavObj = go;
					placingPath = true;
				}
				//Det vill säga om objektet använder navigation
				

            }
            if (Input.GetMouseButtonDown(1))
            {
                Destroy(go);
                previousMousePos = 0;
                rotateGameObject = false;
            }
        }
        else if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButton(0) && lastClick + 0.2 < Time.time)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.CompareTag("EditableObject"))
                {
                    lastClick = Time.time;
                    GameObject o = FindParentEditableObject(hit.collider.gameObject);
                    MoveObject(o);
                }
            }
        }
	}

    /// <summary>
    /// Instantiates selected prefab.
    /// </summary>
    /// <param name="prefab">A game object prefab</param>
    public void PlaceObject(GameObject prefab)
    {
        if (!moveGameObject && !rotateGameObject)
        {
            MoveObject(Instantiate(prefab));
        }
    }

    /// <summary>
    /// Enables movement of game object. Following the mouse.
    /// </summary>
    /// <param name="obj">A game object.</param>
    private void MoveObject(GameObject obj)
    {
        if (!moveGameObject && !rotateGameObject)
        {
            go = obj;
            SetAlpha(0.33f);
            moveGameObject = true;
            ActivateColliders(false);
        }
    }

    /// <summary>
    /// Sets transparency of game object and child objects.
    /// </summary>
    /// <param name="value">Alpha value 0 to 1</param>
    private void SetAlpha(float value)
    {
        MeshRenderer[] allParts = go.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer o in allParts)
        {
            Color color = o.material.color;
            color.a = value;
            o.material.color = color;
        }
    }

    /// <summary>
    /// Activates colliders of object and child objects.
    /// </summary>
    /// <param name="isEnabled">Enabled or not</param>
    private void ActivateColliders(bool isEnabled)
    {
        Collider[] colliders = go.GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = isEnabled;
        }
    }

    /// <summary>
    /// Finds parent object tagged with "EditableObject".
    /// </summary>
    /// <param name="o">A game object</param>
    /// <returns></returns>
    private GameObject FindParentEditableObject(GameObject o)
    {
        if (o.transform.parent != null && o.transform.parent.gameObject.CompareTag("EditableObject"))
        {
            return FindParentEditableObject(o.transform.parent.gameObject);
        }
        return o;
    }
}
