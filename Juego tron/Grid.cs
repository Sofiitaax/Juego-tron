using System;

namespace Juego_tron
{
    public class Grid
    {
        public NodosGrid[,] Nodes { get; private set; }

        public Grid(int rows, int cols)
        {
            Nodes = new NodosGrid[rows, cols];

            // Crear nodos
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    Nodes[row, col] = new NodosGrid { X = col, Y = row };
                }
            }

            // Conectar nodos
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    if (row > 0) Nodes[row, col].Up = Nodes[row - 1, col];
                    if (row < rows - 1) Nodes[row, col].Down = Nodes[row + 1, col];
                    if (col > 0) Nodes[row, col].Left = Nodes[row, col - 1];
                    if (col < cols - 1) Nodes[row, col].Right = Nodes[row, col + 1];
                }
            }
        }
    }
}
