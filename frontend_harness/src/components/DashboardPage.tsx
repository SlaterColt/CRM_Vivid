// FILE: frontend_harness/src/components/DashboardPage.tsx (FINALIZED, COMPLETE)

import { useEffect, useState, useCallback } from "react";
// Removed deleteContact from the top level import to resolve the unused warning.
import {
  apiClient,
  updateContact,
  updateVendor,
  updateEvent,
  submitLead,
  // FIX: scheduleFollowUp removed, using apiClient.post directly
  deleteContact as deleteContactClient, // Use the renamed client here
  initializeTestFixtures, // NEW: For Phase 40 Data Setup
} from "../apiClient";
import { SendEmailModal } from "./SendEmailModal";
// FIX 1: Ensure all necessary types are imported.
import type {
  DashboardStatsDto,
  ContactDto,
  VendorDto,
  EventDto,
  SubmitLeadCommand,
} from "../types";
import { LeadStage, RecipientType } from "../types";

// --- GUID DEFINITIONS ---
const VALID_TEMPLATE_GUID = "8efe1c11-0d4d-49ef-9463-195e6e2bcda5";
// This GUID must match the hardcoded ID in CurrentUserService.cs for the test to see data.
const MOCK_USER_ID = "11111111-2222-3333-4444-555555555555";
// --------------------------------------------------------------------------

// Expected response structure for schedule-followup (Replaces 'any')
interface ScheduleFollowUpResponse {
  jobId: string;
  message: string;
}

// --- NEW SUB-COMPONENT: TestExecutionPanel (PHASE 31) ---
// Define explicit interface for props
interface TestExecutionPanelProps {
  contacts: ContactDto[];
  vendors: VendorDto[];
  events: EventDto[];
  logTestResult: (
    message: string,
    level: "PASS" | "FAIL" | "INFO" | "WARN"
  ) => void;
}

