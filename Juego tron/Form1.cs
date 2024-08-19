using Juego_tron;
using System.Drawing;
using System.Windows.Forms;

public partial class Form1 : Form
{
    private Grid grid;
    private NodosGrid currentNode;
    private PictureBox motoPictureBox;
    private int cellWidth = 49;  // Ancho de cada celda
    private int cellHeight = 49; // Altura de cada celda

    public Form1()
    {
        InitializeComponent();
        InitializeGrid();
        InitializeMoto();
        this.KeyDown += new KeyEventHandler(OnKeyDown);
    }

    private void InitializeGrid()
    {
        int rows = 15;
        int cols = 26;
        grid = new Grid(rows, cols);
        currentNode = grid.Nodes[0, 0]; // Iniciar en la esquina superior izquierda
    }

    private void InitializeMoto()
    {
        motoPictureBox = new PictureBox
        {
            Width = cellWidth,
            Height = cellHeight,
            BackColor = Color.Blue // Cambia el color o la imagen según prefieras
        };

        this.Controls.Add(motoPictureBox);
        UpdatePlayerPosition();
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        switch (e.KeyCode)
        {
            case Keys.Up:
                if (currentNode.Up != null)
                    currentNode = currentNode.Up;
                break;
            case Keys.Down:
                if (currentNode.Down != null)
                    currentNode = currentNode.Down;
                break;
            case Keys.Left:
                if (currentNode.Left != null)
                    currentNode = currentNode.Left;
                break;
            case Keys.Right:
                if (currentNode.Right != null)
                    currentNode = currentNode.Right;
                break;
        }

        // Actualizar la interfaz gráfica o la posición de la moto
        UpdatePlayerPosition();
    }

    private void UpdatePlayerPosition()
    {
        // Actualiza la posición de la moto en la interfaz
        motoPictureBox.Location = new Point(currentNode.X * cellWidth, currentNode.Y * cellHeight);
    }

    private void InitializeComponent()
    {
            this.SuspendLayout();
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(278, 244);
            this.Name = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

    }

    private void Form1_Load(object sender, System.EventArgs e)
    {

    }
}

