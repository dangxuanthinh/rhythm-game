using System;
using UnityEngine;

[RequireComponent(typeof(Lane))]
public class LaneInputHandler : MonoBehaviour
{
    [SerializeField] private KeyCode inputKey;
    private Lane lane;

    private void Awake()
    {
        lane = GetComponent<Lane>();
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
            HandleTap(currentTile, timeStamp, marginOfError, audioTime);
        }
        else if (currentTile.GetTileType() == TileType.Hold)
        {
            HandleHold(currentTile, timeStamp, marginOfError, audioTime);
        }
    }

    private void HandleTap(Tile currentTile, double timeStamp, double marginOfError, double audioTime)
    {
        if (Input.GetKeyDown(inputKey))
        {
            double timingDifference = Math.Abs(audioTime - timeStamp);
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

    private void HandleHold(Tile currentTile, double timeStamp, double marginOfError, double audioTime)
    {
        if (Input.GetKeyDown(inputKey))
        {
            double timingDifference = Math.Abs(audioTime - timeStamp);
            if (timingDifference < marginOfError)
            {
                currentTile.StartHold();
            }
        }

        if (Input.GetKeyUp(inputKey) && currentTile.IsHolding)
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