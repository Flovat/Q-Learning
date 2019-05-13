using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllData {

	public static int X_Matrix = 26;

	public static int Y_Matrix = 24;

	public static int INFINITE_VALUE = -10000;

	//public static int NEUTRAL_REWARD = -1;

	public enum NEIGHBOR_POSITION {NONE,TOP,BOTTOM,LEFT,RIGHT};

	public enum AGENT_ACTION {IDLE,MOVE_TOP,MOVE_BOTTOM,MOVE_LEFT,MOVE_RIGHT};

	public static int NUMBER_OF_ACTIONS = 4;

}
