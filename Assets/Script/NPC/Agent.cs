using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System;
using UnityEngine.Windows;

public class Agent : MonoBehaviour {
    /*This class will manage our Agent's intelligence in the maze.*/

    public int NumberOfEpisodes;
	private int CountEpisodes = 1;
	private float TimeEpisode = 0.0f;
	private int StepsEpisode = 0;

    StreamWriter writer;
    string filePath;

    public float DecisionTime;

	private static Agent singleton = null;

	//Tabella Q
	private float[,] Q_Table;
	private int NumerOfState;

	//Lista di stati che l'agente ha in memoria.
	private Dictionary<int,Node> Q_Agent = null;
	private Node CurrentState = null;
    //private Node FutureCurrentState = null;
    private Node StartState = null;
    private Node FutureNextState = null;
    private AllData.AGENT_ACTION CurrentAction = AllData.AGENT_ACTION.IDLE;
    private AllData.AGENT_ACTION NextAction = AllData.AGENT_ACTION.IDLE;
    private AllData.AGENT_ACTION NextNextAction = AllData.AGENT_ACTION.IDLE;

    //Costante , indica quanto la variazione dei Q futuri incida sul q corrente.
	private float Alpha;
	//Costante, indica quanto "ridimensionare" la variazione dei q futuri.
	private float Gamma;
    private float epsilon = 0.1f;

    public bool QLearning;

    //rewards
    public float NEGATIVE_REWARD;
    public float POSITIVE_REWARD;
    public float GOAL_REWARD;

    //goal position
    public int GOAL_POSITION = 46;


    //Thread dell'agente
    IEnumerator Agent_Thread = null;

    public static Agent getSingleton(){
		return singleton;
	}

	void Awake(){
		singleton = this;
	}

	void Start () {}


    public int getNumberEpisodes(){
		return this.CountEpisodes;
	}

	//Inizializzo la matrice Q, che associa le azioni possibili agli stati che l'agente può raggiungere.
	//Assegno ad ogni coppia stato azione il valore 0.
	//Gli stati sono identificati dalle righe, i.
	//Le azioni sono identificate dalle colonne, j == 0 -> UP, j == 1 -> DOWN, j == 2 -> LEFT, j == 3 -> RIGHT.
	private void Initialize_Q_Table(){
		this.NumerOfState = AllData.X_Matrix * AllData.Y_Matrix;
		Q_Table = new float[this.NumerOfState,AllData.NUMBER_OF_ACTIONS];
		for (int i = 0; i < this.NumerOfState; i++) {
			for (int j = 0; j < AllData.NUMBER_OF_ACTIONS; j++) {
				Q_Table [i, j] = 0f;
			}
		}
		//Notifico alla vista.
		Table_Q_UI.singleton.FillTable(Q_Table);
		Debug.Log ("Q_Initialize, ID start state : "+ this.CurrentState.getID() + " start action : " + this.CurrentAction);

        //Start Core Move
        Agent_Thread = Walk();
        StartCoroutine(Agent_Thread);

    }
		
	//Inizializzo Q, impostando la "matrice conosciuta dall'agente a vuoto."
	//Setto i valori di reward e alpha gamma epsilon dell'agente.
	public void InitializeAgent(Node node){
		Debug.Log ("Inizializzo l'agente. Azzero la Q_table e risetto tutte le sue variabili.");
        this.Alpha = Table_Q_UI.singleton.getAlphaValue();
        this.Gamma = Table_Q_UI.singleton.getGammaValue();
        this.epsilon = Table_Q_UI.singleton.getEpsilonValue();
		this.POSITIVE_REWARD = Table_Q_UI.singleton.getPositiveRewardValue ();
		this.NEGATIVE_REWARD = Table_Q_UI.singleton.getNegativeRewardValue ();
		this.GOAL_REWARD = Table_Q_UI.singleton.getGoalValue ();

        this.CountEpisodes = 1;
        this.Q_Agent = new Dictionary<int,Node> ();
		for (int i = 0; i < GraphStructure.singleton.getLenghtMatrix_X(); i++) {
			for (int j = 0; j < GraphStructure.singleton.getLenghtMatrix_Y(); j++) {
				Node n = GraphStructure.singleton.getMatrix () [i, j];
				this.Q_Agent [n.getID ()] = null;
			}
		}
		this.Q_Agent [node.getID()] = node;
		//Salvo lo stato iniziale dell'agente.
		this.CurrentState = this.Q_Agent [node.getID()];
		//Mi salvo lo stato iniziale
		this.StartState = this.CurrentState;
		//Inizializzo la prima azione da eseguire.
		this.CurrentAction = AllData.AGENT_ACTION.MOVE_RIGHT;

        this.NextAction = AllData.AGENT_ACTION.MOVE_RIGHT;

        this.transform.position = new Vector3(this.CurrentState.getPosition ().x,this.transform.position.y,this.CurrentState.getPosition ().z);

		//Inizializzo la Q Table.
		Initialize_Q_Table ();
	}


	
	void Update () {}

