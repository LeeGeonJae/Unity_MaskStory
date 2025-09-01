using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public enum EObjectDistanceType
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

    Dictionary<EObjectDistanceType, List<GameObject>> backgroundObject = new Dictionary<EObjectDistanceType, List<GameObject>>();

    float distantFarObjectSpawnTimer = 0;
    float farObjectSpawnTimer = 0;
    float midObjectSpawnTimer = 0;
    float nearMidObjectSpawnTimer = 0;
    float nearObjectSpawnTimer = 0;
    float closeObjectSpawnTimer = 0;
    float groundObjectSpawnTimer = 0;

    void Start()
    {
        for (int i = 0; i < (int)EObjectDistanceType.End; i++)
        {
            backgroundObject[(EObjectDistanceType)i] = new List<GameObject>();
        }

        groundObjectSpawnTimer = groundObjectSpawnDelay;
    }

    void Update()
    {
        if (GameManager.instance.currentGameState != EGameState.MoveNextStep)
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
            SpawnObject(distantFarObject, EObjectDistanceType.DistantFar);
        }
        if (farObjectSpawnTimer > farObjectSpawnDelay)
        {
            farObjectSpawnTimer = 0;
            SpawnObject(farObject, EObjectDistanceType.Far);
        }
        if (midObjectSpawnTimer > midObjectSpawnDelay)
        {
            midObjectSpawnTimer = 0;
            SpawnObject(midObject, EObjectDistanceType.Mid);
        }
        if (nearMidObjectSpawnTimer > nearMidObjectSpawnDelay)
        {
            nearMidObjectSpawnTimer = 0;
            SpawnObject(nearMidObject, EObjectDistanceType.NearMid);
        }
        if (nearObjectSpawnTimer > nearObjectSpawnDelay)
        {
            nearObjectSpawnTimer = 0;
            SpawnObject(nearObject, EObjectDistanceType.Near);
        }
        if (closeObjectSpawnTimer > closeObjectSpawnDelay)
        {
            closeObjectSpawnTimer = 0;
            SpawnObject(closeObject, EObjectDistanceType.Close);
        }
        if (groundObjectSpawnTimer > groundObjectSpawnDelay)
        {
            groundObjectSpawnTimer = 0;
            SpawnObject(groundObject, EObjectDistanceType.Ground);
        }
    }

    private void SpawnObject(GameObject prefeb, EObjectDistanceType objectDistanceType)
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

    public float GetObjectTypeSpeed(EObjectDistanceType objectDistanceType)
    {
        switch (objectDistanceType)
        {
            case EObjectDistanceType.DistantFar:
                return distantFarSpeed;
            case EObjectDistanceType.Far:
                return farSpeed;
            case EObjectDistanceType.Mid:
                return midSpeed;
            case EObjectDistanceType.NearMid:
                return nearMidSpeed;
            case EObjectDistanceType.Near:
                return nearSpeed;
            case EObjectDistanceType.Close:
                return closeSpeed;
            case EObjectDistanceType.Ground:
                return groundSpeed;
        }

        return 0;
    }

    public Vector3 GetObjectTypeScale(EObjectDistanceType objectDistanceType)
    {
        switch (objectDistanceType)
        {
            case EObjectDistanceType.DistantFar:
                return distantFarScale;
            case EObjectDistanceType.Far:
                return farScale;
            case EObjectDistanceType.Mid:
                return midScale;
            case EObjectDistanceType.NearMid:
                return nearMidScale;
            case EObjectDistanceType.Near:
                return nearScale;
            case EObjectDistanceType.Close:
                return closeScale;
            case EObjectDistanceType.Ground:
                return groundScale;
        }

        return Vector3.zero;
    }

    public float GetObjectTypeZPosition(EObjectDistanceType objectDistanceType)
    {
        switch (objectDistanceType)
        {
            case EObjectDistanceType.DistantFar:
                return distantFarZPosition;
            case EObjectDistanceType.Far:
                return farZPosition;
            case EObjectDistanceType.Mid:
                return midZPosition;
            case EObjectDistanceType.NearMid:
                return nearMidZPosition;
            case EObjectDistanceType.Near:
                return nearZPosition;
            case EObjectDistanceType.Close:
                return closeZPosition;
            case EObjectDistanceType.Ground:
                return groundZPosition;
        }

        return 0;
    }
}
