using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpatialPartitionPattern
{
    public class GameController : MonoBehaviour
    {
        public GameObject friendlyObj;
        public GameObject enemyObj;

        // change materials to detect which enemy is closes
        public Material enemyMaterial;
        public Material closestEnemyMaterial;

        //To get a cleaner workspace, parent all soldiers to these empty gameobjects
        public Transform enemyParent;
        public Transform friendlyParent;

        //Store all soldiers in these lists
        List<Soldier> enemySoldiers = new List<Soldier>();
        List<Soldier> friendlySoldiers = new List<Soldier>();

        //Save the closest enemies to easier change back its material
        List<Soldier> closestEnemies = new List<Soldier>();

        //Grid data
        float mapWidth = 50f;
        int cellSize = 10;

        public bool useSpatialParition = false;

        //Number of soldiers on each team
        public int numberOfSoldiers = 0;

        //The Spatial Partition grid
        Grid grid;

        // TextMeshPro
        public TMP_Text timer;

        void Start()
        {
            //Create a new grid
            grid = new Grid((int)mapWidth, cellSize);

            //Add random enemies and friendly and store them in a list
            for (int i = 0; i < numberOfSoldiers; i++)
            {
                //Give the enemy a random position
                Vector3 randomPos = new Vector3(Random.Range(0f, mapWidth), Random.Range(0.5f, mapWidth), Random.Range(0f, mapWidth));

                //Create a new enemy
                GameObject newEnemy = Instantiate(enemyObj, randomPos, Quaternion.identity) as GameObject;

                //Add the enemy to a list
                enemySoldiers.Add(new Enemy(newEnemy, mapWidth, grid));

                //Parent it
                newEnemy.transform.parent = enemyParent;


                //Give the friendly a random position
                randomPos = new Vector3(Random.Range(0f, mapWidth), Random.Range(0.5f, mapWidth), Random.Range(0f, mapWidth));

                //Create a new friendly
                GameObject newFriendly = Instantiate(friendlyObj, randomPos, Quaternion.identity) as GameObject;

                //Add the friendly to a list
                friendlySoldiers.Add(new Friendly(newFriendly, mapWidth));

                //Parent it 
                newFriendly.transform.parent = friendlyParent;
            }
        }

        void Update()
        {
            // measuring performance
            float startTime = Time.realtimeSinceStartup ;

            //Move the enemies
            for (int i = 0; i < enemySoldiers.Count; i++)
            {
                enemySoldiers[i].Move();
            }

            //Reset material of the closest enemies
            for (int i = 0; i < closestEnemies.Count; i++)
            {
                closestEnemies[i].soldierMeshRenderer.material = enemyMaterial;
            }

            //Reset the list with closest enemies
            closestEnemies.Clear();
            Soldier closestEnemy;

            //For each friendly, find the closest enemy and change its color and chase it
            for (int i = 0; i < friendlySoldiers.Count; i++)
            {
                if (useSpatialParition)
                {
                    closestEnemy = grid.FindClosestEnemy(friendlySoldiers[i]);
                }
                else
                {
                    closestEnemy = FindClosestEnemySlow(friendlySoldiers[i]);
                }

                //if we found an enemy
                if (closestEnemy != null)
                {
                    //change material
                    closestEnemy.soldierMeshRenderer.material = closestEnemyMaterial;

                    closestEnemies.Add(closestEnemy);

                    //move the friendly in the direction of the enemy
                    friendlySoldiers[i].Move(closestEnemy);
                }
            }

            float elapsedTime = (Time.realtimeSinceStartup - startTime) * 1000f;
            Debug.Log(elapsedTime + "ms");
            timer.SetText(elapsedTime + "ms");
        }

        //Find the closest enemy - slow version
        Soldier FindClosestEnemySlow(Soldier soldier)
        {
            Soldier closestEnemy = null;

            float bestDistSqr = Mathf.Infinity;

            //Loop thorugh all enemies
            for (int i = 0; i < enemySoldiers.Count; i++)
            {
                //The distance sqr between the soldier and this enemy
                float distSqr = (soldier.soldierTrans.position - enemySoldiers[i].soldierTrans.position).sqrMagnitude;

                //If this distance is better than the previous best distance, then we have found an enemy that's closer
                if (distSqr < bestDistSqr)
                {
                    bestDistSqr = distSqr;

                    closestEnemy = enemySoldiers[i];
                }
            }

            return closestEnemy;
        }

        public void ToggleSP()
        {
            useSpatialParition = !useSpatialParition;
        }

        public void AddSoldiers()
        {
            numberOfSoldiers += 100;
            for (int i = 0; i < numberOfSoldiers; i++)
            {
                //Give the enemy a random position
                Vector3 randomPos = new Vector3(Random.Range(0f, mapWidth), Random.Range(0.5f, mapWidth), Random.Range(0f, mapWidth));

                //create a new enemy
                GameObject newEnemy = Instantiate(enemyObj, randomPos, Quaternion.identity) as GameObject;

                //add the enmy to a list
                enemySoldiers.Add(new Enemy(newEnemy, mapWidth, grid));

                //parent it
                newEnemy.transform.parent = enemyParent;

                //give the friendly a random position
                randomPos = new Vector3(Random.Range(0f, mapWidth), 0.5f, Random.Range(0f, mapWidth));

                //Create a new friendly
                GameObject newFriendly = Instantiate(friendlyObj, randomPos, Quaternion.identity) as GameObject;

                //Add the friendly to a list
                friendlySoldiers.Add(new Friendly(newFriendly, mapWidth));

                //Parent it
                newFriendly.transform.parent = friendlyParent;
            }
        }
    }
}
