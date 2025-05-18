using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] private Transform buildingHolder;
    public bool CanBuild = true;
    public GameObject[] buildPrefabs;
    public int currentPrefab;
    public Material canBuildMat;
    public Material cantBuildMat;

    private GameObject previewInstance;
    private bool canBuild = true;
    private Quaternion currentRotation = Quaternion.identity;

    public LayerMask layer;

    public bool isBuilding = false;

    [Header("Destruction Stats")]
    public Material highlightMat;
    public bool destructionMode = false;
    private GameObject hoveredObject;
    private Material originalMat;
    [SerializeField] private GameObject destructionModeText;
    [SerializeField] private GameObject buildingModeText;

    private void StartBuilding()
    {
        isBuilding = true;
        if (currentPrefab < 0)
            currentPrefab = buildPrefabs.Length - 1;
        else if (currentPrefab >= buildPrefabs.Length)
            currentPrefab = 0;

        if (previewInstance != null)
            Destroy(previewInstance.gameObject);
        currentRotation = Quaternion.identity;
        previewInstance = Instantiate(buildPrefabs[currentPrefab]);
        previewInstance.GetComponent<BoxCollider>().enabled = false;
        buildingModeText.SetActive(true);
    }
    public void StopBuilding()
    {
        if (previewInstance != null)
            Destroy(previewInstance.gameObject);
        isBuilding = false;
        buildingModeText.SetActive(false);
    }

    private void Update()
    {
        if (!CanBuild)
            return;

        //Stop building
        if (Input.GetKeyDown(KeyCode.Tab) && isBuilding && !destructionMode)
        {
            StopBuilding();
            return;
        }
        if (Input.GetMouseButtonDown(1) && isBuilding && !destructionMode)
        {
            StopBuilding();
            return;
        }
        if (Input.GetKeyDown(KeyCode.Tab) && !isBuilding && !destructionMode)
        {
            StartBuilding();
            return;
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            destructionMode = !destructionMode;
            if (destructionMode)
                StopBuilding();
            else
                StartBuilding(); //start building

            destructionModeText.SetActive(destructionMode);

            ClearHoverHighlight();
        }

        if (destructionMode)
        {
            HandleDestructionHover();
            return;
        }

        if (!isBuilding)
            return;



        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
        {
            currentPrefab++;
            StartBuilding();
        }
        else if (scroll < 0f)
        {
            currentPrefab--;
            StartBuilding();
        }


        if (previewInstance == null) return;

        HandleRotation();
        UpdatePreviewPosition();
        CheckBuildValidity();

        if (canBuild && Input.GetMouseButtonDown(0))
        {
            GameObject newObj = Instantiate(buildPrefabs[currentPrefab], previewInstance.transform.position, currentRotation, buildingHolder);
            TryAutoConnectConveyors(newObj);
        }

    }
    public void DestroyAllPlayerBuilding()
    {
        foreach (Transform child in buildingHolder)
        {
            Destroy(child.gameObject);
        }
    }
    private void TryAutoConnectConveyors(GameObject obj)
    {
        ConveyorBelt newBelt = obj.GetComponent<ConveyorBelt>();
        if (newBelt == null) return;

        //check behind
        Transform checkerBack = newBelt.inputCheckerBack;
        if (checkerBack == null) return;

        Vector3 halfExtents = new Vector3(0.2f, 0.2f, 0.2f);
        Collider[] overlapsBack = Physics.OverlapBox(checkerBack.position, halfExtents);
        if (overlapsBack.Length > 0)
        {
            foreach (Collider collider in overlapsBack)
            {
                if (collider.TryGetComponent(out ConveyorBelt belt))
                {
                    if (belt.nextBelt != null)
                        return;

                    belt.nextBelt = newBelt; // assign current belt to belt in behind
                }
            }
        }

        //check front
        Transform checkerFront = newBelt.inputCheckerFront;
        if (checkerFront == null) return;

        Collider[] overlapsFront = Physics.OverlapBox(checkerFront.position, halfExtents);
        if (overlapsFront.Length > 0)
        {
            foreach (Collider collider in overlapsFront)
            {
                if (collider.TryGetComponent(out ConveyorBelt belt))
                {
                    newBelt.nextBelt = belt; //assign the belt in front to current belt
                }
            }
        }
    }
    private void HandleRotation()
    {
        if (previewInstance == null) return;

        if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Space))
        {
            currentRotation *= Quaternion.Euler(0, 90, 0);
            previewInstance.transform.rotation = currentRotation;
        }
    }

    private void UpdatePreviewPosition()
    {
        if (previewInstance == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            float gridSize = 2f;
            Vector3 gridPos = new Vector3(
                Mathf.Round(hit.point.x / gridSize) * gridSize,
                0f,
                Mathf.Round(hit.point.z / gridSize) * gridSize
            );
            previewInstance.transform.position = gridPos;
        }
    }

    private void CheckBuildValidity()
    {
        if (previewInstance == null) return;

        Vector3 position = previewInstance.transform.position;
        Vector3 halfExtents = new Vector3(0.45f, 0.45f, 0.45f);
        Collider[] overlaps = Physics.OverlapBox(position, halfExtents, currentRotation, layer);

        canBuild = overlaps.Length == 0;

        var renderer = previewInstance.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            renderer.material = canBuild ? canBuildMat : cantBuildMat;
        }
    }
    #region Destruction
    /// <summary>
    /// This part affects for destruction functions
    /// </summary>
    private void HandleDestructionHover()
    {
        // raycast to mouse -> check if target has belt -> apply
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject target = hit.collider.gameObject;

            if (target.CompareTag("Destructible"))
            {
                if (hoveredObject != target)
                {
                    ClearHoverHighlight();
                    hoveredObject = target;

                    var rend = target.GetComponentInChildren<Renderer>();
                    if (rend != null)
                    {
                        originalMat = rend.material;
                        rend.material = highlightMat;
                    }
                }

                if (Input.GetMouseButtonDown(0))
                {
                    if (target.TryGetComponent(out ConveyorBelt belt) && belt.currentItem != null)
                        Destroy(belt.currentItem.gameObject); // alsö remove the item from belt so it doesnt get bugged
                    Destroy(target);
                    hoveredObject = null;
                }
            }
            else
            {
                ClearHoverHighlight();
            }
        }
        else
        {
            ClearHoverHighlight();
        }
    }

    public void ClearHoverHighlight()
    {
        if (hoveredObject != null)
        {
            var rend = hoveredObject.GetComponentInChildren<Renderer>();
            if (rend != null && originalMat != null)
                rend.material = originalMat;

            hoveredObject = null;
            originalMat = null;
        }
    }

    #endregion
}
