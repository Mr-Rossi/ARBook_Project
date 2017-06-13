using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct EbookMetaData {
    public PageMetaData[] Pages;
	

}
[Serializable]
public struct PageMetaData
{
    public ElementMetaData TopElement;
    public ElementMetaData BottomElement;
    public ElementMetaData SideElement;
}
[Serializable]
public struct ElementMetaData
{
    public string Url;
    public ElementType Type;
}
[Serializable]
public enum ElementType
{
    IMAGE,
    AUDIO,
    VIDEO,
    TEXT,
    ASSET,
    NONE
}