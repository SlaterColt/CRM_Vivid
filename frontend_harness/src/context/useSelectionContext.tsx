import { useContext } from "react";
// Import the raw context from its new, separate file
import { SelectionContext } from "./internalContext";

// This is the only export, and it's a hook.
// This also satisfies the linter.
export const useSelectionContext = () => {
  const context = useContext(SelectionContext);
  if (context === undefined) {
    throw new Error(
      "useSelectionContext must be used within a SelectionProvider"
    );
  }
  return context;
};
