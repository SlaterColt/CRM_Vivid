import React, { useEffect, useState } from "react";
import { apiClient } from "../apiClient";
import type { EventDto, ContactDto } from "../types";
import axios from "axios";
import { useSelectionContext } from "../context/useSelectionContext";
import {
  RelatedTasks,
  RelatedNotes,
  EventParticipantsTable,
} from "./RelatedTables";
import { DocumentsSection } from "./DocumentsSection";
// --- NEW: Import the Financials Module ---
import FinancialsSection from "./FinancialsSection";

type EventFormData = Omit<EventDto, "id">;
type ValidationErrors = { [key: string]: string[] };

// --- SUB-COMPONENT: ContactLinker ---
interface ContactLinkerProps {
  eventId: string;
  onLinkSuccess: () => void;
}

const ContactLinker: React.FC<ContactLinkerProps> = ({
  eventId,
  onLinkSuccess,
}) => {
  const [contacts, setContacts] = useState<ContactDto[]>([]);
  const [selectedContactId, setSelectedContactId] = useState<string>("");
  const [loading, setLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchContacts = async () => {
      setLoading(true);
      try {
        const response = await apiClient.get<ContactDto[]>("/api/contacts");
        setContacts(response.data);
        if (response.data.length > 0) {
          setSelectedContactId(response.data[0].id);
        }
      } catch (err) {
        console.error("Failed to fetch all contacts:", err);
      } finally {
        setLoading(false);
      }
    };
    fetchContacts();
  }, []);

  const handleLink = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedContactId) {
      setError("Please select a contact to link.");
      return;
    }

    setIsSubmitting(true);
    setError(null);
    try {
      const payload = { contactId: selectedContactId };
      await apiClient.post(`/api/events/${eventId}/contacts`, payload);

      onLinkSuccess();
      setError(null);
    } catch (err) {
      const message =
        axios.isAxiosError(err) && err.response?.data?.title
          ? err.response.data.title
          : "Failed to link contact to event.";
      setError(message);
    } finally {
      setIsSubmitting(false);
    }
  };

  if (loading) return <p>Loading contacts for linking...</p>;
  if (contacts.length === 0) return <p>No available contacts to link.</p>;

  return (
    <div
      style={{
        border: "1px dashed #ccc",
        padding: "1rem",
        marginBottom: "1rem",
      }}>
      <h4>Link Contact to Event</h4>
      <form onSubmit={handleLink} style={{ display: "flex", gap: "10px" }}>
        <select
          value={selectedContactId}
          onChange={(e) => setSelectedContactId(e.target.value)}
          disabled={isSubmitting}>
          {contacts.map((contact) => (
            <option key={contact.id} value={contact.id}>
              {contact.firstName} {contact.lastName} ({contact.email})
            </option>
          ))}
        </select>
        <button type="submit" disabled={isSubmitting}>
          {isSubmitting ? "Linking..." : "Link Contact"}
        </button>
      </form>
      {error && (
        <div style={{ color: "red", marginTop: "0.5rem" }}>Error: {error}</div>
      )}
    </div>
  );
};
// --- END ContactLinker ---

