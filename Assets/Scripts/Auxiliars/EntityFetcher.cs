using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityFetcher : MonoBehaviour
{
    private const string PLAYER_TAG = "Player";

    public static EntityFetcher Instance { get; private set; }

    public GameObject Player {  get; private set; }

    public Camera MainCamera { get; private set; }

    private void Awake()
    {
        this.MainCamera = Camera.main;
        this.Player = GameObject.FindGameObjectWithTag(PLAYER_TAG);
        Instance = this;
    }
}
