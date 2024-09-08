using System;

public class ItemsQueue
{
    private ItemNode front;
    private ItemNode rear;

    public ItemsQueue()
    {
        front = null;
        rear = null;
    }

    public void Enqueue(int effect)
    {
        ItemNode newNode = new ItemNode(effect);

        // Si el efecto es combustible, se añade al frente
        if (effect == 1)
        {
            newNode.Next = front;
            front = newNode;
            if (rear == null)
            {
                rear = newNode;
            }
        }
        else
        {
            // Si la cola está vacía
            if (rear == null)
            {
                front = newNode;
                rear = newNode;
            }
            else
            {
                rear.Next = newNode;
                rear = newNode;
            }
        }
    }

    public int Dequeue()
    {
        if (front == null)
        {
            Console.WriteLine("Queue Underflow");
            return -1;
        }
        else
        {
            int effect = front.Effect;
            front = front.Next;
            if (front == null)
            {
                rear = null;
            }
            return effect;
        }
    }

    public int Front()
    {
        if (front == null)
        {
            Console.WriteLine("Queue Underflow");
            return -1;
        }
        else
        {
            return front.Effect;
        }
    }

    public bool IsEmpty()
    {
        return front == null;
    }

    private class ItemNode
    {
        public int Effect { get; set; }
        public ItemNode Next { get; set; }

        public ItemNode(int effect)
        {
            this.Effect = effect;
            this.Next = null;
        }
    }
}