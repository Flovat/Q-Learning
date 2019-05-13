using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {

	/*Questa classe gestirà tutte le informazioni relative ad un nodo del grafo.*/
	private int ID;
	private Vector3 Position;
	private float Reward;
	private bool Walkable;
	private bool Visited;
	private Dictionary<AllData.AGENT_ACTION,int> Actions;
	private GameObject AssociatedGameObject;

	//Ogni nodo conosce i suoi vicini.
	//Tutti i vicini sono salvati in questo Dizionario che ha per chiave il tipo di vicino del vicino e per valore l'oggetto di tipo Nodo.
	Dictionary<AllData.NEIGHBOR_POSITION,Node> Neighbors = null;

	public Node(int id, Vector3 pos, int reward, bool v,GameObject gm){
		this.ID = id;
		this.Position = pos;
		this.Reward = reward;
		this.Visited = v;
		this.AssociatedGameObject = gm;
		InitializeActions ();
		Neighbors = new Dictionary<AllData.NEIGHBOR_POSITION,Node> ();
	}

	private void InitializeActions(){
		this.Actions = new Dictionary<AllData.AGENT_ACTION,int>();
		this.Actions [AllData.AGENT_ACTION.MOVE_TOP] = 0;
		this.Actions [AllData.AGENT_ACTION.MOVE_BOTTOM] = 0;
		this.Actions [AllData.AGENT_ACTION.MOVE_LEFT] = 0;
		this.Actions [AllData.AGENT_ACTION.MOVE_RIGHT] = 0;
		this.Actions [AllData.AGENT_ACTION.IDLE] = 0;
	}

	public int getID(){
		return this.ID;
	}
	public Vector3 getPosition(){
		return this.Position;
	}
	public float getReward(){
		return this.Reward;
	}
	public void setReward(float r){
		this.Reward = r;
	}
	public bool isVisited(){
		return this.Visited;
	}
	public bool isWalkable(){
		return this.Walkable;
	}
	public void setWalkable(bool walk){
		this.Walkable = walk;
	}

	public GameObject getAssociatedGameObject(){
		return this.AssociatedGameObject;
	}

	public void setNeighbor(AllData.NEIGHBOR_POSITION ID_pos,Node node){
		this.Neighbors [ID_pos] = node;
	}

	public Node getNeighbor(AllData.NEIGHBOR_POSITION ID_pos){
		if(this.Neighbors [ID_pos] != null)
			return this.Neighbors [ID_pos];
		return null;
	}

	public int getRewardFromAction(AllData.AGENT_ACTION action){
		return this.Actions [action];
	}

	public void setRewardToSpecificAction(AllData.AGENT_ACTION action, int reward){
		this.Actions [action] = reward;
	}

}
