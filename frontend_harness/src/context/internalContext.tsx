// frontend_harness/src/context/internalContext.tsx
import { createContext } from "react";

// Define the shape of the context state
export interface SelectionContextType {
  selectedContactId: string | null;
  setSelectedContactId: (id: string | null) => void;
  selectedEventId: string | null;
  setSelectedEventId: (id: string | null) => void;
  // NEW: Vendor selection properties
  selectedVendorId: string | null;
  setSelectedVendorId: (id: string | null) => void;
}

// Create the context with an initial undefined value
export const SelectionContext = createContext<SelectionContextType | undefined>(
  undefined
);
