using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThServer
{
    internal class Maze
    {
        private int _size;
        private int _counter = 1;
        private int[] _mazeLine;
        private bool[,] v_walls_;
        private bool[,] h_walls_;

        private bool[,] _mazeThick;
        public bool[,] MazeThick { get; private set; }
        public List<int[]> Road { get; private set; }
        

        Random random = new Random();

        public Maze(int size)
        {
            _size = size;
            _mazeLine = new int[_size];
            h_walls_ = new bool[_size, _size];
            v_walls_ = new bool[_size, _size];
            _mazeThick = new bool[_size * 2, _size * 2];
            MazeThick = new bool[_size * 2, _size * 2];
            Road = new List<int[]>();
        }

        public bool[,] CreateMaze() 
        {
            generateMaze();
            formThickMaze();

            MazeThick = _mazeThick;

            for (int i = 0; i < _size * 2; i++)
            {
                for (int j = 0; j < _size * 2; j++)
                {
                    if (!_mazeThick[i, j])
                    {
                        Road.Add(new int[2] {i,j});
                    }
                }
            }

            return _mazeThick;
        }

        void assignUniqueSet()
        {
            for (int i = 0; i < _size; i++)
            {
                if (_mazeLine[i] == 0)
                {
                    /* Присваиваем ячейки уникальное множество */
                    _mazeLine[i] = _counter;
                    _counter++;
                }
            }
        }

        void mergeSet(int index, int element)
        {
            int mutableSet = _mazeLine[index + 1];
            for (int j = 0; j < _size; j++)
            {
                if (_mazeLine[j] == mutableSet)
                {
                    _mazeLine[j] = element;
                }
            }
        }

        void addingVerticalWalls(int row)
        {
            for (int i = 0; i < _size - 1; i++)
            {
                bool choise = random.NextDouble() < 0.5;
                if (choise == true || _mazeLine[i] == _mazeLine[i + 1])
                {
                    v_walls_[row, i] = true;
                }
                else
                {
                    mergeSet(i, _mazeLine[i]);
                }
            }
            v_walls_[row, _size - 1] = true;
        }

        int calculateUniqueSet(int element)
        {
            int countUniqSet = 0;
            for (int i = 0; i < _size; i++)
            {
                if (_mazeLine[i] == element)
                {
                    countUniqSet++;
                }
            }
            return countUniqSet;
        }

        void addingHorizontalWalls(int row)
        {
            for (int i = 0; i < _size; i++)
            {
                bool choise = random.NextDouble() < 0.5;
                if (calculateUniqueSet(_mazeLine[i]) != 1 && choise == true)
                {
                    h_walls_[row, i] = true;
                }
            }
        }

        int calculateHorizontalWalls(int element, int row)
        {
            int countHorizontalWalls = 0;
            for (int i = 0; i < _size; i++)
            {
                if (_mazeLine[i] == element && h_walls_[row, i] == false)
                {
                    countHorizontalWalls++;
                }
            }
            return countHorizontalWalls;
        }

        void checkedHorizontalWalls(int row)
        {
            for (int i = 0; i < _size; i++)
            {
                if (calculateHorizontalWalls(_mazeLine[i], row) == 0)
                {
                    h_walls_[row, i] = false;
                }
            }
        }

        void preparatingNewLine(int row)
        {
            for (int i = 0; i < _size; i++)
            {
                if (h_walls_[row, i] == true)
                {
                    _mazeLine[i] = 0;
                }
            }
        }

        void checkedEndLine()
        {
            for (int i = 0; i < _size - 1; i++)
            {
                if (_mazeLine[i] != _mazeLine[i + 1])
                {
                    v_walls_[_size - 1, i] = false;
                    mergeSet(i, _mazeLine[i]);
                }
                h_walls_[_size - 1, i] = true;
            }
            h_walls_[_size - 1, _size - 1] = true;
        }

        void addingEndLine()
        {
            assignUniqueSet();
            addingVerticalWalls(_size - 1);
            checkedEndLine();
        }

        public void generateMaze()
        {
            for (int j = 0; j < _size - 1; j++)
            {
                assignUniqueSet();
                addingVerticalWalls(j);
                addingHorizontalWalls(j);
                checkedHorizontalWalls(j);
                preparatingNewLine(j);
            }
            addingEndLine();
        }

        public void formThickMaze()
        {
            for (int i = 0; i < _size; i++)
            {
                for (int j = 0; j < _size; j++)
                {
                    if (v_walls_[i, j])
                    {
                        _mazeThick[i * 2, j * 2 + 1] = true;
                        if (i + 1 < _size * 2) _mazeThick[i * 2 + 1, j * 2 + 1] = true;
                        if (i - 1 >= 0) _mazeThick[i * 2 - 1, j * 2 + 1] = true;
                    }
                    if (h_walls_[i, j])
                    {
                        _mazeThick[i * 2 + 1, j * 2] = true;
                        if (j + 1 < _size * 2) _mazeThick[i * 2 + 1, j * 2 + 1] = true;
                        if (j - 1 >= 0) _mazeThick[i * 2 + 1, j * 2 - 1] = true;
                    }
                }
            }
        }

        public void printMaze()
        {
            for (int i = 0; i < _size * 2; i++)
            {
                for (int j = 0; j < _size * 2; j++)
                {
                    Console.Write(_mazeThick[i, j] ? "\u2588" : " ");
                }
                Console.WriteLine();
            }
        }
    }
}
