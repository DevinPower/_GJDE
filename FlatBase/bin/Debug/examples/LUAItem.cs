using UnityEngine;
using System.Collections;
using LuaFramework;
using MoonSharp.Interpreter;

public class LUAItem : ItemParent
{
	public static ArrayList itemList = new ArrayList();

	public string OnUse{get;set;}
	public string OnDeath{get;set;}

	public string name {get;set;}
	public string description{get;set;}
	public int price{get;set;}
	public bool stacks{get;set;}
	public itemTypes itemType;
	public string sprite{get;set;}


	public static void defineLUAItem(DynValue luaTable)
	{
		itemList.Add( LuaReader.Read<LUAItem>(luaTable) );
	}
}