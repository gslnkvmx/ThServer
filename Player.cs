namespace ThServer
{
  public enum Directions
  {
    Up,
    Down,
    Left,
    Right,
    Stop
  }
  internal class Player
  {
    public int[] Position { get; private set; }
    public byte Score { get; private set; }
    public byte Id { get; private set; }

    public Directions Direction { get; private set; } = Directions.Stop;

    public Player(byte id)
    {
      Position = new int[2];
      Id = id;
      Score = 0;
    }

    public void SetDirection(Directions direction)
    {
      Direction = direction;
    }

    public void UpScore()
    {
      Score++;
    }

    public void SetPosition(int x, int y)
    {
      Position = [x, y];
    }
  }
}