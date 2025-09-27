using System;
using UnityEngine;

/// <summary>
/// Use this interface for saving data.
/// Both classes and structs can implement it.
/// </summary>
public interface ISerializabelDatas
{
    public string Name { get; set; }
}


[Serializable]
public struct DataStructs : ISerializabelDatas //caution : the data structs' first field must be string, don't use this : it exist for test
{
    string ISerializabelDatas.Name { get => _name; set => _name = value; }
    public string _name;
    public string Description;
    public int integer;
    public float floater;
}


