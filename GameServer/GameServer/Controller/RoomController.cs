using System.Text;
using Common;
using GameServer.Servers;
namespace GameServer.Controller
{
    class RoomController:BaseController
    {
        // 构造 交给controllerManager进行管理
        public RoomController()
        {
            requestCode = RequestCode.Room;
        }
        
        // 创建房间
        public string CreateRoom(string data, Client client, Server server)
        {
            server.CreateRoom(client); // 创建房间
            return ((int)ReturnCode.Success).ToString(); // 返回一个创建成功的字符出
                // .ToString()+","+ ((int)RoleType.Blue).ToString();
        }
        
        // 房间列表 通过server取到房间列表 通过room里的list取到client，再通过client取得战绩 返回给客户端
        public string ListRoom(string data, Client client, Server server)
        {
            // 数据不需要读取，用来请求房间列表信息
            StringBuilder sb = new StringBuilder();
            // 遍历房间集合
            foreach(Room room in server.GetRoomList())
            {
                // 先判断房间状态
                if (room.IsWaitingJoin())
                {
                    // 房主信息返回给客户端，组拼字符串
                    sb.Append(room.GetHouseOwnerData()+"|");
                }
            }
            // 空串返回0 客户端判断是否为0  如果是0就是空的房间列表
            if (sb.Length == 0)
            {
                sb.Append("0");
            }
            else // 如果有去除字符串最后的"｜"
            {
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }
        public string JoinRoom(string data, Client client, Server server)
        {
            int id = int.Parse(data);
            // 先得到房间
            Room room = server.GetRoomById(id);
            if(room == null)
            {
                return ((int)ReturnCode.NotFound).ToString();
            }
            // 找到需要判断房间是否可以加入 房间满员
            else if (room.IsWaitingJoin() == false)
            {
                return ((int)ReturnCode.Fail).ToString();
            }
            else
            {
                // 把客户端添加到房间里
                room.AddClient(client);
                // 将房间所有信息返回给客户端
                string roomData = room.GetRoomData();//"returncode,roletype-id,username,tc,wc|id,username,tc,wc"
                // 当心新的玩家加入房间的时候，在这个房间里发送一个广播将信息广播给其他玩家的客户端
                room.BroadcastMessage(client, ActionCode.UpdateRoom, roomData);//roomDate不包含returncode
                return ((int)ReturnCode.Success) +  "-" + roomData;
                // "," + ((int)RoleType.Red).ToString()+
            }
        }
        
        // 退出房间，两种情况，是房主和不是房主 
        public string QuitRoom(string data, Client client, Server server)
        {
            
            bool isHouseOwner = client.IsHouseOwner();
            Room room = client.Room;
            if (isHouseOwner)
            {
                // 先向其他房间广播，让其他客户端进行退出，client房主最后处理 接收到QuitRoom会返回房间列表
                room.BroadcastMessage(client, ActionCode.QuitRoom, ((int)ReturnCode.Success).ToString());
                room.Close(); // 当前房主的退出销毁房间
                return ((int)ReturnCode.Success).ToString();
            }
            else
            {
                // 移除之后通知其他客户端 
                client.Room.RemoveClient(client);
                // 更新时只有一条userdate
                room.BroadcastMessage(client, ActionCode.UpdateRoom, room.GetRoomData());
                return ((int)ReturnCode.Success).ToString();
            }
        }
    }
}