const TestExecutionPanel: React.FC<TestExecutionPanelProps> = ({
  contacts,
  vendors,
  events,
  logTestResult,
}) => {
  const runAllTests = useCallback(() => {
    // Removed 'async' from declaration, moved to where awaited.
    const output = document.getElementById("test-log-output");
    if (output) output.innerHTML = ""; // Clear log on start

    // Calculate entities based on current props (to satisfy linter/compiler)
    const testContact = contacts[0];
    const testVendor =
      vendors.find((v: VendorDto) => v.serviceType === "Catering") ||
      vendors[0];
    const testEvent = events[0];

    logTestResult("Starting CRUD Test Suite...", "INFO");

    if (!testContact || !testVendor || !testEvent) {
      logTestResult(
        "Skipping CRUD tests: Insufficient initial data (need Contact, Vendor, Event).",
        "WARN"
      );
      return;
    }

    // --- 1. LEAD SUBMISSION TEST (POST /leads/submit) ---
    const executeLeadSubmission = async () => {
      let createdLeadId: string | null = null;
      try {
        const leadCommand: SubmitLeadCommand = {
          firstName: "Auto",
          lastName: "Lead",
          email: `auto_lead_${Date.now()}@test.com`,
          source: "Dashboard Test Harness",
        };
        createdLeadId = await submitLead(leadCommand);
        logTestResult(
          `POST /leads/submit: SUCCESS. New Lead ID: ${createdLeadId.substring(
            0,
            8
          )}...`,
          "PASS"
        );
        return createdLeadId; // Return ID for cleanup later
      } catch (e: unknown) {
        logTestResult(
          `POST /leads/submit: FAILED. Error: ${(e as Error).message}`,
          "FAIL"
        );
        return null;
      }
    };

    // --- 2. CONTACT UPDATE TEST (PUT /contacts/{id}) ---
    const executeContactUpdate = async () => {
      try {
        // FIX 1: Spread the entire contact DTO to satisfy NotEmpty() validation for Name, Email, etc.
        const contactPayload = {
          ...testContact,
          lastName: `Updated-${Date.now().toString().slice(-4)}`,
          stage: LeadStage.InDiscussion,
        };

        await updateContact(testContact.id, contactPayload);
        logTestResult(
          `PUT /contacts/${testContact.id.substring(
            0,
            8
          )}...: SUCCESS (Update Contact Name)`,
          "PASS"
        );
      } catch (e: unknown) {
        logTestResult(
          `PUT /contacts: FAILED. Error: ${(e as Error).message}`,
          "FAIL"
        );
      }
    };

    // --- 3. VENDOR UPDATE TEST (PUT /vendors/{id}) ---
    const executeVendorUpdate = async () => {
      try {
        // FIX 2: Spread the entire vendor DTO to satisfy NotEmpty() and JSONB validation
        const vendorPayload = {
          ...testVendor,
          email: `updated_vendor_${Date.now().toString().slice(-4)}@test.com`,
          // CRITICAL: Ensure attributes are explicitly passed, even if null/empty string
          attributes: testVendor.attributes || null,
        };

        await updateVendor(testVendor.id, vendorPayload);
        logTestResult(
          `PUT /vendors/${testVendor.id.substring(
            0,
            8
          )}...: SUCCESS (Update Vendor Email)`,
          "PASS"
        );
      } catch (e: unknown) {
        logTestResult(
          `PUT /vendors: FAILED. Error: ${(e as Error).message}`,
          "FAIL"
        );
      }
    };

    // --- 4. EVENT UPDATE TEST (PUT /events/{id}) ---
    const executeEventUpdate = async () => {
      try {
        // FIX 3: Spread the entire event DTO to satisfy date validation (End > Start)
        const eventPayload = {
          ...testEvent,
          description: `Tested by Harness at ${new Date().toLocaleTimeString()}`,
        };

        await updateEvent(testEvent.id, eventPayload);
        logTestResult(
          `PUT /events/${testEvent.id.substring(
            0,
            8
          )}...: SUCCESS (Update Event Description)`,
          "PASS"
        );
      } catch (e: unknown) {
        logTestResult(
          `PUT /events: FAILED. Error: ${(e as Error).message}`,
          "FAIL"
        );
      }
    };

    // --- 5. SCHEDULE FOLLOW-UP TEST (POST /automation/schedule-followup) ---
    const executeScheduleFollowUp = async () => {
      try {
        // Uses the VALID_TEMPLATE_GUID constant defined at the top
        const scheduleTime = new Date(Date.now() + 60000).toISOString();

        // FIX 5: Explicitly type the response and extract jobId property
        const response = await apiClient.post<ScheduleFollowUpResponse>(
          "/api/automation/schedule-followup",
          {
            contactId: testContact.id,
            templateId: VALID_TEMPLATE_GUID,
            scheduleTime: scheduleTime,
            type: RecipientType.Contact,
          }
        );

        const jobId = response.data.jobId;

        logTestResult(
          `POST /automation/schedule-followup: SUCCESS. Job ID: ${jobId.substring(
            0,
            8
          )}...`,
          "PASS"
        );
      } catch (e: unknown) {
        logTestResult(
          `POST /automation/schedule-followup: FAILED. Error: ${
            (e as Error).message
          }`,
          "FAIL"
        );
      }
    };

    // --- EXECUTION SEQUENCE ---
    const runSequence = async () => {
      const leadId = await executeLeadSubmission();
      await executeContactUpdate();
      await executeVendorUpdate();
      await executeEventUpdate();
      await executeScheduleFollowUp();

      // --- CLEANUP (Delete Auto Lead) ---
      try {
        // Use the createdLeadId from step 1
        const contactsAfterTest = (
          await apiClient.get<ContactDto[]>("/api/contacts")
        ).data;
        const createdLead = contactsAfterTest.find(
          (c) => c.firstName === "Auto" && c.lastName === "Lead"
        );

        const idToClean = leadId || createdLead?.id;

        if (idToClean) {
          // Use the client imported as deleteContactClient
          await deleteContactClient(idToClean);
          logTestResult(
            `DELETE /contacts: SUCCESS (Cleaned up Auto Lead ${idToClean.substring(
              0,
              8
            )}...)`,
            "PASS"
          );
        }
      } catch (e: unknown) {
        logTestResult(
          `CLEANUP DELETE FAILED. Error: ${(e as Error).message}`,
          "WARN"
        );
      }

      logTestResult("CRUD Test Suite Finished.", "INFO");
    };

    runSequence();
  }, [contacts, vendors, events, logTestResult]);

  // --- NEW: PHASE 39 ADVANCED TEST FUNCTION ---
  const runAdvancedTests = useCallback(async () => {
    logTestResult("Starting Advanced Feature Audit...", "INFO");

    // We rely on the first contact, vendor, and event being present for reporting tests
    const testContact = contacts[0];
    const testVendor = vendors.find((v) => v.email !== null) || vendors[0];
    const testEvent = events[0];

    if (!testContact || !testVendor || !testEvent) {
      logTestResult(
        "Skipping Advanced tests: Insufficient initial data.",
        "WARN"
      );
      return;
    }

    let ownedContactId: string | null = null;

    // -----------------------------------------------------------
    // AUDIT 1: SCOPED AUTHORIZATION AND VISIBILITY (PHASE 37)
    // -----------------------------------------------------------
    logTestResult("AUDIT 1: Scoped Authorization Check...", "INFO");

    // 1A. Create a Contact (This contact should be owned by MOCK_USER_ID)
    try {
      const leadCommand: SubmitLeadCommand = {
        firstName: "Scoped",
        lastName: "Owner",
        email: `scoped_test_${Date.now()}@test.com`,
        source: "Auth Test",
      };
      // This creation will assign CreatedByUserId: MOCK_USER_ID via SubmitLeadCommandHandler fix
      ownedContactId = await submitLead(leadCommand);
      logTestResult(
        `1A: Created Scoped Contact ID: ${ownedContactId.substring(0, 8)}...`,
        "PASS"
      );
    } catch (e: unknown) {
      logTestResult(
        `1A: Scoped Contact creation FAILED: ${(e as Error).message}`,
        "FAIL"
      );
    }

    // 1B. Query All Contacts (Should only return the newly created one and any others owned by MOCK_USER_ID)
    if (ownedContactId) {
      try {
        // We expect the result set to be small, containing only records owned by MOCK_USER_ID
        const contactsRes = await apiClient.get<ContactDto[]>("/api/contacts");

        // Check that the newly created contact is present
        const isOwnedContactPresent = contactsRes.data.some(
          (c) => c.id === ownedContactId
        );

        if (isOwnedContactPresent) {
          logTestResult(
            `1B: Global Filter PASSED. Found ${contactsRes.data.length} owned records.`,
            "PASS"
          );
        } else {
          logTestResult(
            `1B: Global Filter FAILED. Cannot find the newly created record. Filter is blocking owned data.`,
            "FAIL"
          );
        }
      } catch (e: unknown) {
        logTestResult(
          `1B: Global Query FAILED (likely 500 crash): ${(e as Error).message}`,
          "FAIL"
        );
      }
    }

    // -----------------------------------------------------------
    // AUDIT 2: CROSS-CUTTING SUMMARY REPORTS (PHASE 35)
    // -----------------------------------------------------------
    logTestResult("AUDIT 2: Reporting & Aggregation Check...", "INFO");

    // 2A. Vendor Summary Check
    try {
      const vendorSummaryRes = await apiClient.get(
        `/api/vendors/${testVendor.id}/summary`
      );
      const summary = vendorSummaryRes.data;

      if (
        typeof summary.totalEventsHiredFor === "number" &&
        typeof summary.totalExpensesPaid === "number"
      ) {
        logTestResult(
          `2A: Vendor Summary PASSED. Events: ${
            summary.totalEventsHiredFor
          }, Expenses: $${summary.totalExpensesPaid.toFixed(2)}.`,
          "PASS"
        );
      } else {
        logTestResult(
          `2A: Vendor Summary FAILED. Invalid report data structure.`,
          "FAIL"
        );
      }
    } catch (e: unknown) {
      logTestResult(
        `2A: Vendor Summary FAILED (likely missing join/data): ${
          (e as Error).message
        }`,
        "FAIL"
      );
    }

    // 2B. Contact Task Summary Check
    try {
      const taskSummaryRes = await apiClient.get(
        `/api/contacts/${testContact.id}/task-summary`
      );
      const summary = taskSummaryRes.data;

      if (
        typeof summary.totalTasksAssigned === "number" &&
        typeof summary.totalTasksOverdue === "number"
      ) {
        logTestResult(
          `2B: Contact Summary PASSED. Total Tasks: ${summary.totalTasksAssigned}.`,
          "PASS"
        );
      } else {
        logTestResult(
          `2B: Contact Summary FAILED. Invalid report data structure.`,
          "FAIL"
        );
      }
    } catch (e: unknown) {
      logTestResult(
        `2B: Contact Summary FAILED (likely 500 crash): ${
          (e as Error).message
        }`,
        "FAIL"
      );
    }

    // -----------------------------------------------------------
    // AUDIT 3: UNIFIED ACTIVITY STREAM (PHASE 36)
    // -----------------------------------------------------------
    logTestResult("AUDIT 3: Activity Stream Check...", "INFO");

    // NOTE: This test relies on existing Notes, Tasks, and EmailLogs associated with contacts[0]
    try {
      const streamRes = await apiClient.get(
        `/api/contacts/${testContact.id}/activity-stream`
      );
      const stream = streamRes.data;

      if (
        Array.isArray(stream) &&
        stream.every((a) => a.activityType && a.timestamp)
      ) {
        logTestResult(
          `3A: Activity Stream PASSED. Found ${stream.length} activities.`,
          "PASS"
        );
      } else {
        logTestResult(
          "3A: Activity Stream FAILED. Response was not a valid array of ActivityDto.",
          "FAIL"
        );
      }
    } catch (e: unknown) {
      logTestResult(
        `3A: Activity Stream FAILED (likely 500 crash): ${
          (e as Error).message
        }`,
        "FAIL"
      );
    }

    // -----------------------------------------------------------
    // 1C. FINAL CLEANUP
    // -----------------------------------------------------------
    try {
      if (ownedContactId) {
        await deleteContactClient(ownedContactId);
        logTestResult(`1C: Cleaned up Scoped Contact.`, "INFO");
      }
    } catch (e: unknown) {
      logTestResult(`1C: Cleanup FAILED: ${(e as Error).message}`, "WARN");
    }

    logTestResult("Advanced Feature Audit Finished.", "INFO");
  }, [contacts, vendors, events, logTestResult]);
  // ---------------------------------------------

  return (
    <div
      style={{
        marginTop: "2rem",
        padding: "1rem",
        border: "1px solid #444",
        borderRadius: "8px",
        backgroundColor: "#222",
      }}>
      <h2 className="text-xl font-semibold text-white mb-3">
        API Test Harness
      </h2>
      <button
        onClick={runAllTests}
        style={{
          padding: "8px 16px",
          backgroundColor: "#007acc",
          color: "white",
          borderRadius: "4px",
          border: "none",
          marginRight: "1rem",
        }}>
        Execute Full CRUD Test Suite
      </button>
      <button
        onClick={runAdvancedTests} // NEW BUTTON
        style={{
          padding: "8px 16px",
          backgroundColor: "#b8860b", // DarkYellow for differentiation
          color: "white",
          borderRadius: "4px",
          border: "none",
        }}>
        Execute Advanced Audit
      </button>
      <div
        style={{
          marginTop: "1rem",
          maxHeight: "300px",
          overflowY: "auto",
          backgroundColor: "#111",
          padding: "10px",
          borderRadius: "4px",
        }}>
        <h4 style={{ color: "#aaa", margin: 0 }}>Test Log:</h4>
        <div
          id="test-log-output"
          style={{
            whiteSpace: "pre-wrap",
            fontFamily: "monospace",
            fontSize: "0.85rem",
          }}>
          {/* Log entries will be appended here */}
        </div>
      </div>
    </div>
  );
};
// --- END TestExecutionPanel ---

