import { useCallback, useEffect, useState } from "react";
import type { EventFinancials, Expense } from "../types";
import { getEventFinancials, addExpense, upsertBudget } from "../apiClient";
import BudgetOverview from "./BudgetOverview";

interface Props {
  eventId: string;
}

export default function FinancialsSection({ eventId }: Props) {
  const [financials, setFinancials] = useState<EventFinancials | null>(null);
  const [loading, setLoading] = useState(true);

  // Form State
  const [showAddExpense, setShowAddExpense] = useState(false);
  const [newExpense, setNewExpense] = useState({
    description: "",
    amount: 0,
    category: "General",
    dateIncurred: new Date().toISOString().split("T")[0],
  });

  const loadData = useCallback(async () => {
    try {
      setLoading(true);
      const data = await getEventFinancials(eventId);
      setFinancials(data);
    } catch (error) {
      console.error("Failed to load financials", error);
    } finally {
      setLoading(false);
    }
  }, [eventId]);

  useEffect(() => {
    loadData();
  }, [loadData]);

  const handleCreateBudget = async () => {
    const amountStr = prompt("Enter Total Budget Amount:", "10000");
    if (!amountStr) return;
    await upsertBudget(eventId, parseFloat(amountStr), "USD", "Initial Budget");
    loadData();
  };

  const handleSubmitExpense = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!newExpense.amount || !newExpense.description) return;

    await addExpense(
      eventId,
      newExpense.description,
      newExpense.amount,
      new Date(newExpense.dateIncurred).toISOString(),
      newExpense.category
    );

    setShowAddExpense(false);
    setNewExpense({
      description: "",
      amount: 0,
      category: "General",
      dateIncurred: new Date().toISOString().split("T")[0],
    });
    loadData();
  };

  if (loading)
    return (
      <div className="p-8 text-center text-gray-400">Calculating Ledger...</div>
    );

  // Scenario 1: No Budget Exists
  if (
    !financials ||
    (financials.budgetTotal === 0 && financials.expenses.length === 0)
  ) {
    return (
      <div className="text-center p-12 bg-[#333] rounded-xl border border-dashed border-gray-600 mb-8">
        <h3 className="text-lg font-medium text-white">No Budget Set</h3>
        <p className="mt-1 text-gray-400">
          Initialize a budget to start tracking expenses.
        </p>
        <button
          onClick={handleCreateBudget}
          className="mt-4 px-6 py-2 bg-indigo-600 text-white rounded-md hover:bg-indigo-700 transition-colors">
          Create Budget
        </button>
      </div>
    );
  }

  return (
    <div className="space-y-8">
      <BudgetOverview data={financials} onEditBudget={handleCreateBudget} />

      <div className="flex justify-between items-center mb-4">
        <h3 className="text-xl font-bold text-white">Expenses</h3>
        <button
          onClick={() => setShowAddExpense(!showAddExpense)}
          className="px-4 py-2 bg-indigo-600 text-white rounded-md text-sm hover:bg-indigo-700 font-medium transition-colors">
          {showAddExpense ? "Cancel" : "+ Add Expense"}
        </button>
      </div>

      {showAddExpense && (
        <form
          onSubmit={handleSubmitExpense}
          className="bg-[#444] p-6 rounded-lg mb-6 border border-gray-600 grid grid-cols-1 md:grid-cols-4 gap-4 items-end">
          <div className="md:col-span-2">
            <label className="block text-xs font-medium text-gray-300 mb-1">
              Description
            </label>
            <input
              type="text"
              value={newExpense.description}
              onChange={(e) =>
                setNewExpense({ ...newExpense, description: e.target.value })
              }
              className="w-full rounded bg-[#333] border-gray-600 text-white p-2 text-sm focus:ring-indigo-500 focus:border-indigo-500"
              required
            />
          </div>
          <div>
            <label className="block text-xs font-medium text-gray-300 mb-1">
              Amount ($)
            </label>
            <input
              type="number"
              value={newExpense.amount}
              onChange={(e) =>
                setNewExpense({
                  ...newExpense,
                  amount: parseFloat(e.target.value),
                })
              }
              className="w-full rounded bg-[#333] border-gray-600 text-white p-2 text-sm focus:ring-indigo-500 focus:border-indigo-500"
              required
            />
          </div>
          <div>
            <label className="block text-xs font-medium text-gray-300 mb-1">
              Category
            </label>
            <select
              value={newExpense.category}
              onChange={(e) =>
                setNewExpense({ ...newExpense, category: e.target.value })
              }
              className="w-full rounded bg-[#333] border-gray-600 text-white p-2 text-sm focus:ring-indigo-500 focus:border-indigo-500">
              {[
                "General",
                "Venue",
                "Catering",
                "Talent",
                "Production",
                "Marketing",
              ].map((c) => (
                <option key={c} value={c}>
                  {c}
                </option>
              ))}
            </select>
          </div>
          <button
            type="submit"
            className="bg-emerald-600 text-white py-2 px-4 rounded hover:bg-emerald-700 font-medium">
            Save
          </button>
        </form>
      )}

      <div className="bg-[#333] border border-gray-600 rounded-lg overflow-hidden shadow-md">
        <table className="table-fixed w-full text-left">
          <thead className="bg-[#222] text-gray-300 uppercase text-xs tracking-wider border-b border-gray-600">
            <tr>
              <th className="px-6 py-4 w-[15%]">Date</th>
              <th className="px-6 py-4 w-[35%]">Description</th>
              <th className="px-6 py-4 w-[15%]">Category</th>
              <th className="px-6 py-4 w-[15%]">Vendor</th>
              <th className="px-6 py-4 w-[20%] text-right">Amount</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-700">
            {financials.expenses.map((expense: Expense) => (
              <tr
                key={expense.id}
                className="hover:bg-[#383838] transition-colors">
                <td className="px-6 py-4 text-sm text-gray-300">
                  {new Date(expense.dateIncurred).toLocaleDateString()}
                </td>
                <td className="px-6 py-4 text-sm font-medium text-white">
                  {expense.description}
                  {expense.linkedDocumentName && (
                    <span className="ml-2 px-2 py-0.5 bg-blue-900 text-blue-200 text-xs rounded-full border border-blue-700">
                      Invoice
                    </span>
                  )}
                </td>
                <td className="px-6 py-4 text-sm text-gray-300">
                  <span className="px-2 py-1 text-xs font-semibold rounded-full bg-[#444] text-gray-200 border border-gray-600">
                    {expense.category}
                  </span>
                </td>
                <td className="px-6 py-4 text-sm text-gray-300">
                  {expense.vendorName || "-"}
                </td>
                <td className="px-6 py-4 text-sm text-right font-mono font-medium text-emerald-400">
                  ${expense.amount.toLocaleString()}
                </td>
              </tr>
            ))}
            {financials.expenses.length === 0 && (
              <tr>
                <td
                  colSpan={5}
                  className="px-6 py-8 text-center text-sm text-gray-500">
                  No expenses recorded yet.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
