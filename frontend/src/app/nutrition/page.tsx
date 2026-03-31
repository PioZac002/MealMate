'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import { nutritionApi, DailyNutritionLog, MealLog } from '@/services/api';

const MEAL_TYPES = ['Breakfast', 'Lunch', 'Dinner', 'Snack'];
const MEAL_COLORS: Record<string, string> = {
  Breakfast: 'bg-yellow-100 text-yellow-800',
  Lunch: 'bg-green-100 text-green-800',
  Dinner: 'bg-blue-100 text-blue-800',
  Snack: 'bg-purple-100 text-purple-800',
};

function formatDate(d: Date): string {
  return d.toISOString().split('T')[0];
}

function MacroBar({ label, current, goal, color }: { label: string; current: number; goal: number; color: string }) {
  const pct = goal > 0 ? Math.min((current / goal) * 100, 100) : 0;
  return (
    <div>
      <div className="flex justify-between text-sm mb-1">
        <span className="font-medium text-gray-700">{label}</span>
        <span className="text-gray-500">{Math.round(current)}g / {Math.round(goal)}g</span>
      </div>
      <div className="w-full bg-gray-200 rounded-full h-2.5">
        <div className={`h-2.5 rounded-full transition-all ${color}`} style={{ width: `${pct}%` }} />
      </div>
    </div>
  );
}

