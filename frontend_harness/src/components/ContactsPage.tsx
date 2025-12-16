// =======================================================
// FILE: frontend_harness/src/components/ContactsPage.tsx
// =======================================================
import { useEffect, useState } from "react";
import { apiClient } from "../apiClient";
import type { ContactDto } from "../types";
import { LeadStage, ConnectionStatus } from "../types";
import axios from "axios";

import { useSelectionContext } from "../context/useSelectionContext";
import {
  RelatedTasks,
  RelatedNotes,
  ContactEventsTable,
  EmailHistoryTable,
} from "./RelatedTables";

type ContactFormData = Omit<ContactDto, "id">;

type ValidationErrors = {
  [key: string]: string[];
};

// Local definition for Template to avoid editing types.ts in this turn
type TemplateDto = {
  id: string;
  name: string;
  content: string;
};

const styleSheet = document.createElement("style");
styleSheet.type = "text/css";
styleSheet.innerText = `
  .contact-row:hover {
    cursor: pointer;
    background-color: #333;
  }
  .selected-row {
    background-color: #004a99;
    color: white;
  }
  .selected-row:hover {
    background-color: #005ac7;
  }
  .modal-overlay {
    position: fixed;
    top: 0; left: 0; right: 0; bottom: 0;
    background: rgba(0,0,0,0.7);
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 1000;
  }
  .modal-content {
    background: #222;
    padding: 2rem;
    border: 1px solid #555;
    min-width: 400px;
    border-radius: 8px;
  }
`;
document.head.appendChild(styleSheet);

