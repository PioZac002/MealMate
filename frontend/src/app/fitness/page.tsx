'use client';

import { useEffect, useState, useCallback } from 'react';
import Link from 'next/link';
import { useRouter } from 'next/navigation';
import { exercisesApi, workoutsApi, Exercise, Workout, WorkoutDetail } from '@/services/api';

const MUSCLE_GROUPS = ['Chest', 'Back', 'Shoulders', 'Arms', 'Legs', 'Core', 'Cardio', 'FullBody'];
const MUSCLE_GROUP_COLORS: Record<string, string> = {
  Chest: 'bg-red-100 text-red-700',
  Back: 'bg-blue-100 text-blue-700',
  Shoulders: 'bg-indigo-100 text-indigo-700',
  Arms: 'bg-yellow-100 text-yellow-700',
  Legs: 'bg-green-100 text-green-700',
  Core: 'bg-orange-100 text-orange-700',
  Cardio: 'bg-pink-100 text-pink-700',
  FullBody: 'bg-purple-100 text-purple-700',
};

type Tab = 'workouts' | 'exercises';

function formatDate(d: Date): string {
  return d.toISOString().split('T')[0];
}

interface SetEntry {
  exerciseId: string;
  exerciseName: string;
  setNumber: number;
  reps: string;
  weight: string;
}

