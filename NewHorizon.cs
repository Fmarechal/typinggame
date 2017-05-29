using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

public class NewHorizon : MonoBehaviour
{

    /*
     * displayedWords est le tableau des mots du jeu.
     * 
     * MatchingWordsArray est un tableau de booléens où chaque index
     * correspond à un index de displayedWords, et son booléen associé 
     * est son statut actif pour le mot en cours ou pas.
     * 
     * ActiveWords est un tableau de booléens où chaque index correspond 
     * à un index de displayedWords, et son booléen associé est 
     * son statut actif ou non pour le jeu.
     * 
     * */

    [Range(1, 11)]
    public int maxWords = 11;
    void Awake()
    {
        MatchingWordsArray = new bool[maxWords]; // tableau de booléens où chaque index correspond à un index de displayedWords, et son booléen associé est son statut actif pour le mot en cours ou pas
        resetMatchingWordsArray();   // initialisation de MatchingWordsArray pour le premier tour

        ActiveWords = new bool[maxWords]; // tableau de booléens où chaque index correspond à un index de displayedWords, et son booléen associé est son statut actif pour la partie
        for(int i = 0; i< maxWords;i++) ActiveWords[i] = true; // initialisation des mots actifs de la partie

        displayedWords = new string[maxWords];
        createNewWords();           // création d'une liste de mots à jouer parmi ceux d'un dico
        eliminateDuplicates();

        points = 0;
    }

    void Update()
    {
        //if(gameOver) Destroy(gameObject);
        if (Input.anyKeyDown)
        {
            Debug.Log("nouvel input");
            getNewInput();

            if (wordIsCompleted() || !thereIsStillAnotherPossibility())// si on a terminé un mot ou que rien ne correspond, on repart sur un nouveau mot
            {
                //Debug.Log("wordIsCompleted() || !thereIsStillAnotherPossibility()");
                if (!thereIsStillAnotherPossibility())
                {
                    //Debug.Log("!thereIsStillAnotherPossibility()");
                    newTurn(false);
                }
                else if (wordIsCompleted()) newTurn(true);
            }

            else // sinon on désactive les bool des mots disqualifiés dans MatchingWordsArray 
            {
                //Debug.Log("!wordIsCompleted() && thereIsStillAnotherPossibility()");
                for (int i = 0; i < displayedWords.Length; i++)
                {
                    if (!(displayedWords[i].Substring(0, currentWord.Length).Equals(currentWord)))
                    {
                        MatchingWordsArray[i] = false;
                    }
                    else Debug.Log("Mot restant : " + displayedWords[i]);
                }
            }                
        }
    }

    bool thereIsStillAnotherPossibility()
    {
        for (int i = 0; i<maxWords; i++)
        {
            if (MatchingWordsArray[i]) // si le mot est encore en jeu
            {
                //Debug.Log("otherposs");
                if (displayedWords[i].Substring(0, currentWord.Length).Equals(currentWord)) return true; //s'il reste un mot dont le currentWord est le substring, alors il reste encore des possibilités
            }
        }
        return false;
    }

    void newTurn(bool b)
    {
        if (b)
        {
            Debug.Log("Mot complété !");            
            points++;               // on gagne un point
            Debug.Log("Points : " + points);
            checkIfGameIsOver();    // si plus de mots à jouer, game over

            Debug.Log("Voici les mots qu'il reste à taper :");      // bloc pour montrer les mots restants dans le jeu
            for (int k = 0; k<maxWords; k++)
            {
                if (ActiveWords[k]) Debug.Log(displayedWords[k]);
            }


        }
        else
        {
            Debug.Log("Ce mot ne correspond à rien du tout. Recommencez avec un nouveau mot.");
        }
        this.currentWord = "";       // reset du mot en cours
        resetMatchingWordsArray(); // remise de tous les booléens de MatchingWordsArray à FALSE
        Debug.Log("Entrez un nouveau mot");
        
    }

    void getNewInput()
    {
        currentWord += Input.inputString;   // on entre un caractère (ajouté au currentWord)
        Debug.Log("Mot en cours :" + currentWord);
    }

    void resetMatchingWordsArray()
    {
        MatchingWordsArray = new bool[maxWords];
        for (int j = 0; j < maxWords; j++) MatchingWordsArray[j] = true;
    }