export default function NutritionPage() {
  const router = useRouter();
  const [date, setDate] = useState(formatDate(new Date()));
  const [log, setLog] = useState<DailyNutritionLog | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [showAddMeal, setShowAddMeal] = useState(false);
  const [showGoals, setShowGoals] = useState(false);
  const [addingMeal, setAddingMeal] = useState(false);
  const [savingGoals, setSavingGoals] = useState(false);
  const [message, setMessage] = useState('');

  const [mealForm, setMealForm] = useState({
    mealType: 'Breakfast',
    customFoodName: '',
    calories: '',
    protein: '',
    carbs: '',
    fat: '',
    servings: '1',
  });

  const [goalsForm, setGoalsForm] = useState({
    calorieGoal: '',
    proteinGoal: '',
    carbsGoal: '',
    fatGoal: '',
    notes: '',
  });

  useEffect(() => {
    const token = localStorage.getItem('accessToken');
    if (!token) { router.push('/login'); return; }
    loadLog();
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [date]);

  async function loadLog() {
    setLoading(true);
    const res = await nutritionApi.getByDate(date);
    if (res.error) {
      setError(res.error);
    } else if (res.data) {
      setLog(res.data);
      setGoalsForm({
        calorieGoal: String(res.data.calorieGoal),
        proteinGoal: String(res.data.proteinGoal),
        carbsGoal: String(res.data.carbsGoal),
        fatGoal: String(res.data.fatGoal),
        notes: res.data.notes || '',
      });
    }
    setLoading(false);
  }

  async function handleAddMeal(e: React.FormEvent) {
    e.preventDefault();
    setAddingMeal(true);
    const res = await nutritionApi.addMeal(date, {
      mealType: mealForm.mealType,
      customFoodName: mealForm.customFoodName || undefined,
      calories: Number(mealForm.calories),
      protein: Number(mealForm.protein),
      carbs: Number(mealForm.carbs),
      fat: Number(mealForm.fat),
      servings: Number(mealForm.servings),
    });
    if (res.error) {
      setMessage(res.error);
    } else {
      setShowAddMeal(false);
      setMealForm({ mealType: 'Breakfast', customFoodName: '', calories: '', protein: '', carbs: '', fat: '', servings: '1' });
      await loadLog();
    }
    setAddingMeal(false);
  }

  async function handleRemoveMeal(mealLogId: string) {
    await nutritionApi.removeMeal(mealLogId);
    await loadLog();
  }

  async function handleSaveGoals(e: React.FormEvent) {
    e.preventDefault();
    setSavingGoals(true);
    const res = await nutritionApi.setGoals(date, {
      calorieGoal: Number(goalsForm.calorieGoal),
      proteinGoal: Number(goalsForm.proteinGoal),
      carbsGoal: Number(goalsForm.carbsGoal),
      fatGoal: Number(goalsForm.fatGoal),
      notes: goalsForm.notes || undefined,
    });
    if (res.error) {
      setMessage(res.error);
    } else {
      setShowGoals(false);
      await loadLog();
    }
    setSavingGoals(false);
  }

  function changeDate(days: number) {
    const d = new Date(date);
    d.setDate(d.getDate() + days);
    setDate(formatDate(d));
  }

  const mealsByType = MEAL_TYPES.reduce((acc, type) => {
    acc[type] = log?.mealLogs.filter(m => m.mealType === type) || [];
    return acc;
  }, {} as Record<string, MealLog[]>);

  const calPct = log && log.calorieGoal > 0 ? Math.min((log.totalCalories / log.calorieGoal) * 100, 100) : 0;
  const calColor = calPct >= 100 ? 'text-red-600' : calPct >= 80 ? 'text-yellow-600' : 'text-green-600';

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Navbar */}
      <nav className="bg-white shadow-sm border-b border-gray-200">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between h-16 items-center">
            <div className="flex items-center space-x-2">
              <span className="text-2xl">🥗</span>
              <span className="text-xl font-bold text-gray-900">MealMate+</span>
            </div>
            <div className="hidden md:flex items-center space-x-6 text-sm font-medium text-gray-600">
              <a href="/dashboard" className="hover:text-green-600">Dashboard</a>
              <a href="/recipes" className="hover:text-green-600">Recipes</a>
              <a href="/fridge" className="hover:text-green-600">Fridge</a>
              <a href="/shopping" className="hover:text-green-600">Shopping</a>
              <a href="/nutrition" className="text-green-600 font-semibold">Nutrition</a>
              <a href="/fitness" className="hover:text-green-600">Fitness</a>
            </div>
          </div>
        </div>
      </nav>

      <main className="max-w-5xl mx-auto px-4 py-8">
        {/* Header + Date nav */}
        <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 mb-6">
          <div>
            <h1 className="text-2xl font-bold text-gray-900">Calorie Tracker</h1>
            <p className="text-gray-500 text-sm mt-1">Track your daily nutrition</p>
          </div>
          <div className="flex items-center gap-3">
            <button onClick={() => changeDate(-1)} className="p-2 rounded-lg border border-gray-300 hover:bg-gray-100 text-gray-600">‹</button>
            <input
              type="date"
              value={date}
              onChange={e => setDate(e.target.value)}
              className="border border-gray-300 rounded-lg px-3 py-1.5 text-sm focus:outline-none focus:ring-2 focus:ring-green-500"
            />
            <button onClick={() => changeDate(1)} className="p-2 rounded-lg border border-gray-300 hover:bg-gray-100 text-gray-600">›</button>
            <button onClick={() => setDate(formatDate(new Date()))} className="text-sm text-green-600 hover:underline">Today</button>
          </div>
        </div>

        {message && (
          <div className="mb-4 p-3 bg-red-50 border border-red-200 text-red-700 rounded-lg text-sm">
            {message}
            <button onClick={() => setMessage('')} className="ml-2 font-bold">×</button>
          </div>
        )}

        {loading ? (
          <div className="text-center py-12 text-gray-500">Loading...</div>
        ) : error ? (
          <div className="text-center py-12 text-red-500">{error}</div>
        ) : log ? (
          <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
            {/* Summary Card */}
            <div className="lg:col-span-1 space-y-4">
              <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-5">
                <div className="flex justify-between items-center mb-4">
                  <h2 className="font-semibold text-gray-900">Daily Summary</h2>
                  <button
                    onClick={() => setShowGoals(true)}
                    className="text-xs text-green-600 hover:underline"
                  >
                    Edit Goals
                  </button>
                </div>

                {/* Calorie summary */}
                <div className="text-center mb-5">
                  <div className={`text-4xl font-bold ${calColor}`}>{Math.round(log.totalCalories)}</div>
                  <div className="text-gray-400 text-sm">of {Math.round(log.calorieGoal)} kcal</div>
                  <div className="w-full bg-gray-100 rounded-full h-3 mt-2">
                    <div
                      className={`h-3 rounded-full transition-all ${calPct >= 100 ? 'bg-red-500' : 'bg-green-500'}`}
                      style={{ width: `${calPct}%` }}
                    />
                  </div>
                  <div className="text-xs text-gray-400 mt-1">
                    {Math.max(0, Math.round(log.calorieGoal - log.totalCalories))} kcal remaining
                  </div>
                </div>

                <div className="space-y-3">
                  <MacroBar label="Protein" current={log.totalProtein} goal={log.proteinGoal} color="bg-blue-500" />
                  <MacroBar label="Carbs" current={log.totalCarbs} goal={log.carbsGoal} color="bg-yellow-500" />
                  <MacroBar label="Fat" current={log.totalFat} goal={log.fatGoal} color="bg-red-400" />
                </div>

                {log.notes && (
                  <p className="mt-4 text-xs text-gray-500 italic border-t pt-3">{log.notes}</p>
                )}
              </div>

              <button
                onClick={() => setShowAddMeal(true)}
                className="w-full bg-green-600 hover:bg-green-700 text-white font-semibold py-2.5 rounded-xl transition"
              >
                + Log Meal
              </button>
            </div>

            {/* Meal Logs */}
            <div className="lg:col-span-2 space-y-4">
              {MEAL_TYPES.map(type => (
                <div key={type} className="bg-white rounded-xl shadow-sm border border-gray-200 p-4">
                  <div className="flex items-center justify-between mb-3">
                    <span className={`text-xs font-semibold px-2 py-0.5 rounded-full ${MEAL_COLORS[type]}`}>
                      {type}
                    </span>
                    <span className="text-sm text-gray-500">
                      {Math.round(mealsByType[type].reduce((s, m) => s + m.calories, 0))} kcal
                    </span>
                  </div>
                  {mealsByType[type].length === 0 ? (
                    <p className="text-sm text-gray-400 italic">No meals logged</p>
                  ) : (
                    <div className="space-y-2">
                      {mealsByType[type].map(meal => (
                        <div key={meal.id} className="flex items-center justify-between bg-gray-50 rounded-lg px-3 py-2">
                          <div>
                            <span className="text-sm font-medium text-gray-800">{meal.foodName}</span>
                            <div className="text-xs text-gray-500 mt-0.5">
                              {Math.round(meal.calories)} kcal · P {Math.round(meal.protein)}g · C {Math.round(meal.carbs)}g · F {Math.round(meal.fat)}g
                            </div>
                          </div>
                          <button
                            onClick={() => handleRemoveMeal(meal.id)}
                            className="text-gray-400 hover:text-red-500 text-xs ml-3"
                            title="Remove"
                          >
                            ✕
                          </button>
                        </div>
                      ))}
                    </div>
                  )}
                </div>
              ))}
            </div>
          </div>
        ) : null}
      </main>

      {/* Add Meal Modal */}
      {showAddMeal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
          <div className="bg-white rounded-2xl shadow-xl w-full max-w-md p-6">
            <div className="flex justify-between items-center mb-4">
              <h2 className="text-lg font-bold text-gray-900">Log a Meal</h2>
              <button onClick={() => setShowAddMeal(false)} className="text-gray-400 hover:text-gray-600 text-xl">×</button>
            </div>
            <form onSubmit={handleAddMeal} className="space-y-3">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Meal Type</label>
                <select
                  value={mealForm.mealType}
                  onChange={e => setMealForm({ ...mealForm, mealType: e.target.value })}
                  className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-green-500"
                >
                  {MEAL_TYPES.map(t => <option key={t} value={t}>{t}</option>)}
                </select>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Food Name</label>
                <input
                  type="text"
                  placeholder="e.g. Oatmeal with banana"
                  value={mealForm.customFoodName}
                  onChange={e => setMealForm({ ...mealForm, customFoodName: e.target.value })}
                  className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-green-500"
                  required
                />
              </div>
              <div className="grid grid-cols-2 gap-3">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Calories (kcal)</label>
                  <input type="number" min="0" step="1" value={mealForm.calories}
                    onChange={e => setMealForm({ ...mealForm, calories: e.target.value })}
                    className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-green-500" required />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Servings</label>
                  <input type="number" min="0.1" step="0.1" value={mealForm.servings}
                    onChange={e => setMealForm({ ...mealForm, servings: e.target.value })}
                    className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-green-500" required />
                </div>
              </div>
              <div className="grid grid-cols-3 gap-3">
                {(['protein', 'carbs', 'fat'] as const).map(macro => (
                  <div key={macro}>
                    <label className="block text-sm font-medium text-gray-700 mb-1 capitalize">{macro} (g)</label>
                    <input type="number" min="0" step="0.1" value={mealForm[macro]}
                      onChange={e => setMealForm({ ...mealForm, [macro]: e.target.value })}
                      className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-green-500" required />
                  </div>
                ))}
              </div>
              <div className="flex gap-3 pt-2">
                <button type="button" onClick={() => setShowAddMeal(false)}
                  className="flex-1 border border-gray-300 text-gray-700 py-2 rounded-xl hover:bg-gray-50 text-sm font-medium transition">Cancel</button>
                <button type="submit" disabled={addingMeal}
                  className="flex-1 bg-green-600 hover:bg-green-700 text-white py-2 rounded-xl text-sm font-semibold transition disabled:opacity-50">
                  {addingMeal ? 'Logging...' : 'Log Meal'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Edit Goals Modal */}
      {showGoals && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
          <div className="bg-white rounded-2xl shadow-xl w-full max-w-sm p-6">
            <div className="flex justify-between items-center mb-4">
              <h2 className="text-lg font-bold text-gray-900">Edit Nutrition Goals</h2>
              <button onClick={() => setShowGoals(false)} className="text-gray-400 hover:text-gray-600 text-xl">×</button>
            </div>
            <form onSubmit={handleSaveGoals} className="space-y-3">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Calories (kcal)</label>
                <input type="number" min="0" value={goalsForm.calorieGoal}
                  onChange={e => setGoalsForm({ ...goalsForm, calorieGoal: e.target.value })}
                  className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-green-500" required />
              </div>
              <div className="grid grid-cols-3 gap-2">
                {([['proteinGoal', 'Protein'], ['carbsGoal', 'Carbs'], ['fatGoal', 'Fat']] as const).map(([key, label]) => (
                  <div key={key}>
                    <label className="block text-xs font-medium text-gray-700 mb-1">{label} (g)</label>
                    <input type="number" min="0" value={goalsForm[key]}
                      onChange={e => setGoalsForm({ ...goalsForm, [key]: e.target.value })}
                      className="w-full border border-gray-300 rounded-lg px-2 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-green-500" required />
                  </div>
                ))}
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Notes (optional)</label>
                <textarea value={goalsForm.notes} onChange={e => setGoalsForm({ ...goalsForm, notes: e.target.value })}
                  rows={2} placeholder="e.g. Cutting phase"
                  className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-green-500" />
              </div>
              <div className="flex gap-3 pt-1">
                <button type="button" onClick={() => setShowGoals(false)}
                  className="flex-1 border border-gray-300 text-gray-700 py-2 rounded-xl hover:bg-gray-50 text-sm font-medium transition">Cancel</button>
                <button type="submit" disabled={savingGoals}
                  className="flex-1 bg-green-600 hover:bg-green-700 text-white py-2 rounded-xl text-sm font-semibold transition disabled:opacity-50">
                  {savingGoals ? 'Saving...' : 'Save Goals'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