function ContactsPage() {
  const [contacts, setContacts] = useState<ContactDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // --- Email Scheduling State ---
  const [templates, setTemplates] = useState<TemplateDto[]>([]);
  const [isEmailModalOpen, setIsEmailModalOpen] = useState(false);
  const [emailConfig, setEmailConfig] = useState({
    contactId: "",
    contactName: "", // For display
    templateId: "",
    sendAt: "",
  });
  // ------------------------------

  const initialFormState: ContactFormData = {
    firstName: "",
    lastName: "",
    email: "",
    phoneNumber: null,
    title: null,
    organization: null,
    // New Pipeline Fields
    stage: LeadStage.NewLead,
    connectionStatus: ConnectionStatus.NeedToMeet,
    isLead: true,
    followUpCount: 0,
    source: null,
    lastContactedAt: null,
  };

  const [formData, setFormData] =
    useState<Partial<ContactDto>>(initialFormState);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [validationErrors, setValidationErrors] = useState<ValidationErrors>(
    {}
  );

  const { selectedContactId, setSelectedContactId } = useSelectionContext();

  const isEditing = editingId !== null;

  const fetchContacts = async () => {
    try {
      setLoading(true);
      setError(null);
      const response = await apiClient.get<ContactDto[]>("/api/contacts");
      setContacts(response.data);
    } catch (err: unknown) {
      console.error(err);
      let errorMessage = "Failed to fetch contacts. An unknown error occurred.";
      if (err instanceof Error) {
        errorMessage = `Failed to fetch contacts: ${err.message}`;
      }
      setError(errorMessage);
    } finally {
      setLoading(false);
    }
  };

  // Fetch Templates for the dropdown
  const fetchTemplates = async () => {
    try {
      const response = await apiClient.get<TemplateDto[]>("/api/templates");
      setTemplates(response.data);
    } catch (err) {
      console.error("Failed to load templates", err);
    }
  };

  useEffect(() => {
    fetchContacts();
    fetchTemplates();
    return () => {
      setSelectedContactId(null);
    };
  }, [setSelectedContactId]);

  const handleDelete = async (id: string) => {
    if (!window.confirm("Are you sure you want to delete this contact?")) {
      return;
    }

    if (id === selectedContactId) {
      setSelectedContactId(null);
    }

    try {
      await apiClient.delete(`/api/contacts/${id}`);
      await fetchContacts();
    } catch (err) {
      console.error(err);
      if (err instanceof Error) {
        alert(`Failed to delete contact: ${err.message}`);
      }
    }
  };

  const handleEdit = (contact: ContactDto) => {
    setEditingId(contact.id);
    setFormData(contact);
    setValidationErrors({});
    window.scrollTo(0, 0);
  };

  const handleCancel = () => {
    setEditingId(null);
    setFormData(initialFormState);
    setValidationErrors({});
  };

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>
  ) => {
    const { name, value, type } = e.target;

    setFormData((prev) => {
      let newValue: string | number | boolean | null =
        value === "" ? null : value;

      // Handle Numeric Enums (Stage & ConnectionStatus)
      if (name === "stage" || name === "connectionStatus") {
        newValue = parseInt(value, 10);
      }

      // Handle Checkbox
      if (type === "checkbox") {
        newValue = (e.target as HTMLInputElement).checked;
      }

      return {
        ...prev,
        [name]: newValue,
      };
    });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setValidationErrors({});

    const payload = {
      ...formData,
      firstName: formData.firstName ?? "",
      lastName: formData.lastName ?? "",
      email: formData.email ?? "",
      stage: formData.stage ?? LeadStage.NewLead,
      connectionStatus:
        formData.connectionStatus ?? ConnectionStatus.NeedToMeet,
      isLead: formData.isLead ?? true,
      followUpCount: formData.followUpCount ?? 0,
    };

    const promise = isEditing
      ? apiClient.put(`/api/contacts/${editingId}`, payload)
      : apiClient.post("/api/contacts", payload);

    try {
      await promise;
      handleCancel();
      await fetchContacts();
    } catch (err: unknown) {
      if (axios.isAxiosError(err) && err.response?.status === 400) {
        const apiErrors = err.response.data.errors || {
          General: ["An unknown validation error occurred."],
        };
        setValidationErrors(apiErrors);
      } else {
        console.error(err);
        alert("An unexpected error occurred.");
      }
    }
  };

  const handleRowClick = (contactId: string) => {
    if (isEditing) {
      handleCancel();
    }

    if (selectedContactId === contactId) {
      setSelectedContactId(null);
    } else {
      setSelectedContactId(contactId);
    }
  };

  // --- Email Modal Handlers ---
  const openEmailModal = (e: React.MouseEvent, contact: ContactDto) => {
    e.stopPropagation(); // Prevent row selection
    setEmailConfig({
      contactId: contact.id,
      contactName: `${contact.firstName} ${contact.lastName}`,
      templateId: templates.length > 0 ? templates[0].id : "",
      sendAt: "",
    });
    setIsEmailModalOpen(true);
  };

  const handleEmailSubmit = async () => {
    if (!emailConfig.templateId) {
      alert("Please select a template.");
      return;
    }

    try {
      await apiClient.post("/api/automation/schedule-email", {
        contactId: emailConfig.contactId,
        templateId: emailConfig.templateId,
        sendAt: emailConfig.sendAt
          ? new Date(emailConfig.sendAt).toISOString()
          : null,
      });
      alert("Email scheduled successfully!");
      setIsEmailModalOpen(false);
    } catch (err) {
      console.error(err);
      alert("Failed to schedule email.");
    }
  };
  // ----------------------------

  if (error) return <p style={{ color: "red" }}>{error}</p>;

  return (
    <div>
      {/* --- Email Modal --- */}
      {isEmailModalOpen && (
        <div className="modal-overlay">
          <div className="modal-content">
            <h3>Send Email to: {emailConfig.contactName}</h3>

            <div style={{ marginBottom: "1rem" }}>
              <label style={{ display: "block", marginBottom: "0.5rem" }}>
                Select Template:
              </label>
              <select
                style={{ width: "100%", padding: "0.5rem" }}
                value={emailConfig.templateId}
                onChange={(e) =>
                  setEmailConfig((prev) => ({
                    ...prev,
                    templateId: e.target.value,
                  }))
                }>
                <option value="">-- Select a Template --</option>
                {templates.map((t) => (
                  <option key={t.id} value={t.id}>
                    {t.name}
                  </option>
                ))}
              </select>
            </div>

            <div style={{ marginBottom: "1rem" }}>
              <label style={{ display: "block", marginBottom: "0.5rem" }}>
                Send At (Optional):
              </label>
              <input
                type="datetime-local"
                style={{ width: "100%", padding: "0.5rem" }}
                value={emailConfig.sendAt}
                onChange={(e) =>
                  setEmailConfig((prev) => ({
                    ...prev,
                    sendAt: e.target.value,
                  }))
                }
              />
              <small>Leave blank to send immediately.</small>
            </div>

            <div
              style={{
                display: "flex",
                justifyContent: "flex-end",
                gap: "1rem",
              }}>
              <button onClick={() => setIsEmailModalOpen(false)}>Cancel</button>
              <button
                onClick={handleEmailSubmit}
                style={{ backgroundColor: "#004a99", color: "white" }}>
                Send Email
              </button>
            </div>
          </div>
        </div>
      )}

      <div
        style={{
          marginBottom: "2rem",
          padding: "1rem",
          border: "1px solid #555",
        }}>
        <h2>{isEditing ? "Edit Contact" : "Create Contact"}</h2>
        <form onSubmit={handleSubmit}>
          {/* --- Identity Fields --- */}
          <div style={{ display: "flex", gap: "1rem" }}>
            <div style={{ marginBottom: "0.5rem", flex: 1 }}>
              <label>First Name: </label>
              <input
                name="firstName"
                value={formData.firstName ?? ""}
                onChange={handleChange}
                style={{ width: "100%" }}
              />
              {validationErrors.FirstName && (
                <div style={{ color: "red" }}>
                  {validationErrors.FirstName[0]}
                </div>
              )}
            </div>
            <div style={{ marginBottom: "0.5rem", flex: 1 }}>
              <label>Last Name: </label>
              <input
                name="lastName"
                value={formData.lastName ?? ""}
                onChange={handleChange}
                style={{ width: "100%" }}
              />
              {validationErrors.LastName && (
                <div style={{ color: "red" }}>
                  {validationErrors.LastName[0]}
                </div>
              )}
            </div>
          </div>

          <div style={{ display: "flex", gap: "1rem" }}>
            <div style={{ marginBottom: "0.5rem", flex: 1 }}>
              <label>Email: </label>
              <input
                name="email"
                value={formData.email ?? ""}
                onChange={handleChange}
                style={{ width: "100%" }}
              />
              {validationErrors.Email && (
                <div style={{ color: "red" }}>{validationErrors.Email[0]}</div>
              )}
            </div>
            <div style={{ marginBottom: "0.5rem", flex: 1 }}>
              <label>Phone: </label>
              <input
                name="phoneNumber"
                value={formData.phoneNumber ?? ""}
                onChange={handleChange}
                style={{ width: "100%" }}
              />
            </div>
          </div>

          {/* --- Pipeline & Strategy Fields (NEW) --- */}
          <div
            style={{
              margin: "1rem 0",
              padding: "1rem",
              border: "1px dashed #666",
              borderRadius: "4px",
            }}>
            <h4 style={{ marginTop: 0, marginBottom: "0.5rem" }}>
              Pipeline Status
            </h4>
            <div style={{ display: "flex", gap: "1rem", flexWrap: "wrap" }}>
              {/* Stage Dropdown */}
              <div style={{ flex: 1 }}>
                <label>Lead Stage: </label>
                <select
                  name="stage"
                  value={formData.stage ?? LeadStage.NewLead}
                  onChange={handleChange}
                  style={{ width: "100%", padding: "4px" }}>
                  {Object.entries(LeadStage).map(([key, value]) => (
                    <option key={key} value={value}>
                      {key}
                    </option>
                  ))}
                </select>
              </div>

              {/* Connection Status Dropdown */}
              <div style={{ flex: 1 }}>
                <label>Connection Status: </label>
                <select
                  name="connectionStatus"
                  value={
                    formData.connectionStatus ?? ConnectionStatus.NeedToMeet
                  }
                  onChange={handleChange}
                  style={{ width: "100%", padding: "4px" }}>
                  {Object.entries(ConnectionStatus).map(([key, value]) => (
                    <option key={key} value={value}>
                      {key}
                    </option>
                  ))}
                </select>
              </div>

              {/* Is Lead Checkbox */}
              <div
                style={{
                  flex: "0 0 auto",
                  display: "flex",
                  alignItems: "center",
                  paddingTop: "1.2rem",
                }}>
                <label
                  style={{
                    display: "flex",
                    alignItems: "center",
                    cursor: "pointer",
                  }}>
                  <input
                    type="checkbox"
                    name="isLead"
                    checked={formData.isLead ?? true}
                    onChange={handleChange}
                    style={{ marginRight: "0.5rem" }}
                  />
                  Is Lead?
                </label>
              </div>
            </div>
          </div>

          <div style={{ marginBottom: "0.5rem" }}>
            <label>Organization: </label>
            <input
              name="organization"
              value={formData.organization ?? ""}
              onChange={handleChange}
            />
          </div>
          <div style={{ marginBottom: "0.5rem" }}>
            <label>Title: </label>
            <input
              name="title"
              value={formData.title ?? ""}
              onChange={handleChange}
            />
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

      <h2>Contacts List</h2>
      {loading ? (
        <p>Loading Contacts...</p>
      ) : contacts.length === 0 ? (
        <p>No contacts found.</p>
      ) : (
        <table border={1} cellPadding={5} style={{ width: "100%" }}>
          <thead>
            <tr>
              <th>ID</th>
              <th>Name</th>
              <th>Email</th>
              <th>Status</th> {/* NEW COLUMN */}
              <th>Organization</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {contacts.map((contact) => (
              <tr
                key={contact.id}
                className={`
                  contact-row 
                  ${selectedContactId === contact.id ? "selected-row" : ""}
                `}
                onClick={(e) => {
                  if (
                    e.target instanceof HTMLElement &&
                    e.target.tagName !== "BUTTON" &&
                    e.target.tagName !== "SELECT"
                  ) {
                    handleRowClick(contact.id);
                  }
                }}>
                <td>{contact.id.substring(0, 8)}...</td>
                <td>
                  {contact.firstName} {contact.lastName}
                </td>
                <td>{contact.email}</td>
                {/* NEW STATUS COLUMN */}
                <td>
                  <span
                    style={{
                      fontSize: "0.8rem",
                      padding: "2px 4px",
                      background: "#444",
                      borderRadius: "4px",
                    }}>
                    {Object.keys(LeadStage).find(
                      (key) =>
                        LeadStage[key as keyof typeof LeadStage] ===
                        contact.stage
                    )}
                  </span>
                </td>
                <td>{contact.organization ?? "N/A"}</td>
                <td>
                  <button
                    onClick={() => handleEdit(contact)}
                    style={{ color: "lightblue", marginRight: "0.5rem" }}>
                    Edit
                  </button>
                  <button
                    onClick={() => handleDelete(contact.id)}
                    style={{ color: "red", marginRight: "0.5rem" }}>
                    Delete
                  </button>
                  <button
                    onClick={(e) => openEmailModal(e, contact)}
                    style={{ color: "lightgreen" }}>
                    Send Email
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}

      {selectedContactId && (
        <div
          style={{
            borderTop: "2px solid #555",
            marginTop: "3rem",
            paddingTop: "1rem",
          }}>
          <h2>Contact Dashboard: ID {selectedContactId}</h2>
          <ContactEventsTable />
          <RelatedTasks />
          <RelatedNotes />
          <EmailHistoryTable />
        </div>
      )}
    </div>
  );
}

export default ContactsPage;
