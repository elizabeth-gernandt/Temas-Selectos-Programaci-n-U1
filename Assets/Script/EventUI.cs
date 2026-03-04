using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;

public class EventUI : MonoBehaviour
{
    public List<GameObject>listaInstrucciones;
    public int currentIndex = 0;
    public List<string> mensajesInstrucciones;
    public TextMeshProUGUI textMeshProUGUI;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        //Actualizar Visibilidad de panels
        UpdateVisibility();
    }

    void Update()
    {
        
    }

    //Metodo para actualizar visibilidad de paneles

    private void UpdateVisibility()
    {
        for (int i = 0; i < listaInstrucciones.Count; i++)
        {
            //Solo el panel en el indice actual esta activo
            listaInstrucciones[i].SetActive(i == currentIndex);
        }
    }
    //Metodo para cambiar de escena
    public void ChangeSceneByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    //Metodo para cambiar de escena por nombre
    public void ChangeSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    //Metodo para cambiar entre paneles
    public void CycleObjects()
    {
        //Incrementa el indice y vuelve al principio
        currentIndex = (currentIndex+1)% listaInstrucciones.Count;

        //Actualizar la visibilidad
        UpdateVisibility();
    }

    //Metodo para actualizar el texto
    private void UpdateText()
    {
        if (mensajesInstrucciones.Count > 0)
        {

        }
    }

    //Metodo para salir de la aplicacion
    public void ExitGame()
    {
        //donde vas a hacer la impresion
        Debug.Log("Va a salir");
        Application.Quit();
        Debug.Log("Ya salio");
    }
}
