using Summer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Model
{
    //在MMO中需要网络同步的实体,作为基类存在
    public class Entity
    {
        #region Entity属性相关
        private int id;
        private Vector3Int position;
        private Vector3Int direction;
        public int ID { get { return id; } }
        public Vector3Int Position{get=>position;set => position = value;}
        public Vector3Int Direction{get => direction; set => direction = value;}



        public Entity(int id )
        {
            this.id = id;
        }
        public Entity(int id,Vector3Int position, Vector3Int direction)
        {
            this.id = id;
            this.position = position;
            this.direction = direction;
        }




        #endregion
    }
}
