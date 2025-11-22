import React, { useState, useEffect, useCallback } from "react";
import { apiClient } from "../apiClient";
import type { EventDto, Guid } from "../types";

interface VendorEventsTableProps {
  vendorId: Guid;
}

const VendorEventsTable: React.FC<VendorEventsTableProps> = ({ vendorId }) => {
  const [events, setEvents] = useState<EventDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // FIX: Wrap in useCallback to satisfy ESLint and allow usage in useEffect + Retry button
  const fetchEvents = useCallback(async () => {
    setLoading(true);
    setError(null);

    try {
      const response = await apiClient.get<EventDto[]>(
        `/api/vendors/${vendorId}/events`
      );
      setEvents(response.data);
    } catch (err) {
      console.error("Error fetching events for vendor:", err);
      setError("Failed to load events.");
    } finally {
      setLoading(false);
    }
  }, [vendorId]); // Re-create function only if vendorId changes

  useEffect(() => {
    if (vendorId) {
      fetchEvents();
    }
  }, [fetchEvents, vendorId]); // FIX: Added fetchEvents dependency

  if (loading) return <div style={{ padding: "10px" }}>Loading events...</div>;

  if (error)
    return (
      <div style={{ color: "red", padding: "10px" }}>
        {error} <button onClick={fetchEvents}>Retry</button>
      </div>
    );

  return (
    <div style={{ marginBottom: "30px" }}>
      <h3>Events Hired For ({events.length})</h3>
      {events.length === 0 ? (
        <p>This vendor is not currently linked to any events.</p>
      ) : (
        <table style={{ width: "100%", borderCollapse: "collapse" }}>
          <thead>
            <tr style={{ borderBottom: "2px solid #ccc" }}>
              <th style={{ padding: "8px", textAlign: "left" }}>Event Name</th>
              <th style={{ padding: "8px", textAlign: "left" }}>Date</th>
              <th style={{ padding: "8px", textAlign: "left" }}>Status</th>
            </tr>
          </thead>
          <tbody>
            {events.map((event) => (
              <tr key={event.id} style={{ borderBottom: "1px solid #eee" }}>
                <td style={{ padding: "8px" }}>{event.name}</td>
                <td style={{ padding: "8px" }}>
                  {new Date(event.startDateTime).toLocaleDateString()}
                </td>
                <td style={{ padding: "8px" }}>{event.status}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
};

export default VendorEventsTable;
