﻿using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 变身
/// </summary>
public class CommandVariation : Command
{
    public float LastTime;
    public Int32 VariModelID;

    public override Resp Do()
    {
        CmdHandler<CommandVariation> call = Del as CmdHandler<CommandVariation>;
        return call == null ? Resp.TYPE_NO : call(this);
    }

    public CommandVariation Update(float lastTime, int variModelID)
    {
        this.LastTime      = lastTime;
        this.VariModelID   = variModelID;
        return this;
    }
}
