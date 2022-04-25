using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [Networked] public string Name { get; set; }
    [Networked] public NetworkBool Ready { get; set; }
    [Networked] public NetworkBool InputEnabled { get; set; }

    public override void Spawned()
    {
        NetworkManager.Instance.SetPlayer(Object.InputAuthority, this);
    }

	[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
	public void RPC_SetIsReady(NetworkBool ready)
	{
		Ready = ready;
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
	public void RPC_SetName(string name)
	{
		Name = name;
	}
}
