using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityFetcher : MonoBehaviour
{
    private const string PLAYER_TAG = "Player";

    public static EntityFetcher Instance { get; private set; }
    public static int PlayerLayer { get; private set; }

	public static int BounceLayer {  get; private set; }

	public GameObject Player {  get; private set; }

    public Camera MainCamera { get; private set; }

    public CameraActions CameraActionsRef { get; private set; }


    private void Awake()
    {
        this.MainCamera = Camera.main;
        this.CameraActionsRef = this.MainCamera.GetComponent<CameraActions>();
        this.Player = GameObject.FindGameObjectWithTag(PLAYER_TAG);
        PlayerLayer = 1 << this.Player.layer;
        BounceLayer = 1 << LayerMask.NameToLayer("Bounceable");
        Instance = this;
    }
}
