using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Generator : MonoBehaviour
{
    [SerializeField] private GameObject _groundAssetReference;
    [SerializeField] private GameObject _cloudAssetReference;
    [SerializeField] private GameObject _heartAssetReference;
    [SerializeField] private GameObject _coinAssetReference;
    [SerializeField] private  float _cloudZDelta;
    [SerializeField] private  float _cloudsPerHeart;
    [SerializeField] private RectTransform _poolGO;

    private ObjectPool _cloudPool;
    private ObjectPool _groundPool;
    private ObjectPool _heartPool;
    private ObjectPool _coinPool;
    
    private float _nextCloudZ, _nextGroundZ;
    private  float _groundZDelta;
    private float _cloudXDelta;
    private bool _previousDoubleCloud; // were there two clouds on the same z coordinate
    private int _cloudsSinceLastHeart;
    private int _doubleCloudChanceCoeff = 5;
    private Transform _camera;
    
    public void Setup()
    {
        _camera = Camera.main.transform;

        _cloudPool = new ObjectPool(_cloudAssetReference, _poolGO);
        _groundPool = new ObjectPool(_groundAssetReference, _poolGO);
        _heartPool = new ObjectPool(_heartAssetReference, _poolGO);
        _coinPool = new ObjectPool(_coinAssetReference, _poolGO);
        
        Vector3 groundSize = _groundAssetReference.GetComponent<BoxCollider>().size;
        _groundZDelta = groundSize.z * 10; // plane is scaled by 10 across z axis
        _cloudXDelta = groundSize.x / 4;
        PrepareGame();
    }
    
    public void PrepareGame()
    {
        _nextCloudZ = 30;
        _nextGroundZ = 0;
        
        GenerateGround();
        GenerateGround();
        GenerateClouds();
    }
    
    public void StartGame()
    {
        InvokeRepeating(nameof(Generate), 0f, 1f);
    }

    public void EndGame()
    {
        CancelInvoke(nameof(Generate));
        ReturnAllObjectsToPool();
    }

    private void Generate()
    {
        _cloudPool.UpdatePool();
        _groundPool.UpdatePool();

        if (_camera.position.z + 300 > _nextGroundZ) GenerateGround();
        if (_camera.position.z + 200 > _nextCloudZ) GenerateClouds();
    }

    private void GenerateGround()
    {
        GameObject ground = _groundPool.GetObject();
        ground.transform.position = new Vector3(0, 0, _nextGroundZ);
        _nextGroundZ += _groundZDelta;
    }

    private void GenerateClouds()
    {
        for (int i = 0; i < 10; i++)
        {
            int firstXPosition = (int)Math.Round((float) Random.Range(-1, 2));
            GenerateOneCloud(firstXPosition);
            int secondXPosition = firstXPosition + Random.Range(1, 3);
            if (secondXPosition > 1) secondXPosition -= 3;
            if (_cloudsSinceLastHeart >= _cloudsPerHeart)
            {
                GenerateHeart(secondXPosition);
                _previousDoubleCloud = false;
            }
            else if (!_previousDoubleCloud && Random.Range(0, _doubleCloudChanceCoeff) == 0)
            {
                GenerateOneCloud(secondXPosition);
                _previousDoubleCloud = true;
            }
            else
            {
                _previousDoubleCloud = false;
            }

            int coinXPosition = (firstXPosition + secondXPosition) * -1;
            GenerateCoin(coinXPosition);
            
            _nextCloudZ += _cloudZDelta;
        }
    }

    private void GenerateOneCloud(float xPosition)
    {
        GameObject cloud = _cloudPool.GetObject();
        cloud.transform.position = new Vector3(xPosition * _cloudXDelta, 0f, _nextCloudZ);
        _cloudsSinceLastHeart++;
    }
    
    private void GenerateHeart(int xPosition)
    {
        GameObject heart = _heartPool.GetObject();
        heart.transform.position = new Vector3(xPosition * _cloudXDelta, 0, _nextCloudZ);
        _cloudsSinceLastHeart = 0;
    }
    
    private void GenerateCoin(int xPosition)
    {
        GameObject coin = _coinPool.GetObject();
        coin.transform.position = new Vector3(xPosition * _cloudXDelta, 0, _nextCloudZ);
    }

    private void ReturnAllObjectsToPool()
    {
        _cloudPool.ReturnAll();
        _groundPool.ReturnAll();
        _heartPool.ReturnAll();
        _coinPool.ReturnAll();
    }
    
    private class ObjectPool
    {
        private Transform _camera;
        private Queue<GameObject> _availableObjects = new Queue<GameObject>();
        private List<GameObject> _takenObjects = new List<GameObject>();
        private GameObject _gameObjectReference;
        private RectTransform _pool;
        
        public ObjectPool(GameObject gameObjectReference, RectTransform pool)
        {
            _camera = Camera.main.transform;
            _gameObjectReference = gameObjectReference;
            _pool = pool;
        }
        
        public GameObject GetObject()
        {
            GameObject go = _availableObjects.Count > 0 ? _availableObjects.Dequeue() : Instantiate(_gameObjectReference, _pool);
            _takenObjects.Add(go);
            go.SetActive(true);
            
            return go;
        }

        private void ReturnObject(GameObject go)
        {
            _takenObjects.Remove(go);
            go.SetActive(false);
            _availableObjects.Enqueue(go);
        }

        public void UpdatePool()
        {
            List<GameObject> objectsToReturn = new List<GameObject>();
            _takenObjects.ForEach(go =>
            {
                if(ShouldReturnObject(go)) objectsToReturn.Add(go);
            });
            objectsToReturn.ForEach(ReturnObject);
        }
        
        private bool ShouldReturnObject(GameObject go)
        {
            return go.transform.position.z  + 100 < _camera.position.z;
        }

        public void ReturnAll()
        {
            if (_takenObjects.Count <= 0) return;
            
            foreach (GameObject go in _takenObjects)
            {
                go.SetActive(false);
                _availableObjects.Enqueue(go);
            }
            _takenObjects = new List<GameObject>();
        }
    }
}