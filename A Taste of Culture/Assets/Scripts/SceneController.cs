using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField] string nextScene;
    // [SerializeField] float applauseDelay = 0.5f;
    // [SerializeField] float sceneTransitionDelay = 3f;

    public enum Ingredient { Chicken, Tofu, Onion, Tomato };

    [SerializeField] private Ingredient currentIngredient;
    public Ingredient CurrentIngredient { get { return currentIngredient; } }
    
    // Will only be referenced if in chopping scene.
    GameObject onion;
    GameObject tomato;

    void Awake() 
    {
        // If ingredient is onion, then grab onion and tomato gameobjects to be referenced on task completion
        if(currentIngredient == Ingredient.Onion)
        {
            onion = GameObject.Find("Onion");
            tomato = GameObject.Find("Tomato");
        }
        
    }
    // Start is called before the first frame update
    void Start()
    {
        // If ingredient is onion, then disable the tomato gameobject
        if(currentIngredient == Ingredient.Onion)
        {
            if(tomato != null) { tomato.SetActive(false); }
        }
    }

    public void TaskComplete()
    {
        StartCoroutine(CompleteTask());
    }

    IEnumerator CompleteTask()
    {
        yield return new WaitForSeconds(0.5f);

        // TODO: Have mentor applaud player
        Debug.Log("GREAT JOB!!");

        // If current ingredient is Onion, disable it and enable tomato
        if(currentIngredient == Ingredient.Onion)
        {
            yield return new WaitForSeconds(1f);
            if(onion != null) { onion.SetActive(false); }

            GameObject spriteMask = GameObject.Find("Chunks Sprite Mask(Clone)");
            if(spriteMask != null) { GameObject.Destroy(spriteMask); }

            if(tomato != null) 
            { 
                // Debug.Log("Found Tomato!");
                tomato.SetActive(true); 
                currentIngredient = Ingredient.Tomato;
            }

            yield break;
        }

        else
        {
            yield return new WaitForSeconds(3f);
            
            // Load next scene
            if(nextScene != null) { SceneManager.LoadScene(nextScene); }
        }
    }
}
