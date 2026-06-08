using UnityEngine;




public class Respawn : MonoBehaviour
{


    //adding a score value deduction for death 
    [Header("Score points lost")]
    [SerializeField] int scoreValue = 40;
    //bool scoreGiven; part of method one deduction

    //this is the script for intial respawn and needs to be paired with respawntrigger objects or zones
    private void OnTriggerEnter2D(Collider2D other)
    {

        //adding modifier for player score value
        // this method works but only applies deduction once

        //if (!scoreGiven)
        //{
        //    scoreGiven = true;

        //    if (scoreSystem.instance != null)
        //    {
        //        scoreSystem.instance.SubtractScore(scoreValue);
        //    }
        //}
        //will deduct points every respawn
        if (scoreSystem.instance != null)
        {
            scoreSystem.instance.SubtractScore(scoreValue);
        }

        if (other.CompareTag("Player"))
        {
            gamemanager.instance.RespawnPlayer();
        }

    }
}
