using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Table_Q_UI : MonoBehaviour {


	public GameObject Cell_Action;
	public GameObject Cell_State_Content;
	public GameObject Cell_State;
	public GameObject Cell_Table_Parent;
	public GameObject Cell_Table_Value;
	public Text NumberOfEpisodes;
    public Text PositiveRewardText;
    public Text NegativeRewardText;
    public Text GoalRewardText;
    public Text AlphaText;
    public Text GammaText;
    public Text EpsilonText;
    public Slider AlphaSlider;
    public Slider GammaSlider;
    public Slider EpsilonSlide;
	public InputField PositiveRewardInput;
	public InputField NegativeRewardInput;
	public InputField GoalInput;
    public GameObject Episode_Data_Point_Spawn;
	public GameObject Episode_Data_Parent;
	public GameObject Episode_Data;
    public GameObject DataPath;
	public Text DebugTextUI;

	public Button ButtonStart;
	public Button ButtonStop;
	public Button ButtonPause;
	public Toggle toggleGrid;

    private bool alreadyStart = false;
	private bool alreadyPause = false;



	private GameObject[,] Q_Table_UI = null;

	public static Table_Q_UI singleton = null;

	void Awake(){
		singleton = this;
	}

	void Start () {
        //DataPath.GetComponent<Text>().text = Application.dataPath;
		DebugTextUI.text = "";
		ButtonStart.interactable = true;
		ButtonStop.interactable = false;
		ButtonPause.interactable = false;

		PositiveRewardInput.text = "-1";
		NegativeRewardInput.text = "-1000";
		GoalInput.text = "1000";

        setSliderValues();
    }

    void Update () {
		setSliderValues ();
		controlCheckBoxGrid ();
		NumberOfEpisodes.text = Agent.getSingleton ().getNumberEpisodes ()+"";	
	}

	public void DrawTable(){
		int NumberOfState = AllData.X_Matrix * AllData.Y_Matrix;
		//La prima riga inizializza tutti gli stati della Tabella, le righe successive sono le celle che associano ad ogni coppia stato,azione un valore.
		Vector2 Position = Cell_State.transform.position;
		//for (int j = 0; j < 5; j++) {
			for (int i = 1; i < NumberOfState + 1; i++) {	
				Position = new Vector2 (Position.x, Position.y - 60);	
				GameObject New_Cell = Instantiate (Cell_State,Position,Quaternion.identity);
			New_Cell.GetComponentInChildren<Text> ().text = (i-1) +"";
				New_Cell.transform.SetParent (Cell_State_Content.transform);
			}
		//}
	}

	public void FillTable(float[,] Matrix){
		int NumberOfState = AllData.X_Matrix * AllData.Y_Matrix;

		Q_Table_UI = new GameObject[NumberOfState, 4];

        GameObject auxRootCell = Instantiate(Cell_Table_Value, Cell_Table_Value.GetComponent<Transform>().position,Quaternion.identity);
		Vector2 StartPosition = Cell_Table_Value.transform.position;
		Vector2 Position = StartPosition;

		//Scorro prima per colonne e poi per righe.
		for (int i = 0; i < NumberOfState; i++) {	
			for (int j = 0; j < 4; j++) {
				GameObject New_Cell = null;
				if (j == 0) 	
					Position = new Vector2 (StartPosition.x, StartPosition.y - (60f * i));
				else
					Position = new Vector2 (StartPosition.x + (100f * j), Position.y);
				//Prima riga e prima colonna.
				if (i == 0 && j == 0) {
					New_Cell = auxRootCell;
				} else {
					New_Cell = Instantiate (Cell_Table_Value,Position,Quaternion.identity);
				}
				New_Cell.GetComponent<Text> ().text = Matrix[i,j] + "";
				New_Cell.transform.SetParent (Cell_Table_Parent.transform);

				Q_Table_UI [i, j] = New_Cell;
			}
		}	
	}

	public void UpdateTableUI(float[,] Matrix){
		int NumberOfState = AllData.X_Matrix * AllData.Y_Matrix;
		for (int i = 0; i < NumberOfState; i++) {	
			for (int j = 0; j < 4; j++) {
				Q_Table_UI [i, j].GetComponent<Text> ().text = Matrix [i, j] + "";	
			}
		}
	}

    private void resetQTable()
    {
        int NumberOfState = AllData.X_Matrix * AllData.Y_Matrix;
        for (int i = 0; i < NumberOfState; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                Destroy(Q_Table_UI[i, j].gameObject);
            }
        }
    }

	public void AddEpisodeData(float time, int steps){
		GameObject gm = (GameObject) Instantiate (Episode_Data,new Vector2(Episode_Data_Point_Spawn.transform.position.x,Episode_Data_Point_Spawn.transform.position.y - (75f * Agent.getSingleton ().getNumberEpisodes ())),Quaternion.identity);
		gm.transform.SetParent (Episode_Data_Parent.transform);
		gm.transform.Find ("Episode").GetComponent<Text>().text = Agent.getSingleton ().getNumberEpisodes () +"";
		gm.transform.Find ("Time").GetComponent<Text>().text = time +"";
		gm.transform.Find ("Steps").GetComponent<Text>().text = steps +"";
	}

    public void ClearAllEpisodeData()
    {
        int children = Episode_Data_Parent.transform.childCount;
        for (int i = 1; i < children; ++i)
            Destroy(Episode_Data_Parent.transform.GetChild(i).gameObject);
    }

	//Metodi per la gestione dei bottoni.
    public void OnClickButtonStart()
    {
		try{
			float aux1 = float.Parse(this.PositiveRewardInput.text);
			float aux2 =  float.Parse(this.NegativeRewardInput.text);
			float aux3 = float.Parse(this.GoalInput.text);
			DebugTextUI.text = "";
			if (!alreadyStart)
			{
				Time.timeScale = 1.0f;
				GraphStructure.singleton.CreateStructure();
				Agent.getSingleton().InitializeAgent(GraphStructure.singleton.getStartState());

				alreadyStart = true;
				enabledInputs(false);

				ButtonStart.interactable = false;
				ButtonPause.interactable = true;
				ButtonPause.GetComponentInChildren<Text>().text = "Pause";
				ButtonStop.interactable = true;
				DebugTextUI.color = Color.green;
				DebugTextUI.text = "Running";

			}
		}catch(System.FormatException e){
			DebugTextUI.color = Color.red;
			DebugTextUI.text = "INPUT ERROR";
			Debug.Log (e.Message);
		}
    }

	public void OnClickButtonStop(){
		//Fermo L'agente se il sistema è già avviato.
		if(alreadyStart)
		{
			Agent.getSingleton().StopWalk();
			this.resetQTable();
			ClearAllEpisodeData();

			alreadyStart = false;
			ButtonStart.interactable = true;
			ButtonStop.interactable = false;
			ButtonPause.interactable = false;
			DebugTextUI.color = Color.red;
			DebugTextUI.text = "";
			enabledInputs(true);
		}	
	}

	public void OnClickButtonPause(){
		if (alreadyStart) {
			if (!alreadyPause) {
				Time.timeScale = 0.0f;
				ButtonPause.GetComponentInChildren<Text> ().text = "Resume";
				ButtonStop.interactable = false;
				DebugTextUI.color = Color.yellow;
				DebugTextUI.text = "Pause";
				alreadyPause = true;
			} else {
				Time.timeScale = 1.0f;
				ButtonPause.GetComponentInChildren<Text> ().text = "Pause";
				ButtonStop.interactable = true;
				DebugTextUI.color = Color.green;
				DebugTextUI.text = "Running";
				alreadyPause = false;
			}
		}
	}

    public void setSliderValues()
    {
        AlphaText.text = AlphaSlider.value +"";
        GammaText.text = GammaSlider.value + "";
        EpsilonText.text = EpsilonSlide.value +"" ;

		PositiveRewardText.text = PositiveRewardInput.text;
		NegativeRewardText.text = NegativeRewardInput.text;
		GoalRewardText.text = GoalInput.text;
    }

    public float getAlphaValue()
    {
        return this.AlphaSlider.value;
    }
    public float getGammaValue()
    {
        return this.GammaSlider.value;
    }
    public float getEpsilonValue()
    {
        return this.EpsilonSlide.value;
    }

	public float getPositiveRewardValue(){
		return float.Parse(this.PositiveRewardInput.text);
	}
	public float getNegativeRewardValue(){
		return float.Parse(this.NegativeRewardInput.text);
	}
	public float getGoalValue(){
		return float.Parse(this.GoalInput.text);
	}

	private void enabledInputs(bool e){
		PositiveRewardInput.interactable = e;
		NegativeRewardInput.interactable = e;
		GoalInput.interactable = e;
		EpsilonSlide.interactable = e;
		GammaSlider.interactable = e;
		AlphaSlider.interactable = e;
	}

	public void controlCheckBoxGrid(){
		PrintNumberOfTiles.singleton.ShowTiles (toggleGrid.isOn);
	}

	public void ExitApp(){
		Application.Quit ();
	}
}
