// FILE: frontend_harness/src/components/NotesPage.tsx

import { useState, useEffect, type FormEvent } from "react";
import { apiClient } from "../apiClient";
import { type NoteDto, type CreateNoteDto } from "../types";
import { AxiosError } from "axios";

// Define an explicit type for the data sent in the command,
// including the new lookup fields for clarity
type NoteCommandData = Omit<CreateNoteDto, "vendorName"> & {
  contactEmail?: string;
  eventName?: string;
  vendorName?: string; // This maps to the backend VendorName field in the command
  taskTitle?: string;
};

type UpdateNoteDto = Pick<NoteDto, "id" | "content">;

const NotesPage = () => {
  const [notes, setNotes] = useState<NoteDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Form state for creating a new note
  const initialNewNoteState: Partial<NoteCommandData> = {
    content: "",
    contactId: null,
    eventId: null,
    taskId: null,
    vendorId: null,
    // String lookup fields
    contactEmail: undefined,
    eventName: undefined,
    vendorName: undefined,
    taskTitle: undefined,
  };

  const [newNote, setNewNote] =
    useState<Partial<NoteCommandData>>(initialNewNoteState);

  // State for which note is being edited
  const [editingNote, setEditingNote] = useState<NoteDto | null>(null);

  // --- NEW: PHASE 29 Dynamic Linkage State ---
  const [entityTypeToLink, setEntityTypeToLink] = useState<
    "Contact" | "Event" | "Vendor" | "Task" | null
  >(null);
  const [linkValue, setLinkValue] = useState("");
  // ------------------------------------------

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

  // --- PHASE 29: Dynamic Linkage Handlers ---
  const handleEntityTypeChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const type = e.target.value as
      | "Contact"
      | "Event"
      | "Vendor"
      | "Task"
      | "None";
    setEntityTypeToLink(type === "None" ? null : type);
    setLinkValue("");
    // Clear all previous link-related fields in newNote state
    setNewNote({
      ...newNote,
      contactId: null,
      eventId: null,
      taskId: null,
      vendorId: null,
      contactEmail: undefined,
      eventName: undefined,
      vendorName: undefined,
      taskTitle: undefined,
    });
  };

  const handleLinkValueChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setLinkValue(e.target.value);
  };

  const getLinkPlaceholder = () => {
    switch (entityTypeToLink) {
      case "Contact":
        return "Contact Email (e.g., jane@example.com)";
      case "Event":
        return "Event Name (e.g., Summer Gala 2026)";
      case "Vendor":
        return "Vendor Name (e.g., The Jazz Trio)";
      case "Task":
        return "Task Title (e.g., Book Venue)";
      default:
        return "Enter Name, Title, or Email";
    }
  };

  // --- PHASE 29: Payload Construction ---
  const constructCreatePayload = (): Partial<NoteCommandData> => {
    const payload: Partial<NoteCommandData> = {
      content: newNote.content,
    };

    if (entityTypeToLink === "Contact" && linkValue) {
      payload.contactEmail = linkValue;
    } else if (entityTypeToLink === "Event" && linkValue) {
      payload.eventName = linkValue;
    } else if (entityTypeToLink === "Vendor" && linkValue) {
      payload.vendorName = linkValue; // Maps to VendorName on backend command
    } else if (entityTypeToLink === "Task" && linkValue) {
      payload.taskTitle = linkValue;
    }
    // If IDs were manually put in (or if no dynamic link is selected,
    // they are sent as null/undefined, which is fine)

    return payload;
  };
  // ------------------------------------------

  const handleCreateSubmit = async (e: FormEvent) => {
    e.preventDefault();

    if (!newNote.content) {
      setError("Content is required.");
      return;
    }

    const payload = constructCreatePayload();

    try {
      // NOTE: We don't map string lookups to GUIDs on the FE. The FE only sends
      // the string values and the BE handler resolves them.
      const response = await apiClient.post<NoteDto>("/api/notes", payload);
      setNotes([response.data, ...notes]);

      setNewNote(initialNewNoteState); // Reset form
      setEntityTypeToLink(null);
      setLinkValue("");

      setError(null);
    } catch (err: unknown) {
      console.error(err);
      if (err instanceof AxiosError && err.response?.data?.errors) {
        // Find the 'Must Have At Least One Link' error and present it clearly
        const validationMessage = Object.values(err.response.data.errors)
          .flat()
          .join(", ");
        setError(
          validationMessage ||
            "Failed to create note. Ensure at least one ID is provided."
        );
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

  if (loading) return <p>Loading notes...</p>;
  return (
    <div className="module-page">
      <h2>Notes Module</h2>
      {error && (
        <p className="error-message" style={{ color: "red" }}>
          {error}
        </p>
      )}

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

        {/* --- Foreign Key Inputs (Dynamic Link for Create) --- */}
        {!editingNote && (
          <fieldset
            style={{
              border: "1px dashed #666",
              padding: "1rem",
              margin: "1rem 0",
            }}>
            <legend style={{ padding: "0 0.5rem" }}>
              Attach Note By ID or Lookup
            </legend>

            {/* Link Type Selector */}
            <div style={{ marginBottom: "0.5rem" }}>
              <label style={{ display: "block" }}>Link Type:</label>
              <select
                value={entityTypeToLink ?? "None"}
                onChange={handleEntityTypeChange}
                style={{ width: "100%", padding: "4px" }}>
                <option value="None">-- Select Entity Type --</option>
                <option value="Contact">Contact (by Email)</option>
                <option value="Event">Event (by Name)</option>
                <option value="Vendor">Vendor (by Name)</option>
                <option value="Task">Task (by Title)</option>
              </select>
            </div>

            {/* Lookup Value Input */}
            {entityTypeToLink && (
              <div style={{ marginBottom: "0.5rem" }}>
                <label style={{ display: "block" }}>Lookup Value:</label>
                <input
                  type={entityTypeToLink === "Contact" ? "email" : "text"}
                  value={linkValue}
                  onChange={handleLinkValueChange}
                  placeholder={getLinkPlaceholder()}
                  style={{ width: "100%" }}
                />
              </div>
            )}

            <p style={{ fontSize: "0.8rem", color: "#aaa", marginTop: "1rem" }}>
              *If you use the dropdown above, only the lookup value is sent. If
              you need to link by GUID, please use the Task/Vendor/Contact
              pages.
            </p>
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
      <table className="harness-table" style={{ width: "100%" }}>
        <thead>
          <tr>
            <th>ID</th>
            <th>Content</th>
            <th>Contact ID</th>
            <th>Event ID</th>
            <th>Task ID</th>
            <th>Vendor ID</th>
            <th>Vendor Name</th> {/* Granularity */}
            <th>Created At</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {notes.map((note) => (
            <tr key={note.id}>
              <td>{note.id.substring(0, 8)}...</td>
              <td>{note.content.substring(0, 50)}...</td>
              <td>
                {note.contactId
                  ? note.contactId.substring(0, 8) + "..."
                  : "N/A"}
              </td>
              <td>
                {note.eventId ? note.eventId.substring(0, 8) + "..." : "N/A"}
              </td>
              <td>
                {note.taskId ? note.taskId.substring(0, 8) + "..." : "N/A"}
              </td>
              <td>
                {note.vendorId ? note.vendorId.substring(0, 8) + "..." : "N/A"}
              </td>
              <td>{note.vendorName ?? "N/A"}</td>
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
