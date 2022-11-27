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
    private int totalScoreMax = 0; //Possible total score counter
    private float matchQuantity = 0f; //Required match number
    private float currentMatchQuantity = 0f; //Matched so far
    private float currentTime = 0f; //Current value left in timer
    private float TimeMax = 0f; //Max value for timer
    [SerializeField] int MatchScoreValue = 100; //Adjustable value for score from each match assignment
    //[SerializeField] int RegularMatchScoreValue = 5; //Adjustable value for generic matches (Both of these score values are based on giving this many points for EACH tile in the combination)
    [SerializeField] List<GameObject> gemTypes;
    private GemType currentType; //Value for the current match assignment gemType
    [SerializeField] Image MatchTaskImage; //Shows image for match task
    [SerializeField] TextMeshProUGUI quantityText; //text that shows how many you need to match of a certain type
    [SerializeField] TextMeshProUGUI scoreText; //Text that shows score
    [SerializeField] Slider TimerBar;
    private void Start() 
    {
        totalScore = 0; //Resets score value upon start
        totalScoreMax = 0;
        GenerateMatchAssignment(); //Creates initial matching assignment
        RefreshScoreText();
    }
    
    private void Update() 
    {
        currentTime += Time.deltaTime; //increments timer
        UpdateTimerVisual(); //Updates image for slider
        if (currentTime >= TimeMax) //This indicated time has run out for an assignment
        {
            StartCoroutine(FinishMatchChallenge(currentMatchQuantity, matchQuantity)); //calculates points to add
            GenerateMatchAssignment(); //Restarts the assignment (this also takes care of the timer)
        }
    }

    public void CheckMatch(List<Gem> Matches) //This method will check a match to see if it fulfills an assignment request
    {
        if(Matches[0].GetGemType() == currentType)
        {
            currentMatchQuantity ++; //Increment Matched gems total
            RefreshQuantityText();
            if (currentMatchQuantity >= matchQuantity)
            {
                StartCoroutine(FinishMatchChallenge(currentMatchQuantity, matchQuantity)); //calculates points to add
                GenerateMatchAssignment(); //Creates a new assignment
            }
        }
        /*else
        {
            totalScore += RegularMatchScoreValue; //Updates score if the match did not fill a condition
        }*/
        RefreshScoreText();
    }

    private void GenerateMatchAssignment() 
    {
        ResetTimer();
        matchQuantity = Random.Range(12, 20); //Chooses an assignment match quantity to be from 12 to 20 (when testing I found anything above 20 is cursed... 25 Extremely cursed)
        currentMatchQuantity = 0; //Resets amounts considered matched thus far
        RefreshQuantityText();
        Gem currentGem = gemTypes[Random.Range(0, gemTypes.Count)].GetComponent<Gem>(); //Sets a current type of gem to be the target
        MatchTaskImage.sprite = currentGem.GetComponent<SpriteRenderer>().sprite; //Sets UI display image to that of the gem (This currently bugs out for some of the similar images, but will work once all the gems have unique sprites)
        MatchTaskImage.color = currentGem.GetComponent<SpriteRenderer>().color; 
        currentType = currentGem.GetGemType(); //need to modify this in order to find a way to choose a random one of the gem types (unsure how they are currently stored)
    }

    private void RefreshScoreText() 
    {
        scoreText.text = "Score: \n" + totalScore.ToString() + " / " + totalScoreMax.ToString(); //Updates Score
    }

    private void RefreshQuantityText()
    {
        quantityText.text = currentMatchQuantity.ToString() + " / " + matchQuantity.ToString();
    }

    private void ResetTimer() //method to initialize timer
    {
        currentTime = 0;
        TimeMax = Random.Range(50, 60); //Sets timer to a value between 50 and 60 seconds
        UpdateTimerVisual();
    }

    private void UpdateTimerVisual()
    {
        TimerBar.maxValue = TimeMax;
        TimerBar.value = currentTime; //These two lines make it so that the timer bar has an amount of it covered to represent how much of the timer has passed
    }

    IEnumerator FinishMatchChallenge(float current, float total) 
    {
        yield return new WaitForSecondsRealtime(0.5f); //waits for .5 seconds before processing (to ensure this process doesnt bug)
        totalScoreMax += MatchScoreValue; //Adds total possible points
        totalScore += (int) (MatchScoreValue * (current/total)); //Adds total completed points
        RefreshScoreText(); //Displays this updated information
    }
}
