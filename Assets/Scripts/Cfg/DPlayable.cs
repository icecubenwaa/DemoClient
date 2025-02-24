﻿using UnityEngine;
using System.Collections;
using System;
using System.Xml;

public class DPlayable : DObj<Int32>
{
    public Int32  Id;
    public string Path = string.Empty;

    public override Int32 GetKey()
    {
        return Id;
    }

    public override void Read(XmlElement element)
    {
        this.Id   = element.GetInt32("Id");
        this.Path = element.GetString("Path");
    }
}

public class ReadCfgPlayable : DReadBase<Int32, DPlayable>
{

}
