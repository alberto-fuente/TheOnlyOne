using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProjectPath : MonoBehaviour
{
    private Scene predictionScene;
    private PhysicsScene predictionPhysicsScene;
    public GameObject dummy;
    public GameObject granadePrefab;
    public GranadeBlueprint granadeData;
    public int maxIterations;
    public LineRenderer lineRenderer;
    List<GameObject> dummyObstacles;
    void CreatePhysicsScene()
    {
        CreateSceneParameters parameters = new CreateSceneParameters(LocalPhysicsMode.Physics3D);
        predictionScene = SceneManager.CreateScene("Prediction", parameters);
        predictionPhysicsScene = predictionScene.GetPhysicsScene();
        Physics.autoSimulation = false;
        dummyObstacles = new List<GameObject>();
        
    }
    // Start is called before the first frame update
    void Start()
    {
        CreatePhysicsScene();
    }

    public void SimulateProjection(Transform shotPoint)
    {
        lineRenderer.enabled = true;
        if (dummy == null)
        {
            dummy = Instantiate(granadePrefab,shotPoint.transform.position,Quaternion.identity);
            SceneManager.MoveGameObjectToScene(dummy, predictionScene);
        }
        //Collider[] obstacles=Physics.OverlapSphere(transform.position, granadeData.throwForce/3);
        /*foreach (Collider coll in obstacles)
        {
            if (coll.gameObject.Equals(this.gameObject))
            {
                continue;
            } 
            GameObject fakeObstacle=Instantiate(coll.gameObject, coll.transform.position, coll.transform.rotation);
            if (fakeObstacle.GetComponent<Renderer>())
            {
                fakeObstacle.GetComponent<Renderer>().enabled = false;
            }
            SceneManager.MoveGameObjectToScene(fakeObstacle, predictionScene);
            dummyObstacles.Add(fakeObstacle);
        }*/
        dummy.transform.position = shotPoint.transform.position;
        dummy.GetComponent<Rigidbody>().AddForce(shotPoint.forward * granadeData.throwForce, ForceMode.Impulse);
        lineRenderer.positionCount = maxIterations;
        for (int i = 0; i < maxIterations; i++)
        {
            lineRenderer.allowOcclusionWhenDynamic = true;
            predictionPhysicsScene.Simulate(Time.fixedDeltaTime);
            lineRenderer.SetPosition(i, dummy.transform.position);
            
        }

        Destroy(dummy);
       // dummyObstacles.Clear();
    }

    public void StopSimulateProjection()
    {
        lineRenderer.enabled=false;
    }
}
