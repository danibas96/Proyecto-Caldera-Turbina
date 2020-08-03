using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.InteropServices;
using System;

public class RunOperation : MonoBehaviour
{
    [SerializeField]
    private Slider sp_va;// valor del setpoint que ser tomado del slider VA.
    [SerializeField]
    private Slider sp_vf;// valor del setpoint que ser tomado del slider VF.
    [SerializeField]
    private Slider sp_vv;// valor del setpoint que ser tomado del slider VV.
    [SerializeField]
    private float temperatura; //valor del volumen que se obtendra de los calculos realizado.
    [SerializeField]
    private float presion; //valor del error que se obtendra de los calculos realizados
    [SerializeField]
    private float potencia;
    [SerializeField]
    public ParticleSystem ps;
    [SerializeField]
    private float  var_fire;
    private ParticleSystem.MainModule pMain;
    private float error;//declaro el valor del error
    private float model; //es el valor del modelo calculado a utilizar

    [SerializeField]
    private GameObject water; // declaracion del objeto;
    private Vector3 positionwater;// Vector para tomar la posicion del objeto water
    private double heigth; // valor del volumen que va a ser calculado

    [SerializeField]
    private GameObject fuel; // declaracion del objeto;
    private Vector3 positionfuel;// Vector para tomar la posicion del objeto water
    private double heigth1; // valor del volumen que va a ser calculado

    [SerializeField]
    private double velocity; // valor con que velocidad se va a relizar la animacion del llenado del recipiente
    private double volumen;

    private int tf; //numero de intreacciones que suceden en el tiempo
    private int t; //numero de intreacciones que suceden en el tiempo
    private float volume; //declaro el valor del volumen a calacular
    [SerializeField]
    private TMP_Text[] label; //declaro los labels que tendran los valores de la variables anterior mente declaradas
    // Start is called before the first frame update

    //MEMORIA COMPARTIDA
    const string dllPath = "smClient64.dll";//importar la dll
    const string Sistema = "Sistema";
    [DllImport(dllPath)] // For 64 Bits System
    static extern int openMemory(String name, int type);  //abrir memoria

    
    [DllImport(dllPath)]
    static extern void setFloat(String memName, int position, float value);// leer

     [DllImport(dllPath)]
    static extern float getFloat(String memName, int position);//escribir

    
   // private Memoryshare memoryshare; //Intanciando la clase para la memoria compartida y poder hacer uso de ello
    private int position; //posicion de la memoria compartida
    private float[] Sppoint, Outpoint, errpoint; //valores de los arrreglos de las entradas, saldas, errores votados al realizar
    
    public GameObject fuego;
    Vector3 posF;
    public GameObject vapor;
    Vector3 posV;

    void Start()
    {

        openMemory(Sistema, 2);
        for (int i = 0; i < label.Length; i++)
        {
            label[i].text = System.Convert.ToString(0); //asignado el valor de cero a los label para la cual utilizamos la transformacion de propo lenguaje.
        }
        setFloat(Sistema, 0, sp_va.value);
        setFloat(Sistema, 1, sp_vf.value);
        setFloat(Sistema, 2, sp_vv.value);
        
        posF= fuego.transform.localPosition;
        posV = vapor.transform.localPosition;
        positionwater = water.transform.localPosition;
        positionfuel = fuel.transform.localPosition;

    }