function EventsPage() {
  const [events, setEvents] = useState<EventDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const { selectedEventId, setSelectedEventId } = useSelectionContext();
  const [dashboardKey, setDashboardKey] = useState(0);

  const initialFormState: EventFormData = {
    name: "",
    description: null,
    startDateTime: "",
    endDateTime: "",
    location: null,
    isPublic: false,
    status: "Planned",
  };

  const [formData, setFormData] = useState<Partial<EventDto>>(initialFormState);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [validationErrors, setValidationErrors] = useState<ValidationErrors>(
    {}
  );

  const isEditing = editingId !== null;

  const forceDashboardRefresh = () => {
    setDashboardKey((prev) => prev + 1);
  };

  const fetchEvents = async () => {
    try {
      setLoading(true);
      setError(null);
      const response = await apiClient.get<EventDto[]>("/api/events");
      setEvents(response.data);
    } catch (err: unknown) {
      console.error(err);
      setError("Failed to fetch events.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchEvents();
  }, []);

  const handleDelete = async (id: string) => {
    if (!window.confirm("Are you sure you want to delete this event?")) return;
    try {
      await apiClient.delete(`/api/events/${id}`);
      setSelectedEventId(null);
      await fetchEvents();
    } catch (err) {
      console.error(err);
      alert("Failed to delete event.");
    }
  };

  const handleEdit = (event: EventDto) => {
    setEditingId(event.id);
    setFormData({
      ...event,
      startDateTime: event.startDateTime.slice(0, 16),
      endDateTime: event.endDateTime.slice(0, 16),
    });
    setValidationErrors({});
    window.scrollTo(0, 0);
  };

  const handleCancel = () => {
    setEditingId(null);
    setFormData(initialFormState);
    setValidationErrors({});
  };

  const handleChange = (
    e: React.ChangeEvent<
      HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement
    >
  ) => {
    const { name, value, type } = e.target;

    if (type === "checkbox") {
      setFormData((prev) => ({
        ...prev,
        [name]: (e.target as HTMLInputElement).checked,
      }));
    } else {
      setFormData((prev) => ({
        ...prev,
        [name]: value === "" ? null : value,
      }));
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setValidationErrors({});

    const payload = {
      ...formData,
      startDateTime: formData.startDateTime
        ? new Date(formData.startDateTime).toISOString()
        : "",
      endDateTime: formData.endDateTime
        ? new Date(formData.endDateTime).toISOString()
        : "",
    };

    const promise = isEditing
      ? apiClient.put(`/api/events/${editingId}`, payload)
      : apiClient.post("/api/events", payload);

    try {
      await promise;
      handleCancel();
      setSelectedEventId(null);
      await fetchEvents();
    } catch (err: unknown) {
      if (axios.isAxiosError(err) && err.response?.status === 400) {
        setValidationErrors(
          err.response.data.errors || { General: ["Validation Error"] }
        );
      } else {
        console.error(err);
        alert("An unexpected error occurred.");
      }
    }
  };

  const handleRowClick = (eventId: string) => {
    if (selectedEventId === eventId) {
      setSelectedEventId(null);
    } else {
      setSelectedEventId(eventId);
    }
    setDashboardKey(0);
  };

  if (error) return <p style={{ color: "red" }}>{error}</p>;

  return (
    <div>
      <div
        style={{
          marginBottom: "2rem",
          padding: "1rem",
          border: "1px solid #555",
        }}>
        <h2>{isEditing ? "Edit Event" : "Create Event"}</h2>
        <form onSubmit={handleSubmit}>
          <div style={{ marginBottom: "0.5rem" }}>
            <label>Name: </label>
            <input
              name="name"
              value={formData.name ?? ""}
              onChange={handleChange}
            />
            {validationErrors.Name && (
              <div style={{ color: "red" }}>{validationErrors.Name[0]}</div>
            )}
          </div>
          <div style={{ marginBottom: "0.5rem" }}>
            <label>Description: </label>
            <textarea
              name="description"
              value={formData.description ?? ""}
              onChange={handleChange}
            />
          </div>
          <div style={{ marginBottom: "0.5rem" }}>
            <label>Start Time: </label>
            <input
              name="startDateTime"
              type="datetime-local"
              value={formData.startDateTime ?? ""}
              onChange={handleChange}
            />
          </div>
          <div style={{ marginBottom: "0.5rem" }}>
            <label>End Time: </label>
            <input
              name="endDateTime"
              type="datetime-local"
              value={formData.endDateTime ?? ""}
              onChange={handleChange}
            />
          </div>
          <div style={{ marginBottom: "0.5rem" }}>
            <label>Location: </label>
            <input
              name="location"
              value={formData.location ?? ""}
              onChange={handleChange}
            />
          </div>
          <div style={{ marginBottom: "0.5rem" }}>
            <label>Status: </label>
            <select
              name="status"
              value={formData.status ?? "Planned"}
              onChange={handleChange}>
              <option value="Planned">Planned</option>
              <option value="Confirmed">Confirmed</option>
              <option value="Cancelled">Cancelled</option>
            </select>
          </div>
          <div style={{ marginBottom: "0.5rem" }}>
            <label>
              <input
                name="isPublic"
                type="checkbox"
                checked={formData.isPublic ?? false}
                onChange={handleChange}
              />
              Is Public
            </label>
          </div>

          <button type="submit">{isEditing ? "Save Changes" : "Create"}</button>
          {isEditing && (
            <button
              type="button"
              onClick={handleCancel}
              style={{ marginLeft: "0.5rem" }}>
              Cancel
            </button>
          )}
          {validationErrors.General && (
            <div style={{ color: "red", marginTop: "1rem" }}>
              {validationErrors.General[0]}
            </div>
          )}
        </form>
      </div>

      <h2>Events List (Click a row for related data)</h2>
      {loading ? (
        <p>Loading Events...</p>
      ) : (
        <table border={1} cellPadding={5} style={{ width: "100%" }}>
          <thead>
            <tr>
              <th>ID</th>
              <th>Name</th>
              <th>Status</th>
              <th>Start</th>
              <th>End</th>
              <th>Location</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {events.map((event) => (
              <tr
                key={event.id}
                onClick={() => handleRowClick(event.id)}
                style={{
                  cursor: "pointer",
                  backgroundColor:
                    event.id === selectedEventId ? "#444" : "inherit",
                }}>
                <td>{event.id}</td>
                <td>{event.name}</td>
                <td>{event.status}</td>
                <td>{new Date(event.startDateTime).toLocaleString()}</td>
                <td>{new Date(event.endDateTime).toLocaleString()}</td>
                <td>{event.location ?? "N/A"}</td>
                <td>
                  <button
                    onClick={(e) => {
                      e.stopPropagation();
                      handleEdit(event);
                    }}
                    style={{ color: "blue" }}>
                    Edit
                  </button>
                  <button
                    onClick={(e) => {
                      e.stopPropagation();
                      handleDelete(event.id);
                    }}
                    style={{ color: "red", marginLeft: "0.5rem" }}>
                    Delete
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}

      {/* --- DASHBOARD --- */}
      {selectedEventId && (
        <div
          key={dashboardKey}
          style={{
            borderTop: "2px solid #555",
            marginTop: "3rem",
            paddingTop: "1rem",
          }}>
          <h2 style={{ marginBottom: "2rem" }}>
            Dashboard: Event ID {selectedEventId}
          </h2>

          {/* 1. Financials (High Priority) */}
          <div style={{ marginBottom: "3rem" }}>
            <FinancialsSection eventId={selectedEventId} />
          </div>

          <hr style={{ margin: "2rem 0", borderColor: "#ccc" }} />

          {/* 2. Contacts & Documents */}
          <div
            style={{
              display: "grid",
              gridTemplateColumns: "1fr 1fr",
              gap: "2rem",
            }}>
            <div>
              <ContactLinker
                eventId={selectedEventId}
                onLinkSuccess={forceDashboardRefresh}
              />
              <EventParticipantsTable onUnlinkSuccess={forceDashboardRefresh} />
            </div>
            <div>
              <DocumentsSection entityId={selectedEventId} entityType="Event" />
            </div>
          </div>

          <hr style={{ margin: "2rem 0", borderColor: "#ccc" }} />

          {/* 3. Operational (Tasks & Notes) */}
          <RelatedTasks />
          <RelatedNotes />
        </div>
      )}
    </div>
  );
}

export default EventsPage;
