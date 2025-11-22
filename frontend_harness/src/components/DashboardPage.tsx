import { useEffect, useState } from "react";
import { apiClient } from "../apiClient";
import type { DashboardStatsDto } from "../types";

export default function DashboardPage() {
  const [stats, setStats] = useState<DashboardStatsDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    // FIXED: Added '/api' prefix to match the Controller route
    apiClient
      .get<DashboardStatsDto>("/api/dashboard")
      .then((res) => {
        setStats(res.data);
        setLoading(false);
      })
      .catch((err) => {
        console.error(err);
        setError("Failed to load dashboard stats.");
        setLoading(false);
      });
  }, []);

  if (loading)
    return (
      <div className="p-8 text-gray-400 animate-pulse">
        Initializing Command Center...
      </div>
    );
  if (error) return <div className="p-8 text-red-400">{error}</div>;
  if (!stats) return null;

  return (
    <div className="space-y-8 animate-fade-in">
      {/* Header */}
      <div>
        <h1 className="text-3xl font-bold text-white tracking-tight">
          Command Center
        </h1>
        <p className="text-gray-400">System Status & Overview</p>
      </div>

      {/* Stat Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
        <StatCard
          label="Total Contacts"
          value={stats.totalContacts}
          color="border-blue-500"
        />
        <StatCard
          label="Active Events"
          value={stats.activeEvents}
          color="border-purple-500"
        />
        <StatCard
          label="Pending Tasks"
          value={stats.pendingTasks}
          color="border-yellow-500"
        />
        <StatCard
          label="Emails (24h)"
          value={stats.recentEmails}
          color="border-green-500"
        />
      </div>

      {/* Upcoming Events Section */}
      <div className="bg-gray-800/50 rounded-xl border border-gray-700 p-6">
        <h2 className="text-xl font-semibold text-white mb-4">
          Upcoming Events
        </h2>
        {stats.upcomingEvents.length === 0 ? (
          <p className="text-gray-500 italic">No upcoming events scheduled.</p>
        ) : (
          <div className="space-y-3">
            {stats.upcomingEvents.map((evt) => (
              <div
                key={evt.id}
                className="flex items-center justify-between p-3 bg-gray-900/50 rounded-lg border border-gray-800 hover:border-gray-600 transition-colors">
                <div>
                  <h3 className="text-white font-medium">{evt.name}</h3>
                  <p className="text-sm text-gray-400">
                    {new Date(evt.startDateTime).toLocaleDateString()} @{" "}
                    {new Date(evt.startDateTime).toLocaleTimeString([], {
                      hour: "2-digit",
                      minute: "2-digit",
                    })}
                  </p>
                </div>
                <div className="px-3 py-1 rounded-full text-xs font-medium bg-gray-800 text-gray-300 border border-gray-700">
                  {evt.location || "TBD"}
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}

function StatCard({
  label,
  value,
  color,
}: {
  label: string;
  value: number;
  color: string;
}) {
  return (
    <div className={`bg-gray-800 p-6 rounded-xl border-l-4 ${color} shadow-lg`}>
      <p className="text-gray-400 text-sm font-medium uppercase tracking-wider">
        {label}
      </p>
      <p className="text-4xl font-bold text-white mt-2">{value}</p>
    </div>
  );
}
