import { useEffect, useState } from "react";
import { apiClient } from "../apiClient";
import type { TaskDto } from "../types";
import axios from "axios";

type TaskFormData = Omit<TaskDto, "id" | "createdAt">;
type ValidationErrors = { [key: string]: string[] };

function TasksPage() {
  const [tasks, setTasks] = useState<TaskDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const initialFormState: TaskFormData = {
    title: "",
    description: null,
    status: "NotStarted", // Default enum string
    priority: "Medium", // Default enum string
    dueDate: null,
    contactId: null,
    eventId: null,
    vendorId: null, // NEW: Initialize vendorId
  };

  const [formData, setFormData] = useState<Partial<TaskDto>>(initialFormState);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [validationErrors, setValidationErrors] = useState<ValidationErrors>(
    {}
  );

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
    // Format date for date input
    setFormData({
      ...task,
      dueDate: task.dueDate ? task.dueDate.slice(0, 10) : null,
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
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value === "" ? null : value,
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setValidationErrors({});

    const payload = {
      ...formData,
      // Ensure nulls are handled
      description: formData.description ?? null,
      dueDate: formData.dueDate ?? null,
      contactId: formData.contactId ?? null,
      eventId: formData.eventId ?? null,
      vendorId: formData.vendorId ?? null, // NEW: Include vendorId in payload
    };

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

          {/* NEW: Vendor ID Input */}
          <div style={{ marginBottom: "0.5rem" }}>
            <label>Vendor ID (Optional): </label>
            <input
              name="vendorId"
              value={formData.vendorId ?? ""}
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
