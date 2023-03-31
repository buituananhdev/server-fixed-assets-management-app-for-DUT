namespace PBL3_Server.Services.RoomService
{
    public interface IRoomService
    {
        Task<List<Room>> GetAllRooms();
    }
}