    /*
     * @Overview: renvoie true et désactive le mot en cours de frappe dans ActiveWords si ce mot est dans la liste des mots encore valables pr le tour, false sinon
     * */
    bool wordIsCompleted()
    {
        for (int i = 0; i< displayedWords.Length;  i++) // pour chaque mot du jeu
        {
            if (MatchingWordsArray[i]) // s'il n'a pas encore été trouvé
            {
                if (displayedWords[i].Equals(currentWord))  // et si le mot en cours correspond à un des mots du jeu
                {
                    ActiveWords[i] = false; // désactive le bool du mot complété dans ActiveWords
                    Debug.Log("mot trouvé ! " + currentWord);                    
                    return true;            // renvoie wordIsCompleted == true
                }
            }            
        }
        return false;   // sinon, renvoie wordIsCompleted == false
    }

    /*
     * @Overview: désactive le jeu si la liste des mots du jeu est vide
     * */
    void checkIfGameIsOver()
    {
        gameOver = true;
        for (int k = 0; k < maxWords; k++)
        {
            if (ActiveWords[k]) gameOver = false;
        }
        if (gameOver) this.enabled = false;
        Debug.Log("Game Over. Points : " + points);
        Debug.Log("game over ? " + gameOver);
    }

    /*
     * @Overview: crée une liste de [maxWords] mots choisis au hasard dans le dictionnaire, sans doublon => MatchingWordsList et displayedWords
     * @Modifies: rand, maxWords, MatchingWordsList, et displayedWords
     * */
    private void createNewWords()
    {
        if (maxWords > words.Length - 1) // cas où on a demandé trop de mots par rapport à la taille du dico
        {
            maxWords = words.Length - 1;
            Debug.Log("Max words excède la longueur du dictionnaire, sa valeur est ramenée à cette dernière");
        }

        for (int k = 0; k < maxWords; k++)  // peuplement de displayedWords avec des mots du dico au hasard
        {
            rand = Random.Range(0, words.Length - 1);   // index au hasard dans la liste du dictionnaire            
            string newWord = words[rand];            
            displayedWords[k] = newWord;    // ajout à displayedWords
            Debug.Log(newWord);             // affichage du mot choisi
        }        
    }

    /*
     * Désactive dans ActiveWords les mots en double
     * */
    private void eliminateDuplicates()
    {
        string checkedAgainst;
        for (int s = 0; s< maxWords; s++)
        {
            checkedAgainst = displayedWords[s];
            for (int j = 0; j < maxWords; j++)
            {
                if (j != s) // si on n'est pas sur le même mot
                {
                    if (displayedWords[j].Equals(checkedAgainst))   ActiveWords[j] = false;
                }
            }
        }
    }

    private string[] displayedWords;  // mots affichés à la fin de chaque tour (un tour est un mot tenté)
    private string[] words = { "am", "an", "as", "at", "be", "by", "cs", "do", "go", "he", "if", "in", "is", "it", "me", "my", "no", "of", "oh", "on", "or", "pi", "re", "so", "to", "up", "us", "we" };
    private int rand;
    private string currentWord;
    private int points;
    private bool[] MatchingWordsArray;
    private bool[] ActiveWords;
    private bool gameOver;

    //#region Public members

    //[Range(1, 11)]
    //public int maxWords = 11;
    //public List<string> displayedWords; // mots affichés à la fin de chaque tour (un tour est un mot tenté)
    //public List<string> MatchingWordsList;


    //#endregion

    //#region System

    //// Use this for initialization
    //void Start()
    //{
    //    displayedWords = new List<string>();
    //    indexForMatchingWords = 0;
    //    createNewWords();
    //    WordsWon = 0;
    //    minMatchingWordLength = calculateMinMatchingWordLength();
    //    //foreach (string s in MatchingWordsList) Debug.Log(s);g
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    comparison();
    //    removeAbsentWords();
    //    checkIfWordCompleted();
    //    checkForGameOver();
    //    Debug.Log("MatchingWordsList :" + MatchingWordsList.Count);
    //    Debug.Log("displayedWords :" + displayedWords.Count);
    //}

    //#endregion

    //#region Main methods

    //void checkForGameOver()
    //{
    //    if (displayedWords.Count == 0)
    //    {
    //        Debug.Log("Game Over. Points : " + WordsWon);
    //        this.enabled = false;
    //    }
    //}

    //void checkIfWordCompleted()
    //{
    //    if (indexForMatchingWords == minMatchingWordLength) // on a tapé le + petit mot
    //    {

    //        string wordsToRemove

