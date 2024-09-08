using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;


public partial class Form1 : Form
{
    private Panel gridPanel;
    private List<PictureBox> motoPictureBoxes;
    private Moto motoList;
    private int cellWidth = 39; 
    private int cellHeight = 39; 
    private int rows = 15;       
    private int cols = 32;       
    public Timer moveTimer;     // Timer para mover la moto automáticamente
    private int currentDx = 1;   // Dirección actual en X (por defecto hacia la derecha)
    private int currentDy = 0;   // Dirección actual en Y (por defecto sin movimiento)
    private Timer itemTimer;
    private int speedUpdateInterval = 5000;
    public static bool isHyperSpeedActive = false;
    public bool isShieldActive = false;
    private Stack<string> poderes = new Stack<string>();
    private ListBox pilaListBox;
    private Button moverPoderButton;
    private Label combustibleLabel;


    public Form1()
    {
        InitializeComponent();
        this.KeyPreview = true;
        InitializeGridPanel();
        InitializeTimer();
        InitializeMoto();
        InitializePilaListBox();
        InitializeCombustibleLabel();
        this.KeyDown += new KeyEventHandler(OnKeyDown);
    }

    private void InitializeGridPanel()
    {
        gridPanel = new Panel
        {
            Location = new Point(10, 10),
            Size = new Size(cols * cellWidth, rows * cellHeight),
            BackColor = Color.Black 
        };
        gridPanel.Paint += GridPanel_Paint;
        this.Controls.Add(gridPanel);
    }

