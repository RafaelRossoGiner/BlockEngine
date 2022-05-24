using Cinemachine;
using UnityEngine;

namespace BlockEngine
{
    [RequireComponent(typeof(Camera))]
    public class OutlinerRaycaster : MonoBehaviour
    {
        [SerializeField]
        private float maxSelectDist;
        [SerializeField]
        private LayerMask highlightLayers;

        private Camera m_camera;

        private static Outline currentOutliner;
        private static Outline prevOutliner;

        private static BlockEngine.InteractableObject currentObject;

        private void Start()
        {
            m_camera = GetComponent<Camera>();
        }
        void Update()
        {
            Vector3 target = m_camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.5f));
            Vector3 direction = (target - m_camera.transform.position).normalized;
            Ray ray = new Ray(m_camera.transform.position, direction);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxSelectDist, highlightLayers))
            {
                currentOutliner = hit.collider.GetComponent<Outline>();
                currentObject = hit.collider.GetComponent<BlockEngine.InteractableObject>();

                if (prevOutliner != currentOutliner)
                {
                    HideOutline();
                    ShowOutline();
                }
                prevOutliner = currentOutliner;
            }
            else
            {
                HideOutline();
                currentObject = null;
            }
        }

        void ShowOutline()
        {
            return;
            if (currentOutliner != null)
            {
                if (currentObject != null)
                {
                    currentOutliner.enabled = true;
                }
            }
        }

        void HideOutline()
        {
            return;
            if (prevOutliner != null)
            {
                prevOutliner.enabled = false;
                prevOutliner = null;
            }
        }
    }
}