    // Update is called once per frame
    void Update()
    {
        temperatura = getFloat(Sistema, 0);
        presion = getFloat(Sistema, 1);
        potencia = getFloat(Sistema, 2);
        Debug.Log(temperatura);
        setFloat(Sistema, 3, sp_va.value);
        setFloat(Sistema, 4, sp_vf.value);
        setFloat(Sistema, 5, sp_vv.value);
        label[0].text = System.Convert.ToString(System.Math.Round(sp_va.value, 2)); //asigno el valor a label haciendo que solo apareca dos decimales
        label[1].text = System.Convert.ToString(System.Math.Round(sp_vf.value, 2));//asigno el valor a label haciendo que solo apareca dos decimales
        label[2].text = System.Convert.ToString(System.Math.Round(sp_vv.value, 2));//asigno el valor a label haciendo que solo apareca dos decimales 
        label[3].text = System.Convert.ToString(System.Math.Round(temperatura, 2));//asigno el valor a label haciendo que solo apareca dos decimales
        label[4].text = System.Convert.ToString(System.Math.Round(presion, 2));//asigno el valor a label haciendo que solo apareca dos decimales
        label[5].text = System.Convert.ToString(System.Math.Round(potencia, 2));//asigno el valor a label haciendo que solo apareca dos decimales

       
        //Slider Fuego
        posF.x = -sp_vf.value * 3.3f+1.9f;
        fuego.transform.position = new Vector3(posF.x, posF.y, posF.z);

        //Slider Vapor
        posV.z = sp_vv.value * 3.7f -0.84f;
        posV.y = 4f;
        posV.x = 0f;
        vapor.transform.position = new Vector3(posV.x, posV.y, posV.z);


        //Vaceado del agua
        heigth = (sp_va.value * 0.68f) ; // calculo de la altura del objeto
        float scaley = System.Convert.ToSingle(positionwater.y - heigth * velocity); // conversion del valor velocidad valor 1
        water.transform.localScale = new Vector3(water.transform.localScale.x, scaley, water.transform.localScale.z); // afectando la escala del objeto 
        float transformy = System.Convert.ToSingle(1.42f-(positionwater.y + heigth * velocity));
        water.transform.localPosition = new Vector3(positionwater.x, transformy, positionwater.z);

        //Vaceado del gasolina
        
        heigth1 = (sp_vf.value * 0.68f); // calculo de la altura del objeto
        float scalefy = System.Convert.ToSingle(positionfuel.y - heigth1 * velocity); // conversion del valor
        fuel.transform.localScale = new Vector3(fuel.transform.localScale.x, scalefy, fuel.transform.localScale.z); // afectando la escala del objeto 
        float transformfy = System.Convert.ToSingle(1.42f - (positionfuel.y + heigth1 * velocity));
        fuel.transform.localPosition = new Vector3(positionfuel.x, transformfy, positionfuel.z);

    }

    private void setFloat(string sistema, int v, object value)
    {
        throw new NotImplementedException();
    }

  

    private void heighResult()
    {
        //float controller; //declaro el valor del controlador
       // sp_va = System.Convert.ToSingle(insp[0].value); //procedo a tomar el valor del slider y transformar a la escala del contendor cabe recalcar que los valos del slider va entre 0 y 1
        //label[0].text = System.Convert.ToString(System.Math.Round(sp_va, 2)); //asigno el valor a label haciendo que solo apareca dos decimales
      //  Setting_BT.setting._sp_va = sp_va; //procedo al almacenamiento de la informacion del valor del setpoint en la clase setting
        //Debug.Log(sp_va);
        //memoryshare.setfloat("sp", 2, sp, position); //enviando el valor del setpoint en la memoria compartida

       // sp_vf = System.Convert.ToSingle(insp[1].value); //procedo a tomar el valor del slider y transformar a la escala del contendor cabe recalcar que los valos del slider va entre 0 y 1
        //label[1].text = System.Convert.ToString(System.Math.Round(sp_vf, 2)); //asigno el valor a label  apareca dos decimales
        //Setting_BT.setting._sp_vf = sp_vf; //procedo al almacenamiento de la informacion del valor del setpoint en la clase setting
        //memoryshare.setfloat("sp", 2, sp, position); //enviando el valor del setpoint en la memoria compartida

 
        //sp_vv = System.Convert.ToSingle(insp[2].value); //procedo a tomar el valor del slider y transformar a la escala del contendor cabe recalcar que los valos del slider va entre 0 y 1
        //label[2].text = System.Convert.ToString(System.Math.Round(sp_vv, 2)); //asigno el valor a label apareca dos decimales
       // Setting_BT.setting._sp_vv = sp_vv; //procedo al almacenamiento de la informacion del valor del setpoint en la clase setting
                                           //memoryshare.setfloat("sp", 2, sp, position); 
    }
}