    private void InitializeMoto()
    {
        motoList = new Moto(this);
        motoList.Add(0, 0); // Posición inicial de la moto

        // Generar la velocidad inicial
        motoList.Velocidad();

        motoPictureBoxes = new List<PictureBox>();

        // Inicializar la cabeza de la moto
        Node current = motoList.GetHead();
        PictureBox motoHead = new PictureBox
        {
            Size = new Size(cellWidth, cellHeight),
            Location = new Point(current.X * cellWidth, current.Y * cellHeight),
            Image = Image.FromFile("C:\\Users\\Usuario\\Desktop\\Datos 1\\Juego tron\\Juego tron\\Resources\\moto.png"),
            BackColor = Color.Transparent,
            SizeMode = PictureBoxSizeMode.StretchImage
        };
        motoPictureBoxes.Add(motoHead);
        gridPanel.Controls.Add(motoHead);

        // Inicializar los tres segmentos de la estela
        for (int i = 0; i < 3; i++)
        {
            motoList.Add(current.X, current.Y);
            PictureBox lightTrail = new PictureBox
            {
                Size = new Size(cellWidth / 2, cellHeight / 2),
                Location = new Point(current.X * cellWidth + cellWidth / 4, current.Y * cellHeight + cellHeight / 4), 
                BackColor = Color.LightBlue,
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            motoPictureBoxes.Add(lightTrail);
            gridPanel.Controls.Add(lightTrail);
        }
    }

    private void InitializeTimer()
    {
        moveTimer = new Timer();
        moveTimer.Tick += MoveTimer_Tick;
        moveTimer.Start();

        // Timer para actualizar la velocidad cada 5 segundos
        Timer speedTimer = new Timer();
        speedTimer.Interval = speedUpdateInterval;
        speedTimer.Tick += (s, e) => motoList.Velocidad(); 
        speedTimer.Start();

        if (motoList != null)
        {
            motoList.Velocidad(); // Generar velocidad inicial aleatoria
        }
        // Inicializar el timer para generar ítems
        itemTimer = new Timer();
        itemTimer.Interval = 15000; // Generar un ítem cada 15 segundos
        itemTimer.Tick += (s, e) => GenerateItemPower();
        itemTimer.Start();

    }
    private void GenerateItemPower()
    {
        Random rand = new Random();
        int itemX = rand.Next(0, cols);
        int itemY = rand.Next(0, rows);

        PictureBox itempowerBox = new PictureBox
        {
            Size = new Size(cellWidth / 2, cellHeight / 2),
            Location = new Point(itemX * cellWidth + cellWidth / 4, itemY * cellHeight + cellHeight / 4),
            BackColor = Color.Transparent,
            SizeMode = PictureBoxSizeMode.StretchImage
        };

        int itemType = rand.Next(0, 5); // Número aleatorio entre 0 y 4 (según el número el item/poder que se genera)

        if (itemType == 0) // Ítem de estela
        {
            itempowerBox.Tag = rand.Next(1, 11); 
            itempowerBox.Image = Image.FromFile("C:\\Users\\Usuario\\Desktop\\Datos 1\\Juego tron\\Juego tron\\Resources\\Estela de luz.png");
        }
        else if (itemType == 1) // Ítem de combustible
        {
            itempowerBox.Tag = "Fuel:" + rand.Next(1, 11);
            itempowerBox.Image = Image.FromFile("C:\\Users\\Usuario\\Desktop\\Datos 1\\Juego tron\\Juego tron\\Resources\\fuel.png");
        }
        else if (itemType == 2) // Bomba
        {
            itempowerBox.Tag = "Bomb"; 
            itempowerBox.Image = Image.FromFile("C:\\Users\\Usuario\\Desktop\\Datos 1\\Juego tron\\Juego tron\\Resources\\bomba.png");
        }
        else if (itemType == 3) // Escudo
        {
            itempowerBox.Tag = "Escudo"; 
            itempowerBox.Image = Image.FromFile("C:\\Users\\Usuario\\Desktop\\Datos 1\\Juego tron\\Juego tron\\Resources\\escudo.png");
        }
        else if (itemType == 4) // Hipervelocidad
        {
            itempowerBox.Tag = "Hipervelocidad"; 
            itempowerBox.Image = Image.FromFile("C:\\Users\\Usuario\\Desktop\\Datos 1\\Juego tron\\Juego tron\\Resources\\hipervelocidad.png");
        }

        gridPanel.Controls.Add(itempowerBox);
    }

    private void MoverPoder()
    {
        if (poderes.Count > 0)
        {
            // Saca el elemento del tope de la pila
            string poder = poderes.Pop();

            // Guarda temporalmente todos los elementos restantes en una lista
            List<string> poderesTemporales = new List<string>();
            while (poderes.Count > 0)
            {
                poderesTemporales.Add(poderes.Pop());
            }

            // Agrega el poder sacado al fondo de la pila
            poderes.Push(poder);

            // Agregar de nuevo los poderes restantes en el orden en que estaban
            for (int i = poderesTemporales.Count - 1; i >= 0; i--)
            {
                poderes.Push(poderesTemporales[i]);
            }

            UpdatePilaListBox();
        }
    }

    private void InitializeCombustibleLabel()
    {
        combustibleLabel = new Label
        {
            Text = "Combustible: 100", 
            Location = new Point(650, 650), 
            AutoSize = true,
            ForeColor = Color.Black 
        };

        this.Controls.Add(combustibleLabel);
    }

    private void UpdateCombustibleLabel()
    {
        combustibleLabel.Text = $"Combustible: {motoList.GetCombustible()}";
    }

    private void InitializePilaListBox()
    {
        pilaListBox = new ListBox
        {
            Location = new Point(500, 600),
            Size = new Size(120, 100)
        };

        Button aplicarPoderButton = new Button
        {
            Text = "Aplicar Poder",
            Location = new Point(pilaListBox.Left - 20, pilaListBox.Bottom)
        };
        aplicarPoderButton.Click += (s, e) => AplicarPoder();

        moverPoderButton = new Button
        {
            Text = "Mover Poder",
            Location = new Point(aplicarPoderButton.Right + 10, aplicarPoderButton.Top) 
        };
        moverPoderButton.Click += (s, e) => MoverPoder();

        this.Controls.Add(pilaListBox);
        this.Controls.Add(aplicarPoderButton);
        this.Controls.Add(moverPoderButton);
    }

    private void MoveTimer_Tick(object sender, EventArgs e)
    {
        MoveMoto(currentDx, currentDy); // Mover según la última dirección
        UpdateCombustibleLabel();
    }




    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.Up:
                currentDx = 0;
                currentDy = -1;
                break;
            case Keys.Down:
                currentDx = 0;
                currentDy = 1;
                break;
            case Keys.Left:
                currentDx = -1;
                currentDy = 0;
                break;
            case Keys.Right:
                currentDx = 1;
                currentDy = 0;
                break;
        }
    }

    public void CheckItemCollision()
    {
        List<PictureBox> itemsToRemove = new List<PictureBox>();

        foreach (Control control in gridPanel.Controls)
        {
            if (control is PictureBox itemBox && itemBox.Tag != null)
            {
                Node motoHead = motoList.GetHead();

                if (motoHead.X == itemBox.Location.X / cellWidth && motoHead.Y == itemBox.Location.Y / cellHeight)
                {
                    if (itemBox.Tag is int effect)
                    {
                        motoList.RecogerItem(effect);  // Aumentar la estela de la moto
                    }
                    else if (itemBox.Tag is string fuelTag && fuelTag.StartsWith("Fuel:"))
                    {
                        int fuelAmount = int.Parse(fuelTag.Substring(5));
                        motoList.RecogerCombustible(fuelAmount);
                    }
                    else if (itemBox.Tag is string bombTag && bombTag == "Bomb")
                    {
                        if (!isShieldActive) // Si el escudo no está activo
                        {
                            // Eliminar la moto 
                            foreach (var motoPart in motoPictureBoxes)
                            {
                                gridPanel.Controls.Remove(motoPart);
                                motoPart.Dispose();
                            }
                            motoPictureBoxes.Clear();

                            PictureBox explosionPictureBox = new PictureBox
                            {
                                Image = Image.FromFile("C:\\Users\\Usuario\\Desktop\\Datos 1\\Juego tron\\Juego tron\\Resources\\explosion.png"),
                                SizeMode = PictureBoxSizeMode.StretchImage,
                                Size = new Size(35, 35), 
                                Location = itemBox.Location, 
                                BackColor = Color.Transparent
                            };

                            gridPanel.Controls.Add(explosionPictureBox);
                            explosionPictureBox.BringToFront();

                            Timer explosionTimer = new Timer
                            {
                                Interval = 2500 // 2.5 segundos
                            };
                            explosionTimer.Tick += (s, ev) =>
                            {
                                // Eliminar la bomba
                                gridPanel.Controls.Remove(itemBox);
                                itemBox.Dispose();

                                // Eliminar la explosión
                                gridPanel.Controls.Remove(explosionPictureBox);
                                explosionPictureBox.Dispose();

                                explosionTimer.Stop();
                            };
                            explosionTimer.Start();

                            LiberarPoderes();

                            moveTimer.Stop();
                        }
                        else
                        {
                            gridPanel.Controls.Remove(itemBox);
                            itemBox.Dispose();
                        }
                    }
                    else if (itemBox.Tag is string shieldTag && shieldTag == "Escudo")
                    {
                        poderes.Push("Escudo");  // Guardar en la pila
                        UpdatePilaListBox();
                    }
                    else if (itemBox.Tag is string speedTag && speedTag == "Hipervelocidad")
                    {
                        poderes.Push("Hipervelocidad");  // Guardar en la pila
                        UpdatePilaListBox();
                    }
                    itemsToRemove.Add(itemBox);
                }
            }
        }

        foreach (var item in itemsToRemove)
        {
            gridPanel.Controls.Remove(item);
            item.Dispose();
        }
    }

    private void UpdatePilaListBox()
    {
        pilaListBox.Items.Clear();
        foreach (var poder in poderes)
        {
            pilaListBox.Items.Add(poder);
        }
    }

    public void AplicarPoder()
    {
        if (poderes.Count > 0)
        {
            string poder = poderes.Pop();

            if (poder == "Escudo")
            {
                isShieldActive = true;
                if (motoPictureBoxes.Count > 0)
                {
                    motoPictureBoxes[0].Image = Image.FromFile("C:\\Users\\Usuario\\Desktop\\Datos 1\\Juego tron\\Juego tron\\Resources\\motoescudada.png");
                }
                Timer shieldTimer = new Timer { Interval = 15000 };
                shieldTimer.Tick += (s, e) =>
                {
                    isShieldActive = false;
                    if (motoPictureBoxes.Count > 0)
                    {
                        motoPictureBoxes[0].Image = Image.FromFile("C:\\Users\\Usuario\\Desktop\\Datos 1\\Juego tron\\Juego tron\\Resources\\moto.png");
                    }
                    shieldTimer.Stop();
                };
                shieldTimer.Start();
            }
            else if (poder == "Hipervelocidad")
            {
                isHyperSpeedActive = true;
                if (motoPictureBoxes.Count > 0)
                {
                    motoPictureBoxes[0].Image = Image.FromFile("C:\\Users\\Usuario\\Desktop\\Datos 1\\Juego tron\\Juego tron\\Resources\\motoveloz.png");
                }
                int hyperSpeed = 50;
                moveTimer.Interval = hyperSpeed;
                Timer speedTimer = new Timer { Interval = 15000 }; 
                speedTimer.Tick += (s, e) =>
                {
                    isHyperSpeedActive = false;
                    motoList.Velocidad(); 
                    if (motoPictureBoxes.Count > 0)
                    {
                        motoPictureBoxes[0].Image = Image.FromFile("C:\\Users\\Usuario\\Desktop\\Datos 1\\Juego tron\\Juego tron\\Resources\\moto.png");
                    }
                    speedTimer.Stop();
                };
                speedTimer.Start();
            }

            UpdatePilaListBox(); 
        }
    }

    public void LiberarPoderes()
    {
        Random rand = new Random();
        while (poderes.Count > 0)
        {
            string poder = poderes.Pop();
            int randomX = rand.Next(0, cols);
            int randomY = rand.Next(0, rows);

            PictureBox poderBox = new PictureBox
            {
                Size = new Size(cellWidth / 2, cellHeight / 2),
                Location = new Point(randomX * cellWidth + cellWidth / 4, randomY * cellHeight + cellHeight / 4),
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            if (poder == "Escudo")
            {
                poderBox.Tag = "Escudo";
                poderBox.Image = Image.FromFile("C:\\Users\\Usuario\\Desktop\\Datos 1\\Juego tron\\Juego tron\\Resources\\escudo.png");
            }
            else if (poder == "Hipervelocidad")
            {
                poderBox.Tag = "Hipervelocidad";
                poderBox.Image = Image.FromFile("C:\\Users\\Usuario\\Desktop\\Datos 1\\Juego tron\\Juego tron\\Resources\\hipervelocidad.png");
            }

            gridPanel.Controls.Add(poderBox);
        }

        UpdatePilaListBox(); 
    }

    private void DestruirMoto(Point location)
    {
        if (!isShieldActive)
        {
            foreach (var motoPart in motoPictureBoxes)
            {
                gridPanel.Controls.Remove(motoPart);
                motoPart.Dispose();
            }
            motoPictureBoxes.Clear();

            PictureBox explosionPictureBox = new PictureBox
            {
                Image = Image.FromFile("C:\\Users\\Usuario\\Desktop\\Datos 1\\Juego tron\\Juego tron\\Resources\\explosion.png"),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Size = new Size(35, 35), 
                Location = location, 
                BackColor = Color.Transparent
            };

            gridPanel.Controls.Add(explosionPictureBox);
            explosionPictureBox.BringToFront();

            
            Timer explosionTimer = new Timer
            {
                Interval = 2500 
            };
            explosionTimer.Tick += (s, ev) =>
            {
                
                gridPanel.Controls.Remove(explosionPictureBox);
                explosionPictureBox.Dispose();

                explosionTimer.Stop();
            };
            explosionTimer.Start();

            // Liberar los poderes cuando la moto se destruye
            LiberarPoderes();

            moveTimer.Stop();
        }
    }



    private void MoveMoto(int dx, int dy)
    {
        Node current = motoList.GetHead();
        int prevX = current.X;
        int prevY = current.Y;

        // Actualiza la posición de la cabeza de la moto
        current.X = (current.X + dx + cols) % cols;
        current.Y = (current.Y + dy + rows) % rows;

        // Comprobar si la cabeza colisiona con la estela
        Node checkCollision = motoList.GetHead().Next; 
        while (checkCollision != null)
        {
            if (checkCollision.X == current.X && checkCollision.Y == current.Y)
            {
                DestruirMoto(new Point(current.X * cellWidth, current.Y * cellHeight));
                return;
            }
            checkCollision = checkCollision.Next;
        }

        current = current.Next;

        // Actualiza la posición de los nodos de la estela
        while (current != null)
        {
            int tempX = current.X;
            int tempY = current.Y;

            current.X = prevX;
            current.Y = prevY;

            prevX = tempX;
            prevY = tempY;

            current = current.Next;
        }

        UpdateMotoPosition(); // Actualiza la posición de la moto
        motoList.Combustible(); // Actualiza el combustible
        CheckItemCollision(); // Comprueba colisiones con ítems después de mover la moto
        motoList.UsarCombustibleEnCola();
    }

    public void UpdateMotoPosition()
    {
        Node current = motoList.GetHead();
        int index = 0;

        // Recorre cada nodo de la lista y actualiza la posición del PictureBox correspondiente
        while (current != null && index < motoPictureBoxes.Count)
        {
            motoPictureBoxes[index].Location = new Point(current.X * cellWidth, current.Y * cellHeight);
            current = current.Next;
            index++;
        }

        while (current != null)
        {
            PictureBox newSegment = new PictureBox
            {
                Size = new Size(cellWidth / 2, cellHeight / 2), // Hacer la estela más delgada
                Location = new Point(current.X * cellWidth + cellWidth / 4, current.Y * cellHeight + cellHeight / 4), // Centrar la estela
                BackColor = Color.LightBlue,
                SizeMode = PictureBoxSizeMode.StretchImage
            };
            motoPictureBoxes.Add(newSegment);
            gridPanel.Controls.Add(newSegment);
            current = current.Next;
        }
    }

    private void GridPanel_Paint(object sender, PaintEventArgs e)
    {
        for (int i = 0; i <= rows; i++)
        {
            for (int j = 0; j <= cols; j++)
            {
                int x = j * cellWidth;
                int y = i * cellHeight;

                using (Pen pen = new Pen(Color.Cyan, 2))
                {
                    e.Graphics.DrawRectangle(pen, x, y, cellWidth, cellHeight);
                }
            }
        }
    }

    private void InitializeComponent()
    {
        this.SuspendLayout();
        // 
        // Form1
        // 
        this.ClientSize = new System.Drawing.Size(800, 600);
        this.Name = "Form1";
        this.Load += new System.EventHandler(this.Form1_Load);
        this.ResumeLayout(false);
        this.Focus();

    }

    private void Form1_Load(object sender, EventArgs e)
    {

    }
}