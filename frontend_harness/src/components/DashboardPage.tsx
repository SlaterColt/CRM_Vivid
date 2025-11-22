import { useEffect, useState } from "react";
import { apiClient } from "../apiClient";
import { SendEmailModal } from "./SendEmailModal";
import type { DashboardStatsDto, ContactDto, VendorDto } from "../types";

export default function DashboardPage() {
  const [stats, setStats] = useState<DashboardStatsDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  // --- Modal State ---
  const [isEmailModalOpen, setIsEmailModalOpen] = useState(false);
  const [selectedEventId, setSelectedEventId] = useState<string>("");

  // --- Auxiliary Data ---
  const [contacts, setContacts] = useState<ContactDto[]>([]);
  const [vendors, setVendors] = useState<VendorDto[]>([]);

  useEffect(() => {
    const loadDashboardData = async () => {
      try {
        setLoading(true);
        const [statsRes, contactsRes, vendorsRes] = await Promise.all([
          apiClient.get<DashboardStatsDto>("/api/dashboard"),
          apiClient.get<ContactDto[]>("/api/contacts"),
          apiClient.get<VendorDto[]>("/api/vendors"),
        ]);

        setStats(statsRes.data);
        setContacts(contactsRes.data);
        setVendors(vendorsRes.data);
        setLoading(false);
      } catch (err) {
        console.error(err);
        setError("Failed to load dashboard data.");
        setLoading(false);
      }
    };

    loadDashboardData();
  }, []);

  const handleOpenEmailModal = (eventId: string) => {
    setSelectedEventId(eventId);
    setIsEmailModalOpen(true);
  };

  if (loading)
    return (
      <div
        style={{
          display: "flex",
          justifyContent: "center",
          alignItems: "center",
          height: "200px",
          color: "#9ca3af",
        }}>
        Initializing Command Center...
      </div>
    );
  if (error) return <div className="p-8 text-red-400">{error}</div>;
  if (!stats) return null;

  return (
    <div className="space-y-8 pb-10 max-w-7xl mx-auto animate-fade-in">
      {/* Header - Forced Flex */}
      <div
        style={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "flex-end",
          borderBottom: "1px solid #1f2937",
          paddingBottom: "1rem",
          marginBottom: "2rem",
        }}>
        <div>
          <h1 className="text-3xl font-bold text-white tracking-tight">
            Command Center
          </h1>
          <p className="text-gray-400 mt-1">System Status & Overview</p>
        </div>
        <div className="hidden sm:block text-right">
          <p className="text-sm text-gray-500">Current Phase</p>
          <p className="text-blue-400 font-mono">Phase 22: The Messenger</p>
        </div>
      </div>

      {/* Stat Cards - Forced Grid */}
      <div
        style={{
          display: "grid",
          gridTemplateColumns: "repeat(auto-fit, minmax(240px, 1fr))",
          gap: "1rem",
          marginBottom: "2rem",
        }}>
        <StatCard
          label="Total Contacts"
          value={stats.totalContacts}
          color="border-blue-500"
          icon={
            <svg
              width="24"
              height="24"
              style={{ minWidth: "24px" }}
              className="text-blue-400"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24">
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 005.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z"
              />
            </svg>
          }
        />
        <StatCard
          label="Active Events"
          value={stats.activeEvents}
          color="border-purple-500"
          icon={
            <svg
              width="24"
              height="24"
              style={{ minWidth: "24px" }}
              className="text-purple-400"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24">
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"
              />
            </svg>
          }
        />
        <StatCard
          label="Pending Tasks"
          value={stats.pendingTasks}
          color="border-yellow-500"
          icon={
            <svg
              width="24"
              height="24"
              style={{ minWidth: "24px" }}
              className="text-yellow-400"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24">
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2m-6 9l2 2 4-4"
              />
            </svg>
          }
        />
        <StatCard
          label="Emails (24h)"
          value={stats.recentEmails}
          color="border-green-500"
          icon={
            <svg
              width="24"
              height="24"
              style={{ minWidth: "24px" }}
              className="text-green-400"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24">
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"
              />
            </svg>
          }
        />
      </div>

      {/* Upcoming Events Section */}
      <div className="bg-gray-800 rounded-xl border border-gray-700 overflow-hidden shadow-sm">
        <div
          className="p-6 border-b border-gray-700 flex justify-between items-center bg-gray-800"
          style={{
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
          }}>
          <h2 className="text-xl font-semibold text-white">Upcoming Events</h2>
          <button className="text-sm text-blue-400 hover:text-blue-300 transition-colors">
            View All
          </button>
        </div>

        <div className="divide-y divide-gray-700">
          {stats.upcomingEvents.length === 0 ? (
            <div className="p-8 text-center text-gray-500 italic">
              No upcoming events scheduled. Time to book something!
            </div>
          ) : (
            stats.upcomingEvents.map((evt) => {
              const dateObj = new Date(evt.startDateTime);
              const month = dateObj.toLocaleString("default", {
                month: "short",
              });
              const day = dateObj.getDate();
              const time = dateObj.toLocaleTimeString([], {
                hour: "2-digit",
                minute: "2-digit",
              });

              return (
                <div
                  key={evt.id}
                  className="p-4 hover:bg-gray-700/50 transition-colors"
                  style={{
                    display: "flex",
                    alignItems: "center",
                    gap: "1rem",
                  }}>
                  {/* Date Badge */}
                  <div
                    className="bg-gray-900 rounded-lg border border-gray-600 shadow-inner"
                    style={{
                      width: "56px",
                      height: "56px",
                      flexShrink: 0,
                      display: "flex",
                      flexDirection: "column",
                      alignItems: "center",
                      justifyContent: "center",
                    }}>
                    <span className="text-xs text-gray-400 uppercase font-bold">
                      {month}
                    </span>
                    <span className="text-xl font-bold text-white leading-none">
                      {day}
                    </span>
                  </div>

                  {/* Event Info - Grow to take space */}
                  <div style={{ flexGrow: 1, minWidth: 0 }}>
                    <h3 className="text-white font-medium truncate">
                      {evt.name}
                    </h3>

                    {/* Meta Row */}
                    <div
                      className="text-sm text-gray-400 mt-1"
                      style={{
                        display: "flex",
                        flexWrap: "wrap",
                        alignItems: "center",
                        gap: "1rem",
                      }}>
                      <span
                        style={{
                          display: "flex",
                          alignItems: "center",
                          gap: "4px",
                        }}>
                        <svg
                          width="16"
                          height="16"
                          fill="none"
                          stroke="currentColor"
                          viewBox="0 0 24 24">
                          <path
                            strokeLinecap="round"
                            strokeLinejoin="round"
                            strokeWidth={2}
                            d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"
                          />
                        </svg>
                        {time}
                      </span>
                      {evt.location && (
                        <span
                          style={{
                            display: "flex",
                            alignItems: "center",
                            gap: "4px",
                          }}>
                          <svg
                            width="16"
                            height="16"
                            fill="none"
                            stroke="currentColor"
                            viewBox="0 0 24 24">
                            <path
                              strokeLinecap="round"
                              strokeLinejoin="round"
                              strokeWidth={2}
                              d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z"
                            />
                            <path
                              strokeLinecap="round"
                              strokeLinejoin="round"
                              strokeWidth={2}
                              d="M15 11a3 3 0 11-6 0 3 3 0 016 0z"
                            />
                          </svg>
                          {evt.location}
                        </span>
                      )}
                    </div>
                  </div>

                  {/* Action Button */}
                  <button
                    onClick={() => handleOpenEmailModal(evt.id)}
                    className="rounded-lg bg-gray-900 hover:bg-blue-600 text-gray-300 hover:text-white border border-gray-600 hover:border-blue-500 transition-all shadow-sm"
                    style={{
                      flexShrink: 0,
                      display: "flex",
                      alignItems: "center",
                      gap: "0.5rem",
                      padding: "0.5rem 1rem",
                    }}
                    title="Send Email">
                    <svg
                      width="20"
                      height="20"
                      fill="none"
                      viewBox="0 0 24 24"
                      stroke="currentColor">
                      <path
                        strokeLinecap="round"
                        strokeLinejoin="round"
                        strokeWidth={2}
                        d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"
                      />
                    </svg>
                    <span className="hidden sm:inline text-sm font-medium">
                      Email
                    </span>
                  </button>
                </div>
              );
            })
          )}
        </div>
      </div>

      {/* The Email Modal */}
      <SendEmailModal
        isOpen={isEmailModalOpen}
        onClose={() => setIsEmailModalOpen(false)}
        eventId={selectedEventId}
        availableContacts={contacts}
        availableVendors={vendors}
      />
    </div>
  );
}

function StatCard({
  label,
  value,
  color,
  icon,
}: {
  label: string;
  value: number;
  color: string;
  icon?: React.ReactNode;
}) {
  return (
    <div
      className={`bg-gray-800 p-6 rounded-xl border-l-4 ${color} shadow-md`}
      style={{
        display: "flex",
        justifyContent: "space-between",
        alignItems: "flex-start",
      }}>
      <div>
        <p className="text-gray-400 text-xs font-bold uppercase tracking-wider mb-1">
          {label}
        </p>
        <p className="text-3xl font-bold text-white">{value}</p>
      </div>
      {/* Icon container with strict styling */}
      {icon && (
        <div
          style={{
            width: "32px",
            height: "32px",
            flexShrink: 0,
            opacity: 0.8,
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
          }}>
          {icon}
        </div>
      )}
    </div>
  );
}
