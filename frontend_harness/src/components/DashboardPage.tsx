// FILE: frontend_harness/src/components/DashboardPage.tsx

import { useEffect, useState, useCallback } from "react";
import {
  apiClient,
  submitLead,
  updateContact,
  deleteContact as deleteContactClient,
  initializeTestFixtures,
} from "../apiClient";
import { SendCommunicationModal } from "./SendCommunicationModal";
import type {
  DashboardStatsDto,
  ContactDto,
  VendorDto,
  EventDto,
} from "../types";

// --- INTERFACES ---

interface TestExecutionPanelProps {
  contacts: ContactDto[];
  logTestResult: (
    message: string,
    level: "PASS" | "FAIL" | "INFO" | "WARN"
  ) => void;
  onSeedDatabase: () => Promise<void>;
}

// --- COMPONENT: Test Execution Panel ---

const TestExecutionPanel: React.FC<TestExecutionPanelProps> = ({
  contacts,
  logTestResult,
  onSeedDatabase,
}) => {
  const [manualContactId, setManualContactId] = useState("");
  const [manualTemplateId, setManualTemplateId] = useState("");

  const executeLiveEmail = async () => {
    if (!manualContactId || !manualTemplateId) {
      alert("Please enter both Contact ID and Template ID.");
      return;
    }

    logTestResult(
      `Attempting to send Email to Contact: ${manualContactId}...`,
      "INFO"
    );

    try {
      await apiClient.post("/api/templates/send-email", {
        contactId: manualContactId,
        templateId: manualTemplateId,
      });
      logTestResult("SUCCESS: Email request sent to SendGrid.", "PASS");
      alert("Email Queued Successfully!");
    } catch (e: unknown) {
      const errorMsg = e instanceof Error ? e.message : String(e);
      logTestResult(`FAILED: Could not send email. Error: ${errorMsg}`, "FAIL");
    }
  };

  const executeLiveSms = async () => {
    if (!manualContactId || !manualTemplateId) {
      alert("Please enter both Contact ID and Template ID.");
      return;
    }

    logTestResult(
      `Attempting to send SMS to Contact: ${manualContactId}...`,
      "INFO"
    );

    try {
      await apiClient.post("/api/automation/send-sms", {
        contactId: manualContactId,
        templateId: manualTemplateId,
      });
      logTestResult("SUCCESS: SMS request sent to Twilio.", "PASS");
      alert("SMS Queued Successfully! Check your phone.");
    } catch (e: unknown) {
      const errorMsg = e instanceof Error ? e.message : String(e);
      logTestResult(`FAILED: Could not send SMS. Error: ${errorMsg}`, "FAIL");
    }
  };

  const runSystemTests = useCallback(async () => {
    const output = document.getElementById("test-log-output");
    if (output) output.innerHTML = "";

    logTestResult("Starting System Integrity Check...", "INFO");

    if (contacts.length === 0) {
      logTestResult(
        "ABORT: No contacts found. Please run 'Seed Database' first.",
        "WARN"
      );
      return;
    }

    const testContact = contacts[0];

    // A. Lead Submission
    try {
      const leadId = await submitLead({
        firstName: "Auto",
        lastName: "Lead",
        email: `auto_lead_${Date.now()}@test.com`,
        source: "Dashboard Test Harness",
        phoneNumber: null,
        organization: null,
      });
      logTestResult(
        `POST /leads/submit: PASS. Created ID: ${leadId.substring(0, 8)}...`,
        "PASS"
      );

      await deleteContactClient(leadId);
      logTestResult(`DELETE /contacts: PASS. Cleaned up test lead.`, "PASS");
    } catch (e: unknown) {
      const errorMsg = e instanceof Error ? e.message : String(e);
      logTestResult(`Lead Flow FAILED: ${errorMsg}`, "FAIL");
    }

    // B. Update Contact
    try {
      await updateContact(testContact.id, {
        ...testContact,
        lastName: "UpdatedByTest",
      });
      logTestResult(
        `PUT /contacts: PASS. Updated Contact ${testContact.id.substring(
          0,
          8
        )}.`,
        "PASS"
      );
    } catch (e: unknown) {
      const errorMsg = e instanceof Error ? e.message : String(e);
      logTestResult(`Contact Update FAILED: ${errorMsg}`, "FAIL");
    }

    logTestResult("System Integrity Check Complete.", "INFO");
  }, [contacts, logTestResult]);

  return (
    <div className="mt-8 p-6 border border-gray-700 rounded-xl bg-gray-900 shadow-2xl">
      <div className="flex justify-between items-center mb-6">
        <h2 className="text-xl font-bold text-white flex items-center gap-2">
          <span className="w-3 h-3 bg-green-500 rounded-full animate-pulse"></span>
          API Test Lab
        </h2>
        <span className="text-xs font-mono text-gray-500">v48.0.6</span>
      </div>

      <div className="bg-gray-800 p-4 rounded-lg mb-6 border border-gray-700">
        <h3 className="text-sm font-bold text-blue-400 uppercase tracking-wider mb-4">
          Live Communication Lab
        </h3>

        <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-4">
          <div>
            <label className="block text-xs text-gray-400 mb-1">
              Target Contact ID (UUID)
            </label>
            <input
              value={manualContactId}
              onChange={(e) => setManualContactId(e.target.value)}
              placeholder="e.g. 3fa85f64-..."
              className="w-full bg-gray-900 border border-gray-600 rounded p-2 text-sm text-white font-mono focus:border-blue-500 outline-none"
            />
          </div>
          <div>
            <label className="block text-xs text-gray-400 mb-1">
              Template ID (UUID)
            </label>
            <input
              value={manualTemplateId}
              onChange={(e) => setManualTemplateId(e.target.value)}
              placeholder="e.g. 8efe1c11-..."
              className="w-full bg-gray-900 border border-gray-600 rounded p-2 text-sm text-white font-mono focus:border-purple-500 outline-none"
            />
          </div>
        </div>

        <div className="flex gap-3">
          <button
            onClick={executeLiveEmail}
            className="flex-1 bg-indigo-600 hover:bg-indigo-500 text-white py-2 rounded font-medium transition-colors text-sm">
            Test Real Email
          </button>
          <button
            onClick={executeLiveSms}
            className="flex-1 bg-green-600 hover:bg-green-500 text-white py-2 rounded font-medium transition-colors text-sm">
            Test Real SMS
          </button>
        </div>
      </div>

      <div className="flex gap-4 mb-4">
        <button
          onClick={onSeedDatabase}
          className="flex-1 bg-gray-700 hover:bg-gray-600 text-blue-300 py-2 rounded font-medium transition-colors text-sm border border-gray-600">
          Seed Database (Fixtures)
        </button>

        <button
          onClick={runSystemTests}
          className="flex-1 bg-gray-700 hover:bg-gray-600 text-gray-200 py-2 rounded font-medium transition-colors text-sm border border-gray-600">
          Run CRUD Integrity Suite
        </button>
      </div>

      <div className="bg-black rounded-lg p-3 border border-gray-800">
        <h4 className="text-gray-500 text-[10px] font-bold uppercase mb-2">
          Execution Log:
        </h4>
        <div
          id="test-log-output"
          className="h-32 overflow-y-auto font-mono text-xs space-y-1 text-gray-300"></div>
      </div>
    </div>
  );
};

