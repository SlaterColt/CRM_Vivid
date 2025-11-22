import {
  SignedIn,
  SignedOut,
  UserButton,
  useAuth,
  RedirectToSignIn,
} from "@clerk/clerk-react";
import { apiClient } from "./apiClient";
import { useEffect, useState } from "react";

import DashboardPage from "./components/DashboardPage"; // NEW IMPORT
import ContactsPage from "./components/ContactsPage";
import EventsPage from "./components/EventsPage";
import TasksPage from "./components/TasksPage";
import VendorsPage from "./components/VendorsPage";
import NotesPage from "./components/NotesPage";
import TemplatesPage from "./components/TemplatesPage";

import { SelectionProvider } from "./context/SelectionProvider";

type PageView =
  | "dashboard" // NEW TYPE
  | "contacts"
  | "events"
  | "tasks"
  | "vendors"
  | "notes"
  | "templates";

function App() {
  const [isApiReady, setIsApiReady] = useState(false);
  // CHANGED: Default to "dashboard"
  const [currentPage, setCurrentPage] = useState<PageView>("dashboard");
  const { getToken } = useAuth();

  useEffect(() => {
    const setupAxiosInterceptor = async () => {
      const interceptorId = apiClient.interceptors.request.use(
        async (config) => {
          const token = await getToken();

          if (token) {
            config.headers.Authorization = `Bearer ${token}`;
          }
          return config;
        },
        (error) => {
          return Promise.reject(error);
        }
      );

      setIsApiReady(true);

      return () => {
        apiClient.interceptors.request.eject(interceptorId);
        setIsApiReady(false);
      };
    };
    setupAxiosInterceptor();
  }, [getToken]);

  const renderPage = () => {
    switch (currentPage) {
      case "dashboard": // NEW CASE
        return <DashboardPage />;
      case "contacts":
        return <ContactsPage />;
      case "events":
        return <EventsPage />;
      case "tasks":
        return <TasksPage />;
      case "vendors":
        return <VendorsPage />;
      case "notes":
        return <NotesPage />;
      case "templates":
        return <TemplatesPage />;
      default:
        return <DashboardPage />;
    }
  };

  return (
    <SelectionProvider>
      <div>
        <header
          style={{
            padding: "1rem",
            display: "flex",
            justifyContent: "flex-end",
            borderBottom: "1px solid #333",
            marginBottom: "1rem",
          }}>
          <SignedIn>
            <UserButton />
          </SignedIn>
        </header>
        <main style={{ padding: "1rem" }}>
          <SignedIn>
            <div
              style={{
                display: "flex",
                alignItems: "center",
                justifyContent: "space-between",
                marginBottom: "1rem",
              }}>
              <h1>CRM_Vivid Command Center</h1>
            </div>

            <nav
              className="harness-nav"
              style={{
                marginBottom: "2rem",
                display: "flex",
                gap: "0.5rem",
                flexWrap: "wrap",
              }}>
              <button
                onClick={() => setCurrentPage("dashboard")}
                disabled={currentPage === "dashboard"}
                style={{
                  fontWeight: currentPage === "dashboard" ? "bold" : "normal",
                }}>
                📊 Dashboard
              </button>
              <button
                onClick={() => setCurrentPage("contacts")}
                disabled={currentPage === "contacts"}>
                Contacts
              </button>
              <button
                onClick={() => setCurrentPage("events")}
                disabled={currentPage === "events"}>
                Events
              </button>
              <button
                onClick={() => setCurrentPage("tasks")}
                disabled={currentPage === "tasks"}>
                Tasks
              </button>
              <button
                onClick={() => setCurrentPage("vendors")}
                disabled={currentPage === "vendors"}>
                Vendors
              </button>
              <button
                onClick={() => setCurrentPage("notes")}
                disabled={currentPage === "notes"}>
                Notes
              </button>
              <button
                onClick={() => setCurrentPage("templates")}
                disabled={currentPage === "templates"}>
                Templates
              </button>
            </nav>

            <main className="harness-main">
              {isApiReady ? renderPage() : <p>Initializing API client...</p>}
            </main>
          </SignedIn>

          <SignedOut>
            <RedirectToSignIn />
          </SignedOut>
        </main>
      </div>
    </SelectionProvider>
  );
}

export default App;
