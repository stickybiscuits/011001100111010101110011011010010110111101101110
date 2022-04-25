using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : SimulationBehaviour, ISpawned
{
    public void Spawned()
    {
        NetworkManager.Instance.Session.Room = this;
    }

    public void SpawnAvatar(Player player, bool lateJoiner)
    {

    }

    public void DespawnAvatar(Player player)
    {

    }

    public override void FixedUpdateNetwork()
    {

    }

    public void OnDisconnect()
    {
        NetworkManager.Instance.Disconnect();
    }
}
