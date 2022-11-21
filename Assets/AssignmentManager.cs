using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //Allows for Image to be used
using TMPro; //Import for text interactions

/*
Need to add to complete this first part: System with UI to display what the current assignment is, and a way to randomize GemTypes in order to compare them with matches recieved from the Gem class 
*/
public class AssignmentManager : MonoBehaviour
{
    private int totalScore = 0; //Total score counter
    [SerializeField] int MatchScoreValue = 20; //Adjustable value for score from successful match assignment
    [SerializeField] int RegularMatchScoreValue = 5; //Adjustable value for generic matches (Both of these score values are based on giving this many points for EACH tile in the combination)
    [SerializeField] List<GameObject> gemTypes;
    private GemType currentType; //Value for the current match assignment gemType
    [SerializeField] Image MatchTaskImage; //Shows image for match task
    [SerializeField] TextMeshProUGUI scoreText; //Text that shows score
    private void Start() 
    {
        totalScore = 0; //Resets score value upon start
        GenerateMatchAssignment(); //Creates initial matching assignment
        RefreshScoreText();
    }
    public void CheckMatch(List<Gem> Matches) //This method will check a match to see if it fulfills an assignment request
    {
        if(Matches[0].GetGemType() == currentType)
        {
            totalScore += MatchScoreValue; //Increments score
            GenerateMatchAssignment(); //Creates a new assignment
        }
        else
        {
            totalScore += RegularMatchScoreValue; //Updates score if the match did not fill a condition
        }
        RefreshScoreText();
    }

    private void GenerateMatchAssignment() 
    {
        Gem currentGem = gemTypes[Random.Range(0, gemTypes.Count)].GetComponent<Gem>(); //Sets a current type of gem to be the target
        MatchTaskImage.sprite = currentGem.GetComponent<SpriteRenderer>().sprite; //Sets UI display image to that of the gem (This currently bugs out for some of the similar images, but will work once all the gems have unique sprites)
        currentType = currentGem.GetGemType(); //need to modify this in order to find a way to choose a random one of the gem types (unsure how they are currently stored)
    }

    private void RefreshScoreText() 
    {
        scoreText.text = "Score: \n" + totalScore.ToString(); //Updates Score
    }
}
