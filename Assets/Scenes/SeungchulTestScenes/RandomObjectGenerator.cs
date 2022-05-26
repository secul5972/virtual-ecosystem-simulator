using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomObjectGenerator : MonoBehaviour
{
	public PFinfo[] animalPrefabs;
	public PFinfo[] plantPrefabs;

	[Tooltip("size x of the map, x value")]
	public float mapWidth=180f;
	[Tooltip("size z of the map, z value")]
	public float mapLength = 180f;
	[Tooltip("maximum height of the map, y value")]
	public float mapMaxHeight = 100f;

	[Tooltip("toggle status bar")]
	public bool enableStatusBar = false;

	private List<Polyperfect.Common.Common_WanderScript> animalGameObjects = new List<Polyperfect.Common.Common_WanderScript>();
	private List<GameObject> plantGameObjects = new List<GameObject>();

	private int terrainLayer;

	public static RandomObjectGenerator instance = null;

	private void Awake()
	{
		mapWidth *= 0.95f;
		mapLength *= 0.95f;
		terrainLayer = LayerMask.GetMask("Terrain");
		instance = this;
	}

	public void Start()
	{
		StartCoroutine(GenerateObject());
#if ENABLE_RESPAWN
		StartCoroutine(RespawnAnimals());
		StartCoroutine(RespawnFood());
#endif
		StartCoroutine(ReproduceChild());
	}

	IEnumerator GenerateObject()
    {
		yield return new WaitForSeconds(1.0f);

		//식물 생성
		foreach (var plant in plantPrefabs) SpawnPlant(plant);
		
		//동물 생성
		foreach (var animal in animalPrefabs) SpawnAnimal(animal);

	}

	IEnumerator RespawnAnimals()
    {
		while(true)
        {
			foreach(var animal in animalGameObjects)
            {
				if (animal.enabled == false) 
					animal.enabled = true;
			}
			yield return new WaitForSeconds(5.0f);
        }
    }
	IEnumerator RespawnFood()
    {
		while(true)
        {
			Instantiate(plantPrefabs[0].prefab, GetRandomPosition(), Quaternion.identity, transform);
			yield return new WaitForSeconds(5.0f);
		}
    }

	IEnumerator ReproduceChild()
    {
		while(true)
        {
            //Instantiate(animalPrefabs[Random.Range(0, animalPrefabs.Length - 1)].prefab, GetRandomPosition(), Quaternion.Euler(0, Random.Range(0, 359f), 0), transform);
            Instantiate(animalPrefabs[0].prefab, GetRandomPosition(), Quaternion.Euler(0, Random.Range(0, 359f), 0), transform);

            yield return new WaitForSeconds(4f);
        }
    }

	public Vector3 GetRandomPosition()
	{
		Vector3 spawnPos = new Vector3(Random.Range(-mapWidth*0.5f, mapWidth*0.5f),
									   mapMaxHeight, 
									   Random.Range(-mapLength*0.5f, mapLength*0.5f));
		Ray ray = new Ray(spawnPos, Vector3.down); //월드 좌표로 변경해서 삽입.
		RaycastHit hitData;
		Physics.Raycast(ray, out hitData, 2*mapMaxHeight, terrainLayer); //현재 랜덤으로 정한 위치(Y축은 maxHeight)에서 땅으로 빛을 쏜다.
		spawnPos.y -= hitData.distance; //땅에 맞은 거리만큼 y에서 뺀다. 동물이 지형 바닥에 딱 맞게 스폰되게끔.
		return spawnPos;
	}

	private void SpawnAnimal(PFinfo animal)
	{

		int cnt = animal.count;
		GameObject animalPrefab = animal.prefab;

		if (enableStatusBar == true) //GetComponentInChildren<>() 은 cost 가 높다. 가독성이 안좋더라도 if 문으로 최적화. default 는 true.
		{
			while (cnt-- > 0)
			{
				//GameObject instance = Instantiate(animalPrefab, GetRandomPosition(), Quaternion.identity, transform);
				GameObject instance = Instantiate(animalPrefab, transform);
				animalGameObjects.Add(instance.GetComponent<Polyperfect.Common.Common_WanderScript>());
			}
		}
		else
        {
			while (cnt-- > 0)
			{
				//GameObject instance = Instantiate(animalPrefab, GetRandomPosition(), Quaternion.identity, transform);
				GameObject instance = Instantiate(animalPrefab, transform);
				instance.GetComponentInChildren<StatBarController>().gameObject.SetActive(false);
				animalGameObjects.Add(instance.GetComponent<Polyperfect.Common.Common_WanderScript>());
			}
		}
	}

	private void SpawnPlant(PFinfo plant)
    {
		int cnt = plant.count;
		GameObject plantPrefab = plant.prefab;

		while(cnt-->0)
        {
			GameObject instance = Instantiate(plantPrefab, GetRandomPosition(), Quaternion.identity, transform);
			plantGameObjects.Add(instance);
		}
    }

	[System.Serializable]
	public struct PFinfo
	{
		//게임 오브젝트
		public GameObject prefab;
		//게임 오브젝트 갯수
		public int count;
	}
}
