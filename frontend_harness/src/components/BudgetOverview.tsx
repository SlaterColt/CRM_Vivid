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
      <div className="flex justify-between items-start mb-6">
        <div>
          <h3 className="text-xl font-bold text-white">Budget Overview</h3>
          <p className="text-sm text-gray-400 mt-1">{data.currency} Ledger</p>
        </div>
        <button
          onClick={onEditBudget}
          className="px-3 py-1 text-sm bg-gray-700 hover:bg-gray-600 text-white rounded border border-gray-500 transition-colors">
          Edit Budget
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
