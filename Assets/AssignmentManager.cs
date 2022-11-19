using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Need to add to complete this first part: System with UI to display what the current assignment is, and a way to randomize GemTypes in order to compare them with matches recieved from the Gem class 
*/
public class AssignmentManager : MonoBehaviour
{
    private int totalScore = 0; //Total score counter
    [SerializeField] int MatchScoreValue = 20; //Adjustable value for score from successful match assignment
    [SerializeField] GemType currentType; //Value for the current match assignment gemType
    private void Start() 
    {
        totalScore = 0; //Resets score value upon start
        GenerateMatchAssignment(); //Creates initial matching assignment
    }
    public void CheckMatch(List<Gem> Matches) //This method will check a match to see if it fulfills an assignment request
    {
        if(Matches[0].GetGemType() == currentType)
        {
            totalScore += MatchScoreValue; //Increments score
            GenerateMatchAssignment(); //Creates a new assignment
        }
    }

    private void GenerateMatchAssignment() 
    {
        currentType = currentType; //need to modify this in order to find a way to choose a random one of the gem types (unsure how they are currently stored)
    }
}
