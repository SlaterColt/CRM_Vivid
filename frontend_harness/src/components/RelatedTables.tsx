import { useEffect, useState } from "react";
import { useSelectionContext } from "../context/useSelectionContext";
import { apiClient } from "../apiClient";
import type {
  TaskDto,
  NoteDto,
  EventDto,
  ContactDto,
  Guid,
  EmailLogDto,
} from "../types";

// Add vendorId to the props of the related tables
interface RelatedTableProps {
  contactId?: Guid | null;
  eventId?: Guid | null;
  vendorId?: Guid | null;
}

// --- Utility function to determine the API path and selection ID ---
const getApiContext = (
  props: RelatedTableProps,
  selectedContactId: Guid | null,
  selectedEventId: Guid | null,
  resource: "tasks" | "notes"
) => {
  // 1. Check for explicit props (used by Command Centers)
  if (props.vendorId) {
    return {
      id: props.vendorId,
      url: `/api/${resource}?vendorId=${props.vendorId}`,
      label: "Vendor",
    };
  }
  if (props.contactId) {
    return {
      id: props.contactId,
      url: `/api/${resource}?contactId=${props.contactId}`,
      label: "Contact",
    };
  }
  if (props.eventId) {
    return {
      id: props.eventId,
      url: `/api/${resource}?eventId=${props.eventId}`,
      label: "Event",
    };
  }

  // 2. Fall back to context (used by old pages/global lists)
  if (selectedContactId) {
    return {
      id: selectedContactId,
      url: `/api/${resource}?contactId=${selectedContactId}`,
      label: "Contact",
    };
  }
  if (selectedEventId) {
    return {
      id: selectedEventId,
      url: `/api/${resource}?eventId=${selectedEventId}`,
      label: "Event",
    };
  }
  return { id: null, url: null, label: null };
};

