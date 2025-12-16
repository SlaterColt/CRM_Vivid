import type { EventFinancials } from "../types";

interface Props {
  data: EventFinancials;
  onEditBudget: () => void;
}

export default function BudgetOverview({ data, onEditBudget }: Props) {
  // Color Logic: Green < 80%, Yellow < 100%, Red >= 100%
  const getProgressColor = () => {
    if (data.burnRate >= 1) return "bg-red-600";
    if (data.burnRate > 0.8) return "bg-yellow-500";
    return "bg-emerald-500";
  };

  const percentage = Math.min(data.burnRate * 100, 100);

  return (
    <div className="p-6 rounded-xl shadow-sm border border-gray-600 bg-[#333] text-white mb-8">
      {/* PHASE 26 ADDITION: BUDGET LOCK STATUS BANNER */}
      {data.isLocked ? (
        <div className="bg-red-900/50 border border-red-700 p-3 rounded-lg flex items-center mb-6">
          <svg
            xmlns="http://www.w3.org/2000/svg"
            viewBox="0 0 20 20"
            fill="currentColor"
            className="w-5 h-5 text-red-400 mr-3">
            <path
              fillRule="evenodd"
              d="M10 1a4.5 4.5 0 0 0-4.5 4.5V9H5a2 2 0 0 0-2 2v6a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2v-6a2 2 0 0 0-2-2h-.5V5.5A4.5 4.5 0 0 0 10 1Zm3 8V5.5a3 3 0 1 0-6 0V9h6Z"
              clipRule="evenodd"
            />
          </svg>
          <span className="text-sm font-medium text-red-300">
            BUDGET LOCKED: Contract Signed. This budget cannot be edited.
          </span>
        </div>
      ) : (
        <div className="bg-emerald-900/50 border border-emerald-700 p-3 rounded-lg flex items-center mb-6">
          <svg
            xmlns="http://www.w3.org/2000/svg"
            viewBox="0 0 20 20"
            fill="currentColor"
            className="w-5 h-5 text-emerald-400 mr-3">
            <path d="M10 1a.75.75 0 0 1 .75.75V3a.75.75 0 1 1-1.5 0V1.75A.75.75 0 0 1 10 1ZM5.5 5.5a.75.75 0 0 0-1.5 0v2.75a.75.75 0 0 0 1.5 0V5.5ZM16 8.25a.75.75 0 0 1 1.5 0v-2.5a.75.75 0 0 1-1.5 0v2.5ZM5.5 12.5a.75.75 0 0 0-1.5 0v2.75a.75.75 0 0 0 1.5 0v-2.75ZM16 14.75a.75.75 0 0 1 1.5 0v-2.5a.75.75 0 0 1-1.5 0v2.5ZM10 6a4 4 0 0 1 4 4v.75c0 1.05.57 2.053 1.5 2.5a.75.75 0 0 1-.6 1.34c-1.288-.574-2.15-1.782-2.3-3.14h-1.3c-.15 1.358-1.012 2.566-2.3 3.14a.75.75 0 0 1-.6-1.34c.93-.447 1.5-1.45 1.5-2.5V10a4 4 0 0 1 4-4Z" />
          </svg>
          <span className="text-sm font-medium text-emerald-300">
            BUDGET UNLOCKED: Ready for updates.
          </span>
        </div>
      )}

      <div className="flex justify-between items-start mb-6">
        <div>
          <h3 className="text-xl font-bold text-white">Budget Overview</h3>
          <p className="text-sm text-gray-400 mt-1">{data.currency} Ledger</p>
        </div>
        <button
          onClick={onEditBudget}
          disabled={data.isLocked} // DISABLES BUTTON WHEN LOCKED
          className={`px-3 py-1 text-sm rounded transition-colors ${
            data.isLocked
              ? "bg-gray-800 text-gray-500 cursor-not-allowed border border-gray-700"
              : "bg-gray-700 hover:bg-gray-600 text-white border border-gray-500"
          }`}>
          {data.isLocked ? "Budget Locked" : "Edit Budget"}
        </button>
      </div>

      {/* Stat Cards */}
      <div className="grid grid-cols-3 gap-6 mb-8">
        <div className="p-4 bg-[#444] rounded-lg border border-gray-600">
          <p className="text-xs text-gray-400 uppercase tracking-wider mb-1">
            Total Budget
          </p>
          <p className="text-2xl font-bold text-white">
            ${data.budgetTotal.toLocaleString()}
          </p>
        </div>
        <div className="p-4 bg-[#444] rounded-lg border border-gray-600">
          <p className="text-xs text-gray-400 uppercase tracking-wider mb-1">
            Spent
          </p>
          <p className="text-2xl font-bold text-white">
            ${data.totalSpent.toLocaleString()}
          </p>
        </div>
        <div className="p-4 bg-[#444] rounded-lg border border-gray-600">
          <p className="text-xs text-gray-400 uppercase tracking-wider mb-1">
            Remaining
          </p>
          <p
            className={`text-2xl font-bold ${
              data.remainingBudget < 0 ? "text-red-400" : "text-emerald-400"
            }`}>
            ${data.remainingBudget.toLocaleString()}
          </p>
        </div>
      </div>

      {/* Progress Bar Section */}
      <div className="relative pt-1">
        <div className="flex mb-2 items-center justify-between text-sm">
          <span className="font-semibold uppercase tracking-wide text-gray-300">
            Burn Rate
          </span>
          <span className="font-bold text-white">
            {Math.round(data.burnRate * 100)}%
          </span>
        </div>
        <div className="overflow-hidden h-3 mb-4 text-xs flex rounded-full bg-gray-700 border border-gray-600">
          <div
            style={{ width: `${percentage}%` }}
            className={`shadow-none flex flex-col text-center whitespace-nowrap text-white justify-center ${getProgressColor()} transition-all duration-500`}></div>
        </div>
      </div>

      {data.notes && (
        <div className="mt-4 text-sm text-gray-400 italic border-l-4 border-gray-500 pl-3">
          "{data.notes}"
        </div>
      )}
    </div>
  );
}
