namespace PBL3_Server.Services.RoomService
{
    public interface IRoomService
    {
        Task<List<Room>> GetAllRooms();
        Task<Room?> GetSingleRoom(string roomID);
        Task<List<Room>> AddRoom(Room room);
    }
}
