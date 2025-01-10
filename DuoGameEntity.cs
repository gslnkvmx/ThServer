using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ThServer
{
  internal class DuoGameEntity
  {
    private System.Timers.Timer gameTimer;  // create a new instance of the dispatcher timer called game timer
    private int timeLeft = 10;

    bool gameOver = false;
    static int size = 16;
    private readonly Maze _maze;
    public Player Player1 { get; private set; }
    public Player Player2 { get; private set; }
    public byte Id { get; private set; }
    public byte[] _treasure;


    public DuoGameEntity(byte gid)
    {
      _maze = new Maze(size / 2);
      _treasure = new byte[2];
      Player1 = new Player(1);
      Player2 = new Player(2);
      Id = gid;
      gameTimer = new System.Timers.Timer(1000);
    }

    public async Task StartGameLoop(CommunicationHandler handler, GamesPool pool)
    {
      var client = pool.GamesClients[Id];

      gameTimer.Elapsed += GameTimer_Tick;
      gameTimer.Start();
      Stopwatch sw = Stopwatch.StartNew();

      const int frameInterval = 200;

      while (!gameOver)
      {
        // Проверяем, прошло ли достаточно времени
        if (sw.ElapsedMilliseconds >= frameInterval)
        {
          MovePlayer();
          PickTreasure();

          await handler.SendResponseBytesAsync(GetStateResponse(), client);

          sw.Restart();
        }
      }

      gameTimer.Stop();

      byte[] response = new byte[6]; //283 для поля 16 на 16

      //TH
      response[0] = 84;
      response[1] = 72;
      response[2] = Id;
      response[3] = winner.Id;
      //100 - окончание игры
      response[4] = 100;
      response[5] = (byte)Score;

      await handler.SendResponseBytesAsync(response, client);
    }


    private void GameTimer_Tick(object sender, EventArgs e)
    {
      timeLeft--; // Уменьшаем оставшееся время

      if (timeLeft <= 0)
      {
        gameOver = true;
      }
    }

    private void MovePlayer(byte palyerId)
    {
      var movingPlayer = Player1;
      if (palyerId == 2)
      {
        movingPlayer = Player2;
      }
      var prevPos = movingPlayer.Position;
      int[] newPos = movingPlayer.Direction switch
      {
        Directions.Stop => prevPos,
        Directions.Up => [prevPos[0], prevPos[1] - 1],
        Directions.Down => [prevPos[0], prevPos[1] + 1],
        Directions.Left => [prevPos[0] - 1, prevPos[1]],
        Directions.Right => [prevPos[0] + 1, prevPos[1]],
        _ => throw new NotImplementedException(),
      };
      if (!(newPos[0] < 0) && !(newPos[0] >= size) && !(newPos[1] < 0) && !(newPos[1] >= size) && !_maze.MazeThick[newPos[1], newPos[0]]) movingPlayer.SetPosition(newPos[0], newPos[1]);
    }

    private void PickTreasure()
    {
      if ((Player1.Position[0] == _treasure[1] && Player1.Position[1] == _treasure[0]) || (Player2.Position[0] == _treasure[1] && Player2.Position[1] == _treasure[0]))
      {
        if (Player1.Position[0] == _treasure[1] && Player1.Position[1] == _treasure[0]) Player1.UpScore();
        else Player2.UpScore();

        Random rnd = new Random();

        int t = rnd.Next(0, _maze.Road.Count - 1);
        _treasure[0] = (byte)_maze.Road[t][0];
        _treasure[1] = (byte)_maze.Road[t][1];
      }
    }

    public byte[] GetStateResponse()
    {
      byte[] response = new byte[9]; //283 для поля 16 на 16

      //TH
      response[0] = 84;
      response[1] = 72;
      //game id (100 <= gid <= 255)
      response[2] = Id;
      response[3] = Player.Id;

      //Player info
      response[4] = (byte)Player.Position[0];
      response[5] = (byte)Player.Position[1];
      response[6] = (byte)Score;

      //_treasures
      Array.Copy(_treasure, 0, response, 7, 2);

      return response;
    }

    public bool InitGame()
    {
      try
      {
        _maze.CreateMaze();
        _maze.printMaze();

        Random rnd = new Random();

        int i = rnd.Next(0, _maze.Road.Count - 1);
        Player.SetPosition(_maze.Road[i][1], _maze.Road[i][0]);

        int t = rnd.Next(0, _maze.Road.Count - 1);
        _treasure[0] = (byte)_maze.Road[t][0];
        _treasure[1] = (byte)_maze.Road[t][1];

        Score = 0;

        return true;
      }
      catch (Exception ex)
      {
        Console.WriteLine("In init: " + ex.Message);
        return false;
      }
    }

    public byte[] GetInitResponse()
    {
      var init = InitGame();
      byte[] response = new byte[5 + size * size + 3 + 2]; //283 для поля 16 на 16

      //TH
      response[0] = 84;
      response[1] = 72;
      //is initializtion successful
      response[2] = (byte)(init ? 1 : 0);
      //game id (100 <= gid <= 255)
      response[3] = Id;
      response[4] = Player.Id;

      byte[] mazeListed = new byte[size * size];

      for (int i = 0; i < size; i++)
      {
        for (int j = 0; j < size; j++)
        {
          mazeListed[i * size + j] = _maze.MazeThick[i, j] ? (byte)1 : (byte)0;
        }
      }

      //maze
      Array.Copy(mazeListed, 0, response, 5, mazeListed.Length);

      //Player info
      response[261] = (byte)Player.Position[0];
      response[262] = (byte)Player.Position[1];
      response[263] = (byte)Score;

      //_treasures
      Array.Copy(_treasure, 0, response, 264, 2);

      return response;
    }
  }
}
