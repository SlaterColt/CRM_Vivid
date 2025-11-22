import React, { useState } from "react";
import axios from "axios";
import { uploadDocument } from "../apiClient";
import type { Document } from "../types"; // Fix: Type-only import

interface FileUploaderProps {
  relatedEntityId: string;
  relatedEntityType: "Event" | "Vendor" | "Contact" | "Task";
  onUploadSuccess?: (document: Document) => void;
}

// Hardcoded categories for The Librarian
const DOCUMENT_CATEGORIES = [
  "General",
  "Invoice",
  "Contract",
  "Rider",
  "Marketing",
  "Legal",
];

export const FileUploader: React.FC<FileUploaderProps> = ({
  relatedEntityId,
  relatedEntityType,
  onUploadSuccess,
}) => {
  const [isUploading, setIsUploading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [category, setCategory] = useState<string>("General"); // Default category

  const handleFileChange = async (e: React.ChangeEvent<HTMLInputElement>) => {
    if (!e.target.files || e.target.files.length === 0) return;

    const file = e.target.files[0];
    await handleUpload(file);
  };

  const handleUpload = async (file: File) => {
    setIsUploading(true);
    setError(null);

    try {
      // Pass the selected category to the API
      const result = await uploadDocument(
        file,
        relatedEntityId,
        relatedEntityType,
        category
      );
      if (onUploadSuccess) {
        onUploadSuccess(result);
      }
    } catch (err) {
      console.error(err);

      let errorMessage = "Failed to upload file.";

      if (axios.isAxiosError(err) && err.response?.data) {
        const data = err.response.data;
        if (typeof data === "string") {
          errorMessage = data;
        } else if (typeof data === "object") {
          if (data.message) {
            errorMessage = data.message;
            if (data.details) errorMessage += ` (${data.details})`;
          } else if (data.title) {
            errorMessage = data.title;
          } else if (data.errors) {
            errorMessage = Object.values(data.errors).flat().join(", ");
          }
        }
      } else if (err instanceof Error) {
        errorMessage = err.message;
      }

      setError(errorMessage);
    } finally {
      setIsUploading(false);
    }
  };

  return (
    <div className="p-4 border-2 border-dashed border-gray-300 rounded-lg bg-gray-50 text-center hover:bg-gray-100 transition-colors">
      {/* Category Selector */}
      <div
        style={{
          marginBottom: "1rem",
          display: "flex",
          justifyContent: "center",
          alignItems: "center",
          gap: "10px",
        }}>
        <label style={{ fontSize: "0.9rem", fontWeight: "600", color: "#555" }}>
          Document Type:
        </label>
        <select
          value={category}
          onChange={(e) => setCategory(e.target.value)}
          disabled={isUploading}
          style={{
            padding: "5px 10px",
            borderRadius: "4px",
            border: "1px solid #ccc",
            fontSize: "0.9rem",
            backgroundColor: "white",
            color: "#333",
          }}>
          {DOCUMENT_CATEGORIES.map((cat) => (
            <option key={cat} value={cat}>
              {cat}
            </option>
          ))}
        </select>
      </div>

      <input
        type="file"
        id={`file-upload-${relatedEntityId}`}
        className="hidden"
        onChange={handleFileChange}
        disabled={isUploading}
      />
      <label
        htmlFor={`file-upload-${relatedEntityId}`}
        className="cursor-pointer flex flex-col items-center justify-center">
        {isUploading ? (
          <span className="text-blue-600 font-semibold">Uploading...</span>
        ) : (
          <>
            <span className="text-gray-700 font-medium">
              Click to Upload Document
            </span>
            <span className="text-xs text-gray-500 mt-1">
              (PDF, DOCX, PNG, JPG)
            </span>
          </>
        )}
      </label>

      {error && (
        <div
          style={{
            color: "red",
            marginTop: "10px",
            fontSize: "0.85rem",
            maxHeight: "100px",
            overflowY: "auto",
            textAlign: "left",
            padding: "5px",
            background: "#fff0f0",
            border: "1px solid #ffcccc",
          }}>
          <strong>Error:</strong> {error}
        </div>
      )}
    </div>
  );
};
