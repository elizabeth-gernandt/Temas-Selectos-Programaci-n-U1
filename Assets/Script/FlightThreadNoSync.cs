using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Threading;
using Unity.VisualScripting;
using System.IO;
using System.Net.WebSockets;

public class FlightThreadNoSync : MonoBehaviour
{
    public float speed = 50f;
    public float rotationSpeed = 100f;
    public Transform cameraTransform;
    public Vector2 movementInput;

    //Control de Iteraciones
    public int turbulenceIterations = 10000;

    //Lista de vectores de posición caculadas
    private List<Vector3> turbulenceForces = new List<Vector3>();

    //Variables para manipular el hilo secundario

    private Thread turbulenceThread; //instancia del hilo secundario
    private bool isTurbulenceRunning = false;
    private bool stopTurbulenceThread = false;
    private float capturedTime; //almacenar tiempo

    //Bandera de control sobre lectura
    public bool read = false;
    //Ruta de almacenamiento de archivo
    string filepath;


    //Metodo para mover la nave
    public void OnMovement(InputValue value)
    {
        movementInput = value.Get<Vector2>();
    }
    void Start()
    {
        filepath = Application.dataPath + "/TurbulenceData.txt";
        Debug.Log("Ruta al archivo" + filepath);
    }

    // Update is called once per frame
    void Update()
    {
        if (cameraTransform == null)
        {
            Debug.LogError("No hay camara asignada");
            return;
        }

        //ACTIVIDAD 1: Proceso pesado que consume recursos

        //tiempo transcurrido
        capturedTime = Time.time;

        //Proceso pesado en hilo secundario
        if (!isTurbulenceRunning)
        {
            isTurbulenceRunning = true;
            stopTurbulenceThread = false;

            turbulenceThread = new Thread(() =>
                SimulateTurbulence(capturedTime));
            turbulenceThread.Start();
        }


        //Mover la nave de forma lineal
        Vector3 moveDirection = cameraTransform.forward * movementInput.y * speed * Time.deltaTime;
        this.transform.position += moveDirection;

        //Mover la nave en rotacion
        float yaw = movementInput.x * rotationSpeed * Time.deltaTime;
        this.transform.Rotate(0, yaw, 0);

        //Actividad 3: Metodo para la lectura del archivo
        TryReadFile();
    }

    public void SimulateTurbulence(float time)
    {
        turbulenceForces.Clear();

        for (int i = 0; i < turbulenceIterations; i++)
        {
            //Verificar si se debe detener el hilo

            if (stopTurbulenceThread)
            {
                break;
            }

            Vector3 force = new Vector3(
                Mathf.PerlinNoise(i * 0.0001f, time) * 2 - 1,
                Mathf.PerlinNoise(i * 0.0002f, time) * 2 - 1,
                Mathf.PerlinNoise(i * 0.0003f, time) * 2 - 1
                );
            turbulenceForces.Add(force);
        }

        //Señal en consola de inicio de hilo

        Debug.Log("Iniciando simulación de turbulencia");

        

        //Escritura del archivo
        using (StreamWriter writer = new StreamWriter(filepath, false))
        {
            foreach( var force in turbulenceForces)
            {
                writer.WriteLine(force.ToString());
            }
            writer.Flush();
        }

        Debug.Log("Archivo Escrito");

        //Simulacion completa
        isTurbulenceRunning = false;
    }

    void TryReadFile()
    {
        try
        {
            string content = File.ReadAllText(filepath);
            Debug.Log("Archivo Leido" + content);
        }
        catch (IOException ex)
        {
            Debug.LogError("Error de accedo al archivo" + ex.Message);
        }
    }

    private void OnDestroy()
    {
        stopTurbulenceThread = true;

        if (turbulenceThread != null && turbulenceThread.IsAlive)
        {
            turbulenceThread.Join();
        }
    }
}
