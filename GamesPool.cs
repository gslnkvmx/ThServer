using System.Net;

namespace ThServer
{
  internal class GamesPool
  {
    public List<SoloGameEntity> SoloGames { get; private set; }
    public Dictionary<int, IPEndPoint> GamesClients { get; private set; }

    public GamesPool()
    {
      SoloGames = new List<SoloGameEntity>() { new SoloGameEntity(0) };
      GamesClients = new Dictionary<int, IPEndPoint>();
    }

    public int AddSoloGame()
    {
      var newGame = new SoloGameEntity((byte)(SoloGames.Count));
      SoloGames.Add(newGame);
      return newGame.Id;
    }

    public void AddClient(IPEndPoint client, int gid)
    {
      GamesClients.Add(gid, client);
    }
  }
}