using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Session : NetworkBehaviour
{
    public SessionProps Props => new SessionProps(Runner.SessionInfo.Properties);
    public SessionInfo Info => Runner.SessionInfo;

    public Room Room { get; set; }

    private HashSet<PlayerRef> _finishedLoading = new HashSet<PlayerRef>();

    public override void Spawned()
    {
        NetworkManager.Instance.Session = this;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority, Channel = RpcChannel.Reliable)]
    public void RPC_FinishedLoading(PlayerRef playerRef)
    {
        _finishedLoading.Add(playerRef);
    }

    public void LoadRoom(string roomName)
    {
        _finishedLoading.Clear();

        foreach (Player player in NetworkManager.Instance.Players)
            player.InputEnabled = false;

        Runner.SetActiveScene(roomName);
    }
}
