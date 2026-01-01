import React, { useEffect, useState } from "react";
import { apiClient, deleteDocument } from "../apiClient";
import type { DocumentDto } from "../types"; // Adjusted import to DocumentDto
import { FileUploader } from "./FileUploader";

interface DocumentsSectionProps {
  entityId: string;
  entityType: "Event" | "Vendor" | "Contact" | "Task";
}

export const DocumentsSection: React.FC<DocumentsSectionProps> = ({
  entityId,
  entityType,
}) => {
  const [documents, setDocuments] = useState<DocumentDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [generating, setGenerating] = useState(false); // New state for button

  useEffect(() => {
    const fetchDocuments = async () => {
      setLoading(true);
      try {
        const response = await apiClient.get<DocumentDto[]>("/api/documents", {
          params: { relatedEntityId: entityId, relatedEntityType: entityType },
        });
        setDocuments(response.data);
      } catch (err) {
        console.error("Failed to load documents", err);
      } finally {
        setLoading(false);
      }
    };

    if (entityId) fetchDocuments();
  }, [entityId, entityType]);

  const handleUploadSuccess = (newDoc: DocumentDto) => {
    setDocuments((prev) => [newDoc, ...prev]);
  };

  // --- NEW: Generate Contract Function ---
  const handleGenerateContract = async () => {
    if (!window.confirm("Generate a PDF contract based on current event data?"))
      return;

    setGenerating(true);
    try {
      // Calls the new POST endpoint
      const response = await apiClient.post<DocumentDto>(
        `/api/documents/generate-contract/${entityId}`
      );
      // Add the new file to the list immediately
      setDocuments((prev) => [response.data, ...prev]);
      alert("Contract generated successfully!");
    } catch (err) {
      console.error("Failed to generate contract", err);
      alert("Failed to generate contract. Ensure Event ID is valid.");
    } finally {
      setGenerating(false);
    }
  };

  const handleDeleteDocument = async (id: number) => {
    if (
      !window.confirm(
        "Are you sure you want to permanently delete this document?"
      )
    )
      return;

    try {
      await deleteDocument(id);
      setDocuments((prev) => prev.filter((doc) => doc.id !== id));
    } catch (err) {
      console.error("Failed to delete document", err);
      alert("Failed to delete document.");
    }
  };

  const formatFileSize = (bytes: number) => {
    if (bytes === 0) return "0 Bytes";
    const k = 1024;
    const sizes = ["Bytes", "KB", "MB", "GB"];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + " " + sizes[i];
  };

  const getCategoryColor = (cat: string) => {
    switch (cat) {
      case "Invoice":
        return "#e0f2fe";
      case "Contract":
        return "#fce7f3";
      case "Legal":
        return "#fee2e2";
      case "Marketing":
        return "#dcfce7";
      default:
        return "#f3f4f6";
    }
  };

  const getCategoryTextColor = (cat: string) => {
    switch (cat) {
      case "Invoice":
        return "#0369a1";
      case "Contract":
        return "#be185d";
      case "Legal":
        return "#b91c1c";
      case "Marketing":
        return "#15803d";
      default:
        return "#374151";
    }
  };

  return (
    <div
      style={{
        border: "1px solid #ddd",
        borderRadius: "8px",
        padding: "1.5rem",
        marginTop: "2rem",
        backgroundColor: "#ffffff",
        color: "#333",
        boxShadow: "0 2px 4px rgba(0,0,0,0.1)",
      }}>
      <div
        style={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          borderBottom: "2px solid #eee",
          marginBottom: "1rem",
          paddingBottom: "0.5rem",
        }}>
        <h3 style={{ margin: 0, color: "#222" }}>{entityType} Documents</h3>

        {/* Only show Generate button for Events */}
        {entityType === "Event" && (
          <button
            onClick={handleGenerateContract}
            disabled={generating}
            style={{
              backgroundColor: "#FF6F61", // Electric Coral
              color: "white",
              border: "none",
              padding: "8px 16px",
              borderRadius: "6px",
              fontWeight: "bold",
              cursor: generating ? "not-allowed" : "pointer",
              boxShadow: "0 4px 10px 0 rgba(255, 111, 97, 0.3)",
              transition: "all 0.2s",
            }}>
            {generating ? "Generating..." : "✨ Generate Contract"}
          </button>
        )}
      </div>

      <div style={{ marginBottom: "1.5rem" }}>
        <FileUploader
          relatedEntityId={entityId}
          relatedEntityType={entityType}
          onUploadSuccess={handleUploadSuccess}
        />
      </div>

      {loading ? (
        <p style={{ color: "#666" }}>Loading documents...</p>
      ) : documents.length === 0 ? (
        <div
          style={{
            textAlign: "center",
            padding: "2rem",
            backgroundColor: "#f9fafb",
            borderRadius: "6px",
            border: "1px dashed #ccc",
            color: "#666",
          }}>
          No documents archived.
        </div>
      ) : (
        <div style={{ overflowX: "auto" }}>
          <table
            style={{
              width: "100%",
              borderCollapse: "collapse",
              fontSize: "0.95rem",
            }}>
            <thead>
              <tr
                style={{
                  backgroundColor: "#374151",
                  color: "#ffffff",
                  textAlign: "left",
                }}>
                <th style={{ padding: "12px", borderTopLeftRadius: "6px" }}>
                  File Name
                </th>
                <th style={{ padding: "12px" }}>Category</th>
                <th style={{ padding: "12px" }}>Size</th>
                <th style={{ padding: "12px" }}>Uploaded</th>
                <th style={{ padding: "12px", borderTopRightRadius: "6px" }}>
                  Action
                </th>
              </tr>
            </thead>
            <tbody>
              {documents.map((doc, index) => (
                <tr
                  key={doc.id}
                  style={{
                    borderBottom: "1px solid #eee",
                    backgroundColor: index % 2 === 0 ? "#fff" : "#f8f9fa",
                  }}>
                  <td
                    style={{
                      padding: "12px",
                      color: "#333",
                      fontWeight: "500",
                    }}>
                    {doc.fileName}
                  </td>
                  <td style={{ padding: "12px" }}>
                    <span
                      style={{
                        backgroundColor: getCategoryColor(doc.category),
                        color: getCategoryTextColor(doc.category),
                        padding: "4px 8px",
                        borderRadius: "12px",
                        fontSize: "0.8rem",
                        fontWeight: "600",
                      }}>
                      {doc.category || "General"}
                    </span>
                  </td>
                  <td style={{ padding: "12px", color: "#555" }}>
                    {formatFileSize(doc.size)}
                  </td>
                  <td style={{ padding: "12px", color: "#555" }}>
                    {new Date(doc.uploadedAt).toLocaleDateString()}
                  </td>
                  <td style={{ padding: "12px" }}>
                    <a
                      href={`http://localhost:5179${doc.url}`}
                      target="_blank"
                      rel="noreferrer"
                      style={{
                        color: "#2563eb",
                        fontWeight: "600",
                        textDecoration: "none",
                        marginRight: "10px",
                      }}>
                      Download
                    </a>
                    <button
                      onClick={() => handleDeleteDocument(doc.id)}
                      style={{
                        color: "#dc2626",
                        fontWeight: "600",
                        background: "none",
                        border: "none",
                        cursor: "pointer",
                      }}>
                      Delete
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
};
