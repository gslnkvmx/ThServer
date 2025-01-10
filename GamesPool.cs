using System.Net;

namespace ThServer
{
  internal class GamesPool
  {
    public List<SoloGameEntity> SoloGames { get; private set; }
    public List<DuoGameEntity> DuoGames { get; private set; }
    public Dictionary<int, IPEndPoint?[]> GamesClients { get; private set; }

    public GamesPool()
    {
      SoloGames = new List<SoloGameEntity>() { new SoloGameEntity(0) };
      DuoGames = new List<DuoGameEntity>() { new DuoGameEntity(0) };
      GamesClients = new Dictionary<int, IPEndPoint[]>();
    }

    public int AddSoloGame()
    {
      var newGame = new SoloGameEntity((byte)((SoloGames.Count)));
      SoloGames.Add(newGame);
      return newGame.Id;
    }

    public void RemoveSoloGame(byte gid)
    {
      SoloGames.RemoveAt(gid);
      GamesClients.Remove(gid);
    }

    public int AddDuoGame()
    {
      var newGame = new DuoGameEntity((byte)(DuoGames.Count + 100));
      DuoGames.Add(newGame);
      return newGame.Id;
    }

    public void RemoveDuoGame(byte gid)
    {
      DuoGames.RemoveAt(gid - 100);
      GamesClients.Remove(gid);
    }

    public void AddClient(IPEndPoint client, int gid)
    {
      if (!GamesClients.ContainsKey(gid))
      {
        GamesClients.Add(gid, new IPEndPoint[2] { client, null });
      }
      else
      {
        GamesClients[gid][1] = client;
      }
    }
  }
}