// --- MAIN PAGE COMPONENT ---

export default function DashboardPage() {
  const [stats, setStats] = useState<DashboardStatsDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const [isEmailModalOpen, setIsEmailModalOpen] = useState(false);
  const [contacts, setContacts] = useState<ContactDto[]>([]);
  const [vendors, setVendors] = useState<VendorDto[]>([]);

  const logTestResult = useCallback(
    (message: string, level: "PASS" | "FAIL" | "INFO" | "WARN") => {
      let color = "#e5e7eb";
      if (level === "PASS") color = "#4ade80";
      if (level === "FAIL") color = "#f87171";
      if (level === "WARN") color = "#fbbf24";

      const logEntry = `<div style="color: ${color}; margin-bottom: 2px;">
      <span style="opacity: 0.5;">[${new Date().toLocaleTimeString()}]</span> 
      <strong>${level}:</strong> ${message}
    </div>`;

      const output = document.getElementById("test-log-output");
      if (output) {
        output.innerHTML += logEntry;
        output.scrollTop = output.scrollHeight;
      }
    },
    []
  );

  // FIXED: Removed synchronous setLoading(true) from the top.
  // We rely on 'loading' being initialized to true for the first run.
  const fetchDashboardData = useCallback(async () => {
    try {
      const [statsRes, contactsRes, vendorsRes] = await Promise.all([
        apiClient.get<DashboardStatsDto>("/api/dashboard"),
        apiClient.get<ContactDto[]>("/api/contacts"),
        apiClient.get<VendorDto[]>("/api/vendors"),
      ]);

      setStats(statsRes.data);
      setContacts(contactsRes.data);
      setVendors(vendorsRes.data);
    } catch (err: unknown) {
      console.error(err);
      setError("Dashboard failed to load. Ensure backend is running.");
    } finally {
      // Ensuring loading is turned off after fetch completes
      setLoading(false);
    }
  }, []);

  const handleSeedDatabase = async () => {
    logTestResult("Seeding Database with Fixtures...", "INFO");

    // FIXED: Manually set loading here for user feedback during manual seed
    setLoading(true);

    try {
      await initializeTestFixtures();
      logTestResult("Fixtures Initialized Successfully.", "PASS");
      await fetchDashboardData();
    } catch {
      logTestResult("Failed to seed database.", "FAIL");
      setLoading(false); // Ensure loading turns off on failure
    }
  };

  useEffect(() => {
    fetchDashboardData();
  }, [fetchDashboardData]);

  if (loading)
    return (
      <div className="min-h-screen flex items-center justify-center text-gray-500 animate-pulse">
        Loading Command Center...
      </div>
    );

  if (error)
    return (
      <div className="p-8 m-8 bg-red-900/20 border border-red-800 text-red-200 rounded-lg">
        <h3 className="font-bold">Connection Error</h3>
        <p>{error}</p>
      </div>
    );

  if (!stats) return null;

  return (
    <div className="space-y-8 pb-10 max-w-7xl mx-auto px-4 sm:px-6">
      <TestExecutionPanel
        contacts={contacts}
        logTestResult={logTestResult}
        onSeedDatabase={handleSeedDatabase}
      />

      <div className="flex flex-col sm:flex-row justify-between items-end border-b border-gray-800 pb-6 gap-4">
        <div>
          <h1 className="text-4xl font-extrabold text-white tracking-tight">
            Command Center
          </h1>
          <p className="text-gray-400 mt-1">
            LCD Entertainment System Overview
          </p>
        </div>
        <div className="text-right">
          <div className="inline-flex items-center px-3 py-1 rounded-full bg-green-900/30 border border-green-800 text-green-400 text-xs font-bold uppercase tracking-wide">
            Phase 48: Active
          </div>
        </div>
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
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
          label="Recent Emails"
          value={stats.recentEmails}
          color="border-green-500"
        />
      </div>

      <div className="bg-gray-800 rounded-2xl border border-gray-700 shadow-xl overflow-hidden">
        <div className="p-6 border-b border-gray-700 bg-gray-800/50">
          <h2 className="text-xl font-bold text-white">Upcoming Schedule</h2>
        </div>
        <div className="divide-y divide-gray-700">
          {stats.upcomingEvents.length === 0 ? (
            <div className="p-8 text-center text-gray-500 italic">
              No events scheduled.
            </div>
          ) : (
            stats.upcomingEvents.map((evt: EventDto) => (
              <div
                key={evt.id}
                className="p-6 flex flex-col sm:flex-row items-start sm:items-center gap-6 hover:bg-gray-700/30 transition-all">
                <div className="bg-gray-900 w-16 h-16 rounded-xl border border-gray-600 flex flex-col items-center justify-center flex-shrink-0">
                  <span className="text-[10px] text-gray-400 uppercase font-black">
                    {new Date(evt.startDateTime).toLocaleString("default", {
                      month: "short",
                    })}
                  </span>
                  <span className="text-2xl font-black text-white">
                    {new Date(evt.startDateTime).getDate()}
                  </span>
                </div>

                <div className="flex-grow">
                  <h3 className="text-lg font-bold text-white">{evt.name}</h3>
                  <div className="flex flex-wrap gap-4 mt-1 text-sm text-gray-400">
                    <span className="flex items-center gap-1">
                      <span>📍</span> {evt.location || "TBD"}
                    </span>
                    <span className="flex items-center gap-1">
                      <span>🕒</span>{" "}
                      {new Date(evt.startDateTime).toLocaleTimeString([], {
                        hour: "2-digit",
                        minute: "2-digit",
                      })}
                    </span>
                  </div>
                </div>

                <button
                  onClick={() => setIsEmailModalOpen(true)}
                  className="w-full sm:w-auto bg-gray-900 hover:bg-blue-600 border border-gray-600 hover:border-blue-500 text-gray-300 hover:text-white px-4 py-2 rounded-lg text-sm font-bold transition-all shadow-sm">
                  Send Comms
                </button>
              </div>
            ))
          )}
        </div>
      </div>

      <SendCommunicationModal
        isOpen={isEmailModalOpen}
        onClose={() => setIsEmailModalOpen(false)}
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
}: {
  label: string;
  value: number;
  color: string;
}) {
  return (
    <div
      className={`bg-gray-800 p-6 rounded-2xl border-b-4 ${color} shadow-lg transition-transform hover:scale-[1.02]`}>
      <p className="text-gray-400 text-xs font-black uppercase mb-1 tracking-widest">
        {label}
      </p>
      <p className="text-4xl font-black text-white">{value}</p>
    </div>
  );
}
