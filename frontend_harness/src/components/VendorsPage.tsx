// FILE: frontend_harness/src/components/VendorsPage.tsx

import React, { useEffect, useState, useCallback, useRef } from "react";
import { apiClient } from "../apiClient";
import type { VendorDto, Guid } from "../types";
import axios from "axios";
import { useSelectionContext } from "../context/useSelectionContext";
import { RelatedTasks, RelatedNotes } from "./RelatedTables";
import VendorEventsTable from "./VendorEventsTable";
import { DocumentsSection } from "./DocumentsSection"; // NEW: The Reusable Component

type VendorFormData = Omit<VendorDto, "id">;
type ValidationErrors = { [key: string]: string[] };

const initialFormState: VendorFormData = {
  name: "",
  phoneNumber: null,
  email: null,
  serviceType: "Other",
  attributes: null, // NEW: Include attributes
};

function VendorsPage() {
  const [vendors, setVendors] = useState<VendorDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const { selectedVendorId, setSelectedVendorId } = useSelectionContext();

  const selectedVendorIdRef = useRef(selectedVendorId);
  useEffect(() => {
    selectedVendorIdRef.current = selectedVendorId;
  }, [selectedVendorId]);

  const [formData, setFormData] =
    useState<Partial<VendorDto>>(initialFormState);
  const [editingId, setEditingId] = useState<string | null>(null);
  const [validationErrors, setValidationErrors] = useState<ValidationErrors>(
    {}
  );

  // --- NEW: Dynamic Attribute State ---
  const [dynamicFields, setDynamicFields] = useState<{
    Capacity?: number | string;
    Genre?: string;
  }>({});

  const isEditing = editingId !== null;
  const selectedVendor = vendors.find((v) => v.id === selectedVendorId);

  // Function to initialize dynamic fields from the current formData.attributes
  const initializeDynamicFields = useCallback(
    (attributesString: string | null) => {
      if (!attributesString) {
        setDynamicFields({});
        return;
      }
      try {
        const parsed = JSON.parse(attributesString);
        // Set the dynamic fields, ensuring Capacity is handled as a number/string
        setDynamicFields({
          Capacity: parsed.Capacity || "",
          Genre: parsed.Genre || "",
        });
      } catch (e) {
        console.error("Failed to parse vendor attributes JSON:", e);
        setDynamicFields({});
      }
    },
    []
  );

  const fetchVendors = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      const response = await apiClient.get<VendorDto[]>("/api/vendors");
      setVendors(response.data);
      return response.data;
    } catch (err: unknown) {
      console.error(err);
      setError("Failed to fetch vendors.");
      return [];
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    let mounted = true;

    fetchVendors().then((data) => {
      if (mounted && data && data.length > 0 && !selectedVendorIdRef.current) {
        setSelectedVendorId(data[0].id);
      }
    });

    return () => {
      mounted = false;
      setSelectedVendorId(null);
    };
  }, [fetchVendors, setSelectedVendorId]);

  const handleDelete = async (id: string) => {
    if (!window.confirm("Are you sure you want to delete this vendor?")) return;
    try {
      await apiClient.delete(`/api/vendors/${id}`);
      if (selectedVendorId === id) {
        setSelectedVendorId(null);
      }
      await fetchVendors();
    } catch (err) {
      console.error(err);
      alert("Failed to delete vendor.");
    }
  };

  const handleEdit = (vendor: VendorDto) => {
    setEditingId(vendor.id);
    setFormData(vendor);
    initializeDynamicFields(vendor.attributes); // Initialize dynamic fields when editing
    setValidationErrors({});
    window.scrollTo(0, 0);
  };

  const handleCancel = () => {
    setEditingId(null);
    setFormData(initialFormState);
    setDynamicFields({}); // Clear dynamic fields
    setValidationErrors({});
  };

  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>
  ) => {
    const { name, value } = e.target;
    // When ServiceType changes, reset the dynamic fields state
    if (name === "serviceType") {
      setDynamicFields({});
    }

    setFormData((prev) => ({
      ...prev,
      [name]: value === "" ? null : value,
    }));
  };

  // NEW: Handle changes for dynamic fields separately
  const handleDynamicChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setDynamicFields((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const getAttributesJson = () => {
    const cleanFields = Object.entries(dynamicFields)
      .filter(([, value]) => value !== null && value !== "" && value !== 0)
      .reduce((acc, [key, value]) => {
        // Attempt to parse number fields back to number types for cleaner JSON
        if (
          key === "Capacity" &&
          typeof value === "string" &&
          !isNaN(Number(value))
        ) {
          acc[key] = Number(value);
        } else {
          acc[key] = value;
        }
        return acc;
      }, {} as Record<string, string | number>);

    if (Object.keys(cleanFields).length === 0) {
      return null;
    }
    return JSON.stringify(cleanFields);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setValidationErrors({});

    // --- PHASE 27: Inject serialized attributes into payload ---
    const attributesJson = getAttributesJson();

    const payload = {
      ...formData,
      name: formData.name ?? "",
      serviceType: formData.serviceType ?? "Other",
      attributes: attributesJson, // INJECT THE JSON STRING
    };
    // -------------------------------------------------------------

    const promise = isEditing
      ? apiClient.put(`/api/vendors/${editingId}`, payload)
      : apiClient.post("/api/vendors", payload);

    try {
      await promise;
      handleCancel();
      await fetchVendors();
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

  // --- NEW: Dynamic Field Renderer ---
  const renderDynamicFields = (serviceType: string | null | undefined) => {
    if (serviceType?.toLowerCase() === "venue") {
      return (
        <div style={{ marginBottom: "0.5rem" }}>
          <label>Venue Capacity: </label>
          <input
            name="Capacity"
            type="number"
            value={dynamicFields.Capacity ?? ""}
            onChange={handleDynamicChange}
            placeholder="Max Capacity (e.g., 500)"
            style={{
              width: "100%",
              padding: "8px",
              backgroundColor: "#333",
              color: "white",
              border: "1px solid #555",
            }}
          />
        </div>
      );
    }
    if (serviceType?.toLowerCase() === "entertainment") {
      return (
        <div style={{ marginBottom: "0.5rem" }}>
          <label>Artist Genre: </label>
          <input
            name="Genre"
            type="text"
            value={dynamicFields.Genre ?? ""}
            onChange={handleDynamicChange}
            placeholder="e.g., Jazz, EDM, Acoustic"
            style={{
              width: "100%",
              padding: "8px",
              backgroundColor: "#333",
              color: "white",
              border: "1px solid #555",
            }}
          />
        </div>
      );
    }
    return null;
  };

  const renderVendorListPanel = () => (
    <div
      style={{
        flex: "0 0 350px",
        padding: "1rem",
        borderRight: "1px solid #555",
        overflowY: "auto",
      }}>
      <h2>Vendors List</h2>
      <button
        onClick={() => {
          setEditingId(null);
          setFormData(initialFormState);
          setDynamicFields({}); // Ensure state is reset
          setSelectedVendorId(null);
        }}
        style={{ marginBottom: "1rem", padding: "10px" }}>
        + Add New Vendor
      </button>

      {loading ? (
        <p>Loading Vendors...</p>
      ) : (
        <ul style={{ listStyle: "none", padding: 0 }}>
          {vendors.map((vendor) => (
            <li
              key={vendor.id}
              onClick={() => setSelectedVendorId(vendor.id as Guid)}
              style={{
                padding: "10px",
                marginBottom: "5px",
                cursor: "pointer",
                border: "1px solid #555",
                backgroundColor:
                  vendor.id === selectedVendorId ? "#444" : "#2a2a2a",
                borderRadius: "4px",
                display: "flex",
                justifyContent: "space-between",
                alignItems: "center",
                color: "white",
              }}>
              <div>
                <strong>{vendor.name}</strong> <br />
                <span style={{ fontSize: "0.8rem", color: "#ccc" }}>
                  {vendor.serviceType}
                  {/* NEW: Display a dynamic attribute if present */}
                  {vendor.attributes &&
                    (() => {
                      try {
                        const attrs = JSON.parse(vendor.attributes);
                        if (attrs.Capacity)
                          return ` | Capacity: ${attrs.Capacity}`;
                        if (attrs.Genre) return ` | Genre: ${attrs.Genre}`;
                        return null;
                      } catch {
                        return null; // Ignore invalid JSON
                      }
                    })()}
                </span>
              </div>
              <div style={{ fontSize: "0.8rem" }}>
                <button
                  onClick={(e) => {
                    e.stopPropagation();
                    handleEdit(vendor);
                  }}
                  style={{
                    color: "#64b5f6",
                    marginRight: "5px",
                    padding: "2px 5px",
                    background: "transparent",
                    border: "none",
                    cursor: "pointer",
                  }}>
                  Edit
                </button>
                <button
                  onClick={(e) => {
                    e.stopPropagation();
                    handleDelete(vendor.id as Guid);
                  }}
                  style={{
                    color: "#ef5350",
                    padding: "2px 5px",
                    background: "transparent",
                    border: "none",
                    cursor: "pointer",
                  }}>
                  Delete
                </button>
              </div>
            </li>
          ))}
        </ul>
      )}
    </div>
  );

  const renderVendorDashboard = () => {
    if (isEditing || !selectedVendorId) {
      return (
        <div style={{ flex: 1, padding: "20px", overflowY: "auto" }}>
          <div
            style={{
              padding: "1rem",
              border: "1px solid #555",
              width: "100%",
            }}>
            <h2>
              {isEditing
                ? `Edit Vendor: ${selectedVendor?.name}`
                : "Create New Vendor"}
            </h2>
            <form onSubmit={handleSubmit}>
              <div style={{ marginBottom: "0.5rem" }}>
                <label>Name: </label>
                <input
                  name="name"
                  value={formData.name ?? ""}
                  onChange={handleChange}
                  style={{
                    width: "100%",
                    padding: "8px",
                    backgroundColor: "#333",
                    color: "white",
                    border: "1px solid #555",
                  }}
                />
                {validationErrors.Name && (
                  <div style={{ color: "#ef5350" }}>
                    {validationErrors.Name[0]}
                  </div>
                )}
              </div>
              <div style={{ marginBottom: "0.5rem" }}>
                <label>Email: </label>
                <input
                  name="email"
                  type="email"
                  value={formData.email ?? ""}
                  onChange={handleChange}
                  style={{
                    width: "100%",
                    padding: "8px",
                    backgroundColor: "#333",
                    color: "white",
                    border: "1px solid #555",
                  }}
                />
              </div>
              <div style={{ marginBottom: "0.5rem" }}>
                <label>Phone Number: </label>
                <input
                  name="phoneNumber"
                  value={formData.phoneNumber ?? ""}
                  onChange={handleChange}
                  style={{
                    width: "100%",
                    padding: "8px",
                    backgroundColor: "#333",
                    color: "white",
                    border: "1px solid #555",
                  }}
                />
              </div>
              <div style={{ marginBottom: "0.5rem" }}>
                <label>Service Type: </label>
                <select
                  name="serviceType"
                  value={formData.serviceType ?? "Other"}
                  onChange={handleChange}
                  style={{
                    width: "100%",
                    padding: "8px",
                    backgroundColor: "#333",
                    color: "white",
                    border: "1px solid #555",
                  }}>
                  {/* NOTE: Added Entertainment option as a requirement example */}
                  <option value="Catering">Catering</option>
                  <option value="Security">Security</option>
                  <option value="Venue">Venue</option>
                  <option value="Entertainment">Entertainment</option>
                  <option value="Florist">Florist</option>
                  <option value="Photography">Photography</option>
                  <option value="Videography">Videography</option>
                  <option value="Other">Other</option>
                </select>
              </div>

              {/* --- PHASE 27: Dynamic Attribute Fields RENDER HERE --- */}
              {renderDynamicFields(formData.serviceType)}

              {validationErrors.Attributes && (
                <div style={{ color: "#ef5350", marginTop: "0.5rem" }}>
                  {/* Display validation error for the JSON field */}
                  {validationErrors.Attributes[0]}
                </div>
              )}
              {/* -------------------------------------------------------- */}

              <button
                type="submit"
                style={{
                  marginTop: "10px",
                  padding: "10px 20px",
                  cursor: "pointer",
                }}>
                {isEditing ? "Save Changes" : "Create"}
              </button>
              {isEditing && (
                <button
                  type="button"
                  onClick={handleCancel}
                  style={{
                    marginLeft: "0.5rem",
                    marginTop: "10px",
                    padding: "10px 20px",
                    cursor: "pointer",
                  }}>
                  Cancel
                </button>
              )}
              {validationErrors.General && (
                <div style={{ color: "#ef5350", marginTop: "1rem" }}>
                  {validationErrors.General[0]}
                </div>
              )}
            </form>
          </div>
        </div>
      );
    }

    const vendorIdString = selectedVendorId as string;

    // --- PHASE 27: Dashboard View ---
    const parsedAttributes = selectedVendor?.attributes
      ? JSON.parse(selectedVendor.attributes)
      : {};

    const attributeDisplay = Object.entries(parsedAttributes).map(
      ([key, value]) => (
        <p key={key}>
          <strong>{key}:</strong> {value as string}
        </p>
      )
    );

    return (
      <div style={{ flex: 1, padding: "20px", overflowY: "auto" }}>
        <h1>Vendor Command Center: {selectedVendor?.name}</h1>
        <div
          style={{
            marginBottom: "20px",
            padding: "15px",
            border: "1px solid #555",
            borderRadius: "5px",
            width: "100%",
          }}>
          <h3>Details</h3>
          <p>
            <strong>ID:</strong> {selectedVendor?.id}
          </p>
          <p>
            <strong>Type:</strong> {selectedVendor?.serviceType}
          </p>
          <p>
            <strong>Contact Info:</strong> {selectedVendor?.phoneNumber} /{" "}
            {selectedVendor?.email}
          </p>
          {/* NEW: Dynamic Attribute Display */}
          {attributeDisplay.length > 0 && (
            <div
              style={{
                marginTop: "10px",
                borderTop: "1px dashed #666",
                paddingTop: "10px",
              }}>
              <h4>Specific Attributes</h4>
              {attributeDisplay}
            </div>
          )}
        </div>

        <h2 style={{ marginTop: "30px" }}>Related Data</h2>

        {/* NEW: Reusable Documents Section for Vendors */}
        <DocumentsSection entityId={vendorIdString} entityType="Vendor" />

        <VendorEventsTable vendorId={vendorIdString as Guid} />
        <RelatedTasks vendorId={vendorIdString} />
        <RelatedNotes vendorId={vendorIdString} />
      </div>
    );
  };

  return (
    <div style={{ display: "flex", height: "100%", minHeight: "800px" }}>
      {renderVendorListPanel()}
      {renderVendorDashboard()}
    </div>
  );
}

export default VendorsPage;
