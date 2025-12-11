using Summer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Model
{
    //空间，地图 场景
    public class Space
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Music { get; set; }

        private Dictionary<int ,Character> characterDic = new Dictionary<int ,Character>();


        //角色进入场景
        public void CharacterJoin(Connection connecttion,Character character)
        {
            characterDic[character.ID] = character;
        }
    }
}
