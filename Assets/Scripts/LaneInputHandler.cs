using System;
using UnityEngine;

[RequireComponent(typeof(Lane))]
public class LaneInputHandler : MonoBehaviour
{
    [SerializeField] private KeyCode inputKey;
    [SerializeField] private float mobileExtraInputWindow = 0.015f;
    private Lane lane;
    private Collider2D col;

    private void Awake()
    {
        lane = GetComponent<Lane>();
        col = GetComponent<Collider2D>();
    }

    private void Start()
    {
#if !UNITY_ANDROID && !UNITY_IOS
    mobileExtraInputWindow = 0f;
#endif
    }

    private void Update()
    {
        if (lane == null || !lane.HasTileToProcess) return;

        Tile currentTile = lane.CurrentTile;
        TileData tileData = lane.CurrentTileData;
        double timeStamp = tileData.timeStamp;
        double marginOfError = SongManager.Instance.MarginOfError;
        double audioTime = SongManager.Instance.GetSongPlaybackTime();

        if (currentTile.GetTileType() == TileType.Tap)
        {
            HandleTap(currentTile, timeStamp, marginOfError, audioTime, IsInputDown(), IsInputUp());
        }
        else if (currentTile.GetTileType() == TileType.Hold)
        {
            HandleHold(currentTile, timeStamp, marginOfError, audioTime, IsInputDown(), IsInputUp());
        }
    }

    private bool IsInputDown()
    {
        // Keyboard
        if (Input.GetKeyDown(inputKey)) return true;

        // Touch
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            if ((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && IsTouchOnLane(touch.position))
                return true;
        }
        return false;
    }

    private bool IsInputUp()
    {
        // Keyboard
        if (Input.GetKeyUp(inputKey)) return true;

        // Touch
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            if ((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && IsTouchOnLane(touch.position))
                return true;
        }

        return false;
    }

    private bool IsTouchOnLane(Vector2 screenPosition)
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPosition);
        return col.OverlapPoint(worldPos);
    }

    private void HandleTap(Tile currentTile, double timeStamp, double marginOfError, double audioTime, bool inputDown, bool inputUp)
    {
        if (inputDown)
        {
            double timingDifference = Math.Abs(audioTime - timeStamp) - mobileExtraInputWindow;
            if (timingDifference < marginOfError)
            {
                HitType hitType = timingDifference < marginOfError / 2f ? HitType.Perfect : HitType.Good;
                currentTile.DestroyTile(hitType);
                lane.AdvanceInputIndex();
                return;
            }
        }
        if (audioTime >= timeStamp + marginOfError)
        {
            currentTile.DestroyTile(HitType.Miss);
            lane.AdvanceInputIndex();
        }
    }

    private void HandleHold(Tile currentTile, double timeStamp, double marginOfError, double audioTime, bool inputDown, bool inputUp)
    {
        if (inputDown)
        {
            double timingDifference = Math.Abs(audioTime - timeStamp);
            if (timingDifference < marginOfError)
            {
                currentTile.StartHold();
            }
        }

        if (inputUp && currentTile.IsHolding)
        {
            currentTile.EndHold();
            lane.AdvanceInputIndex();
            return;
        }

        if (audioTime >= timeStamp + marginOfError && !currentTile.Held)
        {
            currentTile.DestroyTile(HitType.Miss);
            lane.AdvanceInputIndex();
        }
    }
}