export default function FitnessPage() {
  const router = useRouter();
  const [tab, setTab] = useState<Tab>('workouts');

  // Exercises tab state
  const [exercises, setExercises] = useState<Exercise[]>([]);
  const [exerciseSearch, setExerciseSearch] = useState('');
  const [exerciseMuscle, setExerciseMuscle] = useState('');
  const [exercisesLoading, setExercisesLoading] = useState(false);

  // Workouts tab state
  const [workouts, setWorkouts] = useState<Workout[]>([]);
  const [workoutsLoading, setWorkoutsLoading] = useState(false);
  const [selectedWorkout, setSelectedWorkout] = useState<WorkoutDetail | null>(null);

  // Log workout modal
  const [showLog, setShowLog] = useState(false);
  const [logDate, setLogDate] = useState(formatDate(new Date()));
  const [logDuration, setLogDuration] = useState('');
  const [logCalories, setLogCalories] = useState('');
  const [logNotes, setLogNotes] = useState('');
  const [logSets, setLogSets] = useState<SetEntry[]>([]);
  const [loggingWorkout, setLoggingWorkout] = useState(false);
  const [message, setMessage] = useState('');

  // Add exercise to set picker
  const [showExPicker, setShowExPicker] = useState(false);
  const [pickerSearch, setPickerSearch] = useState('');
  const [pickerExercises, setPickerExercises] = useState<Exercise[]>([]);

  useEffect(() => {
    const token = localStorage.getItem('accessToken');
    if (!token) { router.push('/login'); return; }
    loadWorkouts();
    loadExercises();
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  useEffect(() => {
    loadExercises();
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [exerciseSearch, exerciseMuscle]);

  const loadExercises = useCallback(async () => {
    setExercisesLoading(true);
    const res = await exercisesApi.getAll({
      search: exerciseSearch || undefined,
      muscleGroup: exerciseMuscle || undefined,
    });
    if (res.data) setExercises(res.data);
    setExercisesLoading(false);
  }, [exerciseSearch, exerciseMuscle]);

  const loadWorkouts = useCallback(async () => {
    setWorkoutsLoading(true);
    const res = await workoutsApi.getAll(60);
    if (res.data) setWorkouts(res.data);
    setWorkoutsLoading(false);
  }, []);

  async function loadPickerExercises(search: string) {
    const res = await exercisesApi.getAll({ search: search || undefined });
    if (res.data) setPickerExercises(res.data);
  }

  function addExerciseToLog(ex: Exercise) {
    const existing = logSets.filter(s => s.exerciseId === ex.id);
    const setNumber = existing.length + 1;
    setLogSets(prev => [...prev, {
      exerciseId: ex.id,
      exerciseName: ex.name,
      setNumber,
      reps: '',
      weight: '',
    }]);
    setShowExPicker(false);
    setPickerSearch('');
  }

  function removeSet(index: number) {
    setLogSets(prev => prev.filter((_, i) => i !== index));
  }

  async function handleLogWorkout(e: React.FormEvent) {
    e.preventDefault();
    if (logSets.length === 0 && !logNotes) return;
    setLoggingWorkout(true);

    const res = await workoutsApi.log({
      date: logDate,
      durationMinutes: Number(logDuration),
      caloriesBurned: Number(logCalories),
      notes: logNotes || undefined,
      sets: logSets.map(s => ({
        exerciseId: s.exerciseId,
        setNumber: s.setNumber,
        reps: Number(s.reps),
        weight: Number(s.weight),
      })),
    });

    if (res.error) {
      setMessage(res.error);
    } else {
      setShowLog(false);
      setLogSets([]);
      setLogDuration('');
      setLogCalories('');
      setLogNotes('');
      setLogDate(formatDate(new Date()));
      await loadWorkouts();
    }
    setLoggingWorkout(false);
  }

  async function handleViewWorkout(workoutId: string) {
    const res = await workoutsApi.getById(workoutId);
    if (res.data) setSelectedWorkout(res.data);
  }

  async function handleDeleteWorkout(workoutId: string) {
    await workoutsApi.delete(workoutId);
    setSelectedWorkout(null);
    await loadWorkouts();
  }

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
              <Link href="/dashboard" className="hover:text-green-600">Dashboard</Link>
              <Link href="/recipes" className="hover:text-green-600">Recipes</Link>
              <Link href="/fridge" className="hover:text-green-600">Fridge</Link>
              <Link href="/shopping" className="hover:text-green-600">Shopping</Link>
              <Link href="/nutrition" className="hover:text-green-600">Nutrition</Link>
              <Link href="/fitness" className="text-green-600 font-semibold">Fitness</Link>
            </div>
          </div>
        </div>
      </nav>

      <main className="max-w-6xl mx-auto px-4 py-8">
        <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 mb-6">
          <div>
            <h1 className="text-2xl font-bold text-gray-900">Fitness Tracker</h1>
            <p className="text-gray-500 text-sm mt-1">Log workouts and track your progress</p>
          </div>
          <button
            onClick={() => { setShowLog(true); loadPickerExercises(''); }}
            className="bg-green-600 hover:bg-green-700 text-white font-semibold px-5 py-2.5 rounded-xl transition"
          >
            + Log Workout
          </button>
        </div>

        {message && (
          <div className="mb-4 p-3 bg-red-50 border border-red-200 text-red-700 rounded-lg text-sm">
            {message}
            <button onClick={() => setMessage('')} className="ml-2 font-bold">×</button>
          </div>
        )}

        {/* Tabs */}
        <div className="flex gap-1 bg-gray-200 rounded-xl p-1 mb-6 w-fit">
          {(['workouts', 'exercises'] as Tab[]).map(t => (
            <button key={t} onClick={() => setTab(t)}
              className={`px-5 py-2 rounded-lg text-sm font-medium transition capitalize ${tab === t ? 'bg-white shadow text-gray-900' : 'text-gray-600 hover:text-gray-900'}`}>
              {t === 'workouts' ? '🏋️ Workout Log' : '📚 Exercise Library'}
            </button>
          ))}
        </div>

        {/* Workouts Tab */}
        {tab === 'workouts' && (
          <div>
            {workoutsLoading ? (
              <div className="text-center py-12 text-gray-400">Loading workouts...</div>
            ) : workouts.length === 0 ? (
              <div className="text-center py-16 bg-white rounded-xl border border-gray-200">
                <div className="text-5xl mb-3">🏋️</div>
                <h3 className="text-lg font-semibold text-gray-700 mb-1">No workouts logged yet</h3>
                <p className="text-gray-400 text-sm mb-4">Start tracking your fitness journey</p>
                <button onClick={() => { setShowLog(true); loadPickerExercises(''); }}
                  className="bg-green-600 text-white px-5 py-2 rounded-xl text-sm font-semibold hover:bg-green-700 transition">
                  Log Your First Workout
                </button>
              </div>
            ) : (
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                {workouts.map(w => (
                  <div key={w.id} onClick={() => handleViewWorkout(w.id)}
                    className="bg-white rounded-xl border border-gray-200 p-4 cursor-pointer hover:shadow-md transition">
                    <div className="flex justify-between items-start">
                      <div>
                        <div className="font-semibold text-gray-900">{w.workoutPlanName || 'Custom Workout'}</div>
                        <div className="text-sm text-gray-500 mt-0.5">{w.date}</div>
                      </div>
                      <span className="text-xs bg-green-100 text-green-700 px-2 py-0.5 rounded-full font-medium">
                        {w.setCount} sets
                      </span>
                    </div>
                    <div className="flex gap-4 mt-3 text-sm text-gray-600">
                      <span>⏱ {w.durationMinutes} min</span>
                      <span>🔥 {Math.round(w.caloriesBurned)} kcal</span>
                    </div>
                    {w.notes && <p className="mt-2 text-xs text-gray-400 italic truncate">{w.notes}</p>}
                  </div>
                ))}
              </div>
            )}
          </div>
        )}

        {/* Exercises Tab */}
        {tab === 'exercises' && (
          <div>
            <div className="flex flex-col sm:flex-row gap-3 mb-5">
              <input
                type="text"
                placeholder="Search exercises..."
                value={exerciseSearch}
                onChange={e => setExerciseSearch(e.target.value)}
                className="flex-1 border border-gray-300 rounded-xl px-4 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-green-500"
              />
              <select
                value={exerciseMuscle}
                onChange={e => setExerciseMuscle(e.target.value)}
                className="border border-gray-300 rounded-xl px-4 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-green-500"
              >
                <option value="">All Muscle Groups</option>
                {MUSCLE_GROUPS.map(mg => <option key={mg} value={mg}>{mg}</option>)}
              </select>
            </div>

            {exercisesLoading ? (
              <div className="text-center py-8 text-gray-400">Loading...</div>
            ) : (
              <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
                {exercises.map(ex => (
                  <div key={ex.id} className="bg-white rounded-xl border border-gray-200 p-4">
                    <div className="flex justify-between items-start mb-2">
                      <div className="font-semibold text-gray-900 text-sm">{ex.name}</div>
                      <span className={`text-xs px-2 py-0.5 rounded-full font-medium ${MUSCLE_GROUP_COLORS[ex.muscleGroup] || 'bg-gray-100 text-gray-600'}`}>
                        {ex.muscleGroup}
                      </span>
                    </div>
                    {ex.description && <p className="text-xs text-gray-500 mb-2 line-clamp-2">{ex.description}</p>}
                    <div className="text-xs text-gray-400">🔥 {ex.caloriesPerMinute} kcal/min</div>
                  </div>
                ))}
                {exercises.length === 0 && (
                  <div className="col-span-3 text-center py-8 text-gray-400">No exercises found</div>
                )}
              </div>
            )}
          </div>
        )}
      </main>

      {/* Workout Detail Modal */}
      {selectedWorkout && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
          <div className="bg-white rounded-2xl shadow-xl w-full max-w-lg p-6 max-h-[90vh] overflow-y-auto">
            <div className="flex justify-between items-center mb-4">
              <div>
                <h2 className="text-lg font-bold text-gray-900">{selectedWorkout.workoutPlanName || 'Custom Workout'}</h2>
                <p className="text-sm text-gray-500">{selectedWorkout.date}</p>
              </div>
              <button onClick={() => setSelectedWorkout(null)} className="text-gray-400 hover:text-gray-600 text-xl">×</button>
            </div>

            <div className="flex gap-4 text-sm text-gray-600 mb-4 pb-4 border-b">
              <span>⏱ {selectedWorkout.durationMinutes} min</span>
              <span>🔥 {Math.round(selectedWorkout.caloriesBurned)} kcal</span>
            </div>

            {selectedWorkout.notes && (
              <p className="text-sm text-gray-500 italic mb-4">{selectedWorkout.notes}</p>
            )}

            <div className="space-y-3">
              {Object.entries(
                selectedWorkout.sets.reduce((acc, s) => {
                  if (!acc[s.exerciseName]) acc[s.exerciseName] = [];
                  acc[s.exerciseName].push(s);
                  return acc;
                }, {} as Record<string, typeof selectedWorkout.sets>)
              ).map(([exName, sets]) => (
                <div key={exName} className="bg-gray-50 rounded-lg p-3">
                  <div className="flex items-center gap-2 mb-2">
                    <span className="font-medium text-sm text-gray-800">{exName}</span>
                    {sets[0] && (
                      <span className={`text-xs px-1.5 py-0.5 rounded-full ${MUSCLE_GROUP_COLORS[sets[0].muscleGroup] || 'bg-gray-100 text-gray-600'}`}>
                        {sets[0].muscleGroup}
                      </span>
                    )}
                  </div>
                  <div className="grid grid-cols-4 text-xs text-gray-500 mb-1 px-1">
                    <span>Set</span><span>Reps</span><span>Weight</span><span>PR</span>
                  </div>
                  {sets.map(s => (
                    <div key={s.id} className="grid grid-cols-4 text-sm px-1 py-0.5">
                      <span className="text-gray-600">{s.setNumber}</span>
                      <span>{s.reps}</span>
                      <span>{s.weight} kg</span>
                      <span>{s.isPersonalRecord ? '🏆' : ''}</span>
                    </div>
                  ))}
                </div>
              ))}
            </div>

            <div className="mt-5 flex gap-3">
              <button onClick={() => setSelectedWorkout(null)}
                className="flex-1 border border-gray-300 text-gray-700 py-2 rounded-xl hover:bg-gray-50 text-sm font-medium transition">
                Close
              </button>
              <button onClick={() => handleDeleteWorkout(selectedWorkout.id)}
                className="border border-red-300 text-red-600 px-4 py-2 rounded-xl hover:bg-red-50 text-sm font-medium transition">
                Delete
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Log Workout Modal */}
      {showLog && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
          <div className="bg-white rounded-2xl shadow-xl w-full max-w-lg p-6 max-h-[90vh] overflow-y-auto">
            <div className="flex justify-between items-center mb-4">
              <h2 className="text-lg font-bold text-gray-900">Log Workout</h2>
              <button onClick={() => setShowLog(false)} className="text-gray-400 hover:text-gray-600 text-xl">×</button>
            </div>
            <form onSubmit={handleLogWorkout} className="space-y-4">
              <div className="grid grid-cols-3 gap-3">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Date</label>
                  <input type="date" value={logDate} onChange={e => setLogDate(e.target.value)}
                    className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-green-500" required />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Duration (min)</label>
                  <input type="number" min="1" value={logDuration} onChange={e => setLogDuration(e.target.value)}
                    className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-green-500" required />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Calories</label>
                  <input type="number" min="0" value={logCalories} onChange={e => setLogCalories(e.target.value)}
                    className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-green-500" required />
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Notes (optional)</label>
                <input type="text" placeholder="e.g. Felt strong today" value={logNotes} onChange={e => setLogNotes(e.target.value)}
                  className="w-full border border-gray-300 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-green-500" />
              </div>

              {/* Sets */}
              <div>
                <div className="flex justify-between items-center mb-2">
                  <label className="text-sm font-medium text-gray-700">Exercise Sets</label>
                  <button type="button" onClick={() => setShowExPicker(true)}
                    className="text-xs text-green-600 hover:underline font-medium">
                    + Add Exercise
                  </button>
                </div>

                {logSets.length === 0 ? (
                  <div className="border-2 border-dashed border-gray-200 rounded-lg py-4 text-center text-sm text-gray-400">
                    Add exercises to your workout
                  </div>
                ) : (
                  <div className="space-y-2">
                    {logSets.map((s, i) => (
                      <div key={i} className="flex items-center gap-2 bg-gray-50 rounded-lg p-2">
                        <div className="flex-1 text-xs font-medium text-gray-700 truncate">{s.exerciseName} #{s.setNumber}</div>
                        <div className="flex items-center gap-1">
                          <input type="number" min="0" placeholder="Reps" value={s.reps}
                            onChange={e => setLogSets(prev => prev.map((item, idx) => idx === i ? { ...item, reps: e.target.value } : item))}
                            className="w-16 border border-gray-300 rounded px-1 py-1 text-xs focus:outline-none focus:ring-1 focus:ring-green-500" required />
                          <span className="text-xs text-gray-400">reps</span>
                          <input type="number" min="0" step="0.5" placeholder="kg" value={s.weight}
                            onChange={e => setLogSets(prev => prev.map((item, idx) => idx === i ? { ...item, weight: e.target.value } : item))}
                            className="w-16 border border-gray-300 rounded px-1 py-1 text-xs focus:outline-none focus:ring-1 focus:ring-green-500" required />
                          <span className="text-xs text-gray-400">kg</span>
                        </div>
                        <button type="button" onClick={() => removeSet(i)} className="text-gray-400 hover:text-red-500 text-xs">✕</button>
                      </div>
                    ))}
                  </div>
                )}
              </div>

              <div className="flex gap-3">
                <button type="button" onClick={() => setShowLog(false)}
                  className="flex-1 border border-gray-300 text-gray-700 py-2 rounded-xl hover:bg-gray-50 text-sm font-medium transition">Cancel</button>
                <button type="submit" disabled={loggingWorkout}
                  className="flex-1 bg-green-600 hover:bg-green-700 text-white py-2 rounded-xl text-sm font-semibold transition disabled:opacity-50">
                  {loggingWorkout ? 'Saving...' : 'Save Workout'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Exercise Picker Modal */}
      {showExPicker && (
        <div className="fixed inset-0 z-[60] flex items-center justify-center bg-black/40 p-4">
          <div className="bg-white rounded-2xl shadow-xl w-full max-w-sm p-5 max-h-[80vh] flex flex-col">
            <div className="flex justify-between items-center mb-3">
              <h3 className="font-bold text-gray-900">Pick Exercise</h3>
              <button onClick={() => setShowExPicker(false)} className="text-gray-400 hover:text-gray-600 text-xl">×</button>
            </div>
            <input type="text" placeholder="Search..." value={pickerSearch}
              onChange={e => { setPickerSearch(e.target.value); loadPickerExercises(e.target.value); }}
              className="border border-gray-300 rounded-lg px-3 py-2 text-sm mb-3 focus:outline-none focus:ring-2 focus:ring-green-500" />
            <div className="overflow-y-auto flex-1 space-y-1">
              {(pickerSearch ? pickerExercises : exercises).map(ex => (
                <button key={ex.id} type="button" onClick={() => addExerciseToLog(ex)}
                  className="w-full text-left px-3 py-2 rounded-lg hover:bg-green-50 text-sm flex justify-between items-center">
                  <span className="font-medium text-gray-800">{ex.name}</span>
                  <span className={`text-xs px-1.5 py-0.5 rounded-full ${MUSCLE_GROUP_COLORS[ex.muscleGroup] || 'bg-gray-100 text-gray-600'}`}>
                    {ex.muscleGroup}
                  </span>
                </button>
              ))}
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
