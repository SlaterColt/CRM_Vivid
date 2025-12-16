// FILE: frontend_harness/src/components/TasksPage.tsx (COMPLETE FILE)

import { useEffect, useState } from "react";
import { apiClient } from "../apiClient";
import type { TaskDto } from "../types";
import axios from "axios";

// Define an explicit type for the data sent in the command,
// including the new lookup fields for clarity
type TaskCommandData = Omit<TaskDto, "id" | "createdAt" | "vendorName"> & {
  contactEmail?: string;
  eventName?: string;
  vendorName?: string; // This maps to the backend VendorName field in the command
};
type ValidationErrors = { [key: string]: string[] };

function TasksPage() {
  const [tasks, setTasks] = useState<TaskDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const initialFormState: Partial<TaskCommandData> = {
    title: "",
    description: null,
    status: "NotStarted", // Default enum string
    priority: "Medium", // Default enum string
    dueDate: null,
    // Original GUID fields (cleared for the form but used in DTO)
    contactId: null,
    eventId: null,
    vendorId: null,
    // String lookup fields
    contactEmail: undefined,
    eventName: undefined,
    vendorName: undefined,
  };

  const [formData, setFormData] =
    useState<Partial<TaskCommandData>>(initialFormState);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [validationErrors, setValidationErrors] = useState<ValidationErrors>(
    {}
  );

  // --- NEW: PHASE 29 Dynamic Linkage State ---
  const [entityTypeToLink, setEntityTypeToLink] = useState<
    "Contact" | "Event" | "Vendor" | null
  >(null);
  const [linkValue, setLinkValue] = useState("");
  // ------------------------------------------

  const isEditing = editingId !== null;

  const fetchTasks = async () => {
    try {
      setLoading(true);
      setError(null);
      const response = await apiClient.get<TaskDto[]>("/api/tasks");
      setTasks(response.data);
    } catch (err: unknown) {
      console.error(err);
      setError("Failed to fetch tasks.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchTasks();
  }, []);

  const handleDelete = async (id: string) => {
    if (!window.confirm("Are you sure you want to delete this task?")) return;
    try {
      await apiClient.delete(`/api/tasks/${id}`);
      await fetchTasks();
    } catch (err) {
      console.error(err);
      alert("Failed to delete task.");
    }
  };

  const handleEdit = (task: TaskDto) => {
    setEditingId(task.id);

    // Clear dynamic link state when editing, as the fields are GUID-based on update
    setEntityTypeToLink(null);
    setLinkValue("");

    // Format date for date input
    setFormData({
      ...task,
      dueDate: task.dueDate ? task.dueDate.slice(0, 10) : null,
      vendorId: task.vendorId ?? null,
      contactId: task.contactId ?? null,
      eventId: task.eventId ?? null,
      // Clear string lookup fields when editing an existing task by ID
      contactEmail: undefined,
      eventName: undefined,
      vendorName: undefined,
    });
    setValidationErrors({});
    window.scrollTo(0, 0);
  };

  const handleCancel = () => {
    setEditingId(null);
    setFormData(initialFormState);
    setEntityTypeToLink(null); // Reset link state
    setLinkValue(""); // Reset link value
    setValidationErrors({});
  };

  const handleChange = (
    e: React.ChangeEvent<
      HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement
    >
  ) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value === "" ? null : value,
    }));
  };

  // --- PHASE 29: Dynamic Linkage Handler ---
  const handleEntityTypeChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const type = e.target.value as "Contact" | "Event" | "Vendor" | "None";
    setEntityTypeToLink(type === "None" ? null : type);
    setLinkValue("");
    // Clear all previous link-related fields in formData to prevent conflicts
    setFormData({
      ...formData,
      contactId: null,
      eventId: null,
      vendorId: null,
      contactEmail: undefined,
      eventName: undefined,
      vendorName: undefined,
    });
  };

  const handleLinkValueChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setLinkValue(e.target.value);
  };

  // --- PHASE 29: Payload Construction ---
  const constructPayload = (
    baseData: Partial<TaskCommandData>
  ): TaskCommandData => {
    const payload: Partial<TaskCommandData> = {
      ...baseData,
      description: baseData.description ?? null,
      dueDate: baseData.dueDate ?? null,
    };

    // If we are editing, we submit the GUIDs (contactId, eventId, etc.)
    // already present in formData, and string fields are undefined.
    if (isEditing) {
      return payload as TaskCommandData;
    }

    // If creating, we inject the dynamic linkage values into the string lookup fields.
    // GUID fields are null, string fields hold the lookup value.
    if (entityTypeToLink === "Contact" && linkValue) {
      payload.contactEmail = linkValue;
      payload.contactId = null; // Ensure ID is null
    } else if (entityTypeToLink === "Event" && linkValue) {
      payload.eventName = linkValue;
      payload.eventId = null; // Ensure ID is null
    } else if (entityTypeToLink === "Vendor" && linkValue) {
      payload.vendorName = linkValue; // Maps to VendorName on backend command
      payload.vendorId = null; // Ensure ID is null
    }

    return payload as TaskCommandData;
  };
  // ------------------------------------------

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setValidationErrors({});

    const payload = constructPayload(formData);

    const promise = isEditing
      ? apiClient.put(`/api/tasks/${editingId}`, payload)
      : apiClient.post("/api/tasks", payload);

    try {
      await promise;
      handleCancel();
      await fetchTasks();
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

  if (error) return <p style={{ color: "red" }}>{error}</p>;

  // Helper to determine the placeholder text
  const getLinkPlaceholder = () => {
    switch (entityTypeToLink) {
      case "Contact":
        return "Contact Email (e.g., jane@example.com)";
      case "Event":
        return "Event Name (e.g., Summer Gala 2026)";
      case "Vendor":
        return "Vendor Name (e.g., The Jazz Trio)";
      default:
        return "Enter ID, Name, or Email";
    }
  };

  return (
    <div>
      <div
        style={{
          marginBottom: "2rem",
          padding: "1rem",
          border: "1px solid #555",
        }}>
        <h2>{isEditing ? "Edit Task" : "Create Task"}</h2>
        <form onSubmit={handleSubmit}>
          {/* Form fields */}
          <div style={{ marginBottom: "0.5rem" }}>
            <label>Title: </label>
            <input
              name="title"
              value={formData.title ?? ""}
              onChange={handleChange}
            />
            {validationErrors.Title && (
              <div style={{ color: "red" }}>{validationErrors.Title[0]}</div>
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
            <label>Due Date: </label>
            <input
              name="dueDate"
              type="date"
              value={formData.dueDate ?? ""}
              onChange={handleChange}
            />
          </div>
          <div style={{ marginBottom: "0.5rem" }}>
            <label>Status: </label>
            <select
              name="status"
              value={formData.status ?? "NotStarted"}
              onChange={handleChange}>
              <option value="NotStarted">Not Started</option>
              <option value="InProgress">In Progress</option>
              <option value="Completed">Completed</option>
            </select>
          </div>
          <div style={{ marginBottom: "0.5rem" }}>
            <label>Priority: </label>
            <select
              name="priority"
              value={formData.priority ?? "Medium"}
              onChange={handleChange}>
              <option value="Low">Low</option>
              <option value="Medium">Medium</option>
              <option value="High">High</option>
            </select>
          </div>

          {/* --- PHASE 29: DYNAMIC LINKING SECTION --- */}
          {!isEditing && (
            <div
              style={{
                margin: "1rem 0",
                padding: "1rem",
                border: "1px dashed #666",
                borderRadius: "4px",
              }}>
              <h4 style={{ marginTop: 0 }}>
                Link Task To: (Name or Email Lookup)
              </h4>
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
                </select>
              </div>
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
            </div>
          )}

          {/* --- LEGACY GUID INPUTS (Hidden on Create, Used on Edit) --- */}
          {/* On Edit mode, we submit the existing GUIDs. */}
          {/* NOTE: We only display the GUIDs on edit if you need to change the parent via GUID */}
          {isEditing && (
            <>
              <div style={{ marginBottom: "0.5rem" }}>
                <label>Contact ID (Optional): </label>
                <input
                  name="contactId"
                  value={formData.contactId ?? ""}
                  onChange={handleChange}
                />
              </div>
              <div style={{ marginBottom: "0.5rem" }}>
                <label>Event ID (Optional): </label>
                <input
                  name="eventId"
                  value={formData.eventId ?? ""}
                  onChange={handleChange}
                />
              </div>
              <div style={{ marginBottom: "0.5rem" }}>
                <label>Vendor ID (Optional): </label>
                <input
                  name="vendorId"
                  value={formData.vendorId ?? ""}
                  onChange={handleChange}
                />
              </div>
            </>
          )}
          {/* ---------------------------------------------------- */}

          {validationErrors.General && (
            <div style={{ color: "red", marginTop: "1rem" }}>
              {validationErrors.General[0]}
            </div>
          )}

          <button type="submit">{isEditing ? "Save Changes" : "Create"}</button>
          {isEditing && (
            <button
              type="button"
              onClick={handleCancel}
              style={{ marginLeft: "0.5rem" }}>
              Cancel
            </button>
          )}
        </form>
      </div>

      <h2>Tasks List</h2>
      {loading ? (
        <p>Loading Tasks...</p>
      ) : (
        <table border={1} cellPadding={5} style={{ width: "100%" }}>
          <thead>
            <tr>
              <th>ID</th>
              <th>Title</th>
              <th>Status</th>
              <th>Priority</th>
              <th>Due Date</th>
              <th>Vendor</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {tasks.map((task) => (
              <tr key={task.id}>
                <td>{task.id}</td>
                <td>{task.title}</td>
                <td>{task.status}</td>
                <td>{task.priority}</td>
                <td>
                  {task.dueDate
                    ? new Date(task.dueDate).toLocaleDateString()
                    : "N/A"}
                </td>
                <td>{task.vendorName ?? "N/A"}</td>
                <td>
                  <button
                    onClick={() => handleEdit(task)}
                    style={{ color: "blue" }}>
                    Edit
                  </button>
                  <button
                    onClick={() => handleDelete(task.id)}
                    style={{ color: "red", marginLeft: "0.5rem" }}>
                    Delete
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

export default TasksPage;
