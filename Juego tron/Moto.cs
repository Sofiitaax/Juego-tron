using System;
using System.Threading.Tasks;

public class Moto
{
    private Node head; // Nodo cabeza de la lista
    private int size; // Tamaño de la lista
    private Random random; // Generador de números aleatorios
    private Form1 form; // Referencia a la instancia de Form1
    private int combustible; // Cantidad de combustible
    private int celdasRecorridas; // Contador de celdas recorridas
    private ItemsQueue itemQueue;
    private ItemsQueue fuelQueue;

    public Moto(Form1 form)
    {
        this.head = null;
        this.size = 0;
        this.random = new Random();
        this.form = form;
        this.combustible = 100; // Inicializar combustible a 100
        this.celdasRecorridas = 0; // Inicializar contador de celdas recorridas a 0
        this.itemQueue = new ItemsQueue();
        this.fuelQueue = new ItemsQueue();
    }

    public void Add(int x, int y)
    {
        Node newNode = new Node(x, y);
        if (this.head == null)
        {
            this.head = newNode;
        }
        else
        {
            Node current = this.head;
            while (current.Next != null)
            {
                current = current.Next;
            }
            current.Next = newNode;
        }
        this.size++;
    }

    public Node GetHead()
    {
        return head;
    }

    public int GetSize()
    {
        return size;
    }
    public void Velocidad()
    {
        if (Form1.isHyperSpeedActive)
            return;

        // Genera un número aleatorio entre 1 y 10
        int speed = random.Next(1, 11);

        // Ajusta el intervalo del Timer de movimiento basado en la velocidad generada
        form.moveTimer.Interval = speed * 100; // Convertir a milisegundos
    }
    public void Combustible()
    {
        // Incrementar el contador de celdas recorridas
        celdasRecorridas++;

        // Cada vez que se recorren 5 celdas, se consume 1 unidad de combustible
        if (celdasRecorridas % 5 == 0)
        {
            combustible--;
        }

        // Si el combustible llega a 0, detener la moto
        if (combustible <= 0)
        {
            form.moveTimer.Stop(); // Detener el movimiento de la moto
        }
    }

    public int GetCombustible()
    {
        return combustible;
    }


    // Método para recoger un ítems
    public void RecogerItem(int effect)
    {
        if (effect == 1)
        {
            RecogerCombustible(effect); // Tratarlo como combustible
        }
        else
        {
            itemQueue.Enqueue(effect);
            AplicarItemsConDelay(); // Aplicar el ítem con delay
        }
    }

    // Método para recoger una celda de combustible
    public void RecogerCombustible(int fuelAmount)
    {
        if (combustible < 100)
        {
            // Solo aplicar si el combustible no está lleno
            AplicarEfectoCombustible(fuelAmount);
        }
        else
        {
            // Si el combustible está lleno, se aplicará más tarde
            fuelQueue.Enqueue(fuelAmount);
        }
    }

    // Método para aplicar los ítems en la cola con un delay
    private async void AplicarItemsConDelay()
    {
        while (!itemQueue.IsEmpty())
        {
            int effect = itemQueue.Dequeue();

            // Aplicar el efecto del ítem
            AplicarEfectoItem(effect);

            // Espera de 1 segundo antes de aplicar el siguiente ítem
            await Task.Delay(1000);
        }
    }


    // Aplicar el efecto del ítem de estela
    private void AplicarEfectoItem(int effect)
    {
        for (int i = 0; i < effect; i++)
        {
            // Incrementa el tamaño de la estela en el valor del ítem
            Add(head.X, head.Y); // Añadir nuevos nodos a la estela
        }
    }

    // Aplicar el efecto de la celda de combustible
    private void AplicarEfectoCombustible(int fuelAmount)
    {
        if (combustible < 100)
        {
            // Solo aplicar si el combustible no está lleno
            combustible = Math.Min(combustible + fuelAmount, 100);
        }
        else
        {
            // Si el combustible está lleno, se aplicará más tarde
            fuelQueue.Enqueue(fuelAmount);
        }
    }

    // Método para usar la cola de combustible cuando sea necesario
    public void UsarCombustibleEnCola()
    {
        while (!fuelQueue.IsEmpty() && combustible < 100)
        {
            int fuelAmount = fuelQueue.Dequeue();
            combustible = Math.Min(combustible + fuelAmount, 100);
        }
    }
}