    //        displayedWords.Remove(MatchingWordsList[0]);    // retrait du mot gagné de la liste de tous les mots du jeu
    //        MatchingWordsList.Clear();      // vidage de la liste des mots en cours
    //        foreach (string s in displayedWords) MatchingWordsList.Add(s);  // reconstruction de cette liste à partir des mots du jeu
    //        WordsWon++;
    //        indexForMatchingWords = 0;
    //    }
    //}

    //void comparison()
    //{
    //    if (Input.anyKeyDown && displayedWords.Count > 0)
    //    {
    //        Debug.Log(Input.inputString);
    //        compareKeyWithWords(Input.inputString); // comparaison de tous les mots en cours pour l'index actuel
    //        indexForMatchingWords++;    // incrémentation de l'index pour la prochaine lettre            
    //    }
    //}

    //int calculateMinMatchingWordLength()
    //{
    //    int min = 300;
    //    foreach (string s in MatchingWordsList)
    //    {
    //        if (s.Length < min) min = s.Length;
    //    }
    //    Debug.Log("Longueur minimale : " + min);
    //    return min;
    //}

    //void compareKeyWithWords(string s)
    //{
    //    Debug.Log("into compareKeyWithWords with letter " + s);

    //    for (int i = 0; i < MatchingWordsList.Count; i++)
    //    {
    //        string currentWordToCompare = MatchingWordsList[i];
    //        string currentLetterToCompare = currentWordToCompare[indexForMatchingWords].ToString();

    //        Debug.Log("Première boucle avec la lettre " + currentLetterToCompare);

    //        if (currentLetterToCompare.Equals(s))
    //        {
    //            //la lettre correspondante change de couleur
    //            Debug.Log("La lettre " + s + " a été trouvée dans le mot "
    //                + MatchingWordsList[indexForMatchingWords] + " à l'index " + indexForMatchingWords);
    //        }
    //        else
    //        {
    //            wordsToRemoveList.Add(MatchingWordsList[i]);
    //        }

    //    }
    //    if (indexForMatchingWords == minMatchingWordLength) MatchingWordsList.Clear(); // si on a terminé le mot le plus court
    //    // qui contient les lettres entrées, alors on termine le tour et on vide la liste des mots en cours.
    //    // Les conditions "if" dans l'Update vérifient la longueur de cette liste et vont la reconstruire pour le tour suivant.
    //}

    //void removeAbsentWords()
    //{
    //    if (wordsToRemoveList != null)
    //    {
    //        foreach (string s in wordsToRemoveList)
    //        {
    //            MatchingWordsList.Remove(s);    // pr chaque mot marqué à retirer, on le retire de MatchingWordsList
    //            Debug.Log("Remove word réalisé. Il reste "
    //            + MatchingWordsList.Count + " mots potentiellement valables.");
    //        }
    //        minMatchingWordLength = calculateMinMatchingWordLength(); // recalcul de la longueur du + petit mot

    //    }
    //}



    //private void createNewWords()
    //{
    //    int k;
    //    string newWord;
    //    if (maxWords > words.Length - 1)
    //    {
    //        maxWords = words.Length - 1;
    //        Debug.Log("Max words excède la longueur du dictionnaire, sa valeur est ramenée à cette dernière");
    //    }
    //    for (k = 0; k < maxWords; k++)
    //    {
    //        rand = Random.Range(0, words.Length - 1);   // index au hasard dans la liste du dictionnaire
    //        Rect textArea = new Rect(0, 0, 100 * k + 1 % Screen.height, 100 * k % Screen.width);
    //        newWord = words[rand];
    //        while (displayedWords.Contains(newWord))
    //        {
    //            rand = Random.Range(0, words.Length - 1);
    //            newWord = words[rand];
    //        }
    //        displayedWords.Add(words[rand]);
    //        Debug.Log(words[rand]);
    //    }
    //    foreach (string s in displayedWords) MatchingWordsList.Add(s);
    //}

    //#endregion

    //#region Utils
    //#endregion

    //#region Private and Protected Members

    //private string[] words = { "am", "an", "as", "at", "be", "by", "cs", "do", "go", "he", "if", "in", "is", "it", "me", "my", "no", "of", "oh", "on", "or", "pi", "re", "so", "to", "up", "us", "we" };

    //private int rand;
    //private int indexForMatchingWords;  // index des lettres des mots à comparer avec l'input. 
    //                                    //il est incrémenté à chaque nouvelle lettre entrée,
    //                                    //puisqu'on avance dans chaque mot correspondant du dictionnaire
    //private string InputChar;           // caractère entré par l'utilisateur
    //private int minMatchingWordLength;
    //private int WordsWon;
    //private List<string> wordsToRemoveList;

    //#endregion
}