// --- MAIN DashboardPage COMPONENT ---
export default function DashboardPage() {
  const [stats, setStats] = useState<DashboardStatsDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  // --- Modal State ---
  const [isEmailModalOpen, setIsEmailModalOpen] = useState(false);
  const [selectedEventId, setSelectedEventId] = useState<string>("");

  // --- Auxiliary Data for Test Harness ---
  const [contacts, setContacts] = useState<ContactDto[]>([]);
  const [vendors, setVendors] = useState<VendorDto[]>([]);
  const [events, setEvents] = useState<EventDto[]>([]);
  // --- NEW STATE FOR FIXTURE MANAGEMENT ---
  const [isFixturesInitialized, setIsFixturesInitialized] = useState(false); // New state to track initialization
  // ----------------------------------------

  // --- Test Log State ---
  const logTestResult = useCallback(
    (message: string, level: "PASS" | "FAIL" | "INFO" | "WARN") => {
      let color = "#fff";
      if (level === "PASS") color = "#4caf50";
      if (level === "FAIL") color = "#f44336";
      if (level === "WARN") color = "#ff9800";

      const timestamp = new Date().toLocaleTimeString();
      const logEntry = `<span style="color: ${color};">[${level}] ${timestamp}: ${message}</span>\n`;

      const output = document.getElementById("test-log-output");
      if (output) {
        output.innerHTML += logEntry;
        output.scrollTop = output.scrollHeight; // Auto-scroll
      }
    },
    []
  );

  useEffect(() => {
    const loadDashboardData = async () => {
      try {
        setLoading(true);
        setError("");

        // --- PHASE 40: RUN FIXTURES ONCE ---
        if (!isFixturesInitialized) {
          logTestResult("Initializing required test fixtures...", "INFO");
          // InitializeTestFixtures creates the data needed for all subsequent fetches/tests
          await initializeTestFixtures();
          setIsFixturesInitialized(true);
          logTestResult("Fixtures initialized successfully.", "PASS");
        }
        // ----------------------------------

        // Fetch Events for Test Harness and for Upcoming Events List
        const [statsRes, contactsRes, vendorsRes, eventsRes] =
          await Promise.all([
            apiClient.get<DashboardStatsDto>("/api/dashboard"),
            apiClient.get<ContactDto[]>("/api/contacts"),
            apiClient.get<VendorDto[]>("/api/vendors"),
            apiClient.get<EventDto[]>("/api/events"),
          ]);

        setStats(statsRes.data);
        setContacts(contactsRes.data);
        setVendors(vendorsRes.data);
        setEvents(eventsRes.data); // Store events for testing
        setLoading(false);
      } catch (err: unknown) {
        console.error(err);
        setError("Failed to load dashboard data. Check API/Fixtures endpoint.");
        setLoading(false);
      }
    };

    // --- FIX: Reruns fetchDashboardData whenever initialized status changes ---
    loadDashboardData();
  }, [isFixturesInitialized, logTestResult]); // Added logTestResult to deps array

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
      {/* PHASE 31: TEST EXECUTION PANEL */}
      <TestExecutionPanel
        contacts={contacts}
        vendors={vendors}
        events={events}
        logTestResult={logTestResult}
      />

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
          <p className="text-blue-400 font-mono">
            Phase 31: Full API Harness Test Coverage
          </p>
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
            stats.upcomingEvents.map((evt: EventDto) => {
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
