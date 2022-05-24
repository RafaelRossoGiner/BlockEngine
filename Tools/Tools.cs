using System.Collections;
using UnityEngine;


namespace BlockEngine
{

    public enum Tool { NoTool, SpawnTool, GravityGun, MaterialTool, BehaviourTool, DeleteTool }

    public class Tools : MonoBehaviour
    {

        // Non-static attributes.
        public Tool selectedTool;
        public GameObject tool;
        public LineRenderer lineRenderer;
        private GameObject player;
        private MeshFilter meshFilter;
        [System.NonSerialized]
        public MeshRenderer meshRenderer;
        [SerializeField]
        private Mesh[] toolMeshes;
        [SerializeField]
        private Material[] toolMaterials;
        public GravityGun gravityGun;
        public MaterialTool materialTool;


        // Non-static methods.

        public void Init(GameObject player)
        {

            this.player = player;

            meshFilter = tool.GetComponent<MeshFilter>();
            meshRenderer = tool.GetComponent<MeshRenderer>();

            lineRenderer = getPlayer().GetComponentInChildren<LineRenderer>();
            lineRenderer.startWidth = 0.02f;
            lineRenderer.endWidth = 0.02f;
            lineRenderer.useWorldSpace = true;
            lineRenderer.enabled = true;

        }

        public IEnumerator shootRay(Vector3 spawnPos)
        {

            lineRenderer.positionCount = 2;

            lineRenderer.SetPosition(0, meshRenderer.bounds.center);
            lineRenderer.SetPosition(1, spawnPos);

            lineRenderer.enabled = true;
            yield return new WaitForSeconds(0.1f);

            lineRenderer.enabled = false;

        }

        public GameObject getPlayer()
        {

            return player;

        }

        public void selectToolInput()
        {
            // Selección de herramientas.
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {

                selectedTool = Tool.NoTool;
                meshRenderer.enabled = false;

            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {

                selectedTool = Tool.SpawnTool;
                meshFilter.mesh = toolMeshes[0];
                meshRenderer.material = toolMaterials[0];
                meshRenderer.enabled = true;

            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {

                selectedTool = Tool.MaterialTool;
                meshFilter.mesh = toolMeshes[1];
                meshRenderer.material = toolMaterials[1];
                meshRenderer.enabled = true;

            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {

                selectedTool = Tool.BehaviourTool;
                meshFilter.mesh = toolMeshes[2];
                meshRenderer.material = toolMaterials[2];
                meshRenderer.enabled = true;

            }

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {

                selectedTool = Tool.GravityGun;
                meshFilter.mesh = toolMeshes[3];
                meshRenderer.material = toolMaterials[3];
                meshRenderer.enabled = true;

            }

            if (Input.GetKeyDown(KeyCode.Alpha6))
            {

                selectedTool = Tool.DeleteTool;
                meshFilter.mesh = toolMeshes[4];
                meshRenderer.material = toolMaterials[4];
                meshRenderer.enabled = true;

            }

        }

    }

}