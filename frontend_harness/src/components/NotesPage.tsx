import { useState, useEffect, type FormEvent } from "react";
import { apiClient } from "../apiClient";
import { type NoteDto, type CreateNoteDto } from "../types";
import { AxiosError } from "axios";

type UpdateNoteDto = Pick<NoteDto, "id" | "content">;

const NotesPage = () => {
  const [notes, setNotes] = useState<NoteDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Form state for creating a new note
  const [newNote, setNewNote] = useState<CreateNoteDto>({
    content: "",
    contactId: null,
    eventId: null,
    taskId: null,
    vendorId: null,
  });

  // State for which note is being edited
  const [editingNote, setEditingNote] = useState<NoteDto | null>(null);

  const fetchNotes = async () => {
    try {
      setLoading(true);
      const response = await apiClient.get<NoteDto[]>("/api/notes");
      setNotes(response.data);
      setError(null);
    } catch (err) {
      setError("Failed to fetch notes.");
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchNotes();
  }, []);

  const handleCreateSubmit = async (e: FormEvent) => {
    e.preventDefault();
    if (!newNote.content) {
      setError("Content is required.");
      return;
    }

    const payload: CreateNoteDto = {
      ...newNote,
      contactId: newNote.contactId || null,
      eventId: newNote.eventId || null,
      taskId: newNote.taskId || null,
      vendorId: newNote.vendorId || null,
    };

    try {
      const response = await apiClient.post<NoteDto>("/api/notes", payload);
      setNotes([response.data, ...notes]);
      setNewNote({
        content: "",
        contactId: null,
        eventId: null,
        taskId: null,
        vendorId: null,
      });
      setError(null);
    } catch (err: unknown) {
      // <-- FIX 5 (no-explicit-any)
      console.error(err);
      // Type-safe error message
      if (err instanceof AxiosError && err.response?.data?.errors) {
        setError(Object.values(err.response.data.errors).join(", "));
      } else {
        setError("Failed to create note. Ensure at least one ID is provided.");
      }
    }
  };

  const handleUpdateSubmit = async (e: FormEvent) => {
    e.preventDefault();
    if (!editingNote) return;

    const updatePayload: UpdateNoteDto = {
      id: editingNote.id,
      content: editingNote.content,
    };

    try {
      await apiClient.put(`/api/notes/${editingNote.id}`, updatePayload);
      setNotes(notes.map((n) => (n.id === editingNote.id ? editingNote : n)));
      setEditingNote(null);
      setError(null);
    } catch (err: unknown) {
      // <-- FIX 6 (no-explicit-any)
      console.error(err);
      setError("Failed to update note.");
    }
  };

  const handleDelete = async (id: string) => {
    if (window.confirm("Are you sure you want to delete this note?")) {
      try {
        await apiClient.delete(`/api/notes/${id}`);
        setNotes(notes.filter((n) => n.id !== id));
        setError(null);
      } catch (err) {
        setError("Failed to delete note.");
        console.error(err);
      }
    }
  };

  // --- Render Logic ---
  // (No changes below this line)

  if (loading) return <p>Loading notes...</p>;

  return (
    <div className="module-page">
      <h2>Notes Module</h2>
      {error && <p className="error-message">{error}</p>}

      {/* --- Create or Edit Form --- */}
      <form onSubmit={editingNote ? handleUpdateSubmit : handleCreateSubmit}>
        <h3>{editingNote ? "Edit Note" : "Create New Note"}</h3>
        <textarea
          placeholder="Note content..."
          value={editingNote ? editingNote.content : newNote.content}
          onChange={(e) =>
            editingNote
              ? setEditingNote({ ...editingNote, content: e.target.value })
              : setNewNote({ ...newNote, content: e.target.value })
          }
        />

        {/* --- Foreign Key Inputs (Only for Create) --- */}
        {!editingNote && (
          <fieldset>
            <legend>Attach to (at least one):</legend>
            <p>
              (Copy/paste an ID from another harness page. Leave blank for
              null.)
            </p>
            <div className="form-grid">
              <input
                type="text"
                placeholder="Contact ID"
                value={newNote.contactId || ""}
                onChange={(e) =>
                  setNewNote({ ...newNote, contactId: e.target.value })
                }
              />
              <input
                type="text"
                placeholder="Event ID"
                value={newNote.eventId || ""}
                onChange={(e) =>
                  setNewNote({ ...newNote, eventId: e.target.value })
                }
              />
              <input
                type="text"
                placeholder="Task ID"
                value={newNote.taskId || ""}
                onChange={(e) =>
                  setNewNote({ ...newNote, taskId: e.target.value })
                }
              />
              <input
                type="text"
                placeholder="Vendor ID"
                value={newNote.vendorId || ""}
                onChange={(e) =>
                  setNewNote({ ...newNote, vendorId: e.target.value })
                }
              />
            </div>
          </fieldset>
        )}

        <button type="submit">
          {editingNote ? "Update Note" : "Create Note"}
        </button>
        {editingNote && (
          <button type="button" onClick={() => setEditingNote(null)}>
            Cancel Edit
          </button>
        )}
      </form>

      {/* --- Notes Table --- */}
      <h3>Existing Notes</h3>
      <table className="harness-table">
        <thead>
          <tr>
            <th>ID</th>
            <th>Content</th>
            <th>Contact ID</th>
            <th>Event ID</th>
            <th>Task ID</th>
            <th>Vendor ID</th>
            <th>Created At</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {notes.map((note) => (
            <tr key={note.id}>
              <td>{note.id}</td>
              <td>{note.content}</td>
              <td>{note.contactId}</td>
              <td>{note.eventId}</td>
              <td>{note.taskId}</td>
              <td>{note.vendorId}</td>
              <td>{new Date(note.createdAt).toLocaleString()}</td>
              <td>
                <button
                  className="edit-button"
                  onClick={() => setEditingNote(note)}>
                  Edit
                </button>
                <button
                  className="delete-button"
                  onClick={() => handleDelete(note.id)}>
                  Delete
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default NotesPage;
