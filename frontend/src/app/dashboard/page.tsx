'use client';

import { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import Link from 'next/link';
import { useAuth } from '@/contexts/AuthContext';

export default function DashboardPage() {
  const { user, isAuthenticated, isLoading, logout } = useAuth();
  const router = useRouter();

  useEffect(() => {
    if (!isLoading && !isAuthenticated) {
      router.push('/login');
    }
  }, [isLoading, isAuthenticated, router]);

  if (isLoading) {
    return <div className="min-h-screen flex items-center justify-center">Loading...</div>;
  }

  if (!user) return null;

  const handleLogout = () => {
    logout();
    router.push('/');
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <nav className="bg-white shadow-sm border-b">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 flex justify-between items-center">
          <div className="flex items-center gap-2">
            <span className="text-2xl">🍽️</span>
            <span className="text-xl font-bold text-green-700">MealMate</span>
          </div>
          <div className="flex items-center gap-6">
            <Link href="/recipes" className="text-gray-600 hover:text-green-700">Recipes</Link>
            <Link href="/households" className="text-gray-600 hover:text-green-700">Household</Link>
            <Link href="/fridge" className="text-gray-600 hover:text-green-700">Fridge</Link>
            <Link href="/shopping" className="text-gray-600 hover:text-green-700">Shopping</Link>
            <button
              onClick={handleLogout}
              className="text-gray-500 hover:text-red-600 text-sm"
            >
              Sign Out
            </button>
          </div>
        </div>
      </nav>

      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900">
            Good day, {user.firstName}! 👋
          </h1>
          <p className="text-gray-500 mt-1">Here&apos;s your household overview</p>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          <Link href="/recipes" className="bg-white rounded-2xl p-6 shadow-sm border border-gray-100 hover:shadow-md transition-shadow block">
            <div className="text-3xl mb-3">🍳</div>
            <h3 className="text-lg font-semibold text-gray-800">Recipes</h3>
            <p className="text-gray-500 text-sm mt-1">Browse and create recipes</p>
          </Link>

          <Link href="/households" className="bg-white rounded-2xl p-6 shadow-sm border border-gray-100 hover:shadow-md transition-shadow block">
            <div className="text-3xl mb-3">🏠</div>
            <h3 className="text-lg font-semibold text-gray-800">Household</h3>
            <p className="text-gray-500 text-sm mt-1">Manage your household and members</p>
          </Link>

          <Link href="/fridge" className="bg-white rounded-2xl p-6 shadow-sm border border-gray-100 hover:shadow-md transition-shadow block">
            <div className="text-3xl mb-3">🧊</div>
            <h3 className="text-lg font-semibold text-gray-800">Smart Fridge</h3>
            <p className="text-gray-500 text-sm mt-1">Track ingredients and expiry dates</p>
          </Link>

          <Link href="/shopping" className="bg-white rounded-2xl p-6 shadow-sm border border-gray-100 hover:shadow-md transition-shadow block">
            <div className="text-3xl mb-3">🛒</div>
            <h3 className="text-lg font-semibold text-gray-800">Shopping Lists</h3>
            <p className="text-gray-500 text-sm mt-1">Manage household shopping</p>
          </Link>

          <div className="bg-white rounded-2xl p-6 shadow-sm border border-gray-100 opacity-60">
            <div className="text-3xl mb-3">🔥</div>
            <h3 className="text-lg font-semibold text-gray-800">Calorie Tracker</h3>
            <p className="text-gray-500 text-sm mt-1">Coming in Phase 3</p>
          </div>

          <div className="bg-white rounded-2xl p-6 shadow-sm border border-gray-100 opacity-60">
            <div className="text-3xl mb-3">🏋️</div>
            <h3 className="text-lg font-semibold text-gray-800">Fitness Tracker</h3>
            <p className="text-gray-500 text-sm mt-1">Coming in Phase 3</p>
          </div>
        </div>

        <div className="mt-6 bg-green-50 rounded-2xl p-6 border border-green-100">
          <h3 className="font-semibold text-green-800 mb-2">Your Profile</h3>
          <p className="text-green-700 text-sm">
            <strong>Name:</strong> {user.firstName} {user.lastName}<br />
            <strong>Email:</strong> {user.email}
          </p>
        </div>
      </main>
    </div>
  );
}