// --- 1. RelatedTasks ---
export function RelatedTasks(props: RelatedTableProps) {
  const { selectedContactId, selectedEventId } = useSelectionContext();
  const [tasks, setTasks] = useState<TaskDto[]>([]);
  const [loading, setLoading] = useState(false);

  const context = getApiContext(
    props,
    selectedContactId,
    selectedEventId,
    "tasks"
  );
  const selectedId = context.id;
  const apiUrl = context.url;
  const contextLabel = context.label;

  useEffect(() => {
    if (!selectedId || !apiUrl) {
      setTasks([]);
      return;
    }

    const fetchTasks = async () => {
      setLoading(true);
      try {
        const response = await apiClient.get<TaskDto[]>(apiUrl);
        setTasks(response.data);
      } catch (err) {
        console.error("Failed to fetch related tasks:", err);
      } finally {
        setLoading(false);
      }
    };

    fetchTasks();
  }, [selectedId, apiUrl]);

  if (!selectedId) return null;
  if (loading) return <p>Loading related tasks...</p>;

  const safeContextLabel = contextLabel as string;

  return (
    <div style={{ marginTop: "2rem" }}>
      {/* FIX: Show Full ID */}
      <h3>
        Related Tasks (for {safeContextLabel} ID: {selectedId})
      </h3>
      {tasks.length === 0 ? (
        <p>No tasks found for this {safeContextLabel.toLowerCase()}.</p>
      ) : (
        <table border={1} cellPadding={5} style={{ width: "100%" }}>
          <thead>
            <tr>
              <th>ID</th>
              <th>Title</th>
              <th>Status</th>
              <th>Due Date</th>
            </tr>
          </thead>
          <tbody>
            {tasks.map((task) => (
              <tr key={task.id}>
                {/* FIX: Show Full ID */}
                <td>{task.id}</td>
                <td>{task.title}</td>
                <td>{task.status}</td>
                <td>
                  {task.dueDate
                    ? new Date(task.dueDate).toLocaleDateString()
                    : "N/A"}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}

// --- 2. RelatedNotes ---
export function RelatedNotes(props: RelatedTableProps) {
  const { selectedContactId, selectedEventId } = useSelectionContext();
  const [notes, setNotes] = useState<NoteDto[]>([]);
  const [loading, setLoading] = useState(false);

  const context = getApiContext(
    props,
    selectedContactId,
    selectedEventId,
    "notes"
  );
  const selectedId = context.id;
  const apiUrl = context.url;
  const contextLabel = context.label;

  useEffect(() => {
    if (!selectedId || !apiUrl) {
      setNotes([]);
      return;
    }

    const fetchNotes = async () => {
      setLoading(true);
      try {
        const response = await apiClient.get<NoteDto[]>(apiUrl);
        setNotes(response.data);
      } catch (err) {
        console.error("Failed to fetch related notes:", err);
      } finally {
        setLoading(false);
      }
    };

    fetchNotes();
  }, [selectedId, apiUrl]);

  if (!selectedId) return null;
  if (loading) return <p>Loading related notes...</p>;

  const safeContextLabel = contextLabel as string;

  return (
    <div style={{ marginTop: "2rem" }}>
      {/* FIX: Show Full ID */}
      <h3>
        Related Notes (for {safeContextLabel} ID: {selectedId})
      </h3>
      {notes.length === 0 ? (
        <p>No notes found for this {safeContextLabel.toLowerCase()}.</p>
      ) : (
        <table border={1} cellPadding={5} style={{ width: "100%" }}>
          <thead>
            <tr>
              <th>ID</th>
              <th>Content</th>
            </tr>
          </thead>
          <tbody>
            {notes.map((note) => (
              <tr key={note.id}>
                {/* FIX: Show Full ID */}
                <td>{note.id}</td>
                <td>{note.content}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}

// --- 3. ContactEventsTable ---
export function ContactEventsTable() {
  const { selectedContactId } = useSelectionContext();
  const [events, setEvents] = useState<EventDto[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (!selectedContactId) {
      setEvents([]);
      return;
    }

    const fetchEvents = async () => {
      setLoading(true);
      try {
        const response = await apiClient.get<EventDto[]>(
          `/api/contacts/${selectedContactId}/events`
        );
        setEvents(response.data);
      } catch (err) {
        console.error("Failed to fetch contact's events:", err);
      } finally {
        setLoading(false);
      }
    };

    fetchEvents();
  }, [selectedContactId]);

  if (!selectedContactId) return null;
  if (loading) return <p>Loading related events...</p>;

  return (
    <div style={{ marginTop: "2rem" }}>
      {/* FIX: Show Full ID */}
      <h3>Related Events (for Contact ID: {selectedContactId})</h3>
      {events.length === 0 ? (
        <p>No events found linked to this contact.</p>
      ) : (
        <table border={1} cellPadding={5} style={{ width: "100%" }}>
          <thead>
            <tr>
              <th>ID</th>
              <th>Name</th>
              <th>Status</th>
              <th>Start</th>
              <th>Location</th>
            </tr>
          </thead>
          <tbody>
            {events.map((event) => (
              <tr key={event.id}>
                {/* FIX: Show Full ID */}
                <td>{event.id}</td>
                <td>{event.name}</td>
                <td>{event.status}</td>
                <td>{new Date(event.startDateTime).toLocaleString()}</td>
                <td>{event.location ?? "N/A"}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}

// --- 4. EventParticipantsTable ---
interface EventParticipantsTableProps {
  onUnlinkSuccess: () => void;
}

export function EventParticipantsTable({
  onUnlinkSuccess,
}: EventParticipantsTableProps) {
  const { selectedEventId } = useSelectionContext();
  const [contacts, setContacts] = useState<ContactDto[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    const fetchContacts = async () => {
      if (!selectedEventId) {
        setContacts([]);
        return;
      }
      setLoading(true);
      try {
        const response = await apiClient.get<ContactDto[]>(
          `/api/events/${selectedEventId}/contacts`
        );
        setContacts(response.data);
      } catch (err) {
        console.error("Failed to fetch event participants:", err);
      } finally {
        setLoading(false);
      }
    };

    fetchContacts();
  }, [selectedEventId]);

  const handleUnlink = async (contactId: string) => {
    if (!selectedEventId) return;
    if (
      !window.confirm(
        "Are you sure you want to remove this contact from the event?"
      )
    )
      return;

    try {
      await apiClient.delete(
        `/api/events/${selectedEventId}/contacts/${contactId}`
      );
      onUnlinkSuccess();
    } catch (err) {
      console.error("Failed to unlink contact:", err);
      alert("Failed to unlink contact.");
    }
  };

  if (!selectedEventId) return null;
  if (loading) return <p>Loading event participants...</p>;

  return (
    <div style={{ marginTop: "2rem" }}>
      <h3>Event Participants (Staff/Guests)</h3>
      {contacts.length === 0 ? (
        <p>No participants linked to this event.</p>
      ) : (
        <table border={1} cellPadding={5} style={{ width: "100%" }}>
          <thead>
            <tr>
              <th>ID</th>
              <th>First Name</th>
              <th>Last Name</th>
              <th>Email</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {contacts.map((contact) => (
              <tr key={contact.id}>
                {/* FIX: Show Full ID */}
                <td>{contact.id}</td>
                <td>{contact.firstName}</td>
                <td>{contact.lastName}</td>
                <td>{contact.email}</td>
                <td>
                  <button
                    onClick={() => handleUnlink(contact.id)}
                    style={{ color: "red", cursor: "pointer" }}>
                    Unlink
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}

// --- 5. EmailHistoryTable ---
export function EmailHistoryTable() {
  const { selectedContactId } = useSelectionContext();
  const [logs, setLogs] = useState<EmailLogDto[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (!selectedContactId) {
      setLogs([]);
      return;
    }

    const fetchLogs = async () => {
      setLoading(true);
      try {
        const res = await apiClient.get<EmailLogDto[]>(
          `/api/contacts/${selectedContactId}/email-logs`
        );
        setLogs(res.data);
      } catch (err) {
        console.error("Failed to fetch email logs", err);
      } finally {
        setLoading(false);
      }
    };

    fetchLogs();
  }, [selectedContactId]);

  if (!selectedContactId) return null;

  return (
    <div style={{ marginTop: "2rem" }}>
      <h3>Email History (for Contact ID: {selectedContactId})</h3>
      {loading ? (
        <p>Loading logs...</p>
      ) : logs.length === 0 ? (
        <p>No emails sent to this contact.</p>
      ) : (
        <table
          border={1}
          cellPadding={5}
          style={{ width: "100%", borderCollapse: "collapse" }}>
          <thead>
            <tr style={{ background: "#333", color: "#fff" }}>
              <th>Subject</th>
              <th>Sent At</th>
              <th>Status</th>
              <th>Message</th>
            </tr>
          </thead>
          <tbody>
            {logs.map((log) => (
              <tr key={log.id}>
                <td>{log.subject}</td>
                <td>{new Date(log.sentAt).toLocaleString()}</td>
                <td
                  style={{
                    color: log.isSuccess ? "lightgreen" : "#ff6b6b",
                    fontWeight: "bold",
                  }}>
                  {log.isSuccess ? "Success" : "Failed"}
                </td>
                <td
                  style={{
                    maxWidth: "300px",
                    overflow: "hidden",
                    textOverflow: "ellipsis",
                    whiteSpace: "nowrap",
                  }}>
                  {log.errorMessage ? log.errorMessage : "Sent via SendGrid"}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}

export default function RelatedTables() {
  return (
    <div>
      <RelatedTasks />
      <RelatedNotes />
    </div>
  );
}
