using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class FoodPopulationSystem : MonoBehaviour
{
    public Transform foodLocationParent;

    [Header("Normal Food Settings")]
    [Space(5)]
    public GameObject normalHealthyFood;
    public GameObject normalInfectedFood;

    [Space(5)]
    public Transform[] normalFoodLocations;

    [Space(5)]
    public int normalInfectedFoodCount;
    public int NormalInfectedFoodCount
    {
        get { return normalInfectedFoodCount; }
        set { normalInfectedFoodCount = value; }
    }
    public int normalHealthyFoodCount;
    public int totalNormalFoodCount = 0;

    //[Header("PowerUp Food Settings")]
    //[Space(5)]
    //public GameObject healthyPowerUpFood;
    //public GameObject infectedPowerUpFood;

    //[Space(5)]
    //public Transform[] powerUpFoodLocations;
    //[Space(5)]
    //public int powerupInfectedFoodCount = 1;
    //public int powerupHealthyFoodCount = 3;
    //public int powerupTotalFoodCount = 0;

    private int positionTotal;

    private void Awake()
    {

        positionTotal = normalFoodLocations.Length - 1;

        totalNormalFoodCount = normalFoodLocations.Length;
        normalHealthyFoodCount = totalNormalFoodCount - normalInfectedFoodCount;

        //powerupTotalFoodCount = powerUpFoodLocations.Length;

        PopulateFoodList(normalFoodLocations, totalNormalFoodCount, normalHealthyFoodCount, normalInfectedFoodCount, foodLocationParent, normalHealthyFood, normalInfectedFood);
        //PopulateFoodList(powerUpFoodLocations, powerupTotalFoodCount, powerupHealthyFoodCount, powerupInfectedFoodCount, foodLocationParent, healthyPowerUpFood, infectedPowerUpFood);

    }

    public static void Shuffle<T>(T[] array)
    {
        System.Random rng = new System.Random();
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            T temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }

    void PopulateFoodList(Transform[] locations, int totalNumber, int infectedCount, int healthyCount, Transform locationParent, GameObject heathyFood, GameObject infectedFood)
    {
        Shuffle(locations);

        for (int i = 0; i < totalNumber; i++)
        {

            GameObject food;
            //added GameObject to Instantiate method to fix error, it is a clone of the healthyFood prefab
            if (i < infectedCount)
            {
                food = Instantiate(heathyFood, locations[i].position, infectedFood.transform.rotation);
            }
            else
            {
                food = Instantiate(infectedFood, locations[i].position, infectedFood.transform.rotation);
            }
            food.transform.SetParent(locationParent);

            // Fix: assign minimap camera to the canvas
            Canvas canvas = food.GetComponentInChildren<Canvas>();
            if (canvas != null)
            {
                Camera minimapCam = GameObject.Find("MiniMapCamera")?.GetComponent<Camera>();
                if (minimapCam != null)
                {
                    canvas.worldCamera = minimapCam;
                }
            }

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        if (normalFoodLocations == null )   //|| powerUpFoodLocations == null)
        {
            return;
        }
        foreach (Transform location in normalFoodLocations)
        {
            Gizmos.DrawWireSphere(location.position, 0.5f); // Draw a sphere at each food location
        }

        //foreach (Transform location in powerUpFoodLocations)
        //{
        //    Gizmos.DrawWireSphere(location.position, 1f); // Draw a sphere at each food location
        //}
    }

}
