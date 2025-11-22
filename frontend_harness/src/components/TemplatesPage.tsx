import { useEffect, useState } from "react";
import { apiClient } from "../apiClient";
import type { TemplateDto, CreateTemplateDto } from "../types"; // FIX: Added 'type' keyword

export default function TemplatesPage() {
  const [templates, setTemplates] = useState<TemplateDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  // Form State
  const [formData, setFormData] = useState<CreateTemplateDto>({
    name: "",
    subject: "",
    content: "",
    type: "Email", // Default
  });

  useEffect(() => {
    fetchTemplates();
  }, []);

  const fetchTemplates = async () => {
    try {
      const response = await apiClient.get<TemplateDto[]>("/api/templates");
      setTemplates(response.data);
    } catch (err) {
      console.error(err);
      setError("Failed to load templates.");
    } finally {
      setLoading(false);
    }
  };

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await apiClient.post("/api/templates", formData);
      // Reset form
      setFormData({
        name: "",
        subject: "",
        content: "",
        type: "Email",
      });
      fetchTemplates();
    } catch (err) {
      console.error(err);
      alert("Failed to create template.");
    }
  };

  const handleDelete = async (id: string) => {
    if (!confirm("Are you sure you want to delete this template?")) return;
    try {
      await apiClient.delete(`/api/templates/${id}`);
      fetchTemplates();
    } catch (err) {
      console.error(err);
      alert("Failed to delete template.");
    }
  };

  return (
    <div className="p-4">
      <h2 className="text-xl font-bold mb-4">Templates</h2>

      {/* CREATE FORM */}
      <div className="mb-8 p-4 border rounded shadow-sm bg-gray-50">
        <h3 className="font-semibold mb-2">New Template</h3>
        <form onSubmit={handleCreate} className="space-y-3">
          <div>
            <label className="block text-sm font-medium">Name (Internal)</label>
            <input
              className="border p-1 w-full"
              value={formData.name}
              onChange={(e) =>
                setFormData({ ...formData, name: e.target.value })
              }
              required
            />
          </div>

          <div>
            <label className="block text-sm font-medium">Type</label>
            <select
              className="border p-1 w-full"
              value={formData.type}
              onChange={(e) =>
                setFormData({ ...formData, type: e.target.value })
              }>
              <option value="Email">Email</option>
              <option value="SMS">SMS</option>
            </select>
          </div>

          {formData.type === "Email" && (
            <div>
              <label className="block text-sm font-medium">Subject Line</label>
              <input
                className="border p-1 w-full"
                value={formData.subject || ""}
                onChange={(e) =>
                  setFormData({ ...formData, subject: e.target.value })
                }
              />
            </div>
          )}

          <div>
            <label className="block text-sm font-medium">Content Body</label>
            <textarea
              className="border p-1 w-full h-24"
              value={formData.content}
              onChange={(e) =>
                setFormData({ ...formData, content: e.target.value })
              }
              required
            />
          </div>

          <button
            type="submit"
            className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700">
            Save Template
          </button>
        </form>
      </div>

      {/* LIST */}
      {loading ? (
        <p>Loading...</p>
      ) : error ? (
        <p className="text-red-500">{error}</p>
      ) : (
        <table className="w-full border-collapse border">
          <thead>
            <tr className="bg-gray-100">
              <th className="border p-2 text-left">Name</th>
              <th className="border p-2 text-left">Type</th>
              <th className="border p-2 text-left">Subject</th>
              <th className="border p-2 text-left">Preview</th>
              <th className="border p-2 text-center">Actions</th>
            </tr>
          </thead>
          <tbody>
            {templates.map((t) => (
              <tr key={t.id} className="hover:bg-gray-50">
                <td className="border p-2 font-medium">{t.name}</td>
                <td className="border p-2">
                  <span
                    className={`px-2 py-1 rounded text-xs ${
                      t.type === "Email"
                        ? "bg-blue-100 text-blue-800"
                        : "bg-green-100 text-green-800"
                    }`}>
                    {t.type}
                  </span>
                </td>
                <td className="border p-2 text-gray-600">{t.subject || "—"}</td>
                <td className="border p-2 text-gray-500 text-sm">
                  {t.content.substring(0, 50)}
                  {t.content.length > 50 && "..."}
                </td>
                <td className="border p-2 text-center">
                  <button
                    onClick={() => handleDelete(t.id)}
                    className="text-red-600 hover:underline text-sm">
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
