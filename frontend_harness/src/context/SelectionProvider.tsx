// frontend_harness/src/context/SelectionProvider.tsx (FULL FILE - Corrected)
import React, { useState, useMemo } from "react";
import { SelectionContext } from "./internalContext";
import type { Guid } from "../types"; // FIX: Added 'type' keyword (TS1484)

interface SelectionProviderProps {
  children: React.ReactNode;
}

export const SelectionProvider: React.FC<SelectionProviderProps> = ({
  children,
}) => {
  const [selectedContactId, setSelectedContactId] = useState<Guid | null>(null);
  const [selectedEventId, setSelectedEventId] = useState<Guid | null>(null);
  // NEW: State for selected vendor
  const [selectedVendorId, setSelectedVendorId] = useState<Guid | null>(null);

  const contextValue = useMemo(
    () => ({
      selectedContactId,
      setSelectedContactId,
      selectedEventId,
      setSelectedEventId,
      selectedVendorId, // NEW: Include in context value
      setSelectedVendorId, // NEW: Include in context value
    }),
    [selectedContactId, selectedEventId, selectedVendorId]
  ); // NEW: Include in dependency array

  return (
    <SelectionContext.Provider value={contextValue}>
      {children}
    </SelectionContext.Provider>
  );
};
