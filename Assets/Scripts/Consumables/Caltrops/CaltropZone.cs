using UnityEngine;
using UnityEngine.InputSystem;

public class CaltropZone : MonoBehaviour
{

    private LineRenderer circleRenderer;
    public LayerMask enemyMask;
    [SerializeField] public float radiusNum;
    public Ray ray;

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                DrawCircle(hit.point);
            }
        }
    }

    public void Initialize(float radiusNum, LayerMask enemyMask)
    {
        Debug.Log("Initialize Started");
        this.radiusNum = radiusNum;
        this.enemyMask = enemyMask;
    }

    void DrawCircle(Vector3 center)
    {
        circleRenderer = gameObject.GetComponent<LineRenderer>();
        if (circleRenderer == null)
            circleRenderer = gameObject.AddComponent<LineRenderer>();

        circleRenderer.startWidth = 0.15f;
        circleRenderer.endWidth = 0.15f;
        circleRenderer.material = new Material(Shader.Find("Sprites/Default"));
        circleRenderer.startColor = new Color(1f, 0.3f, 7f, 0.9f);
        circleRenderer.endColor = new Color(1f, 0.3f, 7f, 0.9f);
        circleRenderer.positionCount = 64;
        circleRenderer.useWorldSpace = true;

        for (int i = 0; i < 64; i++)
        {
            float angle = i / 64f * Mathf.PI * 2f;
            float x = Mathf.Cos(angle) * radiusNum;
            float z = Mathf.Sin(angle) * radiusNum;
            Vector3 point = new Vector3(center.x + x, 100f, center.z + z);
            if (Physics.Raycast(point, Vector3.down, out RaycastHit hit, 200f))
            {
                circleRenderer.SetPosition(i, hit.point + Vector3.up * .1f);
            }
        }
        Debug.Log("Circle Drawn");
    }
}
