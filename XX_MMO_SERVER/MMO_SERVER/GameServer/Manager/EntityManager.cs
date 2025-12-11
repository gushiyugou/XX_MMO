using GameServer.Model;
using Summer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Manager
{
    public class EntityManager : Singleton<EntityManager>
    {
        private int index = 1;
        private Dictionary<int,Entity> allEntityDic = new Dictionary<int,Entity>();


        public Entity CreateEntity()
        {
            lock (this)
            {
                Entity entity = new Entity(index++, Vector3Int.zero, Vector3Int.zero);
                allEntityDic[entity.ID] = entity;

                return entity;
            }
        }
    }
}
