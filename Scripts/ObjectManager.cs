using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public enum ObjectDistanceType
    {
        DistantFar,
        Far,
        Mid,
        NearMid,
        Near,
        Close,
        Ground,
        End
    }

    [Header("Distnace Setting")]
    [SerializeField] float distantFarSpeed = 1.5f;
    [SerializeField] float farSpeed = 1.8f;
    [SerializeField] float midSpeed = 2.1f;
    [SerializeField] float nearMidSpeed = 2.4f;
    [SerializeField] float nearSpeed = 2.7f;
    [SerializeField] float closeSpeed = 3;
    [SerializeField] float groundSpeed = 3;
                     
    [SerializeField] float distantFarZPosition = 5;
    [SerializeField] float farZPosition = 4;
    [SerializeField] float midZPosition = 3;
    [SerializeField] float nearMidZPosition = 2;
    [SerializeField] float nearZPosition = 1;
    [SerializeField] float closeZPosition = 0;
    [SerializeField] float groundZPosition = 1;
                     
    [SerializeField] Vector3 distantFarScale = new Vector3(0.1f, 0.1f, 1);
    [SerializeField] Vector3 farScale = new Vector3(0.2f, 0.2f, 1);
    [SerializeField] Vector3 midScale = new Vector3(0.4f, 0.4f, 1);
    [SerializeField] Vector3 nearMidScale = new Vector3(0.6f, 0.6f, 1);
    [SerializeField] Vector3 nearScale = new Vector3(0.8f, 0.8f, 1);
    [SerializeField] Vector3 closeScale = new Vector3(1, 1, 1);
    [SerializeField] Vector3 groundScale = new Vector3(1, 1, 1);

    [Header("Object Setting")]
    [SerializeField] GameObject distantFarObject;
    [SerializeField] GameObject farObject;
    [SerializeField] GameObject midObject;
    [SerializeField] GameObject nearMidObject;
    [SerializeField] GameObject nearObject;
    [SerializeField] GameObject closeObject;
    [SerializeField] GameObject groundObject;

    [SerializeField] float distantFarObjectSpawnDelay = 1;
    [SerializeField] float farObjectSpawnDelay = 1;
    [SerializeField] float midObjectSpawnDelay = 1;
    [SerializeField] float nearMidObjectSpawnDelay = 1;
    [SerializeField] float nearObjectSpawnDelay = 1;
    [SerializeField] float closeObjectSpawnDelay = 1;
    [SerializeField] float groundObjectSpawnDelay = 1;

    Dictionary<ObjectDistanceType, List<GameObject>> backgroundObject = new Dictionary<ObjectDistanceType, List<GameObject>>();

    float distantFarObjectSpawnTimer = 0;
    float farObjectSpawnTimer = 0;
    float midObjectSpawnTimer = 0;
    float nearMidObjectSpawnTimer = 0;
    float nearObjectSpawnTimer = 0;
    float closeObjectSpawnTimer = 0;
    float groundObjectSpawnTimer = 0;

    void Start()
    {
        for (int i = 0; i < (int)ObjectDistanceType.End; i++)
        {
            backgroundObject[(ObjectDistanceType)i] = new List<GameObject>();
        }

        groundObjectSpawnTimer = groundObjectSpawnDelay;
    }

    void Update()
    {
        if (GameManager.instance.currentGameState != GameState.MoveNextStep)
        {
            return;
        }

        distantFarObjectSpawnTimer  += Time.deltaTime;
        farObjectSpawnTimer         += Time.deltaTime;
        midObjectSpawnTimer         += Time.deltaTime;
        nearMidObjectSpawnTimer     += Time.deltaTime;
        nearObjectSpawnTimer        += Time.deltaTime;
        closeObjectSpawnTimer       += Time.deltaTime;
        groundObjectSpawnTimer      += Time.deltaTime;

        SpawnObject();
    }

    private void SpawnObject()
    {
        if (distantFarObjectSpawnTimer > distantFarObjectSpawnDelay)
        {
            distantFarObjectSpawnTimer = 0;
            SpawnObject(distantFarObject, ObjectDistanceType.DistantFar);
        }
        if (farObjectSpawnTimer > farObjectSpawnDelay)
        {
            farObjectSpawnTimer = 0;
            SpawnObject(farObject, ObjectDistanceType.Far);
        }
        if (midObjectSpawnTimer > midObjectSpawnDelay)
        {
            midObjectSpawnTimer = 0;
            SpawnObject(midObject, ObjectDistanceType.Mid);
        }
        if (nearMidObjectSpawnTimer > nearMidObjectSpawnDelay)
        {
            nearMidObjectSpawnTimer = 0;
            SpawnObject(nearMidObject, ObjectDistanceType.NearMid);
        }
        if (nearObjectSpawnTimer > nearObjectSpawnDelay)
        {
            nearObjectSpawnTimer = 0;
            SpawnObject(nearObject, ObjectDistanceType.Near);
        }
        if (closeObjectSpawnTimer > closeObjectSpawnDelay)
        {
            closeObjectSpawnTimer = 0;
            SpawnObject(closeObject, ObjectDistanceType.Close);
        }
        if (groundObjectSpawnTimer > groundObjectSpawnDelay)
        {
            groundObjectSpawnTimer = 0;
            SpawnObject(groundObject, ObjectDistanceType.Ground);
        }
    }

    private void SpawnObject(GameObject prefeb, ObjectDistanceType objectDistanceType)
    {
        GameObject spawnObject = null;
        for (int i = 0; i < backgroundObject[objectDistanceType].Count; i++)
        {
            if (!backgroundObject[objectDistanceType][i].activeInHierarchy)
            {
                spawnObject = backgroundObject[objectDistanceType][i];
                spawnObject.SetActive(true);
                break;
            }
        }

        if (spawnObject == null)
        {
            spawnObject = Instantiate(prefeb);
            spawnObject.name = prefeb.name;
            spawnObject.transform.SetParent(transform);
            backgroundObject[objectDistanceType].Add(spawnObject);
        }

        spawnObject.transform.localScale = GetObjectTypeScale(objectDistanceType);
        spawnObject.transform.localPosition = new Vector3(0f, 0f, GetObjectTypeZPosition(objectDistanceType));
        spawnObject.GetComponent<ObjectMove>().Init();
    }

    public float GetObjectTypeSpeed(ObjectDistanceType objectDistanceType)
    {
        switch (objectDistanceType)
        {
            case ObjectDistanceType.DistantFar:
                return distantFarSpeed;
            case ObjectDistanceType.Far:
                return farSpeed;
            case ObjectDistanceType.Mid:
                return midSpeed;
            case ObjectDistanceType.NearMid:
                return nearMidSpeed;
            case ObjectDistanceType.Near:
                return nearSpeed;
            case ObjectDistanceType.Close:
                return closeSpeed;
            case ObjectDistanceType.Ground:
                return groundSpeed;
        }

        return 0;
    }

    public Vector3 GetObjectTypeScale(ObjectDistanceType objectDistanceType)
    {
        switch (objectDistanceType)
        {
            case ObjectDistanceType.DistantFar:
                return distantFarScale;
            case ObjectDistanceType.Far:
                return farScale;
            case ObjectDistanceType.Mid:
                return midScale;
            case ObjectDistanceType.NearMid:
                return nearMidScale;
            case ObjectDistanceType.Near:
                return nearScale;
            case ObjectDistanceType.Close:
                return closeScale;
            case ObjectDistanceType.Ground:
                return groundScale;
        }

        return Vector3.zero;
    }

    public float GetObjectTypeZPosition(ObjectDistanceType objectDistanceType)
    {
        switch (objectDistanceType)
        {
            case ObjectDistanceType.DistantFar:
                return distantFarZPosition;
            case ObjectDistanceType.Far:
                return farZPosition;
            case ObjectDistanceType.Mid:
                return midZPosition;
            case ObjectDistanceType.NearMid:
                return nearMidZPosition;
            case ObjectDistanceType.Near:
                return nearZPosition;
            case ObjectDistanceType.Close:
                return closeZPosition;
            case ObjectDistanceType.Ground:
                return groundZPosition;
        }

        return 0;
    }
}
