using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphStructure : MonoBehaviour {

	public static GraphStructure singleton = null;

	public GameObject StartNode;
	private Node StartState = null;
	private int X_Matrix;
	private int Y_Matrix;
	public int X_Distance;
	public int Y_Distance;

   

    Node[,] MATRIX;

	void Awake(){
		singleton = this;
		X_Matrix = AllData.X_Matrix;
		Y_Matrix = AllData.Y_Matrix;
	}

	void Start () {
		//Richiamo l'agente per settargli lo stato iniziale.
		//Agent.getSingleton ().InitializeAgent (StartState);
		Table_Q_UI.singleton.DrawTable ();
		//DrawMatrix ();
	}
	
	void Update () {
		//DrawMatrix ();
	}

    public void CreateStructure()
    {
        singleton = this;
        X_Matrix = AllData.X_Matrix;
        Y_Matrix = AllData.Y_Matrix;

        CreateMatrix();

        JoinNeighbors();
    }

    public Node getStartState()
    {
        return this.StartState;
    }


	public Node[,] getMatrix(){
		return this.MATRIX;
	}
	public int getLenghtMatrix_X(){
		return this.X_Matrix;
	}
	public int getLenghtMatrix_Y(){
		return this.Y_Matrix;
	}

	//Metodo per creare la matrice di Nodi.
	private void CreateMatrix(){
		Vector3 Position = StartNode.transform.position;
		int ID_NODE = 0;
		MATRIX = new Node[X_Matrix,Y_Matrix];

		for(int i = 0; i< X_Matrix; i++){
			for(int j = 0; j < Y_Matrix; j++){

				//Calcolo la posizione del prossimo Nodo della Matrice.
				Position = new Vector3 (StartNode.transform.position.x + (X_Distance * i),0f,StartNode.transform.position.z + (Y_Distance * j));


				GameObject gm = Instantiate (Resources.Load("NPC/Helper/TagNode"),Position,Quaternion.identity) as GameObject;
				gm.name = ""+ID_NODE;
				gm.transform.parent = GameObject.Find ("Grid").transform;

				//Inizializzo il nodo: id, posizione, reward, visited.
				MATRIX[i,j] = new Node(ID_NODE,Position,0,false,gm);
				ID_NODE++;



				//Per il momento settiamo lo stato iniziale a mano, mettendolo a Tile_1_0;
				if (gm.name == "1") {
					StartState = MATRIX [i, j];
				}
				
				StartCoroutine (setWalkableNode(MATRIX[i,j],gm));
			}
		}
	}

	//Questa coroutine serve per identificare se il punto è percorribile o meno.
	private IEnumerator setWalkableNode(Node node, GameObject gm){
		yield return new WaitForSeconds (0.001f);
		bool walkable = gm.GetComponent<PhysicNode> ().isWalkable ();
		//Debug.Log (gm.name+ " Walkable : "+walkable);
		node.setWalkable (walkable);

		//Assegno i reward ai vari nodi (stati), 1 se non sbatto contro nulla, -5 se sbatto contro un muro.
		if (walkable)
			node.setReward (Agent.getSingleton().POSITIVE_REWARD);
		else
			node.setReward (Agent.getSingleton().NEGATIVE_REWARD);
		//Goal state, inizializzato a mano per il momento.
		if (gm.name == Agent.getSingleton().GOAL_POSITION.ToString())
			node.setReward (Agent.getSingleton().GOAL_REWARD);
	}

	//Metodo per "unire i vicini", tesse la maglia della struttura.
	private void JoinNeighbors(){
		//Il primo ciclo scorre tutte le righe della matrice.
		for(int i = 0; i< X_Matrix; i++){
			//Il secondo ciclo scorre tutte le colonne della matrice.
			for (int j = 0; j < Y_Matrix; j++) {

				//TOP
				if ((j < Y_Matrix - 1)) {
					MATRIX [i, j].setNeighbor (AllData.NEIGHBOR_POSITION.TOP, MATRIX [i, j + 1]);
				} else {
					MATRIX [i, j].setNeighbor (AllData.NEIGHBOR_POSITION.TOP, null);
				}

				//BOTTOM
				if ((j > 0)) {
					MATRIX [i, j].setNeighbor (AllData.NEIGHBOR_POSITION.BOTTOM, MATRIX [i, j - 1]);
				} else {
					MATRIX [i, j].setNeighbor (AllData.NEIGHBOR_POSITION.BOTTOM, null);
				}

				//LEFT
				if ((i > 0)) {
					MATRIX [i, j].setNeighbor (AllData.NEIGHBOR_POSITION.LEFT, MATRIX [i - 1, j]);
				} else {
					MATRIX [i, j].setNeighbor (AllData.NEIGHBOR_POSITION.LEFT, null);
				}

				//RIGHT
				if ((i < X_Matrix - 1)) {
					MATRIX [i, j].setNeighbor (AllData.NEIGHBOR_POSITION.RIGHT, MATRIX [i + 1, j]);
				} else {
					MATRIX [i, j].setNeighbor (AllData.NEIGHBOR_POSITION.RIGHT, null);
				}
			}			
		}		
	}

	//Metodo per disegnare tutte i possibili collegamenti tra i nodi della matrice (tra uno stato e un altro).
	private void DrawMatrix(){
		Color AuxColor;

		for(int i = 0; i < X_Matrix; i++){
			for (int j = 0; j < Y_Matrix; j++) {

				if (MATRIX [i, j].isWalkable ()) {
					if (MATRIX [i, j].getNeighbor (AllData.NEIGHBOR_POSITION.TOP) != null) {
						if (MATRIX [i, j].getNeighbor (AllData.NEIGHBOR_POSITION.TOP).isWalkable ()) {
							//Debug.Log (TileGraph [i, j].getAssociatedGameObject().name+ " Walkable : "+TileGraph[i,j].isWalkable());
							AuxColor = Color.white;
						} else {
							AuxColor = Color.red;
						}


						Debug.DrawRay (MATRIX [i, j].getAssociatedGameObject ().transform.position,
							MATRIX [i, j].getNeighbor (AllData.NEIGHBOR_POSITION.TOP).getAssociatedGameObject ().transform.position - MATRIX [i, j].getAssociatedGameObject ().transform.position,
							AuxColor,
							30f,
							false);
					}

					if (MATRIX [i, j].getNeighbor (AllData.NEIGHBOR_POSITION.RIGHT) != null) {

						if (MATRIX [i, j].getNeighbor (AllData.NEIGHBOR_POSITION.RIGHT).isWalkable ()) {
							//Debug.Log (TileGraph [i, j].getAssociatedGameObject().name+ " Walkable : "+TileGraph[i,j].isWalkable());
							AuxColor = Color.white;
						} else {
							AuxColor = Color.red;
						}

						Debug.DrawRay (MATRIX [i, j].getAssociatedGameObject ().transform.position,
							MATRIX [i, j].getNeighbor (AllData.NEIGHBOR_POSITION.RIGHT).getAssociatedGameObject ().transform.position - MATRIX [i, j].getAssociatedGameObject ().transform.position,
							AuxColor,
							30f,
							false);
					}
						
				} else {	
					AuxColor = Color.red;

					if (MATRIX [i, j].getNeighbor (AllData.NEIGHBOR_POSITION.TOP) != null) {

						Debug.DrawRay (MATRIX [i, j].getAssociatedGameObject ().transform.position,
							MATRIX [i, j].getNeighbor (AllData.NEIGHBOR_POSITION.TOP).getAssociatedGameObject ().transform.position - MATRIX [i, j].getAssociatedGameObject ().transform.position,
							AuxColor,
							30f,
							false);
					}

					if (MATRIX [i, j].getNeighbor (AllData.NEIGHBOR_POSITION.RIGHT) != null) {

						Debug.DrawRay (MATRIX [i, j].getAssociatedGameObject ().transform.position,
							MATRIX [i, j].getNeighbor (AllData.NEIGHBOR_POSITION.RIGHT).getAssociatedGameObject ().transform.position - MATRIX [i, j].getAssociatedGameObject ().transform.position,
							AuxColor,
							30f,
							false);
					}	
				}
			}
		}			
	}
		
}
