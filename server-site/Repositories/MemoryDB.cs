using NotesApi.Models;

namespace NotesApi.Repositories
{
    public static class MemoryDB
    {
        public static List<User> Users = new();
        public static List<Note> Notes = new();
        public static Dictionary<string, string> ActiveSession = new();
    }
}
