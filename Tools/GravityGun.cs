using Unity.Netcode;
using UnityEngine;


namespace BlockEngine
{

    public class GravityGun : MonoBehaviour
    {

        // Non-static attributes.
        private float maxGrabDist = 40;
        private float minGrabDist = 1;
        [SerializeField]
        private Tools tools;
        private InteractableObject grabbedObject;
        private Rigidbody grabbedRB;
        private float grabDist;
        private Vector3 targetPos; // Donde queremos que vaya el objeto que hemos cogido.
        private Vector3 grabForce;


        // Caché del cliente.
        private Vector3 oldVelocity = Vector3.zero;


        // Non-static methods.
        public void Update()
        {

            if (Input.GetKeyDown(KeyCode.Mouse0) && tools.selectedTool == Tool.GravityGun)
                Grab();

            if (grabbedRB && (Input.GetKeyUp(KeyCode.Mouse0) || tools.selectedTool != Tool.GravityGun))
                Release();

            if (grabbedRB && Input.GetKeyDown(KeyCode.Mouse1))
                Release(true);

            grabDist = Mathf.Clamp(grabDist + Input.mouseScrollDelta.y, minGrabDist, maxGrabDist);

        }

        private void LateUpdate()
        {

            if (grabbedObject)
            {

                Vector3 midPoint = (tools.meshRenderer.bounds.center + targetPos) / 2f;
                midPoint += Vector3.ClampMagnitude(grabForce / 2f, 1f);
                DrawQuadraticBezierCurve(tools.lineRenderer, tools.meshRenderer.bounds.center, midPoint, grabbedRB.worldCenterOfMass);

            }

        }

        private void FixedUpdate()
        {

            if (grabbedRB != null)
            {

                Ray ray = Camera.main.ViewportPointToRay(Vector3.one * 0.5f);
                targetPos = (tools.meshRenderer.bounds.center + ray.direction * grabDist);
                grabbedObject.updateGrabbedRBVelocityServerRpc(targetPos);

            }

        }

        private void Grab()
        {

            Ray ray = Camera.main.ViewportPointToRay(Vector3.one * 0.5f);
            if (Physics.Raycast(ray, out RaycastHit hit, maxGrabDist) && hit.rigidbody != null)
            {

                grabDist = hit.distance;
                grabbedRB = hit.rigidbody;
                grabbedObject = hit.collider.gameObject.GetComponent<InteractableObject>();
                grabbedObject.grabServerRpc();
                tools.lineRenderer.enabled = true;

            }

        }

        private void Release(bool setKinematic = false)
        {

            grabbedObject.releaseServerRpc(setKinematic);
            grabbedObject = null;
            grabbedRB = null;
            tools.lineRenderer.enabled = false;

        }

        void DrawQuadraticBezierCurve(LineRenderer lineRenderer, Vector3 point0, Vector3 point1, Vector3 point2)
        {

            lineRenderer.positionCount = 200;
            float t = 0f;
            Vector3 B = new Vector3(0, 0, 0);
            for (int i = 0; i < lineRenderer.positionCount; i++)
            {

                B = (1 - t) * (1 - t) * point0 + 2 * (1 - t) * t * point1 + t * t * point2;
                lineRenderer.SetPosition(i, B);
                t += (1 / (float)lineRenderer.positionCount);

            }

        }

    }

}