	//Questo metodo determina il CORE dell'Agente. Si cerca di massimizzare il Reward a lungo termine.
	/*Vengono svolte le seguenti operazioni (una volta che l'Agente è stato inizializzato).
	 *1) Scelta di un'azione.
	 *2) Adempimento dell'azione.
	 *3) Aggiorno Q.
	 *4) Itero dal punto 1, fino a quando non avrò raggiunto il Goal.
	 *Lo scopo è quello di vedere se l'Agente migliora sempre di più le sue scelte nel tempo.
	*/
	private IEnumerator Walk(){

        //QLEARNING
        if (QLearning)
        {
            ///////////////////////////////////////////////////////////////////////////////////////////////////////REPEAT FOR EACH EPISODE
            while (this.CountEpisodes < NumberOfEpisodes)
            {

                ////////////////////////////////////////////////////////////////////////////////////////////////////INITIALIZE S
                this.CurrentState = this.StartState;

                //////////////////////////////////////////////////////////////////////////////////////////////////REPEAT FOR EACH STEP OF EPISODE
                //Finisco di ciclare non appena ho raggiunto il Goal.
                while (this.CurrentState.getReward() < GOAL_REWARD)
                {
                    yield return new WaitForSeconds(DecisionTime);
                        
                    Node NextState = null;
                    float Reward = 0f;

                    //Stato iniziale e azione iniziale sono fissati a priori. Stato = 1; Azione = Right.

                    float Q_s_a = 0f;
                    float Q_s1_a1 = 0f;

                    float my_epsilon = UnityEngine.Random.Range(0f, 100f) / 100;
                    //Debug.Log("1: " + my_epsilon);

                    //politica epsilon greedy (SE HO UNA EPSILON PICCOLA FAVORISCO LA SCELTA DELLA MIGLIORE AZIONE FUTURA)
                    //se epsilon = 0 NON HO POLITICA EPSILON-GREEDY

                    this.CurrentAction = my_epsilon >= epsilon ? getBestFutureAction(this.CurrentState) : (AllData.AGENT_ACTION)UnityEngine.Random.Range(1, 4);

                    //Recupero lo stato prossimo, se esiste.
                    if (this.CurrentState.getNeighbor(castMovementToNeighborNode(this.CurrentAction)) != null)
                    {
                        NextState = this.CurrentState.getNeighbor(castMovementToNeighborNode(this.CurrentAction));
                        //Recupero il reward del prossimo stato.
                        Reward = NextState.getReward();
                    }
                    else
                    {
                        Reward = NEGATIVE_REWARD;
                    }
                                     
                    NextAction = getBestFutureAction(NextState);

                    //Q(s,a) Valore di Q nello stato corrente con l'azione corrente.
                    Q_s_a = Q_Table[this.CurrentState.getID(), castActionToNumber(this.CurrentAction)];
                    //Q(st+1,a') Valore di Q nello stato prossimo con la prossima migliore azione da eseguire.
                    //Inizializzato a 0, poichè onn sappiamo se esiste effettivamente lo stato prossimo.
                    Q_s1_a1 = 0f;

                    //Se esiste uno stato prossimo (se non sono uscito dai confini della mappa).
                    if (NextState != null)
                    {
                        Q_s1_a1 = Q_Table[NextState.getID(), castActionToNumber(NextAction)];
                    }

                    //Q-LEARNING
                    this.Q_Table[this.CurrentState.getID(), castActionToNumber(this.CurrentAction)] = (1 - Alpha) * Q_s_a + Alpha * (Reward + Gamma * Q_s1_a1);

                    if (NextState != null && NextState.isWalkable())
                    {
                        this.CurrentState = NextState;
                      
                        this.transform.position = new Vector3(this.CurrentState.getPosition().x, this.transform.position.y, this.CurrentState.getPosition().z);
                    }
                    //Update View
                    Table_Q_UI.singleton.UpdateTableUI(this.Q_Table);
                    this.StepsEpisode++;
                    this.TimeEpisode += Time.deltaTime;
                }
                Table_Q_UI.singleton.AddEpisodeData(this.TimeEpisode, this.StepsEpisode);
                this.StepsEpisode = 0;
                this.TimeEpisode = 0.0f;
                this.transform.position = new Vector3(this.StartState.getPosition().x, this.transform.position.y, this.StartState.getPosition().z);
                this.CountEpisodes++;

            }
            writer.Close();
        }
    }

    //Metodo richiamato per stoppare l'andamento dell'agente.
    public void StopWalk()
    {
        if(Agent_Thread != null)
            StopCoroutine(Agent_Thread);
        Agent_Thread = null;
    }


