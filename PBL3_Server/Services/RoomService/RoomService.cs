﻿using Microsoft.EntityFrameworkCore;
using PBL3_Server.Models;

namespace PBL3_Server.Services.RoomService
{
    public class RoomService  : IRoomService
    {
        private static List<Room> Rooms = new List<Room>();

        private readonly DataContext _context;

        public RoomService(DataContext context)
        {
            _context = context;
        }
        public async Task<List<Room>> GetAllRooms()
        {
            var rooms = await _context.Rooms.ToListAsync();
            return rooms;
        }

        public async Task<Room?> GetSingleRoom(string roomID)
        {
            var room = await _context.Rooms.FindAsync(roomID);
            if(room is null)
            {
                return null;
            }
            return room;
        }
    }
}
