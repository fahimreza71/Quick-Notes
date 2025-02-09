using Microsoft.AspNetCore.Mvc;
using NotesApi.Models;
using NotesApi.Repositories;


namespace NotesApi.Controllers
{
    [Route("api/notes")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        [HttpGet("get-notes")]
        public IActionResult GetNotes([FromHeader] string sessionId)
        {
            if (!MemoryDB.ActiveSession.ContainsKey(sessionId))
                return Unauthorized("Invalid session. Please login !!!");
        
            string username = MemoryDB.ActiveSession[sessionId];
            var userNotes = MemoryDB.Notes.Where(n => n.UserId == username).ToList();
            return Ok(userNotes);
        }

        [HttpPost("create-note")]
        public IActionResult CreateNote([FromHeader] string sessionId, [FromBody] Note note)
        {
            if (!MemoryDB.ActiveSession.ContainsKey(sessionId))
                return Unauthorized("Invalid session. Please login !!!");

            note.UserId = MemoryDB.ActiveSession[sessionId];

            if (note.Content?.Length > 100)
                return BadRequest("Note Content Should less than 100 Characters !!!");

            MemoryDB.Notes.Add(note);
            return Ok(note);
        }

        [HttpPut("update-note/{noteId}")]
        public IActionResult UpdateNote(string noteId, [FromHeader] string sessionId, [FromBody] Note updatedNote)
        {
            if (!MemoryDB.ActiveSession.ContainsKey(sessionId))
                return Unauthorized("Invalid session. Please login!");

            string email = MemoryDB.ActiveSession[sessionId];
            var existingNote = MemoryDB.Notes.FirstOrDefault(n => n.NoteId == noteId);

            if (existingNote == null)
                return NotFound("Note not found!");

            existingNote.Type = updatedNote.Type;
            existingNote.Content = updatedNote.Content?.Length <= 100 ? updatedNote.Content : existingNote.Content;
            existingNote.ReminderDate = updatedNote.ReminderDate;
            existingNote.DueDate = updatedNote.DueDate;
            existingNote.IsCompleted = updatedNote.IsCompleted;
            existingNote.BookmarkUrl = updatedNote.BookmarkUrl;

            return Ok(existingNote);
        }

        [HttpDelete("delete-note/{noteId}")]
        public IActionResult DeleteNote(string noteId, [FromHeader] string sessionId)
        {
            if (!MemoryDB.ActiveSession.ContainsKey(sessionId))
                return Unauthorized("Invalid session. Please login!");

            string email = MemoryDB.ActiveSession[sessionId];
            var note = MemoryDB.Notes.FirstOrDefault(n => n.NoteId == noteId && n.UserId == email);

            if (note == null)
                return NotFound("Note not found or does not belong to the logged-in user!");

            MemoryDB.Notes.Remove(note);
            return Ok(new { message = "Note deleted successfully!" });
        }

    }
}
