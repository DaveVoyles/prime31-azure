using UnityEngine;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;


/// <summary>
/// Simple class for testing the Azure web services
/// </summary>
public class TodoItem
{
	public string Id       { get; set; }
	public string UserName { get; set; }
	public int    Score    { get; set; }
	
	
    //public TodoItem()
    //{}
	
	public TodoItem( string text, int score  )
	{
		this.UserName = text;
		this.Score    = score;
	}
	
	
	public override string ToString()
	{
		return "[TodoItem] text: " + UserName + ", score: " + Score + "id:" + Id;
	}

}
