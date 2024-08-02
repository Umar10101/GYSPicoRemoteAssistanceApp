using System.Collections;
using UnityEngine;

public class CreateScript : MonoBehaviour
{
    public GameObject objectToInstantiate; // The prefab to instantiate
    public GameObject referenceObject; // The reference GameObject from which to take the position
    public float instantiationInterval = 0.01f; // Time interval between instantiations in seconds

    private void Start()
    {
        if (objectToInstantiate != null && referenceObject != null)
        {
            StartCoroutine(InstantiateObjectsAtInterval());
        }
        else
        {
            Debug.LogWarning("ObjectToInstantiate or ReferenceObject is not assigned.");
        }
    }

    private IEnumerator InstantiateObjectsAtInterval()
    {
        while (true)
        {
            Vector3 referencePosition = referenceObject.transform.position;

            for (int i = 0; i < 5; i++)
            {
                Instantiate(objectToInstantiate, referencePosition, Quaternion.identity);
            }

            yield return new WaitForSeconds(instantiationInterval);
        }
    }
}
