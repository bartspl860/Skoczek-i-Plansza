using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Algoritm : MonoBehaviour
{
    [SerializeField]
    private AudioSource as_chess_piece;
    [SerializeField]
    private AudioClip chess_piece_clip;
    [SerializeField]
    private AudioClip wrong_clip;
    [SerializeField]
    private AudioClip menu_clip;
    [SerializeField]
    private GameObject visited_sprite;
    [SerializeField]
    private Transform t_visited_sprite;
    [SerializeField]
    private Collider2D c2d_knight;
    [SerializeField]
    private LayerMask chessboard;

    private Vector3 screenPoint;
    private Vector3 offset;
    [SerializeField]
    private Vector3[] fields;
    [SerializeField]
    private int start_field = 0;
    [SerializeField]
    private Canvas menu;
    [SerializeField]
    private Canvas intro;
    [SerializeField]
    private Canvas score_cv;

    [SerializeField]
    private AudioSource music;
    [SerializeField]
    private TMP_Text score;

    [SerializeField]
    private Transform t_poleWyboru;

    [SerializeField]
    private int[] notAllowedIndexes_left;
    [SerializeField]
    private int[] notAllowedIndexes_right;
    [SerializeField]
    private int[] notAllowedIndexes_leftMiddle;
    [SerializeField]
    private int[] notAllowedIndexes_rightMiddle;

    private int actual_field_index;
    private int last_field_index;
    private bool wasMoveRight = true;
    private bool isInMenu = true;
    private bool knight = false;
    private int counter = 0;
    private bool wasFirstMove = true;
    [SerializeField]
    private int[] goldenIndexes;
    private bool isFieldTaken = false;

    List<int> visited_fields = new List<int>();
    private int count_visited_fields = 1;

    List<int> possible_moves = new List<int>();
    [SerializeField]
    private GameObject possible_field;
    [SerializeField]
    private GameObject endMenu;
    [SerializeField]
    private TMP_Text endInfo;
    [SerializeField]
    private TMP_Text infoMenu;

    IEnumerator introFadeOut()
    {
        yield return new WaitForSeconds(5f);
        intro.GetComponent<Canvas>().enabled = false;
        isInMenu = true;
        menu.GetComponent<Canvas>().enabled = true;
    }
    void Start()
    {
        menu.GetComponent<Canvas>().enabled = false;
        StartCoroutine(introFadeOut());
        clearInfo();
    }

    void Update()
    {
        int temp_sc = 64 - counter;
        score.text = "" + temp_sc;
        if (temp_sc == 0)
        {
            isInMenu = true;
            endMenu.GetComponent<Canvas>().enabled = true;
            endInfo.text = "Brawo wygrałeś!";
        }

        if (!knight && !isInMenu)
        {
            t_poleWyboru.GetComponent<SpriteRenderer>().enabled = true;
            c2d_knight.GetComponent<SpriteRenderer>().enabled = false;
            Vector3 choose_field_cursorPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
            Vector3 choose_field_cursorPosition = Camera.main.ScreenToWorldPoint(choose_field_cursorPoint) + offset;

            for (int i = 0; i < 64; i++)
            {
                if (Vector2.Distance(choose_field_cursorPosition, fields[i]) < 3)
                {
                    t_poleWyboru.position = fields[i] + new Vector3(0f, 0.2f, -1f);
                    if (Input.GetMouseButtonDown(0))
                    {
                        counter++;
                        as_chess_piece.clip = chess_piece_clip;
                        as_chess_piece.Play();
                        c2d_knight.GetComponent<SpriteRenderer>().enabled = true;
                        t_poleWyboru.GetComponent<SpriteRenderer>().enabled = false;
                        knight = true;
                        actual_field_index = i;
                        transform.position = fields[i];
                        visited_fields.Add(actual_field_index);
                        break;
                    }
                }
            }
        }
        if (Input.GetKeyDown("escape") && !isInMenu)
        {
            playMenu();
            isInMenu = true;
            menu.GetComponent<Canvas>().enabled = true;
        }
        if (isInMenu)
        {
            score_cv.GetComponent<Canvas>().enabled = false;
        }
        else
        {
            score_cv.GetComponent<Canvas>().enabled = true;
        }
    }

    private int licznik = 0;
    void OnMouseDown()
    {
        if (!isInMenu && knight)
        {
            last_field_index = actual_field_index;
            screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
            offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        }

        for (int j = 0; j < goldenIndexes.Length; j++)
        {
            int nextMove = actual_field_index + goldenIndexes[j];
            isFieldTaken = false;
            for (int i = 0; i < visited_fields.Count; i++)
            {
                if (visited_fields[i] == nextMove)
                {
                    isFieldTaken = true;
                }
            }
            for (int i = 0; i < notAllowedIndexes_left.Length; i++)
            {
                if (actual_field_index == notAllowedIndexes_left[i])
                {
                    if (nextMove == actual_field_index + 15 || nextMove == actual_field_index + 6 || nextMove == actual_field_index - 10 || nextMove == actual_field_index - 17)
                    {
                        isFieldTaken = true;
                    }
                }
                if (actual_field_index == notAllowedIndexes_leftMiddle[i])
                {
                    if (nextMove == actual_field_index + 6 || nextMove == actual_field_index - 10)
                    {
                        isFieldTaken = true;
                    }
                }
                if (actual_field_index == notAllowedIndexes_rightMiddle[i])
                {
                    if (nextMove == actual_field_index + 10 || nextMove == actual_field_index - 6)
                    {
                        isFieldTaken = true;
                    }
                }
                if (actual_field_index == notAllowedIndexes_right[i])
                {
                    if (nextMove == actual_field_index - 15 || nextMove == actual_field_index - 6 || nextMove == actual_field_index + 10 || nextMove == actual_field_index + 17)
                    {
                        isFieldTaken = true;
                    }
                }
            }
            if (nextMove < 0 || nextMove > 63)
                isFieldTaken = true;

            if (!isFieldTaken)
                possible_moves.Add(actual_field_index + goldenIndexes[j]);
        }
        for (int i = 0; i < possible_moves.Count; i++)
        {
            GameObject temp = Instantiate(possible_field);
            temp.transform.position = fields[possible_moves[i]] + new Vector3(0f, 0.2f, 2f);
        }
        if (possible_moves.Count == 0)
        {
            isInMenu = true;
            endMenu.GetComponent<Canvas>().enabled = true;
            endInfo.text = "Przegrałeś!";
        }
    }

    void OnMouseDrag()
    {
        if (!isInMenu && knight)
        {
            Vector3 cursorPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
            Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(cursorPoint) + offset;
            transform.position = cursorPosition;
        }
    }

    void OnMouseUp()
    {
        GameObject[] temp = GameObject.FindGameObjectsWithTag("PossibleMove");
        for (int i = 0; i < temp.Length; i++)
        {
            Destroy(temp[i]);
        }
        possible_moves.Clear();

        if (!isInMenu && knight)
        {
            wasMoveRight = true;
            for (int i = 0; i < 64; i++)
            {
                if (Vector2.Distance(transform.position, fields[i]) < 4)
                {
                    transform.position = fields[i];
                    actual_field_index = i;
                }
                if (!c2d_knight.IsTouchingLayers(chessboard))
                {
                    transform.position = fields[last_field_index];
                    wasMoveRight = false;
                }
            }
            if (actual_field_index == last_field_index + 17 || actual_field_index == last_field_index - 17 || actual_field_index == last_field_index + 15 || actual_field_index == last_field_index - 15 || actual_field_index == last_field_index + 10 || actual_field_index == last_field_index - 10 || actual_field_index == last_field_index + 6 || actual_field_index == last_field_index - 6)
            {
                for (int i = 0; i < notAllowedIndexes_left.Length; i++)
                {
                    if (last_field_index == notAllowedIndexes_left[i])
                    {
                        if (actual_field_index == last_field_index + 15 || actual_field_index == last_field_index + 6 || actual_field_index == last_field_index - 10 || actual_field_index == last_field_index - 17)
                        {
                            wasMoveRight = false;
                        }
                        break;
                    }
                    if (last_field_index == notAllowedIndexes_right[i])
                    {
                        if (actual_field_index == last_field_index + 17 || actual_field_index == last_field_index + 10 || actual_field_index == last_field_index - 6 || actual_field_index == last_field_index - 15)
                        {
                            wasMoveRight = false;
                        }
                        break;
                    }
                    if (last_field_index == notAllowedIndexes_leftMiddle[i])
                    {
                        if (actual_field_index == last_field_index + 6 || actual_field_index == last_field_index + -10)
                        {
                            wasMoveRight = false;
                        }
                        break;
                    }
                    if (last_field_index == notAllowedIndexes_rightMiddle[i])
                    {
                        if (actual_field_index == last_field_index + -6 || actual_field_index == last_field_index + 10)
                        {
                            wasMoveRight = false;
                        }
                        break;
                    }
                }
                for (int i = 0; i < visited_fields.Count; i++)
                {
                    if (actual_field_index == visited_fields[i])
                    {
                        wasMoveRight = false;
                    }
                }
            }
            else
            {
                wasMoveRight = false;
            }
            if (wasMoveRight)
            {
                visited_fields.Add(actual_field_index);
                GameObject newVisited = Instantiate(visited_sprite);
                newVisited.transform.position = fields[last_field_index] + new Vector3(0f, 0.2f, 3f);
                counter++;
                as_chess_piece.clip = chess_piece_clip;
                as_chess_piece.Play();
            }
            else
            {
                as_chess_piece.clip = wrong_clip;
                as_chess_piece.Play();
                transform.position = fields[last_field_index];
                actual_field_index = last_field_index;
                last_field_index = actual_field_index;
            }
        }
    }
    public void backToGame()
    {
        clearInfo();
        isInMenu = false;
        menu.GetComponent<Canvas>().enabled = false;
    }

    public void exitGame()
    {
        Application.Quit();
    }

    public void reset()
    {
        clearInfo();
        knight = false;
        counter = 0;
        visited_fields.Clear();
        menu.GetComponent<Canvas>().enabled = false;
        endMenu.GetComponent<Canvas>().enabled = false;
        isInMenu = false;
        GameObject[] visited = GameObject.FindGameObjectsWithTag("Visited");

        foreach (GameObject elem in visited)
            GameObject.Destroy(elem);
    }

    public void playMenu()
    {
        as_chess_piece.clip = menu_clip;
        as_chess_piece.Play();
    }

    public void showInfo()
    {
        infoMenu.text = "Co Nowego:<br><br>- Poprawiono błędy występujące na przeglądarce Firefox.<br>- Dodano pola następnych możliwych posunięć.<br><br>Błędy proszę zgłaszać na lastcody77@gmail.com<br><br><color=yellow>Podziękowanie dla Pana Tomasza za lekcje informatyki, które zainspirowały mnie do stworzenia tej gry.</color><br><br><br><color=white><sprite name=copy> Bartłomiej Spleśniały 2019";        
    } 
    public void clearInfo()
    {
        infoMenu.text = "";
    }

    public void backToMenu()
    {
        clearInfo();
        reset();
        isInMenu = true;
        menu.GetComponent<Canvas>().enabled = true;
        c2d_knight.GetComponent<SpriteRenderer>().enabled = false;
    }
}