	//Questo metodo mi ritorna la migliore azione conosciuta dello stato prossimo.
	private AllData.AGENT_ACTION getBestFutureAction(Node NextState){
		AllData.AGENT_ACTION action = AllData.AGENT_ACTION.IDLE;
		//Array d'appoggio dove mi salverò le azioni future con valore di Q uguale.
		List<AllData.AGENT_ACTION> arrayActions = new List<AllData.AGENT_ACTION> (); 
		//Controllo che esista uno stato prossimo.
		if (NextState != null) {
			//Inizializzo la mia varibile d'appoggio con il valore della prima colonna.
			float maxValue = AllData.INFINITE_VALUE;
			//Scorro la riga di Q associata allo stato prossimo, sperando di trovare un'azione con esito positivo.
			//Mi salvo dentro un "contenitore" tutte le azioni future aventi lo stesso valore e scelgo casualmente tra quelle.
			for (int i = 0; i < 4; i++) {
				//Uso una varibile di appoggio per memorizzarmi il valore di Q dell'azione futura "i"-esima.
				float aux_value = this.Q_Table [NextState.getID (), i]; 
				//Debug.Log ("Stato : "+NextState.getID()+" Colonna : " + i + " Value : " + aux_value);
				//Controllo se il valore di Q dell'azione futura è migliore o uguale rispetto al valore massimo.
				if (aux_value >= maxValue) {
					//Se ho due volori uguali sceglierò un'azione a random tra quelle selezionate.
					if (aux_value == maxValue) {
						//Debug.Log ("Aggiungo il valore : "+aux_value+", azione: "+castNumberToAction(i)+" alla lista.");
						arrayActions.Add (castNumberToAction (i));
						maxValue = aux_value;
					}else {
						//Se trovo un valore più alto di tutti, azzelo la lista e aggiungo solo quel valore.
						arrayActions = new List<AllData.AGENT_ACTION> (); 
						arrayActions.Add (castNumberToAction (i));
						maxValue = aux_value;
						//Debug.Log ("Cancello la lista e aggiungo il valore : "+aux_value+", azione: "+castNumberToAction(i)+" alla lista.");
					}
					//Debug.Log ("Nuova possibile azione futura : " + castNumberToAction (i) + " Value : " + aux_value);
				}
			}
		} else {
			Debug.Log ("WARNING : Next State doesn't exist.");
		}
		/*Se non ho trovato nessuna azione futura migliore.
		 *Questo può verificarsi se ho compiuto azioni che hanno un valore di Q inferiore allo 0, oppure se non ho ancora nessun valore memorizzato e quindi tutti i valori peri a 0.
		 *Scelgo un azione random dall'elenco di azioni.*/
		if (arrayActions.Count == 0) {
			action = (AllData.AGENT_ACTION)UnityEngine.Random.Range (1, 4);
			//Debug.Log ("Nessuna migliore azione futura, Azione casuale scelta : " + action);
		} else {
			//Se ci sono una o più azioni possibili allora la scelgo random.
			int n = UnityEngine.Random.Range (0, arrayActions.Count);
			action = (AllData.AGENT_ACTION)arrayActions [n];

		}
		//Ritorno la migliore azione futura, in base al valore di Q trovato.
		return action;
	}
		
	private int castActionToNumber(AllData.AGENT_ACTION action){
		switch (action) {
		case AllData.AGENT_ACTION.MOVE_BOTTOM:
			return 1;
		case AllData.AGENT_ACTION.MOVE_TOP:
			return 0;
		case AllData.AGENT_ACTION.MOVE_RIGHT:
			return 3;
		case AllData.AGENT_ACTION.MOVE_LEFT:
			return 2;
		}	
		return -1;	
	}

	private AllData.AGENT_ACTION castNumberToAction(int action){
		switch (action) {
		case 0:
			return AllData.AGENT_ACTION.MOVE_TOP;
		case 1:
			return AllData.AGENT_ACTION.MOVE_BOTTOM;
		case 2:
			return AllData.AGENT_ACTION.MOVE_LEFT;
		case 3:
			return AllData.AGENT_ACTION.MOVE_RIGHT;		
		}	
		return AllData.AGENT_ACTION.IDLE;	
	}

	private AllData.NEIGHBOR_POSITION castMovementToNeighborNode(AllData.AGENT_ACTION action){
		switch (action) {
		case AllData.AGENT_ACTION.MOVE_BOTTOM:
			return AllData.NEIGHBOR_POSITION.BOTTOM;
		case AllData.AGENT_ACTION.MOVE_TOP:
			return AllData.NEIGHBOR_POSITION.TOP;
		case AllData.AGENT_ACTION.MOVE_RIGHT:
			return AllData.NEIGHBOR_POSITION.RIGHT;
		case AllData.AGENT_ACTION.MOVE_LEFT:
			return AllData.NEIGHBOR_POSITION.LEFT;
		}	
		return AllData.NEIGHBOR_POSITION.NONE;
	}
}
