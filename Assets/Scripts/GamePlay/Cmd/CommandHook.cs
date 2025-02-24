﻿using UnityEngine;
using System.Collections;

/// <summary>
/// 被勾取
/// </summary>
public class CommandHook : Command
{
    public float LastTime;

    public override Resp Do()
    {
        CmdHandler<CommandHook> call = Del as CmdHandler<CommandHook>;
        return call == null ? Resp.TYPE_NO : call(this);
    }

    public CommandHook Update(float lastTime)
    {
        this.LastTime = lastTime;
        return this;
    }
}
